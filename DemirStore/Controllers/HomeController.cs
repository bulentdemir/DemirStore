using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DemirStore.Models;
using PagedList;

namespace DemirStore.Controllers
{
    public class HomeController : Controller
    {
        DemirStoreDBEntities db = new DemirStoreDBEntities();
        // GET: Home
        public ActionResult Index()
        {
            
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            tblUsers userModel = new tblUsers();
            return View(userModel);
        }

        [HttpPost]
        public ActionResult Register(tblUsers userModel)
        {
            using (DemirStoreDBEntities dbModel = new DemirStoreDBEntities())
            {
                if (dbModel.tblUsers.Any(x => x.Email == userModel.Email))
                {
                    ViewBag.DuplicateMessage = "Bu e mail daha önceden alındı.";
                    return View("Register", userModel);
                }
                else if (dbModel.tblUsers.Any(x => x.PhoneNumber == userModel.PhoneNumber))
                {
                    ViewBag.DuplicateMessage = "Bu telefon numarası daha önceden alındı.";
                    return View("Register", userModel);
                }
                var crypto = new SimpleCrypto.PBKDF2();
                string encryptedPswd = crypto.Compute(userModel.Pswd);
                userModel.Pswd = encryptedPswd;
                userModel.ConfirmPswd = encryptedPswd;
                userModel.PswdSalt = crypto.Salt;
                userModel.isVerified = false;

                dbModel.tblUsers.Add(userModel);
                dbModel.SaveChanges();
                return View("Login");
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            tblUsers userModel = new tblUsers();
            return View(userModel);
        }

        [HttpPost]
        public ActionResult Login(tblUsers userModel)
        {
            using (DemirStoreDBEntities db = new DemirStoreDBEntities())
            {
                if(new LoginCheck().IsLoginSuccess(userModel))
                {
                    var user = db.tblUsers.Where(x => x.Email == userModel.Email).FirstOrDefault();
                    if (user.isVerified == true)
                    {
                        Session["isAdmin"] = user.isVerified;
                    }
                    Session["userID"] = user.Id;
                    Session["userName"] = user.Name;
                    List<tblShoppingCart> sCart = db.tblShoppingCart.Where(x => x.UserId == userModel.Id && x.OrderId == null).ToList();
                    if (sCart.Count() != 0)
                    {
                        List<ShoppingCart> cart = new List<ShoppingCart>();
                        foreach (var item in sCart)
                        {
                            cart.Add(new ShoppingCart(db.tblProduct.Find(item.ProductId), item.Amount));
                        }
                        Session["cart"] = cart;
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.LoginErrorMessage = "Hatalı E-Mail ve ya Şifre";
                    return View("Login", userModel);
                }
            }
        }

        public ActionResult LogOut()
        {
            using (DemirStoreDBEntities dbModel = new DemirStoreDBEntities())
            {
                int userId = (int)Session["userID"];
                List<tblShoppingCart> oldSCart = dbModel.tblShoppingCart.Where(x => x.UserId == userId && x.OrderId == null).ToList();
                foreach(var item in oldSCart)
                {
                    dbModel.tblShoppingCart.Remove(item);
                }
                if (Session["cart"] != null)
                {
                    foreach (ShoppingCart item in (List<ShoppingCart>)Session["cart"])
                    {
                        tblShoppingCart sCart = new tblShoppingCart();
                        sCart.UserId = (int)Session["userID"];
                        sCart.ProductId = item.Product.Id;
                        sCart.Amount = item.Quantity;
                        sCart.TotalPrice = (item.Product.Price * item.Quantity);
                        dbModel.tblShoppingCart.Add(sCart);
                    }
                }
                dbModel.SaveChanges();
            }
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }

        public ActionResult ProductDetails(int? id)
        {
            using (DemirStoreDBEntities dbModel = new DemirStoreDBEntities())
            {
                tblProduct productModel = dbModel.tblProduct.FirstOrDefault(x => x.Id == id);
                ViewBag.categoryName = dbModel.tblCategory.FirstOrDefault(x => x.Id == productModel.CategoryId).Name;
                return View(productModel);
            }
        }

        public ActionResult ShowCategories()
        {
            using (DemirStoreDBEntities db = new DemirStoreDBEntities())
            {
                List<tblCategory> categories = db.tblCategory.OrderByDescending(x => x.Id).Take(3).ToList();
                return PartialView(db.tblCategory.ToList());
            }
        }

        public ActionResult ShowProducts()
        {
            using (DemirStoreDBEntities db = new DemirStoreDBEntities())
            {
                var products = (from p in db.tblProduct join c in db.tblCategory on p.CategoryId equals c.Id select new ShowProducts {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ProductPicture = p.PictureLink,
                    ProductDescription = p.Description,
                    ProductPrice = p.Price,
                    ProductStock = p.Stock,
                    ProductCategoryId = c.Id,
                    ProductCategoryName = c.Name,
                }).OrderByDescending(x => x.ProductId).Take(10).ToList();
                return PartialView(products);
            }
        }

        public ActionResult Store(int? id, int page = 1)
        {
            using (DemirStoreDBEntities dbModel = new DemirStoreDBEntities())
            {
                ViewBag.categories = dbModel.tblCategory.ToList();
                PagedList.IPagedList<tblProduct> productModel;
                if(id != null)
                {
                    productModel = dbModel.tblProduct.Where(x => x.CategoryId == id).OrderByDescending(x => x.Id).ToPagedList<tblProduct>(page, 9);
                }
                else
                {
                    productModel = dbModel.tblProduct.OrderByDescending(x => x.Id).ToPagedList<tblProduct>(page, 9);
                }

                return View(productModel);
            }
        }
        private int isExisting(int id)
        {
            List<ShoppingCart> cart = (List<ShoppingCart>)Session["cart"];
            for(int i = 0; i < cart.Count; i++)
            {
                if(cart[i].Product.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        //Delete Product From Shopping Cart
        public ActionResult DeleteProductSCart(int id)
        {
            int index = isExisting(id);
            List<ShoppingCart> cart = (List<ShoppingCart>)Session["cart"];
            cart.RemoveAt(index);
            Session["cart"] = cart;
            return View("Cart");
        }

        //Update Product in the Shopping Cart
        public ActionResult UpdateProductSCart(FormCollection fc)
        {
            string[] quantities = fc.GetValues("quantity");
            List<ShoppingCart> cart = (List<ShoppingCart>)Session["cart"];
            for(int i = 0; i < cart.Count; i++)
            {
                int productId = cart[i].Product.Id;
                tblProduct product = db.tblProduct.Where(x => x.Id == productId).FirstOrDefault();
                int quantity = Convert.ToInt32(quantities[i]);
                if(product.Stock >= quantity)
                {
                    cart[i].Quantity = quantity;
                }
            }
            Session["cart"] = cart;
            return View("Cart");
        }

        public ActionResult AddToCart(int id, FormCollection fc)
        {
            int quantity;
            if (fc.GetValue("quantity") != null)
            {
                quantity = Int32.Parse(fc["quantity"]);
            }
            else
            {
                quantity = 1;
            }
            using (DemirStoreDBEntities dbModel = new DemirStoreDBEntities())
            {
                if (Session["cart"] == null)
                {
                    List<ShoppingCart> cart = new List<ShoppingCart>();
                    tblProduct product = dbModel.tblProduct.Find(id);
                    if(product.Stock > 0 && quantity < product.Stock)
                    {
                        cart.Add(new ShoppingCart(dbModel.tblProduct.Find(id), quantity));
                        Session["cart"] = cart;
                    }

                }
                else
                {
                    List<ShoppingCart> cart = (List<ShoppingCart>)Session["cart"];
                    int index = isExisting(id);
                    if(index == -1)
                    {
                        cart.Add(new ShoppingCart(dbModel.tblProduct.Find(id), quantity));
                    }
                    else
                    {
                        cart[index].Quantity = cart[index].Quantity + quantity;
                    }
                    Session["cart"] = cart;
                }
                return View("Cart");
            }
        }

        public ActionResult Cart()
        {
            if(Session["cart"] != null)
            {
                List<ShoppingCart> cart = (List<ShoppingCart>)Session["cart"];

                return View(cart.ToList());
            }
            else
            {
                return View("Cart");
            }
        }

        [HttpGet]
        public ActionResult EditAccount(int id)
        {
            using (DemirStoreDBEntities dbModel = new DemirStoreDBEntities())
            {
                tblUsers userModel = dbModel.tblUsers.FirstOrDefault(x => x.Id == id);

                return View(userModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(tblUsers userModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userModel).State = EntityState.Modified;
                db.SaveChanges();
                return View();
            }
            return View(userModel);
        }

        [HttpGet]
        public ActionResult ChangePswd(int id)
        {
            tblUsers userModel = db.tblUsers.FirstOrDefault(x => x.Id == id);
            userModel.Pswd = "";
            userModel.ConfirmPswd = "";
            return View(userModel);
        }

        [HttpPost]
        public ActionResult ChangePswd(tblUsers userModel)
        {
            var crypto = new SimpleCrypto.PBKDF2();

            if(ModelState.IsValid)
            {
                string encryptedPswd = crypto.Compute(userModel.Pswd);
                userModel.Pswd = encryptedPswd;
                userModel.ConfirmPswd = encryptedPswd;
                userModel.PswdSalt = crypto.Salt;

                db.Entry(userModel).State = EntityState.Modified;
                db.SaveChanges();
                return View();
            }

            return View();
        }

        public ActionResult Address()
        {
            using (DemirStoreDBEntities dbModel = new DemirStoreDBEntities())
            {
                int userId = (int)Session["userID"];
                List<tblAddress> address = dbModel.tblAddress.Where(x => x.UserId == userId).ToList();
                return View(address);
            }
        }

        [HttpGet]
        public ActionResult AddAddress()
        {
            tblAddress addressModel = new tblAddress();
            return View(addressModel);
        }

        [HttpPost]
        public ActionResult AddAddress(tblAddress addressModel)
        {
            using (DemirStoreDBEntities dbModel = new DemirStoreDBEntities())
            {
                addressModel.UserId = (int)Session["userID"];
                dbModel.tblAddress.Add(addressModel);
                dbModel.SaveChanges();
                return RedirectToAction("Address");
            }
        }

        public ActionResult EditAddress(int? id, tblAddress addressModel)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            addressModel = db.tblAddress.Find(id);
            if (addressModel == null)
            {
                return HttpNotFound();
            }
            return View(addressModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAddress(tblAddress addressModel)
        {
            if (ModelState.IsValid)
            {
                addressModel.UserId = (int)Session["userID"];
                db.Entry(addressModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Address");
            }
            return View(addressModel);
        }

        public ActionResult DeleteAddress(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAddress address = db.tblAddress.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        [HttpPost, ActionName("DeleteAddress")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAddressConfirmed(int id)
        {
            tblAddress addressModel = db.tblAddress.Find(id);
            db.tblAddress.Remove(addressModel);
            db.SaveChanges();
            return RedirectToAction("Address");
        }

        [HttpGet]
        public ActionResult Order()
        {
            if(Session["userID"] != null)
            {
                
                if(Session["cart"] != null && ((List<ShoppingCart>)Session["cart"]).Count() != 0)
                {
                    int userId = (int)Session["userID"];
                    int isAdres = db.tblAddress.Where(x => x.UserId == userId).Count();
                    if(isAdres != 0)
                    {
                        ViewBag.address = db.tblAddress.Where(x => x.UserId == userId).ToList();
                        ViewBag.AdressId = new SelectList(db.tblAddress.Where(x => x.UserId == userId), "Id", "Name");
                    }
                    else
                    {
                        return RedirectToAction("AddAddress","Home");
                    }
                }
                else
                {
                    return RedirectToAction("Cart", "Home");
                }
            }
            tblOrder orderModel = new tblOrder();
            return View(orderModel);
        }

        [HttpPost]
        public ActionResult Order(tblOrder orderModel)
        {
            if (ModelState.IsValid)
            {
                tblAddress userAddress = db.tblAddress.Where(x => x.Id == orderModel.AdressId).FirstOrDefault();
                tblAddress orderAddress = new tblAddress{
                    Name = userAddress.Name,
                    Address = userAddress.Address,
                    City = userAddress.City,
                    Country = userAddress.Country,
                    Zip = userAddress.Zip,
                };
                
                db.tblAddress.Add(orderAddress);

                orderModel.AdressId = orderAddress.Id;
                orderModel.isShipped = false;
                orderModel.Date = DateTime.Now;
                db.tblOrder.Add(orderModel);
            }

            int userId = (int)Session["userID"];
            List<tblShoppingCart> oldSCart = db.tblShoppingCart.Where(x => x.UserId == userId && x.OrderId == null).ToList();
            foreach (var item in oldSCart)
            {
                db.tblShoppingCart.Remove(item);
            }
            foreach (ShoppingCart item in (List<ShoppingCart>)Session["cart"])
            {
                tblShoppingCart sCart = new tblShoppingCart {
                    UserId = (int)Session["userID"],
                    ProductId = item.Product.Id,
                    Amount = item.Quantity,
                    TotalPrice = (item.Product.Price * item.Quantity),
                    OrderId = orderModel.Id,
                };
                tblProduct product = db.tblProduct.Where(x => x.Id == sCart.ProductId).FirstOrDefault();
                product.Stock -= sCart.Amount;
                db.Entry(product).State = EntityState.Modified;

                db.tblShoppingCart.Add(sCart);
            }
            db.SaveChanges();
            Session.Remove("cart");

            return View("Index");
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        private int isExistingOrder(List<tblOrder> orders, tblOrder order)
        {
            for(int i = 0; i < orders.Count; i++)
            {
                if(orders[i].Id == order.Id)
                {
                    return i;
                }
            }

            return -1;
        }

        public ActionResult ShowVerifiedOrders(int page = 1)
        {

            if(Session["userID"] != null)
            {
                int userId = (int)Session["userID"];
                var orders = (from sc in db.tblShoppingCart
                             join o in db.tblOrder on sc.OrderId equals o.Id
                             where sc.UserId == userId && o.isShipped == true select o).Distinct().OrderByDescending(x => x.Id).ToPagedList<tblOrder>(page, 10);
                return View(orders);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult ShowPendingOrders(int page = 1)
        {
            if (Session["userID"] != null)
            {
                int userId = (int)Session["userID"];
                var orders = (from sc in db.tblShoppingCart
                              join o in db.tblOrder on sc.OrderId equals o.Id
                              where sc.UserId == userId && o.isShipped == false
                              select o).Distinct().OrderByDescending(x => x.Id).ToPagedList<tblOrder>(page, 10);
                return View(orders);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblOrder orderModel = db.tblOrder.Find(id);
            ViewBag.orderAddress = db.tblAddress.Where(x => x.Id == orderModel.AdressId).FirstOrDefault();
            int userId = (int)Session["userID"];
            ViewBag.orderProducts = (from sc in db.tblShoppingCart
                                     join o in db.tblOrder on sc.OrderId equals o.Id
                                     where sc.UserId == userId && o.Id == id 
                                     select new OrderProducts
                                     {
                                         ProductName = sc.tblProduct.Name,
                                         ProductPicture = sc.tblProduct.PictureLink,
                                         ProductPrice = sc.tblProduct.Price,
                                         ProductQuantity = sc.Amount,


                                     }).ToList();
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        [HttpPost]
        public ActionResult SearchBox(string searching, string category, int page = 1)
        {
            ViewBag.categories = db.tblCategory.ToList();
            PagedList.IPagedList<tblProduct> productModel;
            if (category != "")
            {
                int cat = Convert.ToInt32(category);
                productModel = db.tblProduct.Where(x => x.Name.Contains(searching) && x.CategoryId == cat).OrderByDescending(x => x.Id).ToPagedList<tblProduct>(page, 9);
            }
            else
            {
                productModel = db.tblProduct.Where(x => x.Name.Contains(searching) || searching == null).OrderByDescending(x => x.Id).ToPagedList<tblProduct>(page, 9);
            }
            return View("Store", productModel);
        }


    }
}
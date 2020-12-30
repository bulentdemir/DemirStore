using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DemirStore.Models;
using PagedList;

namespace DemirStore.Controllers
{
    public class AdminController : Controller
    {
        DemirStoreDBEntities db = new DemirStoreDBEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Products(int page = 1)
        {
            PagedList.IPagedList<tblProduct> productModel = db.tblProduct.OrderByDescending(x => x.Id).ToPagedList<tblProduct>(page, 10);
            return View(productModel);
        }

        [HttpGet]
        public ActionResult AddProducts()
        {
            ViewBag.CategoryId = new SelectList(db.tblCategory, "Id", "Name");
            tblProduct productModel = new tblProduct();
            return View(productModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProducts(tblProduct productModel, HttpPostedFileBase fileUpload)
        {
            string pictureLink = "";
            if (fileUpload != null)
            {
                Image img = Image.FromStream(fileUpload.InputStream);
                int width = Convert.ToInt32(ConfigurationManager.AppSettings["ProductWidth"].ToString());
                int height = Convert.ToInt32(ConfigurationManager.AppSettings["ProductHeight"].ToString());

                pictureLink = "/img/ProductsImage/" + Guid.NewGuid() + Path.GetExtension(fileUpload.FileName);

                Bitmap bm = new Bitmap(img, width, height);
                bm.Save(Server.MapPath(pictureLink));

            }
            if (pictureLink == "")
            {
                productModel.PictureLink = "/img/ProductsImage/emptyProducts.jpg";
            }
            else
            {
                productModel.PictureLink = pictureLink;
            }
            db.tblProduct.Add(productModel);
            db.SaveChanges();
            return RedirectToAction("Products");
        }

        public ActionResult EditProducts(int? id, tblProduct productModel)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            productModel = db.tblProduct.Find(id);
            if (productModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.tblCategory, "Id", "Name", productModel.CategoryId);
            return View(productModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProducts(tblProduct productModel, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                string pictureLink = "";
                if (fileUpload != null)
                {
                    Image img = Image.FromStream(fileUpload.InputStream);
                    int width = Convert.ToInt32(ConfigurationManager.AppSettings["ProductWidth"].ToString());
                    int height = Convert.ToInt32(ConfigurationManager.AppSettings["ProductHeight"].ToString());

                    pictureLink = "/img/ProductsImage/" + Guid.NewGuid() + Path.GetExtension(fileUpload.FileName);

                    Bitmap bm = new Bitmap(img, width, height);
                    bm.Save(Server.MapPath(pictureLink));

                }
                if (pictureLink == "")
                {
                    productModel.PictureLink = "/img/ProductsImage/emptyProducts.jpg";
                }
                else
                {
                    productModel.PictureLink = pictureLink;
                }
                db.Entry(productModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Products");
            }
            ViewBag.CategoryId = new SelectList(db.tblCategory, "Id", "Name", productModel.CategoryId);
            return View(productModel);
        }

        public ActionResult DeleteProducts(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblProduct tblProduct = db.tblProduct.Find(id);
            if (tblProduct == null)
            {
                return HttpNotFound();
            }
            return View(tblProduct);
        }

        [HttpPost, ActionName("DeleteProducts")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProductsConfirmed(int id)
        {
            tblProduct productModel = db.tblProduct.Find(id);
            db.tblProduct.Remove(productModel);
            db.SaveChanges();
            return RedirectToAction("Products");
        }

        public ActionResult Category(int page = 1)
        {
            PagedList.IPagedList<tblCategory> categoryModel = db.tblCategory.OrderByDescending(x => x.Id).ToPagedList<tblCategory>(page, 10);
            return View(categoryModel);
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            tblCategory categoryModel = new tblCategory();
            return View(categoryModel);
        }

        [HttpPost]
        public ActionResult AddCategory(tblCategory categoryModel, HttpPostedFileBase fileUpload)
        {
            string pictureLink = "";
            if (fileUpload != null)
            {
                Image img = Image.FromStream(fileUpload.InputStream);
                int width = Convert.ToInt32(ConfigurationManager.AppSettings["CategoryWidth"].ToString());
                int height = Convert.ToInt32(ConfigurationManager.AppSettings["CategoryHeight"].ToString());

                pictureLink = "/img/CategoryImage/" + Guid.NewGuid() + Path.GetExtension(fileUpload.FileName);

                Bitmap bm = new Bitmap(img, width, height);
                bm.Save(Server.MapPath(pictureLink));

            }
            if (pictureLink == "")
            {
                categoryModel.PictureLink = "/img/CategoryImage/emptyCategory.jpg";
            }
            else
            {
                categoryModel.PictureLink = pictureLink;
            }
            db.tblCategory.Add(categoryModel);
            db.SaveChanges();
            return RedirectToAction("Category");
        }

        public ActionResult EditCategory(int? id, tblCategory productModel)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCategory tblCategory = db.tblCategory.Find(id);
            if (tblCategory == null)
            {
                return HttpNotFound();
            }
            return View(tblCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(tblCategory categoryModel, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                string pictureLink = "";
                if (fileUpload != null)
                {
                    Image img = Image.FromStream(fileUpload.InputStream);
                    int width = Convert.ToInt32(ConfigurationManager.AppSettings["CategoryWidth"].ToString());
                    int height = Convert.ToInt32(ConfigurationManager.AppSettings["CategoryHeight"].ToString());

                    pictureLink = "/img/CategoryImage/" + Guid.NewGuid() + Path.GetExtension(fileUpload.FileName);

                    Bitmap bm = new Bitmap(img, width, height);
                    bm.Save(Server.MapPath(pictureLink));

                }
                if (pictureLink == "")
                {
                    categoryModel.PictureLink = "/img/CategoryImage/emptyCategory.jpg";
                }
                else
                {
                    categoryModel.PictureLink = pictureLink;
                }
                db.Entry(categoryModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Category");
            }
            return View(categoryModel);
        }

        public ActionResult DeleteCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCategory categoryModel = db.tblCategory.Find(id);
            if (categoryModel == null)
            {
                return HttpNotFound();
            }
            return View(categoryModel);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategoryConfirmed(int id)
        {
            tblCategory categoryModel = db.tblCategory.Find(id);
            db.tblCategory.Remove(categoryModel);
            db.SaveChanges();
            return RedirectToAction("Category");
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
            if (new LoginCheck().AdminIsLoginSuccess(userModel))
            {
                var user = db.tblUsers.Where(x => x.Email == userModel.Email).FirstOrDefault();
                Session["adminID"] = user.Id;
                Session["adminName"] = user.Name;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.LoginErrorMessage = "Hatalı Kullanıcı adı veya şifre";
                return View("Login", userModel);
            }
        }

        public ActionResult LogOut()
        {
            Session.Remove("adminID");
            Session.Remove("adminName");
            return RedirectToAction("Login", "Admin");
        }

        public ActionResult Users(int page = 1)
        {
            PagedList.IPagedList<tblUsers> userModel = db.tblUsers.OrderByDescending(x => x.Id).ToPagedList<tblUsers>(page, 10);
            return View(userModel);
        }

        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUsers userModel = db.tblUsers.Find(id);
            if (userModel == null)
            {
                return HttpNotFound();
            }
            return View(userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(tblUsers userModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Users");
            }
            return View(userModel);
        }

        [HttpGet]
        public ActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUsers userModel = db.tblUsers.Find(id);
            if (userModel == null)
            {
                return HttpNotFound();
            }
            return View(userModel);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(int id)
        {
            tblUsers userModel = db.tblUsers.Find(id);
            db.tblUsers.Remove(userModel);
            db.SaveChanges();
            return RedirectToAction("Users");
        }

        public ActionResult Orders(int page = 1)
        {
            PagedList.IPagedList<tblOrder> orderModel = db.tblOrder.OrderByDescending(x => x.Id).ToPagedList<tblOrder>(page, 10);
            return View(orderModel);
        }

        public ActionResult VerifiedOrders(int page = 1)
        {
            PagedList.IPagedList<tblOrder> orderModel = db.tblOrder.Where(x => x.isShipped == true).OrderByDescending(x => x.Id).ToPagedList<tblOrder>(page, 10);
            return View(orderModel);
        }

        public ActionResult PendingOrders(int page = 1)
        {
            PagedList.IPagedList<tblOrder> orderModel = db.tblOrder.Where(x => x.isShipped == false).OrderByDescending(x => x.Id).ToPagedList<tblOrder>(page, 10);
            return View(orderModel);
        }

        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblOrder orderModel = db.tblOrder.Find(id);
            ViewBag.orderAddress = db.tblAddress.Where(x => x.Id == orderModel.AdressId).FirstOrDefault();
            ViewBag.orderProducts = (from sc in db.tblShoppingCart
                                     join o in db.tblOrder on sc.OrderId equals o.Id
                                     where o.Id == id
                                     select new OrderProducts
                                     {
                                         ProductName = sc.tblProduct.Name,
                                         ProductPicture = sc.tblProduct.PictureLink,
                                         ProductPrice = sc.tblProduct.Price,
                                         ProductQuantity = sc.Amount,
                                     }).ToList();
            ViewBag.orderUser = (from u in db.tblUsers
                                 join sc in db.tblShoppingCart on u.Id equals sc.UserId
                                 join o in db.tblOrder on sc.OrderId equals o.Id
                                 where o.Id == id
                                 select u).FirstOrDefault();

            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        [HttpGet]
        public ActionResult OrderEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblOrder orderModel = db.tblOrder.Find(id);
            ViewBag.orderAddress = db.tblAddress.Where(x => x.Id == orderModel.AdressId).FirstOrDefault();
            ViewBag.orderProducts = (from sc in db.tblShoppingCart
                                     join o in db.tblOrder on sc.OrderId equals o.Id
                                     where o.Id == id
                                     select new OrderProducts
                                     {
                                         ProductName = sc.tblProduct.Name,
                                         ProductPicture = sc.tblProduct.PictureLink,
                                         ProductPrice = sc.tblProduct.Price,
                                         ProductQuantity = sc.Amount,
                                     }).ToList();
            ViewBag.orderUser = (from u in db.tblUsers
                                 join sc in db.tblShoppingCart on u.Id equals sc.UserId
                                 join o in db.tblOrder on sc.OrderId equals o.Id
                                 where o.Id == id
                                 select u).FirstOrDefault();
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdressId = new SelectList(db.tblAddress, "Id", "Name", orderModel.AdressId);
            return View(orderModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderEdit([Bind(Include = "Id,AdressId,Date,isShipped")]tblOrder orderModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Orders");
            }
            return View(orderModel);
        }

        [HttpGet]
        public ActionResult OrderDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblOrder orderModel = db.tblOrder.Find(id);
            ViewBag.orderAddress = db.tblAddress.Where(x => x.Id == orderModel.AdressId).FirstOrDefault();
            ViewBag.orderProducts = (from sc in db.tblShoppingCart
                                     join o in db.tblOrder on sc.OrderId equals o.Id
                                     where o.Id == id
                                     select new OrderProducts
                                     {
                                         ProductName = sc.tblProduct.Name,
                                         ProductPicture = sc.tblProduct.PictureLink,
                                         ProductPrice = sc.tblProduct.Price,
                                         ProductQuantity = sc.Amount,
                                     }).ToList();
            ViewBag.orderUser = (from u in db.tblUsers
                                 join sc in db.tblShoppingCart on u.Id equals sc.UserId
                                 join o in db.tblOrder on sc.OrderId equals o.Id
                                 select u).FirstOrDefault();
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // POST: tblOrders/Delete/5
        [HttpPost, ActionName("OrderDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult OrderDeleteConfirmed(int id)
        {
            tblOrder orderModel = db.tblOrder.Find(id);
            db.tblOrder.Remove(orderModel);
            db.SaveChanges();
            return RedirectToAction("Orders");
        }
    }
}
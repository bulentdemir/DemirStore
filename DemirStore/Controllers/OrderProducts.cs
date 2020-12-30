using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemirStore.Controllers
{
    public class OrderProducts
    {
        private string productName;
        private string productPicture;
        private int productPrice;
        private int productQuantity;

        public OrderProducts()
        { }

        public OrderProducts(string ProductName, string ProductPicture, int ProductPrice, int ProductQuantity)
        {
            this.productName = ProductName;
            this.productPicture = ProductPicture;
            this.productPrice = ProductPrice;
            this.productQuantity = ProductQuantity;
        }

        public string ProductName { get => productName; set => productName = value; }
        public string ProductPicture { get => productPicture; set => productPicture = value; }
        public int ProductPrice { get => productPrice; set => productPrice = value; }
        public int ProductQuantity { get => productQuantity; set => productQuantity = value; }
    }
}
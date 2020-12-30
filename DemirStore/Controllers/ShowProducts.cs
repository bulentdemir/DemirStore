using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemirStore.Controllers
{
    public class ShowProducts
    {
        int productId;
        string productName;
        string productPicture;
        string productDescription;
        int productPrice;
        int productStock;
        string productCategoryName;
        int productCategoryId;

        public ShowProducts()
        {

        }

        public ShowProducts(int productId, string productName, string productPicture, string productDescription, int productPrice, int productStock, string productCategoryName, int productCategoryId)
        {
            this.productId = productId;
            this.productName = productName;
            this.productPicture = productPicture;
            this.productDescription = productDescription;
            this.productPrice = productPrice;
            this.productStock = productStock;
            this.productCategoryName = productCategoryName;
            this.productCategoryId = productCategoryId;
        }

        public int ProductId { get => productId; set => productId = value; }
        public string ProductName { get => productName; set => productName = value; }
        public string ProductPicture { get => productPicture; set => productPicture = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public int ProductPrice { get => productPrice; set => productPrice = value; }
        public int ProductStock { get => productStock; set => productStock = value; }
        public string ProductCategoryName { get => productCategoryName; set => productCategoryName = value; }
        public int ProductCategoryId { get => productCategoryId; set => productCategoryId = value; }
    }
}
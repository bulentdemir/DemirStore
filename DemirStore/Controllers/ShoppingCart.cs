using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DemirStore.Models;

namespace DemirStore.Controllers
{
    public class ShoppingCart
    {
        private tblProduct product = new tblProduct();
        private int quantity;

        public ShoppingCart()
        { }

        public ShoppingCart(tblProduct product, int quantity)
        {
            this.Product = product;
            this.Quantity = quantity;
        }

        public tblProduct Product { get => product; set => product = value; }
        public int Quantity { get => quantity; set => quantity = value; }
    }
}
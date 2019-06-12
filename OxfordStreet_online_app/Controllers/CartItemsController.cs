using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OxfordStreet_online_app.Models;

namespace OxfordStreet_online_app.Controllers
{
    public class CartItemsController : Controller
    {
        private StoreDbContext db = new StoreDbContext();

        //get all product details for cartId. --> GET
        //delete all rows for cartId. --> POST
        //delete specific product for cartId. --> POST
        //add product to cart-- if cart doesn't exists create new one. --> POST
        //edit product quantity in cartId --> POST

        public ActionResult Details(string id)
        {
            var cartItems = db.CartItems.Where(ci => ci.CartId == id);
            return View(cartItems.ToList());
        }
        public ActionResult DeleteCart(string cartId)
        {
            var cartItems = db.CartItems.Where(ci => ci.CartId == cartId);
            foreach(CartItem cartItem in cartItems)
            {
                db.CartItems.Remove(cartItem);
            }
            return View();//need to update the return address to shop
        }
        public ActionResult DeleteProduct(string cartId, int productId)
        {
            CartItem cartItem = db.CartItems.FirstOrDefault(ci => ci.CartId == cartId && ci.ProductId == productId);
            db.CartItems.Remove(cartItem);
            return View();//need to update the return address to shop
        }

        public ActionResult AddProductToCart(int productId, int quantity)
        {
            string cartId;
            if (Session["cartId"] == null)
            {
                Random rnd = new Random();
                cartId = DateTimeOffset.Now.ToUnixTimeSeconds().ToString() + Session.SessionID;
                Session.Add("cartId", cartId);
            }
            else
            {
                cartId = (string)Session["cartId"];
            }

            Product product = db.Products.Find(productId);
            CartItem ci = new CartItem
            {
                ProductId = productId,
                Product = product,
                Quantity = quantity,
                CartId = cartId,
                CreatedAt = DateTime.Now
            };
            db.CartItems.Add(ci);
            db.SaveChanges();
            return View();
        }

        public ActionResult EditQuantity(int productId, int newQuatity)
        {
            CartItem CartItem = db.CartItems.FirstOrDefault(ci => ci.CartId == (string)Session["cartId"] && ci.ProductId == productId);
            CartItem.Quantity = newQuatity;
            db.Entry(CartItem).State = EntityState.Modified;
            db.SaveChanges();
            return View();
        }
    }
}

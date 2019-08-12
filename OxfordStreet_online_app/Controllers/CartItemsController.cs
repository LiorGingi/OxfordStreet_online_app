using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
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

        public ActionResult Details()
        {

            if (Session["cartId"] == null)
            {
                return View();
            }
            else
            {
                String cartId = (String) Session["cartId"];
                var cartItems = db.CartItems.Include(p => p.Product).Where(ci => ci.CartId == cartId);
                ViewBag.Total = 0;
                return View(cartItems.ToList());
            }
        }
        public ActionResult DeleteCart()
        {
            String cartId = (String)Session["cartId"];
            var cartItems = db.CartItems.Where(ci => ci.CartId == cartId);
            foreach(CartItem cartItem in cartItems)
            {
                db.CartItems.Remove(cartItem);
            }
            db.SaveChanges();
            Session.Remove("cartId");
            return RedirectToAction("Details");
        }
        public ActionResult DeleteProduct(int productId)
        {
            String cartId = (String)Session["cartId"];
            CartItem cartItem = db.CartItems.FirstOrDefault(ci => ci.CartId == cartId && ci.ProductId == productId);
            db.CartItems.Remove(cartItem);
            db.SaveChanges();

            if (db.CartItems.FirstOrDefault(ci => ci.CartId == cartId) == null)
            {
                Session.Remove("cartId");
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddProductToCart(int productId, int quantity)
        {
            string cartId;
            if (Session["cartId"] == null)
            {
                cartId = DateTimeOffset.Now.ToUnixTimeSeconds().ToString() + Session.SessionID;
                Session.Add("cartId", cartId);
            }
            else
            {
                cartId = (string)Session["cartId"];
            }

            //TODO: need to check if product already exists in cart
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
            return RedirectToAction("Details");
        }

        public ActionResult EditQuantity(int productId, int newQuatity)
        {
            String cartId = (String)Session["cartId"];
            CartItem CartItem = db.CartItems.FirstOrDefault(ci => ci.CartId == cartId && ci.ProductId == productId);
            CartItem.Quantity = newQuatity;
            db.Entry(CartItem).State = EntityState.Modified;
            db.SaveChanges();
            return View();
        }

        public ActionResult ClickCheckout(int cartTotal)
        {
            Session.Add("cartTotal", cartTotal);
            return RedirectToAction("Checkout", "Orders");
        }

        public Object CartItemsProducts()
        {
            var queryResult = db.CartItems.Join(db.Products,
                ci => ci.ProductId, p => p.ProductId,(ci, p) => new { CartItems = ci, Product = p }).ToList();
            return queryResult;
        }
    }
}

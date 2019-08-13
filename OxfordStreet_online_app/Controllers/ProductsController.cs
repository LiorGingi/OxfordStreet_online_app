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
    public class ProductsController : Controller
    {
        private StoreDbContext db = new StoreDbContext();

        // GET: Products
        public ActionResult Index()
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                var products = db.Products.Include(p => p.Supplier);
                return View(products.ToList());
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "Name");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,Name,Price,Category,SubCategory,WeatherType,SupplierId,ImageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("CreationConfirmation");
            }

            ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "Name", product.SupplierId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "Name", product.SupplierId);
                return View(product);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,Name,Price,Category,SubCategory,WeatherType,SupplierId,ImageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "Name", product.SupplierId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Products/Category/Sunglasses
        public ActionResult Category(string category = null)
        {
            var items = db.Products.Select(p => p.Category).Distinct();
            ViewBag.prodCategory = new SelectList(items);

            return View((category == null
                ? db.Products
                : db.Products.Where(product => product.Category == category)).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Category(int? prodMaxPrice, string prodCategory = null, string prodName = null)
        {
            var dataQuery = db.Products.Where(product => (prodMaxPrice == null ? true : product.Price <= prodMaxPrice)
                                                                   && (prodCategory == "" ? true : product.Category == prodCategory)
                                                                   && (prodName == "" ? true : product.Name.Contains(prodName)));

            var items = db.Products.GroupBy(p => p.Category).Select(p => p.Key);
            ViewBag.prodCategory = new SelectList(items);

            return View(dataQuery.ToList());
        }

        public ActionResult CreationConfirmation()
        {
            var newProductId = db.Products.Max(o => o.ProductId);
            var product = db.Products.Find(newProductId);

            if (product != null)
            {
                ViewBag.Message = "Check our our new " + product.Name + " product!";
                return View(product);
            }

            return RedirectToAction("Index");
        }

        public ActionResult DetailsOfSuggestedProduct(int? id)
        {
            if (id == null)
            {
                return Json(false);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return Json(HttpNotFound());
            }
            var jsonReturn = new {
                imageUrl = product.ImageUrl,
                name=product.Name,
                price = product.Price
            };
            return Json(jsonReturn);
        }
        [HttpPost]
        public ActionResult SuggestedProduct(int? id)
        {
            if (id == null)
            {
                return TopSale();
            }
            var orders = from op in db.OrderProducts
                         where op.ProductId == id
                         select op.OrderId;
            var products = from op in db.OrderProducts
                           where orders.Any(o => o == op.OrderId && op.ProductId != id)
                           select new { productId = op.ProductId, Quantity = op.Quantity };
            var groupedProducts = from p in products
                                  group p.Quantity by p.productId into g
                                  select new { productId = g.Key, quantity = g.ToList().Sum() };
            var product = groupedProducts.FirstOrDefault(p => p.quantity == groupedProducts.Max(gp => gp.quantity));
            var a= product==null ? TopSale() : DetailsOfSuggestedProduct(product.productId);
            return a;
        }
        public ActionResult TopSale()
        {
            var groupedProducts = from p in db.OrderProducts
                                  group p.Quantity by p.ProductId into g
                                  select new { productId = g.Key, quantity = g.ToList().Sum() };
            int maxSales = groupedProducts.Max(p => p.quantity);
            var topSale = from p in groupedProducts
                          where p.quantity == maxSales
                          select p.productId;
            return DetailsOfSuggestedProduct(topSale.FirstOrDefault());
        }
    }
}

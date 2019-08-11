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
            var products = db.Products.Include(p => p.Supplier);
            return View(products.ToList());
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
            ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "Name");
            return View();
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
                                                                   && (prodCategory == null ? true : product.Category == prodCategory)
                                                                   && (prodName == null ? true : product.Name.Contains(prodName)));

            var items = db.Products.Select(p => p.Category).Distinct();
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
    }
}

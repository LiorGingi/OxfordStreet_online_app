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
    public class OrderProductsController : Controller
    {
        private StoreDbContext db = new StoreDbContext();

        // GET: OrderProducts
        public ActionResult Index()
        {
            var orderProducts = db.OrderProducts.Include(o => o.Order).Include(o => o.Product);
            return View(orderProducts.ToList());
        }

        // GET: OrderProducts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            OrderProduct orderProduct = db.OrderProducts.Find(id);
            if (orderProduct == null)
            {
                return HttpNotFound();
            }

            return View(orderProduct);
        }

        // GET: OrderProducts/Create
        public ActionResult Create()
        {
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Status");
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Category");
            return View();
        }

        // POST: OrderProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,ProductId,Quantity")]
            OrderProduct orderProduct)
        {
            if (ModelState.IsValid)
            {
                db.OrderProducts.Add(orderProduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Status", orderProduct.OrderId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Category", orderProduct.ProductId);
            return View(orderProduct);
        }

        // GET: OrderProducts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            OrderProduct orderProduct = db.OrderProducts.Find(id);
            if (orderProduct == null)
            {
                return HttpNotFound();
            }

            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Status", orderProduct.OrderId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Category", orderProduct.ProductId);
            return View(orderProduct);
        }

        // POST: OrderProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,ProductId,Quantity")]
            OrderProduct orderProduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Status", orderProduct.OrderId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Category", orderProduct.ProductId);
            return View(orderProduct);
        }

        // GET: OrderProducts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            OrderProduct orderProduct = db.OrderProducts.Find(id);
            if (orderProduct == null)
            {
                return HttpNotFound();
            }

            return View(orderProduct);
        }

        // POST: OrderProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderProduct orderProduct = db.OrderProducts.Find(id);
            db.OrderProducts.Remove(orderProduct);
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

    }
}

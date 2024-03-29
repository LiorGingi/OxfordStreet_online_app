﻿using System;
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
    public class SuppliersController : Controller
    {
        private StoreDbContext db = new StoreDbContext();

        // GET: Suppliers
        public ActionResult Index()
        {
            if (Session["isManager"] != null && (bool) Session["isManager"] == true)
            {
                return View(db.Suppliers.ToList());
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Supplier supplier = db.Suppliers.Find(id);
                if (supplier == null)
                {
                    return HttpNotFound();
                }
                return View(supplier);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SupplierId,Name,PhoneNumber,Address")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Suppliers.Add(supplier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Supplier supplier = db.Suppliers.Find(id);
                if (supplier == null)
                {
                    return HttpNotFound();
                }
                return View(supplier);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SupplierId,Name,PhoneNumber,Address")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Supplier supplier = db.Suppliers.Find(id);
                if (supplier == null)
                {
                    return HttpNotFound();
                }
                return View(supplier);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Supplier supplier = db.Suppliers.Find(id);
            db.Suppliers.Remove(supplier);
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

﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OxfordStreet_online_app.Models;

namespace OxfordStreet_online_app.Controllers
{
    public class UsersController : Controller
    {
        private StoreDbContext db = new StoreDbContext();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.Password = Decrypt(user.Password); //testing that the decryption is working
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,FirstName,LastName,Email,IsEmployee,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = Encrypt(user.Password);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,FirstName,LastName,Email,IsEmployee,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = Encrypt(user.Password);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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

        
        public string Encrypt(string password)
        {
            var data = Encoding.Unicode.GetBytes(password);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string encryptedPass)
        {
            byte[] data = Convert.FromBase64String(encryptedPass);
            byte[] decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.LocalMachine);
            return Encoding.Unicode.GetString(decrypted);
        }

        // GET: Users/Login
        public ActionResult Login()
        {
            if (Session["userId"] != null) //if there's a user logged-in already
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            User user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user != null && Decrypt(user.Password) == password)
            {
                Session.Add("userId", user.UserId);
                Session.Add("userName", user.FirstName +" "+ user.LastName);
                Session.Add("isEmployee", user.IsEmployee);
                if (user.IsEmployee)
                {
                    Employee employee = db.Employees.FirstOrDefault(e => e.UserId == user.UserId);
                    Session.Add("employeeId", employee.EmployeeId);
                    Session.Add("employeeRole", employee.Role);
                }
                return RedirectToAction("Index", "Home");
            }
            else //if the authentication failed
            {
                return RedirectToAction("Index");
            }

        }

        public ActionResult Logout()
        {
            if (Session["userId"] != null)
            {
                Session.Remove("userId");
                if ((bool)Session["isEmployee"])
                {
                    Session.Remove("employeeId");
                    Session.Remove("employeeRole");
                }
                Session.Remove("isEmployee");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
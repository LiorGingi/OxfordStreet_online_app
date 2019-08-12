using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using OxfordStreet_online_app.Models;

namespace OxfordStreet_online_app.Controllers
{
    public class EmployeesController : Controller
    {
        private StoreDbContext db = new StoreDbContext();

        // GET: Employees
        public ActionResult Index()
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                var employees = db.Employees.Include(e => e.Branch).Include(e => e.User);
                return View(employees.ToList());
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee employee = db.Employees.Include(e => e.Branch).Include(e => e.User).FirstOrDefault(e => e.EmployeeId == id);
                if (employee == null)
                {
                    return HttpNotFound();
                }
                return View(employee);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {

            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee employee = db.Employees.Include(e => e.Branch).Include(e => e.User).FirstOrDefault(e => e.EmployeeId == id);
                if (employee == null)
                {
                    return HttpNotFound();
                }
                ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "Name", employee.BranchId);
                ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", employee.UserId);
                return View(employee);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeId,UserId,MonthlySalary,BranchId,Role,IsManager")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "Name", employee.BranchId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", employee.UserId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee employee = db.Employees.Find(id);
                if (employee == null)
                {
                    return HttpNotFound();
                }
                return View(employee);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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

        //GET: Employees/Create
        public ActionResult Create()
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "Name");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        //POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string firstName, string lastName, Role role, int monthlySalary, int branchId, string manager,
            string email, string password, string passwordVerification)
        {
            if (String.IsNullOrEmpty(firstName)
                || String.IsNullOrEmpty(lastName)
                || String.IsNullOrEmpty(email)
                || String.IsNullOrEmpty(password)
                || String.IsNullOrEmpty(passwordVerification))
            {
                ViewBag.Error = "All fields are required";
                return View();
            }
            else
            {
                User checkIfExist = db.Users.FirstOrDefault(user => user.Email == email);
                if (checkIfExist == null)
                {
                    if (password == passwordVerification)
                    {
                        User newUser = new User
                        {
                            Email = email,
                            FirstName = firstName,
                            IsEmployee = true,
                            LastName = lastName,
                            Password = new UsersController().Encrypt(password)
                        };
                        db.Users.Add(newUser);
                        db.SaveChanges();

                        User addedUser = db.Users.FirstOrDefault(user => user.Email == email);
                        Boolean isManager;

                        if (manager == "Manager")
                        {
                            isManager = true;
                        }
                        else
                        {
                            isManager = false;
                        }

                        Employee newEmployee = new Employee()
                        {
                            BranchId = branchId,
                            UserId = addedUser.UserId,
                            IsManager = isManager,
                            MonthlySalary = monthlySalary,
                            Role = role
                        };
                        db.Employees.Add(newEmployee);
                        db.SaveChanges();

                        //need to create a new employee here

                        return RedirectToAction("Index", "Home"); //where should I return on success?
                    }
                    else
                    {
                        ViewBag.Error = "Password verification failed, please try again";
                        return View();
                    }

                }
                else
                {
                    ViewBag.Error = "Email already exists";
                    return View();
                }
            }
        }
    }
}

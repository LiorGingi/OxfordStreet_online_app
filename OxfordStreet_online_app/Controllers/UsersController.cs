using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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
            if (Session["userId"] != null)
            {
                return View(db.Users.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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
            user.Password = Decrypt(user.Password);
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
            User user = db.Users.FirstOrDefault(u => u.Email == email && u.Password != null);
            if (user != null && password != null && Decrypt(user.Password) == password)
            {
                Session.Add("userId", user.UserId);
                Session.Add("userName", user.FirstName + " " + user.LastName);
                Session.Add("isEmployee", user.IsEmployee);
                if (user.IsEmployee)
                {
                    Employee employee = db.Employees.FirstOrDefault(e => e.UserId == user.UserId);
                    if (employee != null)
                    {
                        Session.Add("employeeId", employee.EmployeeId);
                        Session.Add("isManager", employee.IsManager);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            else //if the authentication failed
            {
                ViewBag.Error = "Email or password are incorrect";
                return View();
            }

        }

        //GET
        public ActionResult Logout()
        {
            if (Session["userId"] != null)
            {
                Session.Remove("userId");
                if ((bool)Session["isEmployee"])
                {
                    Session.Remove("employeeId");
                    Session.Remove("isManager");
                }
                Session.Remove("isEmployee");
            }
            return RedirectToAction("Index", "Home");
        }

        //GET: Users/Signup
        public ActionResult Signup()
        {
            return View();
        }

        //POST: Users/Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(string firstName, string lastName, string email, string password, string passwordVerification,
            string phone, string address)
        {
            if (String.IsNullOrEmpty(firstName)
                || String.IsNullOrEmpty(lastName)
                || String.IsNullOrEmpty(email)
                || String.IsNullOrEmpty(phone)
                || String.IsNullOrEmpty(address)
                || String.IsNullOrEmpty(password)
                || String.IsNullOrEmpty(passwordVerification))
            {
                ViewBag.Error = "All fields are required";
                return View();
            }
            else
            {
                User checkIfExist = db.Users.FirstOrDefault(user => user.Email == email && user.Password != null);
                if (checkIfExist == null)
                {
                    if (password == passwordVerification)
                    {
                        User checkNotRegistered = db.Users.FirstOrDefault(user => user.Email == email && user.Password == null);
                        if (checkNotRegistered == null)
                        {
                            User newUser = new User
                            {
                                Email = email,
                                FirstName = firstName,
                                IsEmployee = false,
                                LastName = lastName,
                                PhoneNumber = phone,
                                Address = address,
                                Password = Encrypt(password)
                            };
                            db.Users.Add(newUser);
                            db.SaveChanges();
                        }
                        else
                        {
                            checkNotRegistered.FirstName = firstName;
                            checkNotRegistered.LastName = lastName;
                            checkNotRegistered.IsEmployee = false;
                            checkNotRegistered.Email = email;
                            checkNotRegistered.PhoneNumber = phone;
                            checkNotRegistered.Address = address;
                            checkNotRegistered.Password = Encrypt(password);
                            db.Entry(checkNotRegistered).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        Login(email, password);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Error = "Password verification failed, please try again";
                        return View();
                    }

                }
                else
                {
                    ViewBag.Error = "User already exists";
                    return View();
                }
            }
        }
    }
}

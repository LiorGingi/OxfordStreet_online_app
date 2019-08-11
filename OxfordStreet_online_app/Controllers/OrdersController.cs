using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using OxfordStreet_online_app.Models;

namespace OxfordStreet_online_app.Controllers
{
    public class OrdersController : Controller
    {
        private StoreDbContext db = new StoreDbContext();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Branch).Include(o => o.User);
            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = db.Orders.Include(o => o.User).Include(o => o.OrderProducts).Include(o => o.Branch)
                .FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "Name");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,UserId,BranchId,Date,TotalCost,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "Name", order.BranchId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "Name", order.BranchId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,UserId,BranchId,Date,TotalCost,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "Name", order.BranchId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = db.Orders.Include(o => o.Branch).FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Include(o => o.OrderProducts).FirstOrDefault(o => o.OrderId == id);
            foreach (var item in order.OrderProducts) //if deleting an order, delete also rows on OrderProducts
            {
                db.OrderProducts.Remove(item);
            }
            db.Orders.Remove(order);
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

        public ActionResult Search(int? userId = null, int minCost = 0, int? maxCost = null, OrderStatus? status = null)
        {
            var orders = db.Orders.Select(i => i).Where(order =>
                order.TotalCost >= minCost &&
                (userId == null ? true : order.UserId == userId) &&
                (maxCost == null ? true : order.TotalCost <= maxCost) &&
                (status == null ? true : order.Status == status));
            return View(orders.ToList());
        }

        public ActionResult CountOrdersPerUser()
        {
            var result = from o in db.Orders.GroupBy(o => o.UserId)
                         select new { count = o.Count() };

            return View(result.ToList());

            //select count(orderId) from orders
            //group by userId

        }

        //Need to add branch choose
        public ActionResult Checkout()
        {
            if (Session["cartId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (Session["userId"] != null)
                {
                    var userId = Session["userId"];
                    User user = db.Users.Find(userId);
                    if (user != null)
                    {
                        return View(user);
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string firstName, string lastName, string address, string phone, string email)
        {
            if (String.IsNullOrEmpty(firstName)
                || String.IsNullOrEmpty(lastName)
                || String.IsNullOrEmpty(email)
                || String.IsNullOrEmpty(phone)
                || String.IsNullOrEmpty(address))
            {
                ViewBag.Error = "All fields are required";
                return View();
            }
            else
            {
                int userId = -1;
                if (Session["userId"] == null)
                {
                    if (db.Users.FirstOrDefault(u => u.Email == email && u.Password != null) != null) //if there's a registered user
                    {
                        ViewBag.Error = "Email address already in use, please login to your account or use different address.";
                        return View();
                    }
                    else
                    {
                        User notRegisteredUser = db.Users.FirstOrDefault(u => u.Email == email && u.Password == null);
                        if (notRegisteredUser != null)
                        {
                            notRegisteredUser.FirstName = firstName;
                            notRegisteredUser.LastName = lastName;
                            notRegisteredUser.PhoneNumber = phone;
                            notRegisteredUser.Email = email;
                            db.Entry(notRegisteredUser).State = EntityState.Modified;
                            db.SaveChanges();
                            userId = notRegisteredUser.UserId;
                        }
                        else
                        {
                            User newUser = new User
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                Email = email,
                                Address = address,
                                PhoneNumber = phone,
                                IsEmployee = false,
                                Password = null
                            };
                            db.Users.Add(newUser);
                            db.SaveChanges();
                            userId = db.Users.Max(u => u.UserId);
                        }
                    }
                }

                if (userId == -1)
                {
                    userId = (int)Session["userId"];
                }
                String cart = (String)Session["cartId"];

                Order order = new Order
                {
                    UserId = userId,
                    Date = DateTime.Now,
                    TotalCost = (int)Session["cartTotal"],
                    Status = OrderStatus.New,
                    BranchId = 1
                };
                db.Orders.Add(order);
                db.SaveChanges();
                int orderId = db.Orders.Max(o => o.OrderId);
                IEnumerable<CartItem> items = db.CartItems.Where(ci => ci.CartId == cart).ToList();
                foreach (var cartItem in items)
                {
                    OrderProduct orderProduct = new OrderProduct
                    {
                        OrderId = orderId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity
                    };
                    db.OrderProducts.Add(orderProduct);
                }

                db.SaveChanges();
                Session.Remove("cartId");
                Session.Remove("cartTotal");
                return RedirectToAction("ThankYouPage", new { orderId = orderId });
            }
        }

        public ActionResult ThankYouPage(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        public ActionResult OrderDetails()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderDetails(string email, int orderId)
        {
            Order order = db.Orders.Include(o => o.User).Include(o => o.OrderProducts).FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                if (order.User.Email == email)
                {
                    ViewBag.OrderId = orderId;
                    ViewBag.OrderStatus = order.Status;
                    return View(order);
                }
                else
                {
                    ViewBag.Error = "Email address or order ID are incorrect.";
                    return View();
                }
            }
            else
            {
                ViewBag.Error = "Email address or order ID are incorrect.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int orderId)
        {
            Order order = db.Orders.Include(o => o.User).FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                if (order.Status != OrderStatus.Cancelled)
                {
                    order.Status = OrderStatus.Cancelled;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Error = "Order cancelled successfully.";
                    return View();
                }
                else
                {
                    ViewBag.Error = "Order already cancelled.";
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OxfordStreet_online_app.Models;
using OxfordStreet_online_app.ViewModels;

namespace OxfordStreet_online_app.Controllers
{
    public class AdminController : Controller
    {

        private StoreDbContext db = new StoreDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult MonthlyGrossing()
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                List<MonthlyGrossing> mg = new List<MonthlyGrossing>();
                var results = db.Orders.Where(o => o.Status != OrderStatus.Cancelled)
                    .GroupBy(o => o.Date.Month)
                    .Select(o => new
                    {
                        Month = o.Key,
                        Total = o.Sum(x => x.TotalCost)
                    }).ToList();

                foreach (var item in results)
                {
                    mg.Add(new MonthlyGrossing()
                    {
                        Month = item.Month,
                        Total = item.Total
                    });
                }
                return View(mg);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SalesPerProduct()
        {
            if (Session["isManager"] != null && (bool)Session["isManager"] == true)
            {
                List<SalesPerProduct> spp = new List<SalesPerProduct>();
                var results = db.OrderProducts
                    .GroupBy(o => o.ProductId)
                    .Select(o => new
                    {
                        Id = o.Key,
                        Sales = o.Sum(x => x.Quantity)
                    }).ToList();

                foreach (var item in results)
                {
                    spp.Add(new SalesPerProduct()
                    {
                        ProductId = item.Id,
                        CountSales = item.Sales
                    });
                }
                return View(spp);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ApiTest()
        {
            var products = db.Products.Include(p => p.Supplier);
            ViewBag.CurrentTemp = -1;
            return View(products.ToList());
        }

    }
}
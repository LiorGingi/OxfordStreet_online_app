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
            return View();
        }

        [Route("admin/products")]
        public ActionResult GetAllProducts()
        {
            return new ProductsController().Index();
        }

        [Route("admin/users")]
        public ActionResult GetAllUsers()
        {
            return new UsersController().Index();
        }

        public ActionResult MonthlyGrossing()
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

        public ActionResult SalesPerProduct()
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

        public ActionResult ApiTest()
        {
            var products = db.Products.Include(p => p.Supplier);
            ViewBag.CurrentTemp = -1;
            return View(products.ToList());
        }

    }
}
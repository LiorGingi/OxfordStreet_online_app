using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OxfordStreet_online_app.Models;

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
    }
}
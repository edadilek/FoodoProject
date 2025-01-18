using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using foodoProject.Models;

namespace foodo.Controllers
{
    public class ProductsController : Controller
    {
        FoodoProjectDbEntities fd = new FoodoProjectDbEntities();

        // GET: Product
        public ActionResult Index(int restaurantID)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "LogIn"); // Giriş yapmamışsa giriş sayfasına yönlendir
            }
            var products = fd.Products.Where(p => p.RestaurantID == restaurantID).ToList();
            return View(products);
        }

    }

}

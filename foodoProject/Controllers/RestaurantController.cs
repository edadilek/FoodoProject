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
    public class RestaurantController : Controller
    {
        FoodoProjectDbEntities fd = new FoodoProjectDbEntities();

        // GET: Restaurant
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "LogIn"); // Giriş yapmamışsa giriş sayfasına yönlendir
            }
            return View(fd.Restaurants.ToList());
        }

        [Authorize(Roles = "A")]
        [HttpGet]
        public ActionResult Ekle()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "LogIn"); // Giriş yapmamışsa giriş sayfasına yönlendir
            }
            return View();
        }
        [HttpPost]
        public ActionResult Ekle(Restaurant r)
        {
            fd.Restaurants.Add(r);
            fd.SaveChanges();
            return RedirectToAction("Index");
        }






    }


}


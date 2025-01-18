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
    public class ProductController : Controller
    {
        FoodoProjectDbEntities fd = new FoodoProjectDbEntities();

        // GET: Product/Ekle
        [Authorize(Roles = "R")]
        [HttpGet]
        public ActionResult Ekle()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "LogIn"); // Giriş yapmamışsa giriş sayfasına yönlendir
            }
            ViewBag.Restaurants = new SelectList(fd.Restaurants.ToList(), "RestaurantID", "ResName");
            ViewBag.Categories = new SelectList(fd.Categories.ToList(), "CategoryID", "CName");
            return View();
        }

        // POST: Product/Ekle
        [HttpPost]
        public ActionResult Ekle(Product p)
        {
            if (ModelState.IsValid)
            {
                fd.Products.Add(p);
                fd.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Restaurants = new SelectList(fd.Restaurants.ToList(), "RestaurantID", "ResName", p.RestaurantID);
            ViewBag.Categories = new SelectList(fd.Categories.ToList(), "CategoryID", "CName", p.CategoryID);
            return View(p);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using foodoProject.Models;

namespace foodoProject.Controllers
{
    public class HomeController : Controller
    {
        FoodoProjectDbEntities fd = new FoodoProjectDbEntities();
        // GET: Home
        public ActionResult Index()
        {
            if (Session["ShowWelcomeMessage"] != null && (bool)Session["ShowWelcomeMessage"] == true)
            {
                ViewBag.ShowWelcomeMessage = true;
                Session["ShowWelcomeMessage"] = false; // Bayrağı sıfırlayın
            }
            else
            {
                ViewBag.ShowWelcomeMessage = false;
            }

            return View();
        }
    }
}
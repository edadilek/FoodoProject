using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using foodoProject.Models;
using foodoProject.ViewModels;
using System.Data.Entity;
using System.IO;
using System.Web.Helpers;
using System.Web;


namespace foodoProject.Controllers
{
    public class AdminController : Controller
    {
        // GET: Restaurant/Manage
        FoodoProjectDbEntities db = new FoodoProjectDbEntities();
        public ActionResult Manage()
        {
            var restaurants = db.Restaurants.ToList();
            return View(restaurants);
        }

        // POST: Restaurant/Add
        [HttpPost]
        public ActionResult Add(Restaurant model, HttpPostedFileBase ResImage)
        {
            if (ModelState.IsValid)
            {
                if (ResImage.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ResImage.FileName);
                    var path = Path.Combine("C:\\Users\\lenovo\\source\\repos\\foodoProject\\foodoProject\\Content\\Images", fileName);
                    ResImage.SaveAs(path);
                    model.ResImage = "/Content/Images/" + fileName; // Dosya yolunu modelde sakla
                }

                model.isActive = true;
                db.Restaurants.Add(model);
                db.SaveChanges();
            }
            return RedirectToAction("Manage");
        }

        // POST: Restaurant/Edit
        [HttpPost]
        public ActionResult Edit(Restaurant model, HttpPostedFileBase ResImage)
        {
            var restaurant = db.Restaurants.Find(model.RestaurantID);
            if (restaurant != null)
            {
                restaurant.ResName = model.ResName;
                restaurant.ResDescription = model.ResDescription;

                if (ResImage != null && ResImage.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ResImage.FileName);
                    var path = Path.Combine("C:\\Users\\lenovo\\source\\repos\\foodoProject\\foodoProject\\Content\\Images", fileName);
                    ResImage.SaveAs(path);
                    restaurant.ResImage = "/Content/Images/" + fileName; // Dosya yolunu güncelle
                }

                db.SaveChanges();
            }
            return RedirectToAction("Manage");
        }

        // POST: Restaurant/Delete
        [HttpPost]
        public ActionResult Delete(int RestaurantID)
        {
            var restaurant = db.Restaurants.Find(RestaurantID);
            if (restaurant != null)
            {
                restaurant.isActive=false;
                db.SaveChanges();
            }
            return RedirectToAction("Manage");
        }

        public ActionResult ManageUser()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        // POST: User/Add
        [HttpPost]
        public ActionResult AddUser(User model)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(model);
                db.SaveChanges();
                return RedirectToAction("ManageUser");
            }
            return View(model);
        }

        // POST: User/Edit
        [HttpPost]
        public ActionResult EditUser(User model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.UserID);
                if (user != null)
                {
                    user.UserName = model.UserName;
                    user.UName = model.UName;
                    user.Surname = model.Surname;
                    user.Email = model.Email;
                    user.Roles = model.Roles;
                    db.SaveChanges();
                }
                return RedirectToAction("ManageUser");
            }
            return View(model);
        }

        // POST: User/Delete
        [HttpPost]
        public ActionResult DeleteUser(int userID)
        {
            var user = db.Users.Find(userID);
            if (user != null)
            {
                user.isActive = false;
                db.SaveChanges();
            }
            return RedirectToAction("ManageUser");
        }

        // POST: User/ChangeRole
        [HttpPost]
        public ActionResult ChangeRole(int userID, string newRole)
        {
            var user = db.Users.Find(userID);
            if (user != null)
            {
                user.Roles = newRole;
                db.SaveChanges();
            }
            return RedirectToAction("ManageUser");
        }

        // GET: Order/Manage
        public ActionResult ManageOrder(int? userId)
        {
            var orders = db.Orders
                       .Include(o => o.OrderItems)
                       .Include(o => o.User)
                       .Include(o => o.Restaurant)
                       .AsQueryable();

            if (userId.HasValue)
            {
                orders = orders.Where(o => o.UserID == userId.Value);
            }

            return View(orders.ToList());
        }

        // POST: Order/Cancel
        [HttpPost]
        public ActionResult Cancel(int orderID)
        {
            var order = db.Orders.Find(orderID);
            if (order != null)
            {
                order.OStatus = "Cancelled";
                db.SaveChanges();
            }
            return RedirectToAction("ManageOrder");
        }

        // GET: Order/Details
        public ActionResult Details(int orderID)
        {
            var order = db.Orders
                      .Include(o => o.OrderItems)
                      .Include(o => o.User)
                      .Include(o => o.Restaurant)
                      .FirstOrDefault(o => o.OrderID == orderID);

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: User/ChangeValid
        [HttpPost]
        public ActionResult ChangeValid(int userID, bool newValid)
        {
            var user = db.Users.Find(userID);
            if (user != null)
            {
                user.isActive = newValid;
                db.SaveChanges();
            }
            return RedirectToAction("ManageUser");
        }
    }

}

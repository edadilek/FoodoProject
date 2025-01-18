using foodoProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace foodoProject.Controllers
{
    public class ResPanelController : Controller
    {
        FoodoProjectDbEntities db = new FoodoProjectDbEntities();

        // GET: Restaurant/ManageProducts
        public ActionResult ManageProducts()
        {
            // Kullanıcının giriş yaptığı restoranın ID'sini al
            int restaurantID = Convert.ToInt32(Session["RestaurantID"]);

            var products = db.Products
                              .Where(p => p.RestaurantID == restaurantID)
                              .Include(p => p.Category)
                              .ToList();

            return View(products);
        }

        // GET: Restaurant/AddProduct
        public ActionResult AddProduct()
        {
            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CName");
            return View();
        }

        // POST: Restaurant/AddProduct
        [HttpPost]

        public ActionResult AddProduct(Product product, HttpPostedFileBase PImage)
        {
            if (ModelState.IsValid)
            {
                if (PImage.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(PImage.FileName);
                    var path = Path.Combine("C:\\Users\\lenovo\\source\\repos\\foodoProject\\foodoProject\\Content\\Images", fileName);
                    PImage.SaveAs(path);
                    product.PImage = "/Content/Images/" + fileName; // Dosya yolunu modelde sakla
                }
                product.RestaurantID = GetCurrentRestaurantID(); // Kullanıcının restoran ID'sini ayarla
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("ManageProducts");
            }

            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CName", product.CategoryID);
            return View(product);
        }

        // GET: Restaurant/EditProduct/5
        public ActionResult EditProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product == null || product.RestaurantID != GetCurrentRestaurantID())
            {
                return HttpNotFound();
            }
            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CName", product.CategoryID);
            return View(product);
        }

        // POST: Restaurant/EditProduct/5
        [HttpPost]
        public ActionResult EditProduct(Product product, HttpPostedFileBase PImage)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = db.Products.Find(product.ProductID);
                if (existingProduct != null && existingProduct.RestaurantID == GetCurrentRestaurantID())
                {
                    if (PImage.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(PImage.FileName);
                        var path = Path.Combine("C:\\Users\\lenovo\\source\\repos\\foodoProject\\foodoProject\\Content\\Images", fileName);
                        PImage.SaveAs(path);
                        product.PImage = "/Content/Images/" + fileName; // Dosya yolunu modelde sakla
                    }
                    existingProduct.PName = product.PName;
                    existingProduct.PDescription = product.PDescription;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryID = product.CategoryID;
                    existingProduct.PImage = product.PImage;

                    db.SaveChanges();
                    return RedirectToAction("ManageProducts");
                }

                return HttpNotFound(); // Ürün bulunamadı veya kullanıcı bu ürünü düzenlemeye yetkili değil
            }

            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CName", product.CategoryID);
            return View(product);
        }

        // POST: Restaurant/DeleteProduct/5
        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product == null || product.RestaurantID != GetCurrentRestaurantID())
            {
                return HttpNotFound();
            }

            product.isActive = false;
            db.SaveChanges();
            return RedirectToAction("ManageProducts");
        }


        // GET: Restaurant/ManageCategories
        public ActionResult ManageCategories()
        {
            var categories = db.Categories.Where(c => c.isActive == true).ToList();
            return View(categories);
        }

        // GET: Restaurant/AddCategory
        public ActionResult AddCategory()
        {
            return View();
        }

        // POST: Restaurant/AddCategory
        [HttpPost]
        public ActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                category.isActive = true;
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("ManageCategories");
            }
            return View(category);
        }

        // POST: Restaurant/DeleteCategory/5
        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            category.isActive = false;
            db.SaveChanges();
            return RedirectToAction("ManageCategories");
        }

        // Kullanıcının giriş yaptığı restoranın ID'sini al
        private int GetCurrentRestaurantID()
        {
            return Convert.ToInt32(Session["RestaurantID"]);
        }

        public ActionResult ManageOrders()
        {
            int restaurantID = GetCurrentRestaurantID(); // Giriş yapan restoranın ID'sini al
            var orders = db.Orders
                           .Where(o => o.RestaurantID == restaurantID)
                           .OrderBy(o => o.ODate) // Siparişleri tarihe göre sıralar
                           .Include(o => o.User)
                           .ToList();

            return View(orders);
        }

        // POST: Restaurant/UpdateOrderStatus
        [HttpPost]
        public ActionResult UpdateOrderStatus(int orderId, string action)
        {
            var order = db.Orders.Find(orderId);
            if (order != null && order.RestaurantID == GetCurrentRestaurantID())
            {
                if (action == "accept")
                {
                    order.OStatus = "preparing";
                }
                else if (action == "reject")
                {
                    order.OStatus = "res_cancelled";
                }

                db.SaveChanges();
            }

            return RedirectToAction("ManageOrders");
        }

    }

}
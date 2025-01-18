using foodoProject.Models;
using foodoProject.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace foodoProject.Controllers
{
    public class OrderController : Controller
    {
        FoodoProjectDbEntities db = new FoodoProjectDbEntities();

        // Sipariş formunu göster
        public ActionResult Checkout()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "LogIn"); // Giriş yapmamışsa giriş sayfasına yönlendir
            }

            int userId = (int)Session["UserID"];
            var cart = db.Carts.FirstOrDefault(c => c.UserID == userId && c.isActive == true);

            if (cart == null)
            {
                return RedirectToAction("Index", "Cart"); // Aktif sepet yoksa sepete yönlendir
            }

            var cartProducts = db.CartsProducts
                .Where(cp => cp.CartID == cart.CartID)
                .Select(cp => new CartViewModel
                {
                    PName = cp.Product.PName,
                    Price = cp.Product.Price ?? 0, 
                    Quantity = cp.Quantity
                })
                .ToList();

            // TotalPrice'ı hesapla
            var totalPrice = cartProducts.Sum(p => p.Total);

            ViewBag.TotalPrice = totalPrice;
            ViewBag.Addresses = db.Addresses.Where(a => a.UserID == userId && a.isActive == true).ToList();
            ViewBag.Payments = db.Payments.Where(p => p.UserID == userId && p.isActive == true).ToList();
            ViewBag.CartProducts = cartProducts;

            return View();
        }

        [HttpPost]
        public ActionResult PlaceOrder(int addressId, int paymentId)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "LogIn"); // Giriş yapmamışsa giriş sayfasına yönlendir
            }

            int userId = (int)Session["UserID"];
            var cart = db.Carts.FirstOrDefault(c => c.UserID == userId && c.isActive == true);

            if (cart == null)
            {
                return RedirectToAction("Index", "Cart"); // Aktif sepet yoksa sepete yönlendir
            }

            var cartProducts = db.CartsProducts
                .Where(cp => cp.CartID == cart.CartID)
                .ToList();

            // Yeni bir sipariş oluştur
            var order = new Order
            {
                OStatus = "Order Received",
                ODate = DateTime.Now,
                PaymentID = paymentId,
                UserID = userId,
                RestaurantID = cartProducts.FirstOrDefault()?.Product.RestaurantID // Tüm ürünlerin aynı restorandan olduğunu varsayar
            };

            db.Orders.Add(order);
            db.SaveChanges();

            // Siparişe ürünleri ekle
            foreach (var cartProduct in cartProducts)
            {
                var orderItem = new OrderItem
                {
                    OrderID = order.OrderID,
                    ProductID = cartProduct.ProductID,
                    Quantity = cartProduct.Quantity
                };

                db.OrderItems.Add(orderItem);
            }

            db.SaveChanges();

            // Sepeti devre dışı bırak
            cart.isActive = false;
            db.SaveChanges();

            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderID });
        }

        public ActionResult OrderConfirmation(int orderId)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderID == orderId);

            if (order == null)
            {
                return HttpNotFound(); // Sipariş bulunamazsa 404 döner
            }

            ViewBag.Order = order;
            ViewBag.OrderItems = db.OrderItems
                .Where(oi => oi.OrderID == orderId)
                .Select(oi => new CartViewModel
                {
                    PName = oi.Product.PName,
                    Price = oi.Product.Price ?? 0,
                    Quantity = oi.Quantity ?? 0
                }).ToList();

            return View();
        }
    }
}

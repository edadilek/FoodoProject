using foodoProject.Models;
using foodoProject.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace foodoProject.Controllers
{
    public class CartController : Controller
    {
        FoodoProjectDbEntities db = new FoodoProjectDbEntities();

        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "LogIn"); // Giriş yapmamışsa giriş sayfasına yönlendir
            }

            int userId = (int)Session["UserID"];
            var cart = db.Carts.FirstOrDefault(c => c.UserID == userId && c.isActive == true);

            if (cart == null)
            {
                ViewBag.TotalPrice = 0;
                return View(Enumerable.Empty<CartViewModel>()); // Boş bir ViewModel listesi döner
            }

            var cartProducts = db.CartsProducts
                .Where(cp => cp.CartID == cart.CartID)
                .Select(cp => new CartViewModel
                {
                    CartProductId = cp.CartsProductsID,
                    RestaurantName = cp.Product.Restaurant.ResName,
                    PName = cp.Product.PName,
                    Price = cp.Product.Price ?? 0,
                    Quantity = cp.Quantity
                })
                .ToList();

            // TotalPrice'ı hesapla
            var totalPrice = cartProducts.Sum(p => p.Total);
            ViewBag.TotalPrice = totalPrice;

            ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(cartProducts);
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "LogIn"); // Giriş yapmamışsa giriş sayfasına yönlendir
            }

            int userId = (int)Session["UserID"];
            var cart = db.Carts.FirstOrDefault(c => c.UserID == userId && c.isActive == true);

            var product = db.Products.Find(productId);
            if (product == null)
            {
                // Ürün bulunamadı
                return HttpNotFound();
            }

            if (cart == null)
            {
                // Yeni bir sepet oluşturuluyor
                cart = new Cart
                {
                    UserID = userId,
                    TotalPrice = 0,
                    isActive = true
                };
                db.Carts.Add(cart);
                db.SaveChanges();
            }
            else
            {
                // Mevcut sepetin restoranını kontrol et
                var cartRestaurantId = db.CartsProducts
                    .Where(cp => cp.CartID == cart.CartID)
                    .Select(cp => cp.Product.RestaurantID)
                    .FirstOrDefault();

                if (cartRestaurantId != product.RestaurantID)
                {
                    // Kullanıcı farklı bir restoran ürününü sepete eklemeye çalışıyor
                    TempData["ErrorMessage"] = "You cannot add products from different restaurants to the same cart.";
                    return RedirectToAction("Index", "Restaurant");
                }
            }

            var existingCartProduct = db.CartsProducts
                .FirstOrDefault(cp => cp.CartID == cart.CartID && cp.ProductID == productId);

            if (existingCartProduct != null)
            {
                // Ürün zaten sepetin içinde var, miktarı güncelle
                existingCartProduct.Quantity += quantity;
            }
            else
            {
                // Ürün sepetin içinde yok, yeni bir sepet ürünü ekle
                var cartProduct = new CartsProduct
                {
                    CartID = cart.CartID,
                    ProductID = productId,
                    Quantity = quantity
                };
                db.CartsProducts.Add(cartProduct);
            }

            // Toplam fiyatı güncelle
            var totalPrice = db.CartsProducts
                .Where(cp => cp.CartID == cart.CartID)
                .Sum(cp => cp.Quantity * cp.Product.Price);

            cart.TotalPrice = totalPrice;
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult IncreaseQuantity(int cartProductId)
        {
            var cartProduct = db.CartsProducts.Find(cartProductId);
            if (cartProduct != null)
            {
                cartProduct.Quantity += 1;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DecreaseQuantity(int cartProductId)
        {
            var cartProduct = db.CartsProducts.Find(cartProductId);
            if (cartProduct != null && cartProduct.Quantity > 1)
            {
                cartProduct.Quantity -= 1;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int cartProductId)
        {
            var cartProduct = db.CartsProducts.Find(cartProductId);
            if (cartProduct != null)
            {
                var cartId = cartProduct.CartID;
                db.CartsProducts.Remove(cartProduct);
                db.SaveChanges();

                // Sepetin içinde başka ürün kalıp kalmadığını kontrol et
                var cart = db.Carts.Find(cartId);
                if (!db.CartsProducts.Any(cp => cp.CartID == cartId))
                {
                    // Eğer sepet boşsa isActive'i false yap
                    cart.isActive = false;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

    }
}

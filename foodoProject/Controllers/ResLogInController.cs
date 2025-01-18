using System;
using System.Linq;
using System.Web.Mvc;
using foodoProject.Models;

namespace foodoProject.Controllers
{
    public class ResLogInController : Controller
    {
        FoodoProjectDbEntities db = new FoodoProjectDbEntities();

        // GET: LogIn
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResRegister(int restaurantID, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "All fields are required.");
            }
            if (ModelState.IsValid)
            {
                var newResLogin = new ResLogIn
                {
                    RestaurantID = restaurantID,
                    UserName = username,
                    UPassword = password,
                };

                db.ResLogIns.Add(newResLogin);
                db.SaveChanges();

                // Kullanıcı başarıyla kaydedildikten sonra oturum bilgilerini ayarla
                Session["UserName"] = newResLogin.UserName; // Kullanıcı adını oturumda sakla
                Session["RestaurantID"] = newResLogin.RestaurantID; // Kullanıcı ID'sini oturumda sakla

                Session["ShowWelcomeMessage"] = true;
                return RedirectToAction("ManageProducts", "ResPanel");
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult ResLogin(string username, string password)
        {
            var res = db.ResLogIns.FirstOrDefault(u => u.UserName == username && u.UPassword == password);
            if (res != null)
            {
                // Oturum yönetimi
                Session["UserName"] = res.UserName; // Kullanıcı adını oturumda sakla
                Session["RestaurantID"] = db.Restaurants.FirstOrDefault(u => u.RestaurantID == res.RestaurantID)?.RestaurantID;



                Session["ShowWelcomeMessage"] = true;

                return RedirectToAction("ManageProducts", "ResPanel");
            }



            ModelState.AddModelError("", "Invalid username or password.");
            return View("Index");

        }

        public ActionResult ResLogOut()
        {
            // Kullanıcının oturumunu sonlandır
            Session.Clear(); // Tüm session verilerini temizler
            Session.Abandon(); // Session'ı sonlandırır
            return RedirectToAction("Index", "ResLogIn"); // Giriş sayfasına yönlendir
        }
    }
}

using System;
using System.Linq;
using System.Web.Mvc;
using foodoProject.Models;

namespace foodoProject.Controllers
{
    public class LogInController : Controller
    {
        FoodoProjectDbEntities db = new FoodoProjectDbEntities();

        // GET: LogIn
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string Name, string Surname, string Email, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Surname) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "All fields are required.");
            }
            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    UserName = username,
                    UName = Name,
                    Surname = Surname,
                    Email = Email,
                    RegDate = DateTime.Now,
                    Roles = "U",
                    isActive = true
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                var newLogin = new LogIn
                {
                    UserName = username,
                    UPassword = password,
                    isActive = true,
                };

                db.LogIns.Add(newLogin);
                db.SaveChanges();

                // Kullanıcı başarıyla kaydedildikten sonra oturum bilgilerini ayarla
                Session["UserName"] = newUser.UserName; // Kullanıcı adını oturumda sakla
                Session["UserID"] = newUser.UserID; // Kullanıcı ID'sini oturumda sakla
                Session["UserRole"] = newUser.Roles; // Kullanıcı rolünü oturumda sakla
                Session["UName"] = newUser.UName; // Kullanıcı adını oturumda sakla

                Session["ShowWelcomeMessage"] = true;
                return RedirectToAction("Index", "Home");
            }

            return View("Index");
        }


        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var userLogin = db.LogIns.FirstOrDefault(u => u.UserName == username && u.UPassword == password);
            if (userLogin != null)
            {
                var user = db.Users.FirstOrDefault(u => u.UserName == username);

                if (user != null && (user.isActive!=false))
                {
                    // Oturum yönetimi
                    Session["UserName"] = user.UserName; // Kullanıcı adını oturumda sakla
                    Session["UserID"] = user.UserID; // Kullanıcı ID'sini oturumda sakla
                    Session["UserRole"] = user.Roles; // Kullanıcı rolünü oturumda sakla
                    Session["UName"] = user.UName; // Kullanıcı adını oturumda sakla

                    Session["ShowWelcomeMessage"] = true;

                    if (user.Roles == "A")
                    {
                        return RedirectToAction("Manage", "Admin");
                    }
                    else if (user.Roles == "R")
                    {
                        return RedirectToAction("ManageProducts", "ResPanel");
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Your account is inactive. Please contact support.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View("Index");
        }



        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            // Kullanıcının oturumunu sonlandır
            Session.Clear(); // Tüm session verilerini temizler
            Session.Abandon(); // Session'ı sonlandırır
            return RedirectToAction("Index", "LogIn"); // Giriş sayfasına yönlendir
        }
    }
}

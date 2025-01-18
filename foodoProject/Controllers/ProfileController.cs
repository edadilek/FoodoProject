using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using foodoProject.Models;
using foodoProject.ViewModels;


namespace foodoProject.Controllers
{
    public class ProfileController : Controller
    {
        FoodoProjectDbEntities db = new FoodoProjectDbEntities();

        // GET: Profile
        public ActionResult Index()
        {
            int userId = (int)Session["UserID"];
            var user = db.Users.Find(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "LogIn");
            }

            var orders = db.Orders
                .Where(o => o.UserID == userId)
                .Select(o => new OrderViewModel
                {
                    OrderID = o.OrderID,
                    RestaurantName = db.Restaurants
                                .Where(r => r.RestaurantID == o.RestaurantID)
                                .Select(r => r.ResName)
                                .FirstOrDefault(),
                    ODate = o.ODate,  
                    OStatus = o.OStatus,
                    TotalPrice = o.OrderItems.Sum(oi => oi.Quantity * oi.Product.Price ?? 0)
                })
                .ToList();

            var viewModel = new ProfileViewModel
            {
                User = user,
                Orders = orders
            };

            return View(viewModel);
        }
        public ActionResult UpdateUserInfo()
        {
            int userId = (int)Session["UserID"];
            var user = db.Users.Find(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "LogIn");
            }

            return View(user); // User modelini view'a gönder
        }

        [HttpPost]
        public ActionResult UpdateUserInfo(User model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.UserID);
                if (user != null)
                {
                    user.UName = model.UName;
                    user.Surname = model.Surname;
                    user.Email = model.Email;

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index"); // Profil sayfasına yönlendir
            }

            return View(model); // Eğer model geçerli değilse, view'a geri dön
        }
        public ActionResult ManageAddresses()
        {
            var model = db.Addresses.ToList(); // Fetch addresses from the database
            ViewBag.Cities = new SelectList(db.Cities, "CityID", "CityName"); // Populate cities for dropdown
            return View(model);
            //ViewBag.Cities = new SelectList(db.Cities, "CityID", "CityName"); // Populate cities for dropdown
            //return View(addresses);
            //int userId = (int)Session["UserID"]; // Session'dan kullanıcı ID'sini al
            //var model = db.Addresses.Where(a => a.UserID == userId).ToList();
            //return View(model);
        }

        [HttpPost]
        public ActionResult AddAddress(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı ID'sini session'dan al
                int userId = (int)Session["UserID"];

                var address = new Address
                {
                    isActive = true, // isActive'yi true olarak ayarlıyoruz
                    AddressName = model.AddressName,
                    Street = model.Street,
                    Neighbourhood = model.Neighbourhood,
                    District = model.District,
                    BuildingNo = model.BuildingNo,
                    ApartmentNo = model.ApartmentNo,
                    CityID = model.CityID,
                    UserID = userId // UserID'yi session'dan alıyoruz
                };

                // Veritabanına ekle
                db.Addresses.Add(address);

                try
                {
                    db.SaveChanges(); // Kaydet
                }
                catch (Exception ex)
                {
                    // Hata durumunda bir mesaj göster
                    ModelState.AddModelError("", "An error occurred while saving the address: " + ex.Message);
                    //ViewBag.Cities = new SelectList(db.Cities, "CityID", "CityName");
                    return View("ManageAddresses", db.Addresses.ToList());
                }

                return RedirectToAction("ManageAddresses");
            }

            // Eğer model geçerli değilse, şehirleri tekrar yükleyip view'a geri dön
            ViewBag.Cities = new SelectList(db.Cities, "CityID", "CityName");
            return View("ManageAddresses", db.Addresses.ToList());
        }


        [HttpPost]
        public ActionResult UpdateAddress(Address model)
        {
            var address = db.Addresses.Find(model.AddressID);

            if (address != null)
            {
                address.AddressName = model.AddressName;
                address.Street = model.Street;
                address.Neighbourhood = model.Neighbourhood;
                address.District = model.District;
                address.BuildingNo = model.BuildingNo;
                address.ApartmentNo = model.ApartmentNo;
                address.CityID = model.CityID;

                db.SaveChanges();
            }

            return RedirectToAction("ManageAddresses");
        }

        public ActionResult DeleteAddress(int id)
        {
            var address = db.Addresses.Find(id);

            if (address != null)
            {
                address.isActive = false;
                db.SaveChanges();
            }

            return RedirectToAction("ManageAddresses");
        }


        public ActionResult ManagePayments()
        {
            int userId = (int)Session["UserID"];
            var payments = db.Payments.Where(p => p.UserID == userId && p.isActive!=false).ToList();
            return View(payments);
        }

        [HttpPost]
        public ActionResult AddPayment(Payment model)
        {
            if (ModelState.IsValid)
            {
                model.isActive = true;
                model.UserID = (int)Session["UserID"];
                db.Payments.Add(model);
                db.SaveChanges();
                return RedirectToAction("ManagePayments");
            }

            return View("ManagePayments", db.Payments.Where(p => p.UserID == model.UserID && p.isActive!=false).ToList());
        }

        [HttpPost]
        public ActionResult UpdatePayment(Payment model)
        {
            var payment = db.Payments.Find(model.PaymentID);

            if (payment != null)
            {
                payment.PName = model.PName;
                payment.CardNo = model.CardNo;
                payment.ExpiryDate = model.ExpiryDate;
                payment.CvvNo = model.CvvNo;

                db.SaveChanges();
            }

            return RedirectToAction("ManagePayments");
        }

        public ActionResult DeletePayment(int id)
        {
            var payment = db.Payments.Find(id);

            if (payment != null)
            {
                payment.isActive = false;
                db.SaveChanges();
            }

            return RedirectToAction("ManagePayments");
        }




    }
}

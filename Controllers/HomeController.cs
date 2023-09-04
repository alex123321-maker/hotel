using hotel.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hotel.Controllers
{
    public class HomeController : Controller
    {
        ModelsContext db = new ModelsContext();

        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            ViewBag.Discounts = JsonConvert.SerializeObject(db.Discounts.ToList());
            // Теперь у вас есть ID текущего авторизованного пользователя
            ViewBag.CurrentUserId = userId;
            ViewBag.ActiveTab = "bookings";
            var RoomTypes = db.RoomTypes.ToList();
            HashSet<string> types = new HashSet<string>();
            HashSet<int> caps = new HashSet<int>();
            foreach (var RoomType in RoomTypes)
            {
                types.Add(RoomType.Type);
                caps.Add(RoomType.Capacity);
            }
            ViewBag.RoomTypes = types;
            ViewBag.Caps = caps;

            ViewBag.Rooms = JsonConvert.SerializeObject(db.Rooms.ToList());

            ViewBag.Clients = JsonConvert.SerializeObject(db.Customers.ToList());

            return View();
        }

        public ActionResult Clients()
        {
            ViewBag.ActiveTab = "clients";

            return View();
        }
        public JsonResult GetClientsData()
        {
            
            var clientsData = db.Customers.ToList();

            var json = JsonConvert.SerializeObject(clientsData);

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReservations()
        {
            var reservData = db.Reservations.ToList();
           

            var json = JsonConvert.SerializeObject(reservData);

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteClient(int id)
        {
            var client = db.Customers.Find(id);

            if (client != null)
            {
                db.Customers.Remove(client);
                db.SaveChanges();
                return Json(new { success = true, message = "Запись успешно удалена." });
            }
            else
            {
                return Json(new { success = false, message = "Запись не найдена." });
            }
        }


        public ActionResult DeleteReservation(int id)
        {
            var reserv = db.Reservations.Find(id);

            if (reserv != null)
            {
                db.Reservations.Remove(reserv);
                db.SaveChanges();
                return Json(new { success = true, message = "Запись успешно удалена." });
            }
            else
            {
                return Json(new { success = false, message = "Запись не найдена." });
            }
        }


        [HttpPost]
        public JsonResult CreateReservation(ReservationModel reservation) // Принимайте объект Customer как параметр
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Reservations.Add(reservation);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Запись успешно добавлена." });
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Ошибка валидации: " + string.Join(", ", errors) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ошибка при добавлении записи: " + ex.Message });
            }
        }

        [HttpPost] 
        public JsonResult CreateClient(CustomerModels customer) // Принимайте объект Customer как параметр
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Запись успешно добавлена." });
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Ошибка валидации: " + string.Join(", ", errors) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ошибка при добавлении записи: " + ex.Message });
            }
        }

        public ActionResult Services()
        {
            ViewBag.ActiveTab = "services";
            return View();
        }

        public ActionResult Discount()
        {
            ViewBag.ActiveTab = "discount";

            return View();
        }

        public ActionResult Rooms()
        {
            ViewBag.ActiveTab = "rooms";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
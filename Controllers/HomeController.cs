using hotel.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace hotel.Controllers
{
    public class RoomTypeGroup
    {
        public string Type { get; set; }
        public List<int> Capacities { get; set; }
        public List<int> Prices { get; set; }
    }

    [Authorize]
    public class HomeController : Controller
    {
        ModelsContext db = new ModelsContext();

        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
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
            ViewBag.Discounts = JsonConvert.SerializeObject(db.Discounts.OrderBy(el=>el.Days).ToList());
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
        public JsonResult CreateClient(CustomerModel customer) // Принимайте объект Customer как параметр
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
            List<string> daysOfWeekOrder = new List<string>
            {
                
                "Понедельник",
                "Вторник",
                "Среда",
                "Четверг",
                "Пятница",
                "Суббота",
                "Воскресенье"
            };

            ViewBag.ActiveTab = "discount";

            var groupedRoomTypes = db.RoomTypes
                .GroupBy(rt => rt.Type)
                .Select(group => new RoomTypeGroup
                {
                    Type = group.Key,
                    Capacities = group.OrderBy(rt => rt.Capacity).Select(rt => rt.Capacity).ToList(),
                    Prices = group.OrderBy(rt => rt.Capacity).Select(rt => rt.Price).ToList()
                })
                .ToList();

            ViewBag.RoomTypeGroups = groupedRoomTypes;

            var pricingModels = db.Pricings.ToList();

            // Отсортируйте список в памяти с использованием порядка дней недели
            ViewBag.PricingModel = pricingModels.OrderBy(pm => daysOfWeekOrder.IndexOf(pm.day_of_week)).ToList();
            
            return View();
        }

        public ActionResult Rooms()
        {
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
            ViewBag.ActiveTab = "rooms";

            return View();
        }


        [HttpPost]
        public JsonResult AddDiscount(DiscountModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Discounts.Add(model);
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

        public JsonResult GetDiscount()
        {

            var discountsData = db.Discounts.OrderBy(el => el.Days).ToList();

            var json = JsonConvert.SerializeObject(discountsData);

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateDiscountField(int id, string field, int newValue)
        {
            try
            {
                var discount = db.Discounts.Find(id);

                if (discount != null)
                {
                    // Обновление соответствующего поля скидки
                    if (field.Equals("Days", StringComparison.OrdinalIgnoreCase))
                    {
                        discount.Days = newValue;
                    }
                    else if (field.Equals("Amount", StringComparison.OrdinalIgnoreCase))
                    {
                        discount.Amount = newValue;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Недопустимое поле." });
                    }

                    // Сохранение изменений в базе данных
                    db.SaveChanges();

                    return Json(new { success = true, message = "Изменения успешно сохранены." });
                }
                else
                {
                    return Json(new { success = false, message = "Скидка не найдена." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Произошла ошибка при обновлении скидки: " + ex.Message });
            }
        }
        [HttpPost]
        public ActionResult UpdatePricing(string id, double newValue)
        {
            try
            {
                var pricing = db.Pricings.Find(id);

                if (pricing != null)
                {
                    pricing.multiplier = newValue;
                    db.SaveChanges();

                    return Json(new { success = true, message = "Изменения успешно сохранены." });
                }
                else
                {
                    return Json(new { success = false, message = "Цена не найдена." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Произошла ошибка при обновлении скидки: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateRoomPricing(string type,int cap, int newValue)
        {
            try
            {
                var pricing = db.RoomTypes.FirstOrDefault(rp => rp.Type == type && rp.Capacity == cap);
                if (pricing != null)
                {
                    // Обновите поле price на newValue
                    pricing.Price = newValue;

                    // Сохраните изменения в базе данных
                    db.SaveChanges();

                    return Json(new { success = true, message = "Изменения успешно сохранены." });
                }
                else
                {
                    return Json(new { success = false, message = "Запись не найдена." });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Произошла ошибка при обновлении скидки: " + ex.Message });
            }
        }

        public ActionResult DeleteDiscount(int id)
        {
            var discount = db.Discounts.Find(id);

            if (discount != null)
            {
                db.Discounts.Remove(discount);
                db.SaveChanges();
                return Json(new { success = true, message = "Запись успешно удалена." });
            }
            else
            {
                return Json(new { success = false, message = "Запись не найдена." });
            }
        }

    }
}
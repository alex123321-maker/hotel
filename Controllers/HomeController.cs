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
        public List<decimal> Prices { get; set; }
    }

    [Authorize]
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            // Теперь у вас есть ID текущего авторизованного пользователя
            ViewBag.CurrentUserId = userId;
            ViewBag.ActiveTab = "bookings";
            var RoomTypes = db.RoomTypes.ToList();
            var RoomCaps= db.RoomCapacities.ToList();
            
            HashSet<string> types = new HashSet<string>();
            HashSet<int> caps = new HashSet<int>();
           
            foreach (var RoomType in RoomTypes)
            {
                types.Add(RoomType.Type);
            }

            foreach (var RoomCap in RoomCaps)
            {
                caps.Add(RoomCap.Capacity);
            }
            ViewBag.RoomTypes = types;
            ViewBag.Caps = caps;

            ViewBag.Pricing = JsonConvert.SerializeObject(db.Pricings.ToList());
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
        public JsonResult GetReservationServices(int reservationId)
        {
            var services = db.ReservationServices
                .Where(rs => rs.ReservationId == reservationId)
                .Select(rs => new
                {
                    rs.Service.Id,
                    rs.Service.Title,
                    rs.Service.Price,
                    rs.Service.Description,
                    rs.Complited 
                })
                .ToList();

            return Json(services, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddServiceToReservation(int reservationId, int serviceId)
        {
            var reservationService = new ReservationServiceModel
            {
                ReservationId = reservationId,
                ServiceId = serviceId
            };

            db.ReservationServices.Add(reservationService);
            db.SaveChanges();

            return Json(new { success = true, message = "Услуга успешно добавлена!" });
        }

        [HttpPost]
        public JsonResult RemoveServiceFromReservation(int reservationId, int serviceId)
        {
            var reservationService = db.ReservationServices
                .FirstOrDefault(rs => rs.ReservationId == reservationId && rs.ServiceId == serviceId);

            if (reservationService != null)
            {
                db.ReservationServices.Remove(reservationService);
                db.SaveChanges();
                return Json(new { success = true, message = "Услуга успешно удалена!" });
            }

            return Json(new { success = false, message = "Услуга не найдена!" });
        }

        public JsonResult GetReservations()
        {
            // Загружаем необходимые данные из базы
            var reservations = db.Reservations
                .Include(r => r.Room)
                .Include(r => r.Room.RoomType)
                .Include(r => r.Room.Capacity)
                .Include(r => r.Customer)
                .Include(r => r.Admin)
                .Include(r => r.ReservationServices.Select(rs => rs.Service))
                .ToList();

            // Загружаем множители для дней недели
            var dayOfWeekMultipliers = db.Pricings.ToDictionary(p => p.day_of_week.ToLower(), p => p.multiplier);

            var discount = db.Discounts.ToList();


            // Формируем результат
            var result = reservations.Select(reservation =>
            {
                // Получаем базовую информацию
                var checkInDate = reservation.CheckInDate;
                var checkOutDate = reservation.CheckOutDate;

                double dis = 1;


                var days = (checkOutDate - checkInDate).Days;
                foreach (var el in discount)
                {
                    if (el.Days<= days)
                    {
                        dis = (double)(100 - el.Amount)/ (double)100;
                    }
                }
                var dailyRate = reservation.Room.Price; // Базовая стоимость номера
                double fullPrice = 0;

                // Рассчитываем стоимость за проживание
                for (var date = checkInDate; date < checkOutDate; date = date.AddDays(1))
                {
                    var dayOfWeek = date.ToString("dddd"); // Название дня недели
                    var multiplier = dayOfWeekMultipliers.ContainsKey(dayOfWeek)
                        ? dayOfWeekMultipliers[dayOfWeek]
                        : 1; // Используем множитель или 1 по умолчанию
                    fullPrice += (double)(dailyRate * (decimal)multiplier);
                }

                // Добавляем стоимость сервисов
                var services = reservation.ReservationServices.Select(rs => rs.Service).ToList();
                var servicesCost = services.Sum(service => service.Price); // Сумма стоимости всех сервисов
                fullPrice *= dis;
                fullPrice += servicesCost;


                // Формируем результат для одного бронирования
                return new
                {
                    reservation.Id,
                    Customer = new
                    {
                        reservation.Customer.Id,
                        reservation.Customer.Name,
                        reservation.Customer.Surname,
                        reservation.Customer.Lastname
                    },
                    Room = new
                    {
                        reservation.Room.Id,
                        reservation.Room.RoomType.Type,
                        reservation.Room.Capacity.Capacity,
                        reservation.Room.Price
                    },
                    Admin = new
                    {
                        reservation.Admin.Id,
                        reservation.Admin.UserName,
                        reservation.Admin.Email
                    },
                    reservation.CheckInDate,
                    reservation.CheckOutDate,
                    reservation.CreatedDate,
                    FullPrice = Math.Round(fullPrice, 2), // Округляем до двух знаков
                    Services = services.Select(service => new
                    {
                        service.Id,
                        service.Title,
                        service.Price,
                        service.Description
                    }).ToList()
                };
            });

            var serializedResult = JsonConvert.SerializeObject(result, Formatting.Indented);

            return Json(serializedResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ToggleServiceCompletion(int reservationId, int serviceId)
        {
            try
            {
                // Найти запись ReservationServiceModel
                var reservationService = db.ReservationServices
                    .FirstOrDefault(rs => rs.ReservationId == reservationId && rs.ServiceId == serviceId);

                if (reservationService == null)
                {
                    return Json(new { success = false, message = "Услуга не найдена для данного бронирования." });
                }

                // Обновить статус выполнения
                reservationService.Complited = !(reservationService.Complited);
                db.SaveChanges();

                return Json(new { success = true, message = "Статус услуги успешно обновлен." });
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                return Json(new { success = false, message = "Ошибка при обновлении статуса услуги: " + ex.Message });
            }
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

       

        public ActionResult Discount()
        {
            ViewBag.ActiveTab = "discount";

            // Получаем сгруппированные данные о типах комнат, вместимости и ценах
            var groupedRoomTypes = db.Rooms
                .GroupBy(rm => rm.RoomType.Type)
                .Select(group => new RoomTypeGroup
                {
                    Type = group.Key,
                    Capacities = group.Select(rm => rm.Capacity.Capacity).Distinct().OrderBy(c => c).ToList(),
                    Prices = group.OrderBy(rm => rm.Capacity.Capacity).Select(rm => rm.Price).Distinct().ToList()
                })
                .ToList();

            ViewBag.RoomTypeGroups = groupedRoomTypes;

            // Получаем модели ценообразования и сортируем по дням недели
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

            var pricingModels = db.Pricings.ToList();
            ViewBag.PricingModel = pricingModels
                .OrderBy(pm => daysOfWeekOrder.IndexOf(pm.day_of_week))
                .ToList();

            return View();
        }


        public ActionResult Rooms()
        {
            var RoomTypes = db.RoomTypes.ToList();
            var RoomCaps = db.RoomCapacities.ToList();
            HashSet<string> types = new HashSet<string>();
            HashSet<int> caps = new HashSet<int>();

            foreach (var RoomType in RoomTypes)
            {
                types.Add(RoomType.Type);
            }

            foreach (var RoomCap in RoomCaps)
            {
                caps.Add(RoomCap.Capacity);
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
        public ActionResult UpdateRoomPricing(string type, int cap, int newValue)
        {
            try
            {
                // Находим все комнаты с указанным типом и вместительностью
                var roomsToUpdate = db.Rooms
                    .Where(r => r.RoomType.Type == type && r.Capacity.Capacity == cap)
                    .ToList();

                // Если такие комнаты найдены, обновляем их цену
                if (roomsToUpdate.Any())
                {
                    foreach (var room in roomsToUpdate)
                    {
                        room.Price = newValue;
                    }

                    // Сохраняем изменения в базе данных
                    db.SaveChanges();

                    return Json(new { success = true, message = "Цены успешно обновлены для всех подходящих номеров." });
                }
                else
                {
                    return Json(new { success = false, message = "Номера с указанным типом и вместительностью не найдены." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ошибка при обновлении цен: " + ex.Message });
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
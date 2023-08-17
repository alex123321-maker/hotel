using hotel.Models;
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
            ViewBag.ActiveTab = "bookings";

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


        [HttpPost] // Добавьте этот атрибут, чтобы указать, что метод принимает данные через POST запрос
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
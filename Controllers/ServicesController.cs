using hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace hotel.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // Отображение страницы услуг
        public ActionResult Services()
        {
            ViewBag.ActiveTab = "services";
            var servicesData = db.Services.ToList();
            ViewBag.Services = servicesData;
            return View();
        }

        // Метод для поиска услуг
        [HttpGet]
        public JsonResult SearchServices(string title)
        {
            var results = db.Services
                .Where(s => s.Title.Contains(title))
                .ToList();

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        // Метод для добавления услуги
        [HttpPost]
        public JsonResult AddService(ServiceModel service)
        {
            if (ModelState.IsValid)
            {
                db.Services.Add(service);
                db.SaveChanges();
                return Json(new { success = true, message = "Услуга добавлена успешно!" });
            }

            return Json(new { success = false, message = "Ошибка добавления услуги!" });
        }
        public JsonResult GetAll()
        {
            var services = db.Services
                .Select(service => new
                {
                    service.Id,
                    service.Title,
                    service.Price,
                    service.Description
                })
                .ToList();

            return Json(services, JsonRequestBehavior.AllowGet);
        }

        // Метод для изменения услуги
        [HttpPost]
        public JsonResult UpdateService(ServiceModel service)
        {
            if (ModelState.IsValid)
            {
                var existingService = db.Services.Find(service.Id);
                if (existingService != null)
                {
                    existingService.Title = service.Title;
                    existingService.Description = service.Description;
                    existingService.Price = service.Price;

                    db.SaveChanges();
                    return Json(new { success = true, message = "Услуга обновлена успешно!" });
                }

                return Json(new { success = false, message = "Услуга не найдена!" });
            }

            return Json(new { success = false, message = "Некорректные данные!" });
        }

        // Метод для удаления услуги
        [HttpPost]
        public JsonResult DeleteService(int id)
        {
            var service = db.Services.Find(id);
            if (service != null)
            {
                db.Services.Remove(service);
                db.SaveChanges();
                return Json(new { success = true, message = "Услуга удалена успешно!" });
            }

            return Json(new { success = false, message = "Услуга не найдена!" });
        }
    }

}

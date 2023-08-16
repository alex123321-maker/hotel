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
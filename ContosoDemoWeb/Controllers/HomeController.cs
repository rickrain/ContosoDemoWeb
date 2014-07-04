using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models.Library;

namespace ContosoDemoWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Pictures picturesDB = new Pictures();
            var pics = picturesDB.PictureEntities.ToList();
            return View(pics);
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
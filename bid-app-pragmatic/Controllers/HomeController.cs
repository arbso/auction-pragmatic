using bid_app_pragmatic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace bid_app_pragmatic.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                ViewBag.Username = HttpContext.Session.GetString("Username");
            }

            return View();
        }
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrimerProyecto.Models;

namespace OldMates.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index(bool sino)
        {
            if (sino)
            {
                return View("~/Views/Account/Login.cshtml");
            }
            else
            {
                return View("~/Views/Account/Index.cshtml");
            }
        }
    }
}


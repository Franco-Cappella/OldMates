using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrimerProyecto.Models;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(bool sino)
    {
        if (sino)
            return View("~/Views/Account/Login.cshtml");
        else
            return View("~/Views/Account/Index.cshtml");
    }


}

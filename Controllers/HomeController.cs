using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrimerProyecto.Models;

namespace PrimerProyecto.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(bool sino)
    {
        if(sino) return View("Login", "Home");
        else return View("Registro", "Home");
    }
    public IActionResult CrearEvento(Evento evento){
        if(CrearEvento(Evento evento);
    }
}

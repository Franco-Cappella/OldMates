using Microsoft.AspNetCore.Mvc;
using OldMates.Models;

namespace OldMates.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult CrearEvento()
        {
            if (HttpContext.Session.GetString("Estado") != "true")
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult CrearEvento(Evento nuevoEvento)
        {
            if (string.IsNullOrWhiteSpace(nuevoEvento.Titulo) ||
                string.IsNullOrWhiteSpace(nuevoEvento.Descripcion))
            {
                ViewBag.Error = "Debe completar todos los campos.";
                return View(nuevoEvento);
            }

            nuevoEvento.IDCreador = int.Parse(HttpContext.Session.GetString("IDdelUsuario")!);

            if (BD.CrearEvento(nuevoEvento))
                return RedirectToAction("Index");
            else
            {
                ViewBag.Error = "Error al crear el evento.";
                return View(nuevoEvento);
            }
        }

        [HttpGet]
        public IActionResult ModificarEvento(int IDEvento)
        {
            Evento evento = BD.ObtenerEventoPorId(IDEvento);
            if (evento == null) return RedirectToAction("Index");

            int IDUsuario = int.Parse(HttpContext.Session.GetString("IDdelUsuario")!);
            if (evento.IDCreador != IDUsuario)
            {
                ViewBag.Error = "No puede modificar un evento que no creó.";
                return RedirectToAction("Index");
            }

            return View(evento);
        }

        [HttpPost]
        public IActionResult ModificarEventoRecibir(Evento eventoEditado)
        {
            if (string.IsNullOrWhiteSpace(eventoEditado.Titulo))
            {
                ViewBag.Error = "Debe ingresar un título.";
                return View(eventoEditado);
            }

            if (BD.ModificarEvento(eventoEditado))
                return RedirectToAction("Index");

            ViewBag.Error = "Error al modificar el evento.";
            return View(eventoEditado);
        }

        public IActionResult BorrarEvento(int IDEvento)
        {
            int IDUsuario = int.Parse(HttpContext.Session.GetString("IDdelUsuario")!);
            Evento evento = BD.ObtenerEventoPorId(IDEvento);

            if (evento == null || evento.IDCreador != IDUsuario)
                return RedirectToAction("Index");

            BD.BorrarEvento(IDEvento);
            return RedirectToAction("Index");
        }

        public IActionResult Inscribirse(int IDEvento)
        {
            int IDUsuario = int.Parse(HttpContext.Session.GetString("IDdelUsuario")!);
            if (!BD.EstaInscripto(IDUsuario, IDEvento))
                BD.InscribirseAEvento(IDUsuario, IDEvento);

            return RedirectToAction("MisEventos");
        }

        public IActionResult Desinscribirse(int IDEvento)
        {
            int IDUsuario = int.Parse(HttpContext.Session.GetString("IDdelUsuario")!);
            BD.DesinscribirseDeEvento(IDUsuario, IDEvento);
            return RedirectToAction("MisEventos");
        }

        public IActionResult MisEventos()
        {
            int IDUsuario = int.Parse(HttpContext.Session.GetString("IDdelUsuario")!);
            List<Evento> eventos = BD.ObtenerEventosInscripto(IDUsuario);
            return View(eventos);
        }
    }
}


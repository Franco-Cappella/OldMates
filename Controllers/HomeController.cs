using Microsoft.AspNetCore.Mvc;
using OldMates.Models;

namespace OldMates.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult CrearEvento()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View(ObtenerIntegranteDesdeSession().ID);
        }

        [HttpPost]
        public IActionResult CrearEventoRecibir(Evento nuevoEvento)
        {
            if (string.IsNullOrWhiteSpace(nuevoEvento.Titulo) ||
                string.IsNullOrWhiteSpace(nuevoEvento.Descripcion))
            {
                ViewBag.Error = "Debe completar todos los campos.";
                return View(nuevoEvento);
            }
            Usuario Creador = ObtenerIntegranteDesdeSession();
            nuevoEvento.IDCreador = Creador.ID;
            BD.CrearEvento(nuevoEvento);
            if (BD.CrearEvento(nuevoEvento))
                return RedirectToAction("Index");
            else
            {
                ViewBag.Error = "Error al crear el evento.";
                return View("Actividades");
            }
        }

        [HttpPost]
        public IActionResult CargarEvento()
        {
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            Usuario usuario = ObtenerIntegranteDesdeSession();
            ViewBag.Evento = BD.ObtenerEventoPorId(usuario.ID);
            return View("VerEvento");
        }

        public IActionResult VerEventos()
        {
            return CargarEvento();
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

            return View();
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

        public IActionResult Desinscribirse(int IDEvento)
        {
            int IDUsuario = int.Parse(HttpContext.Session.GetString("IDdelUsuario")!);
            BD.DesInscribirseAEvento(IDUsuario, IDEvento);
            return RedirectToAction("MisEventos");
        }

        public IActionResult MisEventos()
        {
            int IDUsuario = int.Parse(HttpContext.Session.GetString("IDdelUsuario")!);
            List<Evento> eventos = BD.ObtenerEventosInscripto(IDUsuario);
            return View(eventos);
        }

        private Usuario ObtenerIntegranteDesdeSession()//Busca si un jugador ya tiene un integrante en sesión, si no lo tiene crea uno nuevo
        {

            Usuario usuario = Objeto.StringToObject<Usuario>(HttpContext.Session.GetString("Usuario"));

            return usuario;

        }

        public IActionResult DesInscribirse(int IDEvento)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            BD.DesInscribirseAEvento(usuario.ID, IDEvento);
            return View("Index", "Home");
        }
        public IActionResult Landing()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Landing", "Home");
        }

        public IActionResult Explorar()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Explorar", "Home");
        }

        public IActionResult Mensajes()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Mensajes", "Home");
        }

        public IActionResult Tutoriales()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Tutoriales", "Home");
        }

        public IActionResult Juegos()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Juegos", "Home");
        }
        public IActionResult Actividades()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Actividades", "Home");
        }

        public IActionResult Menu()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Menu", "Home");
        }
        public IActionResult MiPerfil()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("MiPerfil", "Home");
        }

        public IActionResult EditarPerfil()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("EditarPerfil", "Home");
        }
        public IActionResult MisActividades()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            BD.MisActividades(usuario.ID);
            return View("MisActividades", "Home");
        }
        public IActionResult InvitarAmigos()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("InvitarAmigos", "Home");
        }
        public IActionResult Recomendaciones()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Recomendaciones", "Home");
        }
        public IActionResult MiAgenda()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("MiAgenda", "Home");
        }
        public IActionResult ListaDeAmigos()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("ListaDeAmigos", "Home");
        }

        public IActionResult CerrarSesion()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Index", "Home");
        }
    }
}


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
            if (ObtenerIntegranteDesdeSession() == null)
            {
                return RedirectToAction("Index", "Account");
            }
            else{
                return View(ObtenerIntegranteDesdeSession().ID);
            }
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
            nuevoEvento.Anotados = new List<int>();
            nuevoEvento.Anotados.Add(nuevoEvento.IDCreador);
            if (BD.ObtenerEventoPorId != null)
                return RedirectToAction("Landing", "Home");
            else
            {
                return View("Actividades");
            }
        }

        [HttpPost]
        public IActionResult CargarEvento()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            ViewBag.Evento = BD.MisActividades(usuario.ID);
            return View("MisActividades");
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
        public IActionResult Perfil()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Perfil", "Home");
        }

        public IActionResult Notificaciones()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Notificaciones", "Home");
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
            if (usuario == null)
                return RedirectToAction("Index", "Home");

            List<Evento> actividades = BD.MisActividades(usuario.ID);
            return View(actividades);
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
        public IActionResult Calendario()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Calendario", "Home");
        }
        public IActionResult ListaDeAmigos()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("ListaDeAmigos", "Home");
        }

        [HttpGet]
        public IActionResult InvitarAmigosPost(string NombreAmigo, string Telefono, string Mensaje)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Account");
            }
            if (string.IsNullOrEmpty(NombreAmigo) || string.IsNullOrEmpty(Telefono) || string.IsNullOrEmpty(Mensaje))
            {
                return View("Landing", "Home");
            }

            // Aca lo tendriamos que guardar en algun lado
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult CerrarSesion()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Account");
        }
    }
}


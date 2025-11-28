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
            else
            {
                return View(ObtenerIntegranteDesdeSession().ID);
            }
        }

        [HttpPost]
        public IActionResult CrearEventoRecibir(Evento nuevoEvento)
        {
            if (string.IsNullOrWhiteSpace(nuevoEvento.Titulo) ||
                string.IsNullOrWhiteSpace(nuevoEvento.Descripcion))
            {
                return View(nuevoEvento);
            }
            Usuario Creador = ObtenerIntegranteDesdeSession();
            nuevoEvento.IDCreador = Creador.ID;
            bool eventoCreado = BD.CrearEvento(nuevoEvento);

            if (eventoCreado)
            {
                return RedirectToAction("Landing", "Home");
            }
            else
            {
                ViewBag.Error = "Hubo un problema al crear el evento.";
                return View("Actividades", "Home");
            }
        }


        [HttpGet]
        public IActionResult ModificarEvento(int IDEvento)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();

            if (usuario == null)
                return RedirectToAction("Index", "Account");

            Evento evento = BD.ObtenerEventoPorId(IDEvento);

            if (evento == null || evento.IDCreador != usuario.ID)
                return RedirectToAction("Landing", "Home");

            ViewBag.Evento = evento;
            ViewBag.IDUsuario = usuario.ID;

            return View("ModificarEvento");
        }

        [HttpPost]
        public IActionResult ModificarEventoRecibir(int ID, string Titulo, TimeSpan Duracion, string Descripcion, string Intereses, int Capacidad, DateTime Fecha, string Localidad, string Imagen)
        {
            Evento eventoOriginal = BD.ObtenerEventoPorId(ID);

            if (eventoOriginal == null)
                return RedirectToAction("Landing", "Home");
            eventoOriginal.Titulo = Titulo; eventoOriginal.Duracion = Duracion; eventoOriginal.Descripcion = Descripcion; eventoOriginal.Intereses = Intereses; eventoOriginal.Capacidad = Capacidad; eventoOriginal.Fecha = Fecha; eventoOriginal.Localidad = Localidad;

            if (string.IsNullOrWhiteSpace(eventoOriginal.Titulo))
            {
                ViewBag.Error = "Debe ingresar un título.";
                ViewBag.Evento = eventoOriginal;
                return View("ModificarEvento");
            }

            if (BD.ModificarEvento(eventoOriginal))
                return RedirectToAction("MisActividades", "Home");

            ViewBag.Error = "Error al modificar el evento.";
            ViewBag.Evento = eventoOriginal;

            return View("ModificarEvento");
        }



        public IActionResult BorrarEvento(int IDEvento)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();

            if (usuario == null)
            {
                return RedirectToAction("Index", "Account");
            }

            Evento evento = BD.ObtenerEventoPorId(IDEvento);

            if (evento == null || evento.IDCreador != usuario.ID)
            {
                return RedirectToAction("Index", "Account");
            }

            BD.BorrarEvento(IDEvento);

            return RedirectToAction("MisActividades", "Home");
        }


        private Usuario ObtenerIntegranteDesdeSession()
        {

            Usuario usuario = Objeto.StringToObject<Usuario>(HttpContext.Session.GetString("Usuario"));

            return usuario;

        }

        [HttpPost]
        public IActionResult DesInscribirse(int IDEvento)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null) return RedirectToAction("Index", "Account");

            BD.DesInscribirseAEvento(usuario.ID, IDEvento);

            return RedirectToAction("Landing", "Home");
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
            if (usuario == null)
                return RedirectToAction("Index", "Account");

            List<Evento> eventos = BD.ObtenerEventos();
            List<int> eventosInscripto = BD.ObtenerEventosInscripto(usuario.ID);

            ViewBag.Eventos = eventos;
            ViewBag.IDUsuario = usuario.ID;
            ViewBag.EventosInscripto = eventosInscripto;

            return View("Actividades");
        }

        public IActionResult Perfil()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            ViewBag.Usuario = usuario;
            return View("Perfil", "Home");
        }

        public IActionResult PerfilMenu()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            ViewBag.Usuario = usuario;
            return View("PerfilMenu", "Home");
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
                return RedirectToAction("Index", "Account");

            List<Evento> Eventos = BD.MisActividades(usuario.ID);
            List<Anotados> Anotados = BD.MisAnotados(usuario.ID);

            ViewBag.Eventos = Eventos;
            ViewBag.Anotados = Anotados;
            ViewBag.IDUsuario = usuario.ID;

            return View();
        }



        public IActionResult InvitarAmigos()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("InvitarAmigos", "Home");
        }
        public IActionResult ActividadUnica(int IDEvento)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
                return RedirectToAction("Index", "Home");

            Evento evento = BD.ObtenerEventoPorId(IDEvento);

            if (evento == null)
            {
                ViewBag.Eventos = null;
                ViewBag.EventosInscripto = new List<int>();
                ViewBag.IDUsuario = usuario.ID;
                return View();
            }

            List<int> eventosInscripto = BD.ObtenerEventosInscripto(usuario.ID);
            if (eventosInscripto == null)
                eventosInscripto = new List<int>();

            ViewBag.Eventos = new List<Evento> { evento };
            ViewBag.EventosInscripto = eventosInscripto;
            ViewBag.IDUsuario = usuario.ID;

            return View();
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


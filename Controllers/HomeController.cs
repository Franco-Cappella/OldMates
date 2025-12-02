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
                ViewBag.Error = "No estas logueado";
            }
            else
            {
                ViewBag.usuario = usuario;
                return View();
            }
        }

        [HttpPost]
        public IActionResult CrearEventoRecibir(Evento nuevoEvento)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "Debes iniciar sesión para crear un evento.";
                return RedirectToAction("Index", "Account");
            }

            if (string.IsNullOrWhiteSpace(nuevoEvento.Titulo))
            {
                ViewBag.Error = "El título es obligatorio.";
                return View("CrearEvento", nuevoEvento);
            }

            if (string.IsNullOrWhiteSpace(nuevoEvento.Descripcion))
            {
                ViewBag.Error = "La descripción es obligatoria.";
                return View("CrearEvento", nuevoEvento);
            }

            if (string.IsNullOrWhiteSpace(nuevoEvento.Localidad))
            {
                ViewBag.Error = "La localidad es obligatoria.";
                return View("CrearEvento", nuevoEvento);
            }

            if (nuevoEvento.Capacidad <= 0) ViewBag.Error = "No estas logueado";

            string nombreArchivo = "default-evento.png";
            string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
            string rutaCompleta = Path.Combine(carpeta, nombreArchivo);
            if (archivo != null && archivo.Length > 0)
            {
                ViewBag.Error = "La capacidad debe ser mayor a 0.";
                return View("CrearEvento", nuevoEvento);
            }

            if (nuevoEvento.Fecha < DateTime.Now)
            {
                ViewBag.Error = "La fecha del evento no puede ser en el pasado.";
                return View("CrearEvento", nuevoEvento);
            }

            if (string.IsNullOrWhiteSpace(nuevoEvento.Foto))
            {
                nuevoEvento.Foto = "evento_default.png";
            }

            nuevoEvento.IDCreador = usuario.ID;
                nombreArchivo = Path.GetFileName(archivo.FileName);
                rutaCompleta = Path.Combine(carpeta, nombreArchivo);
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    archivo.CopyTo(stream);
                }

            Usuario Creador = ObtenerIntegranteDesdeSession();
            nuevoEvento.IDCreador = Creador.ID;

            bool eventoCreado = BD.CrearEvento(nuevoEvento);

            if (eventoCreado)
            {
                ViewBag.Success = "Evento creado exitosamente.";
                return RedirectToAction("Landing", "Home");
            }
            else
            {
                ViewBag.Error = "Ya existe un evento con ese título o la fecha es inválida.";
                return View("CrearEvento", nuevoEvento);
            }
        }

        [HttpGet]
        public IActionResult ModificarEvento(int IDEvento)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "Debes iniciar sesión para modificar eventos.";
                return RedirectToAction("Index", "Account");
            }

            Evento evento = BD.ObtenerEventoPorId(IDEvento);

            if (evento == null)
            {
                ViewBag.Error = "El evento no existe o fue eliminado.";
                return RedirectToAction("Landing", "Home");
            }

            if (evento.IDCreador != usuario.ID)
            {
                ViewBag.Error = "No tienes permiso para modificar este evento.";
                return RedirectToAction("Landing", "Home");
            }

            ViewBag.Evento = evento;
            ViewBag.IDUsuario = usuario.ID;

            return View("ModificarEvento");
        }

        [HttpPost]
        public IActionResult ModificarEventoRecibir(Evento eventoActualizado)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "Debes iniciar sesión para modificar eventos.";
                return RedirectToAction("Index", "Account");
            }

            Evento eventoOriginal = BD.ObtenerEventoPorId(eventoActualizado.ID);

            if (eventoOriginal == null)
            {
                ViewBag.Error = "El evento no existe o fue eliminado.";
                return RedirectToAction("Landing", "Home");
            }

            if (eventoOriginal.IDCreador != usuario.ID)
            {
                ViewBag.Error = "No tienes permiso para modificar este evento.";
                return RedirectToAction("Landing", "Home");
            }

            if (string.IsNullOrWhiteSpace(eventoActualizado.Titulo))
            {
                ViewBag.Error = "El título es obligatorio.";
                ViewBag.Evento = eventoActualizado;
                return View("ModificarEvento");
            }

            if (string.IsNullOrWhiteSpace(eventoActualizado.Descripcion))
            {
                ViewBag.Error = "La descripción es obligatoria.";
                ViewBag.Evento = eventoActualizado;
                return View("ModificarEvento");
            }

            if (eventoActualizado.Capacidad <= 0)
            {
                ViewBag.Error = "La capacidad debe ser mayor a 0.";
                ViewBag.Evento = eventoActualizado;
                return View("ModificarEvento");
            }

            if (string.IsNullOrWhiteSpace(eventoActualizado.Foto))
            {
                eventoActualizado.Foto = eventoOriginal.Foto;
            }

            eventoActualizado.IDCreador = eventoOriginal.IDCreador;

            if (BD.ModificarEvento(eventoActualizado))
            {
                ViewBag.Success = "Evento modificado exitosamente.";
                return RedirectToAction("MisActividades", "Home");
            }

            ViewBag.Error = "Error al modificar el evento. Intenta nuevamente.";
            ViewBag.Evento = eventoActualizado;

            return View("ModificarEvento");
        }



        public IActionResult BorrarEvento(int IDEvento)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }

            Evento evento = BD.ObtenerEventoPorId(IDEvento);

            if (evento == null || evento.IDCreador != usuario.ID)
            {
                return RedirectToAction("Index", "Account");
            }

            BD.BorrarEvento(IDEvento);

            return RedirectToAction("Actividades", "Home");
        }


        private Usuario ObtenerIntegranteDesdeSession()
        {

            Usuario usuario = Objeto.StringToObject<Usuario>(HttpContext.Session.GetString("Usuario"));

            return usuario;

        }

        private string GuardarFoto(IFormFile imagen, string nombrePorDefecto)
        {
            string nombreFinal = nombrePorDefecto;

            if (imagen != null && imagen.Length > 0)
            {
                string extension = Path.GetExtension(imagen.FileName);
                nombreFinal = Guid.NewGuid().ToString() + extension;

                string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string rutaCompleta = Path.Combine(carpeta, nombreFinal);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    imagen.CopyTo(stream);
                }
            }

            return nombreFinal;
        }

        public IActionResult DesInscribirse(int IDEvento)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }

            bool resultado = BD.DesInscribirseAEvento(usuario.ID, IDEvento);
            return RedirectToAction("Actividades", "Home");
        }

        public IActionResult Landing()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            ViewBag.EventosHoy = BD.ObtenerEventosDeHoy();
            ViewBag.ProximosEventos = BD.ObtenerProximosEventos();

            ViewBag.Usuario = usuario;

            return View();
        }


        public IActionResult Explorar()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("Explorar", "Home");
        }

        public IActionResult Mensajes()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            return View("Mensajes", "Home");
        }

        public IActionResult Tutoriales()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            return View("Tutoriales", "Home");
        }

        public IActionResult Juegos()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            return View("Juegos", "Home");
        }
        public IActionResult Actividades()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }

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
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            ViewBag.Usuario = usuario;
            return View("Perfil", "Home");
        }

        public IActionResult PerfilMenu()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            ViewBag.Usuario = usuario;
            return View("PerfilMenu", "Home");
        }

        public IActionResult Notificaciones()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            return View("Notificaciones", "Home");
        }


        [HttpGet]
        public IActionResult EditarPerfil()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            ViewBag.Usuario = usuario;
            return View("EditarPerfil");
        }

        [HttpPost]
        public IActionResult EditarPerfilRecibir(Usuario usuarioActualizado)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }

            if (string.IsNullOrWhiteSpace(usuarioActualizado.Foto))
            {
                usuarioActualizado.Foto = usuario.Foto ?? "usuario_default.png";
            }

            usuarioActualizado.ID = usuario.ID;

            bool perfilActualizado = BD.EditarPerfil(usuarioActualizado);

            if (perfilActualizado)
            {
                GuardarIntegranteEnSession(usuarioActualizado);
                return RedirectToAction("Perfil", "Home");
            }
            else
            {
                ViewBag.Error = "Hubo un problema al actualizar el perfil.";
                ViewBag.Usuario = usuarioActualizado;
                return View("EditarPerfil");
            }
        }
        public IActionResult MisActividades()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }

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
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            return View("InvitarAmigos", "Home");
        }
        public IActionResult ActividadUnica(int IDEvento)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }

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
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            return View("Calendario", "Home");
        }
        public IActionResult ListaDeAmigos()
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            if (ObtenerIntegranteDesdeSession() == null) RedirectToAction("Index", "Home");
            return View("ListaDeAmigos", "Home");
        }

        [HttpGet]
            public IActionResult InvitarAmigosPost(string NombreAmigo, string Telefono, string Mensaje)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
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
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Account");
        }
            private bool GuardarIntegranteEnSession(Usuario usuario)
        {
            HttpContext.Session.SetString("Usuario", Objeto.ObjectToString(usuario));

            string valor = HttpContext.Session.GetString("Usuario");

            if (!string.IsNullOrEmpty(valor)) return true;
            else return false;
        }
    }
}


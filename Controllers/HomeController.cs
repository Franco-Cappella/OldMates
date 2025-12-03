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
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }
            
            ViewBag.usuario = usuario;
            return View();
        }

        [HttpPost]
        public IActionResult CrearEventoRecibir(string titulo, TimeSpan duracion, string descripcion, string intereses, int capacidad, DateTime fecha, string localidad, IFormFile imagen)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "Debes iniciar sesión para crear un evento.";
                return RedirectToAction("Index", "Account");
            }

            if (fecha < DateTime.Now)
            {
                ViewBag.Error = "La fecha del evento no puede ser en el pasado.";
                return View("CrearEvento");
            }

            string rutaRelativa = GuardarFoto(imagen, "/img/default-evento.png");
            
            int IDCreador = usuario.ID;

            Evento nuevoEvento = new Evento(IDCreador, titulo, descripcion, duracion, localidad, capacidad, fecha, intereses, rutaRelativa);

            bool eventoCreado = BD.CrearEvento(nuevoEvento);

            if (eventoCreado)
            {
                ViewBag.Success = "Evento creado exitosamente.";
                return RedirectToAction("Landing", "Home");
            }
            else
            {
                ViewBag.Error = "Ya existe un evento con ese título o la fecha es inválida.";
                return View("CrearEvento");
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
        public IActionResult ModificarEventoRecibir(Evento eventoActualizado, IFormFile imagen)
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

            // Guardar nueva imagen si se proporciona
            if (imagen != null && imagen.Length > 0)
            {
                eventoActualizado.Foto = GuardarFoto(imagen, eventoOriginal.Foto);
            }
            else
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
            return View();
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
            return View();
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
            return View();
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
        public IActionResult EditarPerfilRecibir(string Nombre, string Apellido, string Localidad, string Intereses, IFormFile fotoArchivo)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }

            usuario.Nombre = Nombre;
            usuario.Apellido = Apellido;
            usuario.Localidad = Localidad;
            usuario.Intereses = Intereses;

            if (fotoArchivo != null && fotoArchivo.Length > 0)
            {
                string fotoActual = usuario.Foto ?? "/img/usuario_default.png";
                usuario.Foto = GuardarFoto(fotoArchivo, fotoActual);
            }

            bool perfilActualizado = BD.EditarPerfil(usuario);

            if (perfilActualizado)
            {
                GuardarIntegranteEnSession(usuario);
                return RedirectToAction("Perfil", "Home");
            }
            else
            {
                ViewBag.Error = "Hubo un problema al actualizar el perfil.";
                ViewBag.Usuario = usuario;
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

        public IActionResult Calendario(int mes = 0, int año = 0)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                ViewBag.Error = "No estas logueado";
                return RedirectToAction("Index", "Account");
            }

            DateTime fechaActual = DateTime.Now;
            int mesActual = mes > 0 ? mes : fechaActual.Month;
            int añoActual = año > 0 ? año : fechaActual.Year;
            
            if (mesActual < 1) { mesActual = 12; añoActual--; }
            if (mesActual > 12) { mesActual = 1; añoActual++; }

            List<Evento> eventosInscritos = BD.MisActividades(usuario.ID);
            
            List<Evento> todosEventos = BD.ObtenerEventos();
            List<Evento> eventosCreados = todosEventos.Where(e => e.IDCreador == usuario.ID).ToList();
            
            var todosLosEventos = eventosInscritos
                .Union(eventosCreados)
                .GroupBy(e => e.ID)
                .Select(g => g.First())
                .ToList();
            
            ViewBag.Eventos = todosLosEventos;
            ViewBag.Usuario = usuario;
            ViewBag.Mes = mesActual;
            ViewBag.Año = añoActual;
            
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

            ViewBag.Amigos = BD.ObtenerAmigos(usuario.ID);
            ViewBag.SolicitudesPendientes = BD.ObtenerSolicitudesPendientes(usuario.ID);
            ViewBag.Usuario = usuario;

            return View();
        }

        [HttpGet]
        public IActionResult BuscarAmigos(string busqueda)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Account");
            }

            if (string.IsNullOrWhiteSpace(busqueda))
            {
                ViewBag.Usuarios = new List<Usuario>();
            }
            else
            {
                var usuarios = BD.BuscarUsuarios(busqueda, usuario.ID);
                foreach (var usu in usuarios)
                {
                    usu.Intereses = BD.ObtenerEstadoAmistad(usuario.ID, usu.ID) ?? "ninguno";
                }
                ViewBag.Usuarios = usuarios;
            }

            ViewBag.Busqueda = busqueda;
            ViewBag.Usuario = usuario;

            return View();
        }

        [HttpPost]
        public IActionResult EnviarSolicitudAmistad(int IDAmigo)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Account");
            }

            BD.EnviarSolicitudAmistad(usuario.ID, IDAmigo);
            return RedirectToAction("BuscarAmigos", new { busqueda = "" });
        }

        [HttpPost]
        public IActionResult AceptarSolicitud(int IDSolicitud)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Account");
            }

            BD.AceptarSolicitudAmistad(IDSolicitud, usuario.ID);
            return RedirectToAction("ListaDeAmigos");
        }

        [HttpPost]
        public IActionResult RechazarSolicitud(int IDSolicitud)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Account");
            }

            BD.RechazarSolicitudAmistad(IDSolicitud, usuario.ID);
            return RedirectToAction("ListaDeAmigos");
        }

        [HttpPost]
        public IActionResult EliminarAmigo(int IDAmigo)
        {
            Usuario usuario = ObtenerIntegranteDesdeSession();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Account");
            }

            BD.EliminarAmigo(usuario.ID, IDAmigo);
            return RedirectToAction("ListaDeAmigos");
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
            return !string.IsNullOrEmpty(valor);
        }

        private string GuardarFoto(IFormFile archivo, string fotoPorDefecto)
        {
            if (archivo == null || archivo.Length == 0)
                return fotoPorDefecto;

            try
            {
                string extension = Path.GetExtension(archivo.FileName);
                string nombreUnico = Guid.NewGuid().ToString() + extension;
                string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string rutaCompleta = Path.Combine(carpeta, nombreUnico);
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    archivo.CopyTo(stream);
                }

                return "/img/" + nombreUnico;
            }
            catch
            {
                return fotoPorDefecto;
            }
        }
    }
}


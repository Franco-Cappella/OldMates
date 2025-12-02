using Microsoft.AspNetCore.Mvc;
using OldMates.Models;

namespace OldMates.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View("Index", "Account");
        }
        public IActionResult Login()
        {
            ViewBag.mensaje = "";
            return View();
        }

        [HttpPost]
        public IActionResult LoginPost(string Username, string Contraseña)
        {

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Contraseña))
            {
                ViewBag.mensaje = "Completa tus datos";
                return View("Login");
            }

            else if (BD.VerificarContraseña(Username, Contraseña))
            {
                Usuario usuario = BD.ObtenerPorUsername(Username);
                GuardarIntegranteEnSession(usuario);
                return RedirectToAction("Landing", "Home");
            }
            else
            {
                ViewBag.mensaje = "El usuario o la contraseña son incorrectos";
                return View("Login");
            }
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegistroPost(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.Username) || string.IsNullOrEmpty(usuario.Contraseña))
            {
                return View("Registro");
            }
            else if (BD.Registro(usuario))
            {
                GuardarIntegranteEnSession(usuario);
                return RedirectToAction("Landing", "Home");
            }
            else
            {
                return View("Registro");
            }
        }

        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        private bool GuardarIntegranteEnSession(Usuario usuario)
        {
            HttpContext.Session.SetString("Usuario", Objeto.ObjectToString(usuario));

            string valor = HttpContext.Session.GetString("Usuario");

            if (!string.IsNullOrEmpty(valor)) return true;
            else return false;
        }

        private Usuario ObtenerIntegranteDesdeSession()//Busca si un jugador ya tiene un integrante en sesión, si no lo tiene crea uno nuevo
        {

            Usuario usuario = Objeto.StringToObject<Usuario>(HttpContext.Session.GetString("Usuario"));

            return usuario;

        }
    }

}
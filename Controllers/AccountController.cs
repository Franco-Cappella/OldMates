using Microsoft.AspNetCore.Mvc;
using OldMates.Models;

namespace OldMates.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View("Login");
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginPost(string Username, string Contraseña)
        {

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Contraseña))
            {
                return View("Login");
            }

            else if (BD.VerificarContraseña(Username, Contraseña))
            {
                Usuario usuario = BD.ObtenerPorUsername(Username);
                HttpContext.Session.SetString("IDdelUsuario", usuario.ID.ToString());
                /* HttpContext.Session.SetString("Username", usuario.Username);
                HttpContext.Session.SetString("Estado", "true"); */
                return RedirectToAction("Index", "Home");
            }
            else
            {
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
                return RedirectToAction("Login");
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


        private void GuardarIntegranteEnSession(Usuario usuario) //Para guardar el objeto integrante en la sesión(guarda la informacion de jugador a medida que avanza)
        {
            HttpContext.Session.SetString("Usuario", Objeto.ObjectToString(usuario));
        }

        private Usuario ObtenerIntegranteDesdeSession()//Busca si un jugador ya tiene un integrante en sesión, si no lo tiene crea uno nuevo
        {
            Usuario usuario = Objeto.StringToObject<Usuario>(HttpContext.Session.GetString("Usuario"));

            return usuario;

        }
    }

}
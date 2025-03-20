using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PracticaMvcCore2MPB.Models;
using PracticaMvcCore2MPB.Repositories;
using PracticaMvcCore2MPB.Filters;

namespace PracticaMvcCore2MPB.Controllers
{
    public class ManagedController : Controller
    {
        private RepositoryLibros repo;

        public ManagedController(RepositoryLibros repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var usuario = await repo.GetUsuariosAsync();
            var user = usuario.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }

            // Guardar datos del usuario en la sesión
            HttpContext.Session.SetInt32("IdUsuario", user.IdUsuario);
            HttpContext.Session.SetString("Nombre", user.Nombre);
            HttpContext.Session.SetString("Email", user.Email);

            // Crear Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Libros");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AuthorizeLibros]
        public async Task<IActionResult> PerfilUsuario()
        {
            var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idUsuarioClaim == null)
            {
                return RedirectToAction("Login");
            }

            int idUsuario = int.Parse(idUsuarioClaim.Value);
            Usuario usuario = await repo.FindUsuarioByIdAsync(idUsuario);
            if (usuario == null)
            {
                return RedirectToAction("Login");
            }

            List<Pedido> pedidos = await this.repo.GetPedidosAsync();

            ViewData["PEDIDOS"] = pedidos;

            return View(usuario);
        }
    }
}

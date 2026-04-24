using Microsoft.AspNetCore.Mvc;
using Barbearia.Data;
using Barbearia.Models;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Mime;

using Microsoft.AspNetCore.Authorization;

namespace Barbearia.Controllers
{
   


    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Cadastro(Usuario usuario)
        {
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return RedirectToAction("Login", "Usuario");
        }
        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
       [HttpPost]
 public async Task<IActionResult> Login(string email, string senha)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario != null && BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Email),
                    new Claim("Id", usuario.Id.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Dashboard", "Home");

            }
            else
            {
            ViewBag.Error = "Email ou senha inválidos.";
            return View();
            }
        }
        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }
}
}
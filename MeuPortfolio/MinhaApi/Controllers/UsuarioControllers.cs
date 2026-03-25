using Microsoft.AspNetCore.Mvc;
using Barbearia.Data;
using Barbearia.Models;
using BCrypt.Net;

namespace Barbearia.Controllers
{
    [ApiController]
[Route("api/[controller]")]

    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Registrar(Usuario usuario)
        {
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }
    }
}
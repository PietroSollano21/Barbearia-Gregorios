using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Barbearia.Data;
using Barbearia.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Barbearia.Controllers
{
    public class AdmController : Controller
    {
        private readonly AppDbContext _context;
        public AdmController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Barbeiro")]
        public async Task<IActionResult> Barbeiro()
        {
            var barbeiroLogado = User.Identity?.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == barbeiroLogado);
            if (usuario == null || usuario.Perfil != "Barbeiro")            {
                return RedirectToAction("Login", "Usuario");
            }
            var agendamentos = await _context.Agendamentos.Where(a => (a.statuspagamento == "approved" ||  a.statuspagamento == "Pago") && a.BarbeiroNome == usuario.Nome).OrderBy(a => a.Data).ThenBy(a => a.Hora).ToListAsync();
            return View(agendamentos);
        }
        [Authorize(Roles = "Barbeiro")]
        public async Task<IActionResult> Usuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return View(usuarios);
        }
        [HttpPost]
        public async Task<IActionResult> Promover(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            if (usuario != null)
            {
                usuario.Perfil = "Barbeiro";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Usuarios));
        }
        [HttpPost]
        public async Task<IActionResult> Rebaixar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            if (usuario != null)
            {
                usuario.Perfil = "Cliente";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Usuarios));
        }
    }
}
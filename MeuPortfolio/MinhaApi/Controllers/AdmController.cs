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
            DateTime dataHoje = DateTime.Today;
            var agendamentos = await _context.Agendamentos.Where(a => (a.statuspagamento == "Pagar na hora" ||  a.statuspagamento == "Pago") && a.BarbeiroNome == usuario.Nome).Where(a => a.Data.Date >= dataHoje.Date).OrderBy(a => a.Data).ThenBy(a => a.Hora).ToListAsync();
            var diasConfigurados = await _context.DiaBarbeiros .Where(d => d.BarbeiroId == barbeiroLogado).ToListAsync();
    var modeloParaAView = (Agendamentos: agendamentos, Dias: diasConfigurados);


    return View(modeloParaAView);
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
       [HttpGet]
       [Authorize]

public async Task<IActionResult> ConfigurarDias()
    {

        var barbeiroid = User.Identity?.Name;
        var datasconfig = _context.DiaBarbeiros.Where(d => d.BarbeiroId == barbeiroid && d.Data >= DateTime.Today).OrderBy(d => d.Data).ToList();
        var agendamentos = _context.Agendamentos.Where(a => a.BarbeiroNome == barbeiroid && a.Data >= DateTime.Today && (a.statuspagamento == "Pagar na hora" || a.statuspagamento == "Pago")).ToList();
        return View(datasconfig);
    }
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DataExata(DateTime novaData, string status)
    {
        if(novaData == DateTime.MinValue || string.IsNullOrEmpty(status))
        {
            return BadRequest("Erro de Binding: Os dados do formulário não chegaram no Controller. Verifique os nomes (name=) no HTML.");
        }
       var barbeiroId = User.Identity.Name;
       bool vaiTrabalhar = status == "Disponível";
       bool folga = status == "Folga";
       Console.WriteLine($"BarbeiroId salvo: {User.Identity.Name}");
         var dataExistente = await _context.DiaBarbeiros.FirstOrDefaultAsync(d => d.BarbeiroId == barbeiroId && d.Data.Date == novaData.Date);
    if(!vaiTrabalhar)
            {
                var agendamentodia = await _context.Agendamentos.Where(a => a.BarbeiroNome == barbeiroId && a.Data.Date == novaData.Date && (a.statuspagamento == "Pagar na hora" || a.statuspagamento == "Pago")).ToListAsync();
            foreach (var agendamento in agendamentodia)
                {
                    agendamento.statuspagamento = "Cancelado";
                    _context.Agendamentos.Update(agendamento);
                }
            }
        if (dataExistente != null)
        {
            dataExistente.Disponivel = vaiTrabalhar;
            _context.DiaBarbeiros.Update(dataExistente);
        }
        else
        {
            var novaConfig = new DiaBarbeiro
            {
                BarbeiroId = barbeiroId,
                Data = novaData.Date,
                Disponivel = vaiTrabalhar
            };
            await _context.DiaBarbeiros.AddAsync(novaConfig);
        }
        var alteraçoes = await _context.SaveChangesAsync();
        Console.WriteLine($"Alterações salvas: {alteraçoes}");
        TempData["Sucesso"] = vaiTrabalhar ? $"Esse dia {novaData.Date} foi configurado como disponível." : $"Esse dia {novaData.Date} foi configurado como folga.";
        return RedirectToAction("Barbeiro");
        
    }   
}
}
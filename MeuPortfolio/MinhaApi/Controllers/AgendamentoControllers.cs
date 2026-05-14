using Microsoft.AspNetCore.Mvc;
using Barbearia.Models; 
using Barbearia.Services;
using Barbearia.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.PortableExecutable;
using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using Barbearia.Data;
using Microsoft.EntityFrameworkCore;

namespace Barbearia.Controllers
{
    
    public class AgendamentoController : Controller
    {
        private readonly AgendamentoService _agendamentoService;
        private readonly AgendamentoRepository _agendamentoRepository;
        private readonly AppDbContext _context;
        private readonly AgendamentoRepository _repo;
        private readonly IConfiguration _configuration;
       
        public  AgendamentoController(AgendamentoService agendamentoService, AgendamentoRepository agendamentoRepository, IConfiguration configuration, AppDbContext context)
        {
            _agendamentoService = agendamentoService;
            _agendamentoRepository = agendamentoRepository;
            _configuration = configuration;
            _context= context;
        }

        [HttpPost]
        public IActionResult Criar(Agendamento agendamento)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _agendamentoService.CalcularValor(agendamento);
            _agendamentoRepository.Adicionar(agendamento);

            return Ok(agendamento);
        }

        [Authorize]
        [HttpPost("agendar")]
        public  IActionResult Agendar(Agendamento model)
        {
           
            if(ModelState.IsValid)
            {
                var _repo = new AgendamentoRepository(_context, _configuration);
                _repo.SalvarAgendamento(model);
                return RedirectToAction("Dashboard", "Home");
            }
            return View("Agenda");
        }
            [Authorize]
        [HttpGet("Agendar")]
        public async Task<IActionResult> Agendar(DateTime? dataSelecionada)
        {
           
            DateTime data = dataSelecionada ?? DateTime.Today;
            var BarbeiroNome = await _context.Usuarios.Where(u => u.Perfil == "Barbeiro").Select(u => u.Nome).ToListAsync();
            ViewBag.BarbeiroNome = BarbeiroNome;
            List<TimeSpan> grandeTotal = new List<TimeSpan>
            {
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 0, 0),
                new TimeSpan(11, 0, 0),
                new TimeSpan(14, 0, 0),
                new TimeSpan(15, 0, 0),
                new TimeSpan(16, 0, 0),
                new TimeSpan(17, 0, 0),
                new TimeSpan(18, 0, 0),
                new TimeSpan(19, 0, 0)
            };
            var repo = new AgendamentoRepository(_context, _configuration);
            var ocupados = repo.BuscarHorariosOcupados(data);
            var disponiveis = grandeTotal.Where(h => !ocupados.Contains(h)).ToList();
            ViewBag.HorariosDisponiveis = disponiveis;
            ViewBag.DataSelecionada = data.ToString("yyyy-MM-dd");
            return View();
        }
        [HttpGet("Agendamento/BuscarBarbeiros")]
        public async Task<IActionResult> BuscarBabrbeiros()
        {
            var Babrbeiros = await _context.Usuarios.Where(u => u.Perfil == "Barbeiro").Select(u => u.Nome).ToListAsync();
            return Json(Babrbeiros);
        }
       
    }

}
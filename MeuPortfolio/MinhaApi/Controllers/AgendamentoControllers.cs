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

namespace Barbearia.Controllers
{
    
    public class AgendamentoController : Controller
    {
        private readonly AgendamentoService _agendamentoService;
        private readonly AgendamentoRepository _agendamentoRepository;
        private readonly AppDbContext _context;
        private readonly AgendamentoRepository _repo;
        public AgendamentoController(AppDbContext context)
        {
            _context = context;
            _repo = new AgendamentoRepository(_context);
        }   
        public  AgendamentoController(AgendamentoService agendamentoService, AgendamentoRepository agendamentoRepository)
        {
            _agendamentoService = agendamentoService;
            _agendamentoRepository = agendamentoRepository;
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
        public IActionResult Agendar(Agendamento model)
        {
           
            if(ModelState.IsValid)
            {
                var _repo = new AgendamentoRepository(_context);
                _repo.SalvarAgendamento(model);
                return RedirectToAction("Dashboard", "Home");
            }
            return View("Agenda");
        }
            [Authorize]
        [HttpGet("Agendar")]
        public IActionResult Agendar(DateTime? dataSelecionada)
        {
           
            DateTime data = dataSelecionada ?? DateTime.Today;
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
            var repo = new AgendamentoRepository(_context);
            var ocupados = repo.BuscarHorariosOcupados(data);
            var disponiveis = grandeTotal.Where(h => !ocupados.Contains(h)).ToList();
            ViewBag.HorariosDisponiveis = disponiveis;
            ViewBag.DataSelecionada = data.ToString("yyyy-MM-dd");
            return View();
        }
    }
}
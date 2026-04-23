using System.ComponentModel.Design;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Barbearia.Repositories;
using Barbearia.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Barbearia.Models;
using Barbearia.Data;
using Barbearia.Repositories;


public class HomeController : Controller
{
    private readonly AppDbContext _context;
    public IActionResult Index()
    {
        if(User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Dashboard");
        }
        return View();
    }
    
    
    
    [Authorize]
    public IActionResult Dashboard()
    {
        return View();
    }
    [Authorize]
    public IActionResult Agenda()
    {
        return View();
    }
    [Authorize]
    [HttpGet]
    public IActionResult BuscarHorarios(string dataSelecionada)
    {
        if (DateTime.TryParse(dataSelecionada, out DateTime data))
        {
           
            var repository = new AgendamentoRepository(_context);
            
            
            var horariosOcupadosTimeSpan = repository.BuscarHorariosOcupados(data);
            List<string> horariosOcupados = horariosOcupadosTimeSpan
                                                .Select(t => t.ToString(@"hh\:mm"))
                                                .ToList();

            
            List<string> todosOsHorarios = new List<string>();
            TimeSpan abertura = new TimeSpan(9, 0, 0);
            TimeSpan fechamento = new TimeSpan(19, 0, 0);
            TimeSpan intervalo = new TimeSpan(1, 0, 0);
            TimeSpan horarioAtual = abertura;

            while (horarioAtual <= fechamento)
            {
                TimeSpan horarioAlmocoInicio = new TimeSpan(12, 0, 0);
                TimeSpan horarioAlmocoFim = new TimeSpan(13, 0, 0);

                if (horarioAtual < horarioAlmocoInicio || horarioAtual >= horarioAlmocoFim)
                {
                    todosOsHorarios.Add(horarioAtual.ToString(@"hh\:mm"));
                }
                horarioAtual = horarioAtual.Add(intervalo);
            }

            
            List<string> horariosLivres = todosOsHorarios.Except(horariosOcupados).ToList();

            return Json(horariosLivres);
        }

        return BadRequest("Data inválida");
    }
}
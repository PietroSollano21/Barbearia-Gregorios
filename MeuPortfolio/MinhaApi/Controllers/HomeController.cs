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
using System.Data;
using System.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MySql.Data.MySqlClient;
using Barbearia.Services;
using MercadoPago.Config;
using MercadoPago.Client.Payment;
using MercadoPago.Resource.Payment;
using System.Security.Cryptography.X509Certificates;


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
    [HttpPost]
         public async Task<IActionResult> ConfirmarAgendamento(string nome, string data, string hora, string corte, string valor)
        {
            string Email = User.Identity.Name;
           

             MercadoPagoConfig.AccessToken = "TEST-7924299277998791-042410-c0ede1ae8aaeb41b355ae90a65caf0bd-2350643855";
        decimal valorDecimal = Convert.ToDecimal(valor.Replace(",", "."));

        var request = new PaymentCreateRequest
        {
            TransactionAmount = valorDecimal,
            Description = $"Corte: {corte} - Cliente: {nome}",
            PaymentMethodId= "pix",
            Payer = new PaymentPayerRequest
            {
                Email = Email,
                FirstName = nome,
            }
        };
        var client = new PaymentClient();
        Payment payment = await client.CreateAsync(request);
        Agendamento novo = new Agendamento
        {
            NomeCliente = nome,
            Data = DateTime.Parse(data),
            Hora = TimeSpan.Parse(hora),
            Corte = corte,
            Valor = valorDecimal
        };
       new AgendamentoRepository(_context).SalvarAgendamento(novo);
        var dadosPix = new Pagamento
        {
            CopiaECola = payment.PointOfInteraction.TransactionData.QrCode,
            QrCodeBase64 = payment.PointOfInteraction.TransactionData.QrCodeBase64,
            Valor = valorDecimal
        };
        return View("Pagamento", dadosPix);
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
            TimeSpan intervalo = new TimeSpan(0, 30, 0);
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
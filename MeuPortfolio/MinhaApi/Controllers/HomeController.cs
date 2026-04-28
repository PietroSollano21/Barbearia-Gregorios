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
using System.Net;
using System.Reflection.Metadata;
using System.Net.Mime;


public class HomeController : Controller
{
    private readonly AppDbContext _context;
    public HomeController(AppDbContext context)
    {
        _context = context;
    }
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
        [HttpPost]
        [Route("pagamento/webhook")]
        public async Task<IActionResult> MercadoPagoWebhook([FromBody] Payment payment)
        {
            if (Type == "payment" )
            {
               var pagamento = await paymentClient.GetAsync(long.Parse(id));
               if (pagamento.Status == PaymentStatus.Approved)
               {
                   var agendamento = _context.Agendamentos.FirstOrDefault(a => a.Id == pagamento.Id);
                       agendamento.statuspagamento = "Pago";
                       _context.SaveChanges();
                   
               }
            }

            return Ok();
        }
    
    
    [Authorize]
    public IActionResult Dashboard()
    { 
        if (_context == null)
        {
            return Content("Err0r: Contexto de banco de dados não disponível.");
        }
        if (User == null || User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
        {
            return RedirectToAction("Login", "Usuario");
        }
        string emailLogado = User.Identity.Name;
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == emailLogado);
        if (usuario == null)
        {
            return RedirectToAction("Login", "Usuario");
        }
        var meusAgendamentos = _context.Agendamentos.Where(a => a.NomeCliente == usuario.Nome).OrderBy(a => a.Data).ToList();
        return View(meusAgendamentos);
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
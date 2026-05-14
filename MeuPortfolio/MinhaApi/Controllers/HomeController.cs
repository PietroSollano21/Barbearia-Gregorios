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
using MercadoPago.Error;
using MercadoPago.Client.Common;
using Microsoft.EntityFrameworkCore;


public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly AgendamentoRepository _repository;
    private readonly IConfiguration _configuration;

    public HomeController(AppDbContext context, IConfiguration configuration, AgendamentoRepository agendamentoRepository)
    {
        _context = context;
        _repository = agendamentoRepository;
        _configuration = configuration;
    }
    public IActionResult Index()
    {
        if(User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Dashboard");
        }
        return View();
    }
   
public IActionResult AprovarTeste(long id)
{
    var agendamento = _context.Agendamentos.Find(id);
    if (agendamento != null)
    {
        agendamento.statuspagamento = "approved";
        _context.SaveChanges();
        return Content("Sucesso! Agora olha lá na Dashboard.");
    }
    return Content("ID não encontrado.");
}
    [HttpPost]
public async Task<IActionResult> ConfirmarAgendamento(string nome,string email ,string data, string hora, string corte, string valor)
{
    string emailCliente = User.Identity?.Name ?? "cliente@sememail.com";
    MercadoPagoConfig.AccessToken = "TEST-7924299277998791-042410-c0ede1ae8aaeb41b355ae90a65caf0bd-2350643855";
    
    decimal valorDecimal = Convert.ToDecimal(valor.Replace(",", "."));

    var novoAgendamento = new Agendamento
    {
        NomeCliente = nome,
        Data = DateTime.Parse(data),
        Hora = TimeSpan.Parse(hora),
        Corte = corte,
        Valor = valorDecimal,
        statuspagamento = "Pendente"
    };

    _context.Agendamentos.Add(novoAgendamento);
    await _context.SaveChangesAsync();

    
    var request = new PaymentCreateRequest
    {
        TransactionAmount = 01.08m,
        ExternalReference = novoAgendamento.Id.ToString(),
        Description = $"Teste de integraçao Pix",
        PaymentMethodId = "pix",
        Payer = new PaymentPayerRequest
        {
            Email = emailCliente,
            FirstName = nome,
            Identification = new IdentificationRequest
            {
                Type = "CPF",
                Number = "82721197061"
        },},
        
        NotificationUrl = "https://oxygen-egging-said.ngrok-free.dev/pagamento/webhook"
    };
    var client = new PaymentClient();
    
    
    var resultadoMP = await client.CreateAsync(request);
    Console.WriteLine($"Debug: o codigo do pix gerado e:{resultadoMP.PointOfInteraction?.TransactionData?.QrCode}");
    Console.WriteLine($"Requisição de pagamento criada com ID: {resultadoMP.Id}");
    novoAgendamento.PaymentId = resultadoMP.Id; 
    await _context.SaveChangesAsync();

    var ViewModel = new PagamentoViewModel
    {
        AgendamentoId = novoAgendamento.Id ?? 0,
        Valor = valorDecimal,
        QrCode = resultadoMP.PointOfInteraction.TransactionData.QrCode,
        QrCodeBase64 = resultadoMP.PointOfInteraction.TransactionData.QrCodeBase64
};
    return View("Pagamento", "Home");
}
   
    


[HttpPost]
[IgnoreAntiforgeryToken]
[Route("webhook")]
public async Task<IActionResult> Webhook([FromQuery(Name = "data.id")] string dataId, [FromQuery] string id, [FromQuery] string type, [FromQuery] string topic)
{
   MercadoPagoConfig.AccessToken = "TEST-7924299277998791-042410-c0ede1ae8aaeb41b355ae90a65caf0bd-2350643855";
    string idPagamento = id ?? dataId;
    string tipoEvento = type ?? topic;
    
    try
    {
    if (tipoEvento == "payment" && !string.IsNullOrEmpty(idPagamento))
    {
        var paymentClient = new PaymentClient(); 
        Payment pagamento = await paymentClient.GetAsync(long.Parse(idPagamento));
        if (pagamento.Status == "approved")
        {
            
            int idAgendamento = int.Parse(pagamento.ExternalReference);
            var agendamento = await _context.Agendamentos.FindAsync(idAgendamento);
            
            if (agendamento != null)
            {    
                agendamento.statuspagamento = "Pago";
                await _context.SaveChangesAsync();     
            }
            else
                    {
                        return Content("Erro: Agendamento não encontrado para o pagamento ID " + idPagamento);
                    }
        }
    }
    }
    catch (MercadoPagoApiException ex) when (ex.StatusCode == 404)
    {
        Console.WriteLine($"⚠️ AVISO: Notificação de teste recebida. ID {idPagamento} não existe no MP.");
        return Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao processar webhook: {ex.Message}");
        return StatusCode(500, "Erro interno ao processar webhook");
    }
    return Ok();
}
   public IActionResult teste()
    {
        return Content("Teste de endpoint funcionando!");
    } 
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelarAgendamento(long? id)
    {
        
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento == null)
        {
            return NotFound();
        }
        if(!agendamento.Cancelado )
        {
            TempData["Erro"] = "Nao é possível cancelar um agendamento com menos de 6 horas de antecedência.";
            return RedirectToAction("Dashboard");
        }
        try
        {
            
            var client = new PaymentClient();
             if(agendamento.PaymentId.HasValue)
             {
             await client.RefundAsync(agendamento.PaymentId.Value);
             }
            _context.Agendamentos.Remove(agendamento);
         
            await _context.SaveChangesAsync();
            TempData["Sucesso"]= "Corte cancelado e dinheiro reembolsado com sucesso!";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao cancelar agendamento: {ex.Message}");
            TempData["Erro"] = "Erro ao cancelar o agendamento.";
        }

        return RedirectToAction("Dashboard");
    }
    
    [Authorize]
    public IActionResult Dashboard()
    { 
        if(User.Identity.IsAuthenticated && User.IsInRole("Barbeiro"))
        {
            return RedirectToAction("Barbeiro", "Adm");
        }
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
        var meusAgendamentos = _context.Agendamentos.Where(a => a.NomeCliente == usuario.Nome && (a.statuspagamento == "pago" || a.statuspagamento == "approved")).OrderBy(a => a.Data).ToList();
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
            
            var horariosOcupadosTimeSpan = _repository.BuscarHorariosOcupados(data);
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
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ConsultarStatus(int id)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento == null)
        {
            return NotFound();
        }
        return Json(new { status = agendamento.statuspagamento });
    }
    
}
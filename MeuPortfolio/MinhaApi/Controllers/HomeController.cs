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

namespace Barbearia.Controllers
{
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
    [HttpGet]
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
public async Task<IActionResult> ConfirmarAgendamento(string nome,string email ,string data, string hora, string servico, string CPF, string formapagamento, string statuspagamento, string BarbeiroNome, int IdCorte, Agendamento model )
{
var barbeiroId = User.Identity.Name;
    var emailBarbeiro = await _context.Usuarios
    .Where(u => u.Nome == model.BarbeiroNome)
    .Select(u => u.Email)
    .FirstOrDefaultAsync();
var datasBloqueadas = await _context.DiaBarbeiros
    .Where(d => !d.Disponivel &&
                d.Data >= DateTime.Today)
    .Select(d => d.Data.ToString("yyyy-MM-dd"))
    .ToListAsync();
    ViewBag.DatasBloqueadas = datasBloqueadas;
    Console.WriteLine($"BarbeiroId salvo no banco: {User.Identity?.Name}");
Console.WriteLine($"BarbeiroNome do agendamento: {model.BarbeiroNome}");
var dataAgendamento = DateTime.Parse(data);
Console.WriteLine($"Email {emailBarbeiro} encontrado para o barbeiro.");
    Console.WriteLine($"Data do agendamento: {model.Data.Date}");
    
 var bloqueado = await _context.DiaBarbeiros.AnyAsync(d => d.BarbeiroId == emailBarbeiro && d.Data.Date == dataAgendamento.Date && !d.Disponivel);
    Console.WriteLine($"Barbeiro bloqueado nesta data? {bloqueado}");
            var servicoescolhido = await _context.Servicos.FindAsync(IdCorte);
    if(servicoescolhido == null)
        {
            return BadRequest("Escolha um Serviço");
        }
    string emailCliente = User.Identity?.Name ?? "cliente@sememail.com";
    MercadoPagoConfig.AccessToken = "TEST-7924299277998791-042410-c0ede1ae8aaeb41b355ae90a65caf0bd-2350643855";
    
    decimal valorDecimal = servicoescolhido.Preco;
    var novoAgendamento = new Agendamento
    {
        NomeCliente = nome,
        Data = DateTime.Parse(data),
        Hora = TimeSpan.Parse(hora),
        Corte = servicoescolhido.NomeCorte,
        Valor = servicoescolhido.Preco,
        statuspagamento = formapagamento == "Pix" ? "Pendente" : "Pagar na hora",
        EmailCliente = emailCliente,
        BarbeiroNome = BarbeiroNome 
    };
    //var barbeiroFolga = await _context.DiaBarbeiros.AnyAsync(d => d.BarbeiroId == BarbeiroNome && d.Data.Date == novoAgendamento.Data.Date && !d.Disponivel);
  if(bloqueado)
            {
              TempData["Erro"] = "Este barbeiro está de folga nesta data.";
            return RedirectToAction("Agenda");
              //return Content("O barbeiro está indisponível nesta data. Por favor, escolha outra data ou barbeiro.");  
            }
Console.WriteLine("Email que vai salvar '{novoAgendamento.EmailCliente}'");
    _context.Agendamentos.Add(novoAgendamento);
    await _context.SaveChangesAsync();
    
    if (formapagamento == "Pix")
    {
    var request = new PaymentCreateRequest
    {
        TransactionAmount = valorDecimal,
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
                Number = CPF
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
        CopiaECola = resultadoMP.PointOfInteraction?.TransactionData?.QrCode ?? string.Empty,
        QrCode = resultadoMP.PointOfInteraction?.TransactionData?.QrCode,
        QrCodeBase64 = resultadoMP.PointOfInteraction?.TransactionData?.QrCodeBase64?? string.Empty,    
};
    
       
    return View("Pagamento", ViewModel);
    }
       else
        {
            return RedirectToAction("Dashboard", "Home");
        }  
}

[HttpGet]
public IActionResult Pagamento()
{
    return View();
}


[HttpPost]
[IgnoreAntiforgeryToken]
[Route("webhook")]
public async Task<IActionResult> Webhook([FromQuery] string id, [FromQuery] string type, [FromBody] System.Text.Json.JsonElement body)
{
    Console.WriteLine("webhook chamou bucetya");
    MercadoPagoConfig.AccessToken = "TEST-7924299277998791-042410-c0ede1ae8aaeb41b355ae90a65caf0bd-2350643855";
    string idPagamento = id;
    string tipoEvento = type;
    try
    {
        if (body.ValueKind == System.Text.Json.JsonValueKind.Object)
        {
            if (body.TryGetProperty("type", out var typeProp)) tipoEvento = typeProp.GetString();
            else if (body.TryGetProperty("action", out var actionProp)) tipoEvento = actionProp.GetString();
            if (body.TryGetProperty("data", out var dataProp))
            {
                if (dataProp.ValueKind == System.Text.Json.JsonValueKind.Object && dataProp.TryGetProperty("id", out var idProp))
                {
                    idPagamento = idProp.ValueKind == System.Text.Json.JsonValueKind.Number ? idProp.GetInt64().ToString() : idProp.GetString();
                }
                else if (dataProp.ValueKind == System.Text.Json.JsonValueKind.Number || dataProp.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    idPagamento = dataProp.GetRawText().Trim('"');
                }
            }
            else if (body.TryGetProperty("id", out var rootIdProp))
            {
                idPagamento = rootIdProp.ValueKind == System.Text.Json.JsonValueKind.Number ? rootIdProp.GetInt64().ToString() : rootIdProp.GetString();
            }
        }
        if (tipoEvento == "payment.updated") tipoEvento = "payment";
        if (string.IsNullOrEmpty(tipoEvento)) tipoEvento = "payment";
        Console.WriteLine($"🧾 DADOS RECEBIDOS -> Tipo: {tipoEvento} | ID: {idPagamento}");
        if (tipoEvento == "payment" && !string.IsNullOrEmpty(idPagamento))
        {
            Console.WriteLine($"🔍 Processando pagamento ID: {idPagamento}");
            var paymentClient = new PaymentClient(); 
            Payment pagamento = await paymentClient.GetAsync(long.Parse(idPagamento));

            if (pagamento.Status == "Pago" || pagamento.Status == "pending")
            {
                var agendamento = await _context.Agendamentos
                    .FirstOrDefaultAsync(a => a.PaymentId == long.Parse(idPagamento));
                if (agendamento == null && !string.IsNullOrEmpty(pagamento.ExternalReference))
                {
                    int idAgendamento = int.Parse(pagamento.ExternalReference);
                    agendamento = await _context.Agendamentos.FindAsync(idAgendamento);
                }
                if (agendamento != null)
                {    
                    agendamento.statuspagamento = "Pago";
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"✅ SUCESSO: Agendamento ID {agendamento.Id} atualizado para PAGO no MySQL!");
                    return Ok();
                }
                else
                {
                    Console.WriteLine($"❌ ERRO: Nenhum agendamento local possui o PaymentId {idPagamento}");
                    return Content("Erro: Agendamento não encontrado.");
                }
            }
        }
    }
    catch (MercadoPagoApiException ex) when (ex.StatusCode == 404)
    {
        Console.WriteLine($"⚠️ AVISO: ID {idPagamento} retornou 404 na API do Mercado Pago.");
        return Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro crítico ao processar webhook: {ex.Message}");
        return StatusCode(500, "Erro interno ao processar webhook");
    }

    return Ok();
}


   public IActionResult teste()
    {
        return Content("Teste de endpoint funcionando!");
    } 
    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelarAgendamentos(long? id)
    {
        Console.WriteLine("CHEGOU");
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento == null)
        {
            return NotFound();
        }
        if(agendamento.Cancelado )
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
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Dashboard()
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
        
        DateTime dataHoje = DateTime.Today;
       var meusAgendamentos = await _context.Agendamentos
    .Where(a => a.EmailCliente == usuario.Email)
    .Where(a =>
        a.statuspagamento == "Pago" ||
        a.statuspagamento == "Pagar na hora")
    .Where(a => a.Data.Date >= dataHoje)
    .OrderBy(a => a.Data)
    .ThenBy(a => a.Hora)
    .ToListAsync();
    return View(meusAgendamentos);
    }
    [Authorize]
    public IActionResult Agenda()
    {
        ViewBag.Servicos = _context.Servicos.ToList();
        return View();
    }
    [Authorize]
    [HttpGet]
    public IActionResult BuscarHorarios(string dataSelecionada)
    {
        if (DateTime.TryParse(dataSelecionada, out DateTime data))
        {
            DateTime dataHoje = DateTime.Today;
            if (data.Date < dataHoje)
            {
                return Json(new List <string>());
            }
            var horariosOcupadosTimeSpan = _repository.BuscarHorariosOcupados(data);
            List<string> horariosOcupados = horariosOcupadosTimeSpan.Select(t => t.ToString(@"hh\:mm")).ToList();
            List<string> todosOsHorarios = new List<string>();
            TimeSpan abertura = new TimeSpan(9, 0, 0);
            TimeSpan fechamento = new TimeSpan(19, 0, 0);
            TimeSpan intervalo = new TimeSpan(0, 30, 0);
            TimeSpan horarioAtual = abertura;
            TimeSpan horaAgora = DateTime.Now.TimeOfDay;
            
            while (horarioAtual <= fechamento)
            {
                TimeSpan horarioAlmocoInicio = new TimeSpan(12, 0, 0);
                TimeSpan horarioAlmocoFim = new TimeSpan(13, 0, 0);

                if (horarioAtual < horarioAlmocoInicio || horarioAtual >= horarioAlmocoFim)
                {
                    if(data.Date == dataHoje)
                    {
                        if(horarioAtual > horaAgora){
                    todosOsHorarios.Add(horarioAtual.ToString(@"hh\:mm"));
                        }
                }
                else
                    {
                        todosOsHorarios.Add(horarioAtual.ToString(@"hh\:mm"));
                    }
                   
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
}
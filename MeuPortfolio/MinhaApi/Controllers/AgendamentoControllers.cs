using Microsoft.AspNetCore.Mvc;
using Barbearia.Models; 
using Babearia.Services;
using Barbearia.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace MinhaApi.Controllers
{
    [ApiController]
[Route("api/[controller]")]

    public class AgendamentoController : ControllerBase
    {
        private readonly AgendamentoService _agendamentoService;
        private readonly AgendamentoRepository _agendamentoRepository;

        public AgendamentoController(AgendamentoService agendamentoService, AgendamentoRepository agendamentoRepository)
        {
            _agendamentoService = agendamentoService;
            _agendamentoRepository = agendamentoRepository;
        }

        [HttpPost]
        public IActionResult Criar(Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {
                _agendamentoService.CalcularValor(agendamento);
                _agendamentoRepository.Adicionar(agendamento);
                return RedirectToAction("Index", "Home");
            }
        return View("Confirmacao",agendamento);
        }
        
[Authorize]
[HttpPost("agendar")]
public IActionResult Agendar()
{
    return Ok("Agendamento realizado com sucesso");
}
    }
}

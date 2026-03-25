using Microsoft.AspNetCore.Mvc;
using Barbearia.Models; 
using Barbearia.Services;
using Barbearia.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Barbearia.Controllers
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _agendamentoService.CalcularValor(agendamento);
            _agendamentoRepository.Adicionar(agendamento);

            return Ok(agendamento);
        }

        [Authorize]
        [HttpPost("agendar")]
        public IActionResult Agendar()
        {
            return Ok("Agendamento realizado com sucesso");
        }
    }
}
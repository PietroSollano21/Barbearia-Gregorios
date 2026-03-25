using Barbearia.Data;
using Barbearia.Models;

namespace Barbearia.Repositories
{
    public class AgendamentoRepository
    {
        private readonly AppDbContext _context;

        public AgendamentoRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Adicionar(Agendamento agendamento)
        {
            _context.Agendamentos.Add(agendamento);
            _context.SaveChanges();
        }
    }
}
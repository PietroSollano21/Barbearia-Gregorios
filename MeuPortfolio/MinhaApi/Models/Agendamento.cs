namespace Barbearia.Models
{
    public class Agendamento
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public string Servico { get; set; }
        public decimal Valor { get; set; }

    }
}
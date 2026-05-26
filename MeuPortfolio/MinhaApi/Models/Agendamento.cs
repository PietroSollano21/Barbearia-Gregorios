using System.ComponentModel.DataAnnotations.Schema;

namespace Barbearia.Models
{
    [Table("agendamento")]
    public class Agendamento
    {
        public long? Id { get; set; }
        public string? NomeCliente { get; set; } = string.Empty;
        [Column("DataDia")]
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public string Corte { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public required string? statuspagamento  { get; set; }
        public long? PaymentId { get; set; }
        public bool Cancelado 
        { 
            get
            {
                DateTime momentocorte = Data.Date.Add(Hora);
                return momentocorte < DateTime.Now.AddHours(6);
            }
        }
        public string? BarbeiroNome { get; set; } = string.Empty; 
        public string EmailCliente { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace Barbearia.Models
{
    [Table("agendamento")]
    public class Agendamento
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        [Column("DataDia")]
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public string Corte { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string statuspagamento { get; set; } = "Pendente";

    }
}
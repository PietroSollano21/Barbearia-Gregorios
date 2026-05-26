using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Barbearia.Enums;

namespace Barbearia.Models
{
    public class Servico
    {
        [Key]
        public int IdCorte { get; set; }
        public  TipoCorte Tipo { get; set; } 
        public string NomeCorte { get; set; } 
        public decimal Preco { get; set; } 
    }
}
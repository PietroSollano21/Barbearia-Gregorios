using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barbearia.Models
{
public class Usuario
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; }= string.Empty;

    [NotMapped]
    public string Senha { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
}
}
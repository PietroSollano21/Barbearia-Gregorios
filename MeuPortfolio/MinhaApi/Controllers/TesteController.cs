using Microsoft.AspNetCore.Mvc;
using Barbearia.Data;
using System.Runtime.ExceptionServices;

[ApiController]
[Route("api/[controller]")]
public class TesteController : ControllerBase
{
    private readonly Conexao _conexao;

    public TesteController(Conexao conexao)
    {
        _conexao = conexao;
    }
    [HttpGet]
    public string Testar()
    {
        using var conn = _conexao.GetConnection();
        conn.Open();
        return "Conexão bem sucedida!";
    }
}
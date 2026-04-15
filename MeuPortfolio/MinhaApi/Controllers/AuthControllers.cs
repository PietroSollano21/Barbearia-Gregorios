using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Barbearia.Models;
using Barbearia.Repositories;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(LoginDTO login)
    {
        // Simulação (depois vamos ligar no banco)
        if (login.Email != "admin@email.com" || login.Senha != "123")
            return Unauthorized("Email ou senha inválidos");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("CHAVE_SUPER_SECRETA_123");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, login.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new
        {
            token = tokenHandler.WriteToken(token)
        });
    }
    private readonly UsuarioRepository _repo;
    public AuthController(UsuarioRepository repo)
    {
        _repo = repo;
    }
    [HttpPost("register")]
    public IActionResult Register(Usuario usuario)
    {
        _repo.Cadastrar(usuario);
        return Ok("Usuário cadastrado com sucesso!");
    }
    [HttpPost("login2")]
    public IActionResult Login([FromBody]Usuario usuario)
    {
        var user = _repo.Login(usuario.Email, usuario.SenhaHash);
    if (user==null)
    return Unauthorized("Email ou senha inválidos");
    return Ok(user);
    }
}
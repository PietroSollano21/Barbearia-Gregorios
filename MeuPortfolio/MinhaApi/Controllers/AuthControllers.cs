using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text;
using Barbearia.Models;
using Barbearia.Repositories;
using System.Net;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Barbearia.Data;
using System.Net.Cache;


public class AuthController : Controller
{
 private readonly AppDbContext _context;
 private readonly UsuarioRepository _repo;
public AuthController(AppDbContext context, UsuarioRepository repo)
{
    _context = context;
    _repo = repo;
}
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO login)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == login.Email && u.SenhaHash == login.Senha);
        if (usuario != null && BCrypt.Net.BCrypt.Verify(login.Senha, usuario.SenhaHash))
        {
           var claims = new List<Claim>// Criamos uma lista de claims para armazenar as informações do usuário
           {
               new Claim(ClaimTypes.Name, usuario.Email), // Usamos o email como nome de usuário
               new Claim("Id", usuario.Id.ToString()), // Adiciona o ID do usuário como claim personalizada
               new Claim(ClaimTypes.Role, usuario.Perfil) // Adiciona a role do usuário como claim
           };
              var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
              await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = true });
              if(usuario.Perfil == "Barbeiro")
              {
                  return RedirectToAction("Barbeiro", "Adm");
              }
              return RedirectToAction("Dashboard", "Home");
        }
        ViewBag.Erro = "Email ou senha invalidos";
        return View("~/Views/Usuario/Login.cshtml");
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
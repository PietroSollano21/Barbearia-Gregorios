using Microsoft.AspNetCore.Mvc;
using Barbearia.Data;
using Barbearia.Models;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Barbearia.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Barbearia.Controllers
{
   


    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Cadastro(Usuario usuario)
        {
            var usuarioExistente = _context.Usuarios.FirstOrDefault(u => u.Email == usuario.Email || u.Nome == usuario.Nome); ;
            if (usuarioExistente != null)
            {
                ViewBag.Erro = "Email ou nome já cadastrados.";
                return View();
            }

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return RedirectToAction("Login", "Usuario");
        }
        [HttpGet]
        public async Task<IActionResult> Cadastrar([Bind("Id,Nome,Email,Senha,CPF")] Usuario usuarios)
        {
            if(ModelState.IsValid)
            {
             if (!IsCpfvalido(usuarios.CPF))
        {
            ModelState.AddModelError("CPF", "CPF inválido.");
            return View();
        }
        _context.Usuarios.Add(usuarios);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Dashboard));
            }
            return View("Cadastro","Usuario");
        }
        private bool IsCpfvalido(string cpf)
    {
        if(string.IsNullOrEmpty(cpf))
            return false;
        cpf = cpf.Replace(".", "").Replace("-", "");
        if(cpf.Length != 11 || cpf.All(c => c == cpf[0]))
            return false;
            bool todosiguais = true;
            for(int i = 1; i < cpf.Length; i++)
            {
                if(cpf[i] != cpf[0])
                {
                    todosiguais = false;
                    break;
                }
            }
            if(todosiguais)
                return false;
        int[] multiplicador1 = new int[9] {10, 9, 8, 7, 6, 5, 4, 3, 2};
        int[] multiplicador2 = new int[10] {11, 10, 9, 8, 7, 6, 5, 4, 3, 2};
    string tempCpf = cpf.Substring(0, 9);
    int soma;
    int resto;

    tempCpf = cpf.Substring(0, 9);
    soma = 0;
    for(int i = 0; i < 9; i++)
        soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
        resto = soma % 11;
        if(resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        string digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;
        for(int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
        resto = soma % 11;
       
        if(resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        digito = digito + resto.ToString();
        return cpf.EndsWith(digito);
    }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
       [HttpPost]
 public async Task<IActionResult> Login(string email, string senha)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario != null && BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Email),
                    new Claim("Id", usuario.Id.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Perfil)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
               
               if(usuario.IsAdmin)
                {
                    return RedirectToAction("Barbeiro", "Adm");
                }
                else{
                return RedirectToAction("Dashboard", "Home");
                }
            }
            else
            {
            ViewBag.Error = "Email ou senha inválidos.";
            return View();
            }
        }
        [Authorize]
        public IActionResult Dashboard()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Barbeiro"))
            {
                return RedirectToAction("Barbeiro", "Adm");
            }
            return View();
        }
}
}
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Cache;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using MySql.Data.MySqlClient;
using Barbearia.Models;
using Barbearia.Repositories;
using Barbearia.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<Conexao>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login.html";
        options.AccessDeniedPath = "/login.html";
    });

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 

app.MapGet("/testar-conexao",() => 
{
    try
    {
        var connStr = "Server=localhost;Database=barbeariadb;user=sollano;password=cavalo;";
        using var conn = new MySqlConnection(connStr);
        conn.Open();
        return Results.Ok("Conexão bem sucedida!");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Erro ao conectar: {ex.Message}");
    }
});



app.MapPost("/cadastro",async (HttpContext context, UsuarioRepository repo) =>
{
    var form = await context.Request.ReadFormAsync();
       var nome = form["nome"].ToString();
       var email = form["email"].ToString();
        var SenhaHash = BCrypt.Net.BCrypt.HashPassword(form["senha"].ToString());
        try
        {
            var addr = new MailAddress(email).ToString();
        }
        catch (System.Exception)
        {
            return Results.BadRequest("Email invalido");
        };
    var usuario = new Usuario
    {
        Nome = nome,
        Email = email,
        SenhaHash = SenhaHash
    };
    repo.Cadastrar(usuario);
    return Results.Redirect("/", false, true);
});

app.MapPost("/login", async(HttpContext context, UsuarioRepository repo)=>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var senha = form["senha"].ToString();
    Console.WriteLine("email recebido: " + email);
    Console.WriteLine("senha recebida: " + senha);
    var usuario = repo.BuscaPorEmail(email);
    if (usuario == null)
    {
        return Results.BadRequest("Usuario não encontrado");
    }
    bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);
    if (!senhaValida)
    {
        return Results.BadRequest("Senha incorreta");
    }
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.Nome),
        new Claim(ClaimTypes.Email, usuario.Email)
    };
    var identity = new ClaimsIdentity(claims,"cookie");
    var principal = new ClaimsPrincipal(identity);
    await context.SignInAsync("cookie",principal);
    return Results.Ok("Login Realizado com Sucesso!");
});
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync("cookie");
    return Results.Redirect("/login.html", false, true);
});
app.MapGet("/area-logada",(HttpContext context)=>
{
    var nome = context.User.Identity.Name;
    return Results.Ok($"Bem-vindo, {nome}!");
})
.RequireAuthorization();
app.MapGet("/eu",(HttpContext context)=>
{
    if (!context.User.Identity.IsAuthenticated)
    {
        return Results.Unauthorized();
    }
    return Results.Ok(new
    {
        nome = context.User.Identity.Name,
        email = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
});
    });
app.Run();


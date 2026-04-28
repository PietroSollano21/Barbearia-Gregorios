using Barbearia.Data;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Cache;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using MySql.Data.MySqlClient;
using Barbearia.Models;
using Barbearia.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<Conexao>();
builder.Services.AddScoped<UsuarioRepository>();

   string conexao = "Server=localhost;Database=barbeariadb;user=sollano;password=cavalo;";
   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseMySql(conexao, ServerVersion.AutoDetect(conexao)));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuario/Login";
        options.LogoutPath = "/Usuario/Logout";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });
    builder.Services.AddControllersWithViews();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
); 

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



app.MapPost("/Cadastro",async (HttpContext context, UsuarioRepository repo) =>
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
    return Results.Redirect("http://localhost:5165/index.html", false, true);
});
app.Run();


var builder = WebApplication.CreateBuilder(args);

// 🔹 TODOS os serviços vêm ANTES do Build
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Configuração do pipeline vem depois
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); // 🔥 MUITO IMPORTANTE

app.Run();
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

public class Conexao
{
    private readonly string _connectionString;

    public Conexao(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Database=barbeariadb;user=sollano;password=cavalo;";
    }

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
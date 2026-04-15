using MySql.Data.MySqlClient;
using Barbearia.Data;
using Barbearia.Models;
using System.Net.Mail;


namespace Barbearia.Repositories
{   
public class UsuarioRepository
{
    private readonly Conexao _conexao;
    public UsuarioRepository(Conexao conexao)
    {
        _conexao = conexao;
    }
    public void Cadastrar(Usuario usuario)
    {
        using var conn = _conexao.GetConnection();
        conn.Open();

        string query = "INSERT INTO Usuarios (Nome, Email, SenhaHash) VALUES (@Nome, @Email, @SenhaHash)";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
        cmd.Parameters.AddWithValue("@Email", usuario.Email);
        cmd.Parameters.AddWithValue("@SenhaHash", usuario.SenhaHash);

        cmd.ExecuteNonQuery();
    }

    
    
    public Usuario Login(string email, string senha)
    {
        using var conn = _conexao.GetConnection();
        conn.Open();

        string query = "SELECT * FROM Usuarios WHERE Email = @Email";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Email", email);



        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var usuario = new Usuario
            {
                Id = reader.GetInt32("Id"),
                Nome = reader.GetString("Nome"),
                Email = reader.GetString("Email"),
                SenhaHash = reader.GetString("SenhaHash")
            };

            if (BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            {
                return usuario;
            }
        }

        return null;
    
    }
    public bool EmailExiste(string email)
    {
        using var conn = _conexao.GetConnection();
        conn.Open();

        string query = "SELECT COUNT(*) FROM Usuarios WHERE Email = @Email";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Email", email);

        long count = (long)cmd.ExecuteScalar();
        return count > 0;
    }

    public Usuario? BuscaPorEmail(string email)
    {
        using var conn = _conexao.GetConnection();
        conn.Open();

        string query = "SELECT * FROM Usuarios WHERE Email = @Email";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Email", email);
        
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Usuario
            {
            Id = reader.GetInt32("Id"),
            Nome = reader.GetString("Nome"),
            Email = reader.GetString("Email"),
            SenhaHash = reader.GetString("SenhaHash")    
        };
    }
    return null;
}}}
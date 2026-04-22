using Barbearia.Data;
using Barbearia.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Barbearia.Data;

namespace Barbearia.Repositories
{
    public class AgendamentoRepository
    {
        private readonly AppDbContext _context;

        public AgendamentoRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Adicionar(Agendamento agendamento)
        {
            _context.Agendamentos.Add(agendamento);
            _context.SaveChanges();
        }
        public void SalvarAgendamento(Agendamento agendamento)
        {
            using (MySqlConnection conexao = new MySqlConnection(connString))
            {
                conexao.Open();
                string query = "INSERT INTO Agendamento (NomeCliente, DataDia, Hora, Corte, Valor) VALUES (@NomeCliente, @DataDia, @Hora, @Corte, @Valor)";
                MySqlCommand comando = new MySqlCommand(query, conexao);
                comando.Parameters.AddWithValue("@NomeCliente", agendamento.NomeCliente);
                comando.Parameters.AddWithValue("@DataDia", agendamento.Data);
                comando.Parameters.AddWithValue("@Hora", agendamento.Hora);
                comando.Parameters.AddWithValue("@Corte", agendamento.Corte);
                comando.Parameters.AddWithValue("@Valor", agendamento.Valor);
                comando.ExecuteNonQuery();
            }
        }
    private string connString = "server=localhost;database=barbeariadb;user=sollano;password=cavalo";
    public List<TimeSpan> BuscarHorariosOcupados(DateTime data)
{
    var ocupados = new List<TimeSpan>(); 

    using (var conexao = new MySqlConnection(connString))
    {
        conexao.Open();
        string sql = "SELECT Hora FROM agendamentos WHERE DataDia = @data";
        
        using (var cmd = new MySqlCommand(sql, conexao))
        {
            cmd.Parameters.AddWithValue("@data", data.Date);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ocupados.Add(reader.GetTimeSpan(0));
                }
            }
        } 
    }
    return ocupados;
}
    }

}
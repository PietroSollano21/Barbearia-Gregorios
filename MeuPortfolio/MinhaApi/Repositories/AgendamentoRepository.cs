using Barbearia.Data;
using Barbearia.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Barbearia.Data;
using System.Linq;
using Barbearia.Controllers;
using Microsoft.AspNetCore.Authorization;
using Barbearia.Services;


namespace Barbearia.Repositories
{
    public class AgendamentoRepository
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;
        private readonly IConfiguration Configuration;


        public AgendamentoRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("A string de conexão não foi encontrada.");
            }
        }

        public void Adicionar(Agendamento agendamento)
        {
            _context.Agendamentos.Add(agendamento);
            _context.SaveChanges();
        }
      

        [Authorize]
        public void SalvarAgendamento(Agendamento agendamento)
        {
            using (MySqlConnection conexao = new MySqlConnection(_connectionString))
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
   
    public List<TimeSpan> BuscarHorariosOcupados(DateTime data)
{
    var ocupados = new List<TimeSpan>(); 

    using (var conexao = new MySqlConnection(_connectionString))
    {
        conexao.Open();
        string sql = "SELECT Hora FROM agendamento WHERE DataDia = @data";
        
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
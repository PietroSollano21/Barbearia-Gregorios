using Microsoft.EntityFrameworkCore;
using Barbearia.Models;
using Barbearia.Enums;

namespace Barbearia.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Servico> Servicos {get; set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Servico>().Property(s => s.Tipo).HasConversion<string>();
            modelBuilder.Entity<Servico>().HasKey(s => s.IdCorte);
            modelBuilder.Entity<Servico>().HasData(
                new Servico { IdCorte = 1, Tipo = TipoCorte.Degrade, NomeCorte = "Corte Degradê", Preco = 30.00m},
                new Servico { IdCorte = 2, Tipo = TipoCorte.Barba, NomeCorte = "Barba", Preco = 15.00m},
                new Servico { IdCorte = 3, Tipo = TipoCorte.Sobrancelha, NomeCorte = "Sobrancelha", Preco = 5.00m},
                new Servico { IdCorte = 4, Tipo = TipoCorte.Social, NomeCorte = "Corte Social", Preco = 25.00m},
                new Servico { IdCorte = 5, Tipo = TipoCorte.Pézinho, NomeCorte = "Pézinho", Preco = 10.00m},
                new Servico { IdCorte = 6, Tipo = TipoCorte.Cavanhaque, NomeCorte = "Cavanhaque", Preco = 10.00m},
                new Servico { IdCorte = 7, Tipo = TipoCorte.Alisamento, NomeCorte = "Alisamento", Preco = 25.00m},
                new Servico { IdCorte = 8, Tipo = TipoCorte.CorteTesoura, NomeCorte = "Corte só na Tesoura", Preco = 30.00m},
                new Servico { IdCorte = 9, Tipo = TipoCorte.CorteMáquina, NomeCorte = "Corte só na Máquina", Preco = 15.00m},
                new Servico { IdCorte = 10, Tipo = TipoCorte.CorteCavanhaque, NomeCorte = "Corte e Cavanhaque", Preco = 40.00m},
                new Servico { IdCorte = 11, Tipo = TipoCorte.CortePlatinado, NomeCorte = "Corte e Platinado", Preco = 100.00m},
                new Servico { IdCorte = 12, Tipo = TipoCorte.SocialBarba, NomeCorte = "Social e Barba", Preco = 40.00m},
                new Servico { IdCorte = 13, Tipo = TipoCorte.DegradeAlisamento, NomeCorte = "Degradê e Alisamento", Preco = 55.00m},
                new Servico { IdCorte = 14, Tipo = TipoCorte.DegradeAlisamentoBarba, NomeCorte = "Degradê, Alisamento e Barba", Preco = 70.00m},
                new Servico { IdCorte = 15, Tipo = TipoCorte.DegradeLuzes, NomeCorte = "Degradê e Luzes", Preco = 90.00m},
                new Servico { IdCorte = 16, Tipo = TipoCorte.DegradeReflexo, NomeCorte = "Degradê e Reflexo", Preco = 95.00m},
                new Servico { IdCorte = 17, Tipo = TipoCorte.DegradePigmentaçaoCavanhaque, NomeCorte = "Degradê, Pigmentação e Cavanhaque", Preco = 65.00m},
                new Servico { IdCorte = 18, Tipo = TipoCorte.DegradeRisquinhoPigmentaçao, NomeCorte = "Degradê,Risquinho e Pigmentação", Preco = 65.00m},
                new Servico { IdCorte = 19, Tipo = TipoCorte.RasparGilette, NomeCorte = "Raspar na Gilette", Preco = 20.00m},
                new Servico { IdCorte = 20, Tipo = TipoCorte.Pigmentação, NomeCorte = "Pigmentação", Preco = 25.00m},
                new Servico { IdCorte = 21, Tipo = TipoCorte.DegradeBarba, NomeCorte = "Degradê e Barba", Preco = 45.00m}

            );
        }
    }
}
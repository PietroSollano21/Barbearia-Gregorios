using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MinhaApi.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaServicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CPF",
                table: "Usuarios",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Perfil",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BarbeiroNome",
                table: "agendamento",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    IdCorte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    NomeCorte = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Preco = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.IdCorte);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Servicos",
                columns: new[] { "IdCorte", "NomeCorte", "Preco", "Tipo" },
                values: new object[,]
                {
                    { 1, "Corte Degradê", 30.00m, 0 },
                    { 2, "Barba", 15.00m, 1 },
                    { 3, "Sobrancelha", 5.00m, 2 },
                    { 4, "Corte Social", 25.00m, 3 },
                    { 5, "Pézinho", 10.00m, 4 },
                    { 6, "Cavanhaque", 10.00m, 7 },
                    { 7, "Alisamento", 25.00m, 20 },
                    { 8, "Corte só na Tesoura", 30.00m, 6 },
                    { 9, "Corte só na Máquina", 15.00m, 5 },
                    { 10, "Corte e Cavanhaque", 40.00m, 18 },
                    { 11, "Corte e Platinado", 100.00m, 14 },
                    { 12, "Social e Barba", 40.00m, 10 },
                    { 13, "Degradê e Alisamento", 55.00m, 16 },
                    { 14, "Degradê, Alisamento e Barba", 70.00m, 17 },
                    { 15, "Degradê e Luzes", 90.00m, 12 },
                    { 16, "Degradê e Reflexo", 95.00m, 13 },
                    { 17, "Degradê, Pigmentação e Cavanhaque", 65.00m, 19 },
                    { 18, "Degradê,Risquinho e Pigmentação", 65.00m, 15 },
                    { 19, "Raspar na Gilette", 20.00m, 8 },
                    { 20, "Pigmentação", 25.00m, 9 },
                    { 21, "Degradê e Barba", 45.00m, 11 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropColumn(
                name: "CPF",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Perfil",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "BarbeiroNome",
                table: "agendamento");
        }
    }
}

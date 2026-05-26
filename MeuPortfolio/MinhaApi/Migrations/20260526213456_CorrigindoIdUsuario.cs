using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaApi.Migrations
{
    /// <inheritdoc />
    public partial class CorrigindoIdUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Servicos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "statuspagamento",
                table: "agendamento",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NomeCliente",
                table: "agendamento",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EmailCliente",
                table: "agendamento",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 1,
                column: "Tipo",
                value: "Degrade");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 2,
                column: "Tipo",
                value: "Barba");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 3,
                column: "Tipo",
                value: "Sobrancelha");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 4,
                column: "Tipo",
                value: "Social");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 5,
                column: "Tipo",
                value: "Pézinho");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 6,
                column: "Tipo",
                value: "Cavanhaque");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 7,
                column: "Tipo",
                value: "Alisamento");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 8,
                column: "Tipo",
                value: "CorteTesoura");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 9,
                column: "Tipo",
                value: "CorteMáquina");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 10,
                column: "Tipo",
                value: "CorteCavanhaque");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 11,
                column: "Tipo",
                value: "CortePlatinado");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 12,
                column: "Tipo",
                value: "SocialBarba");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 13,
                column: "Tipo",
                value: "DegradeAlisamento");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 14,
                column: "Tipo",
                value: "DegradeAlisamentoBarba");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 15,
                column: "Tipo",
                value: "DegradeLuzes");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 16,
                column: "Tipo",
                value: "DegradeReflexo");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 17,
                column: "Tipo",
                value: "DegradePigmentaçaoCavanhaque");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 18,
                column: "Tipo",
                value: "DegradeRisquinhoPigmentaçao");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 19,
                column: "Tipo",
                value: "RasparGilette");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 20,
                column: "Tipo",
                value: "Pigmentação");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 21,
                column: "Tipo",
                value: "DegradeBarba");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailCliente",
                table: "agendamento");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "Servicos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "agendamento",
                keyColumn: "statuspagamento",
                keyValue: null,
                column: "statuspagamento",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "statuspagamento",
                table: "agendamento",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "agendamento",
                keyColumn: "NomeCliente",
                keyValue: null,
                column: "NomeCliente",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "NomeCliente",
                table: "agendamento",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 1,
                column: "Tipo",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 2,
                column: "Tipo",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 3,
                column: "Tipo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 4,
                column: "Tipo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 5,
                column: "Tipo",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 6,
                column: "Tipo",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 7,
                column: "Tipo",
                value: 20);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 8,
                column: "Tipo",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 9,
                column: "Tipo",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 10,
                column: "Tipo",
                value: 18);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 11,
                column: "Tipo",
                value: 14);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 12,
                column: "Tipo",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 13,
                column: "Tipo",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 14,
                column: "Tipo",
                value: 17);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 15,
                column: "Tipo",
                value: 12);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 16,
                column: "Tipo",
                value: 13);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 17,
                column: "Tipo",
                value: 19);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 18,
                column: "Tipo",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 19,
                column: "Tipo",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 20,
                column: "Tipo",
                value: 9);

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "IdCorte",
                keyValue: 21,
                column: "Tipo",
                value: 11);
        }
    }
}

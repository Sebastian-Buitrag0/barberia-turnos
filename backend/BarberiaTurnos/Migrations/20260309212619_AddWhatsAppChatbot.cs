using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberiaTurnos.Migrations
{
    /// <inheritdoc />
    public partial class AddWhatsAppChatbot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DiaTurno",
                table: "Turnos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "WhatsAppStates",
                columns: table => new
                {
                    Telefono = table.Column<string>(type: "text", nullable: false),
                    EstadoActual = table.Column<string>(type: "text", nullable: false),
                    NombreTemporal = table.Column<string>(type: "text", nullable: true),
                    BarberoIdTemporal = table.Column<int>(type: "integer", nullable: true),
                    DiaTurnoTemporal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UltimaInteraccion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsAppStates", x => x.Telefono);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhatsAppStates");

            migrationBuilder.DropColumn(
                name: "DiaTurno",
                table: "Turnos");
        }
    }
}

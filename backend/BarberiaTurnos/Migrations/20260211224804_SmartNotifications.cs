using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberiaTurnos.Migrations
{
    /// <inheritdoc />
    public partial class SmartNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicioAtencion",
                table: "Turnos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NivelAviso",
                table: "Turnos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaInicioAtencion",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "NivelAviso",
                table: "Turnos");
        }
    }
}

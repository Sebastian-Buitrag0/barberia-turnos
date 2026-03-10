using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberiaTurnos.Migrations
{
    /// <inheritdoc />
    public partial class FixCompositeClientIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clientes_Telefono",
                table: "Clientes");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Telefono_Nombre",
                table: "Clientes",
                columns: new[] { "Telefono", "Nombre" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clientes_Telefono_Nombre",
                table: "Clientes");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Telefono",
                table: "Clientes",
                column: "Telefono",
                unique: true);
        }
    }
}

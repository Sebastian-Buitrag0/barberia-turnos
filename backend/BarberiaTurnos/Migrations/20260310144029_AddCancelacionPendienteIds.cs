using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberiaTurnos.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelacionPendienteIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelacionPendienteIds",
                table: "WhatsAppStates",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelacionPendienteIds",
                table: "WhatsAppStates");
        }
    }
}

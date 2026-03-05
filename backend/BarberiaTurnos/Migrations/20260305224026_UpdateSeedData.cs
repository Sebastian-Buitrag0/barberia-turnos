using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BarberiaTurnos.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Nombre", "Precio" },
                values: new object[] { "Corte y Barba Marcada", 23000m });

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Nombre", "Precio" },
                values: new object[] { "Corte y Barba", 25000m });

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Nombre", "Precio" },
                values: new object[] { "Corte y Cejas", 20000m });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "Id", "Activo", "Nombre", "Precio" },
                values: new object[,]
                {
                    { 6, true, "Corte y Raya", 19000m },
                    { 7, true, "Corte y Figura", 22000m },
                    { 8, true, "Despunte de Cabello", 10000m },
                    { 9, true, "Barba", 8000m },
                    { 10, true, "Cejas con Cuchilla", 5000m },
                    { 11, true, "Cejas con Cera", 10000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Nombre", "Precio" },
                values: new object[] { "Corte y Barba", 25000m });

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Nombre", "Precio" },
                values: new object[] { "Cejas Hombre", 3000m });

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Nombre", "Precio" },
                values: new object[] { "Barba", 8000m });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "IsAvailable", "Nombre", "Pin", "Rol" },
                values: new object[,]
                {
                    { 2, false, "Barbero 1", "1111", "Barbero" },
                    { 3, false, "Barbero 2", "2222", "Barbero" }
                });
        }
    }
}

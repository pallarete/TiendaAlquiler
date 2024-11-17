using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TiendaAlquiler.Migrations
{
    /// <inheritdoc />
    public partial class Cars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Coche",
                columns: new[] { "CocheId", "AnioFabricacion", "CarroceriaId", "ColorId", "DecadaId", "Description", "EstaAlquilado", "Marca", "Modelo", "PaisId", "PrecioAlquiler" },
                values: new object[,]
                {
                    { 10, 1967, 6, 1, 4, null, false, "Ford", "Mustang", 6, 500m },
                    { 11, 1969, 6, 2, 5, null, false, "Chevrolet", "Camaro", 6, 450m },
                    { 12, 1950, 7, 8, 3, null, false, "Volkswagen", "Beetle", 5, 100m },
                    { 13, 1975, 4, 5, 5, null, false, "Alfa Romeo", "Giulia", 2, 200m },
                    { 14, 1992, 5, 4, 7, null, false, "Mazda", "RX-7", 7, 300m },
                    { 15, 1972, 6, 6, 5, null, false, "BMW", "2002", 5, 220m },
                    { 16, 1957, 7, 7, 4, null, false, "Citroën", "2CV", 3, 120m },
                    { 17, 1973, 6, 1, 5, null, false, "Lancia", "Stratos", 2, 600m },
                    { 18, 1968, 7, 2, 5, null, false, "Fiat", "500", 2, 150m },
                    { 19, 1981, 6, 3, 6, null, false, "DeLorean", "DMC-12", 6, 800m },
                    { 20, 1994, 6, 4, 7, null, false, "Toyota", "Supra", 7, 400m },
                    { 21, 1988, 5, 6, 6, null, false, "Peugeot", "205 GTI", 3, 180m },
                    { 22, 1965, 6, 5, 4, null, false, "Renault", "Alpine A110", 3, 300m },
                    { 23, 1932, 6, 3, 1, null, false, "Bugatti", "Type 35", 3, 1000m },
                    { 24, 1969, 6, 1, 5, null, false, "Lamborghini", "Miura", 2, 2000m },
                    { 25, 1998, 5, 4, 7, null, false, "Mitsubishi", "Lancer Evolution VI", 7, 250m },
                    { 26, 1995, 5, 5, 7, null, false, "Nissan", "Skyline GT-R", 7, 450m },
                    { 27, 1959, 3, 7, 4, null, false, "Cadillac", "Eldorado", 6, 600m },
                    { 28, 1961, 6, 3, 4, null, false, "Volvo", "P1800", 8, 200m },
                    { 29, 1991, 6, 1, 7, null, false, "Honda", "NSX", 7, 350m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Coche",
                keyColumn: "CocheId",
                keyValue: 29);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TiendaAlquiler.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Carroceria",
                columns: table => new
                {
                    CarroceriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Carrocer__EBEFDD7F34F2C214", x => x.CarroceriaId);
                });

            migrationBuilder.CreateTable(
                name: "Color",
                columns: table => new
                {
                    ColorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Color__8DA7674D8D39398E", x => x.ColorId);
                });

            migrationBuilder.CreateTable(
                name: "Decada",
                columns: table => new
                {
                    DecadaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnioInicio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Decada__B60F61813E1A9096", x => x.DecadaId);
                });

            migrationBuilder.CreateTable(
                name: "Pais",
                columns: table => new
                {
                    PaisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pais__E30419353E3D377E", x => x.PaisId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Coche",
                columns: table => new
                {
                    CocheId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnioFabricacion = table.Column<int>(type: "int", nullable: false),
                    PrecioAlquiler = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    EstaAlquilado = table.Column<bool>(type: "bit", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    CarroceriaId = table.Column<int>(type: "int", nullable: false),
                    DecadaId = table.Column<int>(type: "int", nullable: false),
                    PaisId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Coche__9E980F6B023783F5", x => x.CocheId);
                    table.ForeignKey(
                        name: "FK_Coche_Carroceria",
                        column: x => x.CarroceriaId,
                        principalTable: "Carroceria",
                        principalColumn: "CarroceriaId");
                    table.ForeignKey(
                        name: "FK_Coche_Color",
                        column: x => x.ColorId,
                        principalTable: "Color",
                        principalColumn: "ColorId");
                    table.ForeignKey(
                        name: "FK_Coche_Decada",
                        column: x => x.DecadaId,
                        principalTable: "Decada",
                        principalColumn: "DecadaId");
                    table.ForeignKey(
                        name: "FK_Coche_Pais",
                        column: x => x.PaisId,
                        principalTable: "Pais",
                        principalColumn: "PaisId");
                });

            migrationBuilder.CreateTable(
                name: "Alquiler",
                columns: table => new
                {
                    AlquilerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CocheId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaAlquiler = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaDevolucion = table.Column<DateOnly>(type: "date", nullable: false),
                    PrecioFinal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Alquiler__F28020B524CE1122", x => x.AlquilerId);
                    table.ForeignKey(
                        name: "FK_Alquiler_Coche",
                        column: x => x.CocheId,
                        principalTable: "Coche",
                        principalColumn: "CocheId");
                    table.ForeignKey(
                        name: "FK_Alquiler_Usuario",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Foto",
                columns: table => new
                {
                    FotoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CocheId = table.Column<int>(type: "int", nullable: false),
                    RutaAcceso = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Foto__4EA1C119D86B5C92", x => x.FotoId);
                    table.ForeignKey(
                        name: "FK_Foto_Coche",
                        column: x => x.CocheId,
                        principalTable: "Coche",
                        principalColumn: "CocheId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Carroceria",
                columns: new[] { "CarroceriaId", "Tipo" },
                values: new object[,]
                {
                    { 1, "Coupe" },
                    { 2, "Targa" },
                    { 3, "Descapotable" },
                    { 4, "Berlinetta" },
                    { 5, "HacthBack" },
                    { 6, "GT" },
                    { 7, "Urbano" },
                    { 8, "MicroCoche" }
                });

            migrationBuilder.InsertData(
                table: "Color",
                columns: new[] { "ColorId", "Nombre" },
                values: new object[,]
                {
                    { 1, "Rojo Ferrari" },
                    { 2, "British Racing Green" },
                    { 3, "Silver Ghost" },
                    { 4, "Blue Subaru" },
                    { 5, "Negro Medianoche" },
                    { 6, "Amarillo" },
                    { 7, "White Cream" },
                    { 8, "Marron" }
                });

            migrationBuilder.InsertData(
                table: "Decada",
                columns: new[] { "DecadaId", "AnioInicio" },
                values: new object[,]
                {
                    { 1, 1930 },
                    { 2, 1940 },
                    { 3, 1950 },
                    { 4, 1960 },
                    { 5, 1970 },
                    { 6, 1980 },
                    { 7, 1990 },
                    { 8, 2000 }
                });

            migrationBuilder.InsertData(
                table: "Pais",
                columns: new[] { "PaisId", "Nombre" },
                values: new object[,]
                {
                    { 1, "España" },
                    { 2, "Italia" },
                    { 3, "Francia" },
                    { 4, "Reino Unido" },
                    { 5, "Alemania" },
                    { 6, "Estados Unidos" },
                    { 7, "Japon" },
                    { 8, "Holanda" },
                    { 9, "Belgica" },
                    { 10, "Portugal" },
                    { 11, "Korea" }
                });

            migrationBuilder.InsertData(
                table: "Coche",
                columns: new[] { "CocheId", "AnioFabricacion", "CarroceriaId", "ColorId", "DecadaId", "Description", "EstaAlquilado", "Marca", "Modelo", "PaisId", "PrecioAlquiler" },
                values: new object[,]
                {
                    { 1, 1984, 4, 1, 6, null, false, "Ferrari", "F40", 2, 1500m },
                    { 2, 1963, 6, 3, 4, null, false, "Aston Martin", "DB5", 4, 3000m },
                    { 3, 1970, 6, 6, 5, null, false, "Porsche", "911", 5, 2000m },
                    { 4, 1955, 4, 1, 3, null, false, "Ferrari", "250 GTO", 2, 300m },
                    { 5, 1958, 3, 2, 4, null, false, "Jaguar", "E-Type", 4, 250m },
                    { 6, 1980, 3, 3, 5, null, false, "Rolls Royce", "Silver Cloud", 3, 400m },
                    { 7, 1999, 5, 4, 7, null, false, "Subaru", "Impreza WRX", 7, 120m },
                    { 8, 1955, 2, 5, 6, null, false, "Mercedes-Benz", "300 SL", 5, 350m },
                    { 9, 1966, 7, 8, 6, null, false, "Austin", "Mini Cooper", 4, 100m },
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

            migrationBuilder.CreateIndex(
                name: "IX_Alquiler_CocheId",
                table: "Alquiler",
                column: "CocheId");

            migrationBuilder.CreateIndex(
                name: "IX_Alquiler_UsuarioId",
                table: "Alquiler",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Coche_CarroceriaId",
                table: "Coche",
                column: "CarroceriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Coche_ColorId",
                table: "Coche",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Coche_DecadaId",
                table: "Coche",
                column: "DecadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Coche_PaisId",
                table: "Coche",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Foto_CocheId",
                table: "Foto",
                column: "CocheId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alquiler");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Foto");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Coche");

            migrationBuilder.DropTable(
                name: "Carroceria");

            migrationBuilder.DropTable(
                name: "Color");

            migrationBuilder.DropTable(
                name: "Decada");

            migrationBuilder.DropTable(
                name: "Pais");
        }
    }
}

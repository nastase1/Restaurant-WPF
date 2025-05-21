using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Restaurant.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alergeni",
                columns: table => new
                {
                    AlergenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denumire = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergeni", x => x.AlergenID);
                });

            migrationBuilder.CreateTable(
                name: "Categorii",
                columns: table => new
                {
                    CategorieID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denumire = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorii", x => x.CategorieID);
                });

            migrationBuilder.CreateTable(
                name: "ConturiUtilizatori",
                columns: table => new
                {
                    ContUtilizatorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Prenume = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AdresaEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumarTelefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AdresaLivrare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parola = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConturiUtilizatori", x => x.ContUtilizatorID);
                });

            migrationBuilder.CreateTable(
                name: "Meniuri",
                columns: table => new
                {
                    MeniuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denumire = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descriere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategorieID = table.Column<int>(type: "int", nullable: false),
                    DiscountProcent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ListaFotografii = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meniuri", x => x.MeniuID);
                    table.ForeignKey(
                        name: "FK_Meniuri_Categorii_CategorieID",
                        column: x => x.CategorieID,
                        principalTable: "Categorii",
                        principalColumn: "CategorieID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Preparate",
                columns: table => new
                {
                    PreparatID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denumire = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Pret = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CantitatePortie = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CantitateTotala = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    CategorieID = table.Column<int>(type: "int", nullable: false),
                    ListaFotografii = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preparate", x => x.PreparatID);
                    table.ForeignKey(
                        name: "FK_Preparate_Categorii_CategorieID",
                        column: x => x.CategorieID,
                        principalTable: "Categorii",
                        principalColumn: "CategorieID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comenzi",
                columns: table => new
                {
                    ComandaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataComanda = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CodComanda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostMancare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostTransport = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OraEstimativaLivrare = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Stare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContUtilizatorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comenzi", x => x.ComandaID);
                    table.ForeignKey(
                        name: "FK_Comenzi_ConturiUtilizatori_ContUtilizatorID",
                        column: x => x.ContUtilizatorID,
                        principalTable: "ConturiUtilizatori",
                        principalColumn: "ContUtilizatorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlergeniPreparate",
                columns: table => new
                {
                    AlergenID = table.Column<int>(type: "int", nullable: false),
                    PreparatID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlergeniPreparate", x => new { x.AlergenID, x.PreparatID });
                    table.ForeignKey(
                        name: "FK_AlergeniPreparate_Alergeni_AlergenID",
                        column: x => x.AlergenID,
                        principalTable: "Alergeni",
                        principalColumn: "AlergenID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlergeniPreparate_Preparate_PreparatID",
                        column: x => x.PreparatID,
                        principalTable: "Preparate",
                        principalColumn: "PreparatID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeniuPreparate",
                columns: table => new
                {
                    MeniuID = table.Column<int>(type: "int", nullable: false),
                    PreparatID = table.Column<int>(type: "int", nullable: false),
                    Cantitate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeniuPreparate", x => new { x.MeniuID, x.PreparatID });
                    table.ForeignKey(
                        name: "FK_MeniuPreparate_Meniuri_MeniuID",
                        column: x => x.MeniuID,
                        principalTable: "Meniuri",
                        principalColumn: "MeniuID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeniuPreparate_Preparate_PreparatID",
                        column: x => x.PreparatID,
                        principalTable: "Preparate",
                        principalColumn: "PreparatID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComenziPreparate",
                columns: table => new
                {
                    ComandaID = table.Column<int>(type: "int", nullable: false),
                    PreparatID = table.Column<int>(type: "int", nullable: false),
                    Bucati = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComenziPreparate", x => new { x.ComandaID, x.PreparatID });
                    table.ForeignKey(
                        name: "FK_ComenziPreparate_Comenzi_ComandaID",
                        column: x => x.ComandaID,
                        principalTable: "Comenzi",
                        principalColumn: "ComandaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComenziPreparate_Preparate_PreparatID",
                        column: x => x.PreparatID,
                        principalTable: "Preparate",
                        principalColumn: "PreparatID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Alergeni",
                columns: new[] { "AlergenID", "Denumire" },
                values: new object[,]
                {
                    { 1, "Lactate" },
                    { 2, "Gluten" },
                    { 3, "Nuci" }
                });

            migrationBuilder.InsertData(
                table: "Categorii",
                columns: new[] { "CategorieID", "Denumire" },
                values: new object[,]
                {
                    { 1, "Aperitive" },
                    { 2, "Fel Principal" },
                    { 3, "Deserturi" }
                });

            migrationBuilder.InsertData(
                table: "Meniuri",
                columns: new[] { "MeniuID", "CategorieID", "Denumire", "Descriere", "DiscountProcent", "ListaFotografii" },
                values: new object[,]
                {
                    { 1, 1, "Meniu Dejun", "Bruschete + Cafea", 10m, "../Images\\meniuDejun.jpg" },
                    { 2, 2, "Meniu Romantic", "Pizza + Tiramisu", 15m, "../Images\\meniuRomantic.jpg" },
                    { 3, 2, "Meniu Vegetarian", "Ciorbă + Omletă + Salată", 12m, "../Images\\meniuVegetarian.jpg" },
                    { 4, 2, "Meniu Copii", "Pizza + Clătite", 10m, "../Images\\meniuCopii.jpg" },
                    { 5, 2, "Meniu Fitness", "Pui + Salată", 8m, "../Images\\meniuFitness.jpg" },
                    { 6, 3, "Meniu Dulce", "Tiramisu + Cheesecake", 15m, "../Images\\meniuDulce.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Preparate",
                columns: new[] { "PreparatID", "CantitatePortie", "CantitateTotala", "CategorieID", "Denumire", "ListaFotografii", "Pret" },
                values: new object[,]
                {
                    { 1, 150m, 4500m, 1, "Bruschete", "../Images\\bruschete.jpg", 12.50m },
                    { 2, 350m, 10500m, 2, "Pizza Margherita", "../Images\\pizza.jpg", 28.00m },
                    { 3, 150m, 4500m, 3, "Tiramisu", "../Images\\tiramisu.jpg", 15.00m },
                    { 4, 200m, 6000m, 1, "Omletă cu legume", "../Images\\omleta.jpg", 18.00m },
                    { 5, 300m, 9000m, 2, "Ciorbă de legume", "../Images\\ciorba.jpg", 14.00m },
                    { 6, 350m, 7000m, 2, "Paste Carbonara", "../Images\\paste.jpg", 32.00m },
                    { 7, 200m, 6000m, 2, "Pui la grătar", "../Images\\pui.jpg", 27.00m },
                    { 8, 250m, 7500m, 1, "Salată Caesar", "../Images\\salata.jpg", 22.00m },
                    { 9, 120m, 3600m, 3, "Cheesecake", "../Images\\cheesecake.jpg", 17.00m },
                    { 10, 180m, 5400m, 3, "Clătite cu ciocolată", "../Images\\clatite.jpg", 16.00m }
                });

            migrationBuilder.InsertData(
                table: "AlergeniPreparate",
                columns: new[] { "AlergenID", "PreparatID" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 3 },
                    { 1, 4 },
                    { 1, 6 },
                    { 1, 8 },
                    { 1, 9 },
                    { 1, 10 },
                    { 2, 2 },
                    { 2, 6 },
                    { 2, 9 },
                    { 2, 10 },
                    { 3, 3 }
                });

            migrationBuilder.InsertData(
                table: "MeniuPreparate",
                columns: new[] { "MeniuID", "PreparatID", "Cantitate" },
                values: new object[,]
                {
                    { 1, 1, 1m },
                    { 2, 2, 1m },
                    { 2, 3, 1m },
                    { 3, 4, 1m },
                    { 3, 5, 1m },
                    { 3, 8, 1m },
                    { 4, 2, 1m },
                    { 4, 10, 1m },
                    { 5, 7, 1m },
                    { 5, 8, 1m },
                    { 6, 3, 1m },
                    { 6, 9, 1m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlergeniPreparate_PreparatID",
                table: "AlergeniPreparate",
                column: "PreparatID");

            migrationBuilder.CreateIndex(
                name: "IX_Comenzi_ContUtilizatorID",
                table: "Comenzi",
                column: "ContUtilizatorID");

            migrationBuilder.CreateIndex(
                name: "IX_ComenziPreparate_PreparatID",
                table: "ComenziPreparate",
                column: "PreparatID");

            migrationBuilder.CreateIndex(
                name: "IX_MeniuPreparate_PreparatID",
                table: "MeniuPreparate",
                column: "PreparatID");

            migrationBuilder.CreateIndex(
                name: "IX_Meniuri_CategorieID",
                table: "Meniuri",
                column: "CategorieID");

            migrationBuilder.CreateIndex(
                name: "IX_Preparate_CategorieID",
                table: "Preparate",
                column: "CategorieID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlergeniPreparate");

            migrationBuilder.DropTable(
                name: "ComenziPreparate");

            migrationBuilder.DropTable(
                name: "MeniuPreparate");

            migrationBuilder.DropTable(
                name: "Alergeni");

            migrationBuilder.DropTable(
                name: "Comenzi");

            migrationBuilder.DropTable(
                name: "Meniuri");

            migrationBuilder.DropTable(
                name: "Preparate");

            migrationBuilder.DropTable(
                name: "ConturiUtilizatori");

            migrationBuilder.DropTable(
                name: "Categorii");
        }
    }
}

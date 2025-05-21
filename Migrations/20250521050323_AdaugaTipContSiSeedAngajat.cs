using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Migrations
{
    /// <inheritdoc />
    public partial class AdaugaTipContSiSeedAngajat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipCont",
                table: "ConturiUtilizatori",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "ConturiUtilizatori",
                columns: new[] { "ContUtilizatorID", "AdresaEmail", "AdresaLivrare", "NumarTelefon", "Nume", "Parola", "Prenume", "TipCont" },
                values: new object[] { 2, "angajat@restaurant.com", "Adresa Restaurantului", "0000000000", "Angajat", "restaurant", "Restaurant", "Angajat" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConturiUtilizatori",
                keyColumn: "ContUtilizatorID",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "TipCont",
                table: "ConturiUtilizatori");
        }
    }
}

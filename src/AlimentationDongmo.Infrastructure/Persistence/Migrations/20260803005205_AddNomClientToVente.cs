using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlimentationDongmo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNomClientToVente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NomClient",
                table: "Ventes",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomClient",
                table: "Ventes");
        }
    }
}

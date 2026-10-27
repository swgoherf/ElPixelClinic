using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Office.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressValueObjectToOffice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Offices");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Offices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HouseNumber",
                table: "Offices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Offices",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Offices");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Offices",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}

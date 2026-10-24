using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameServicesToMedicalServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Services",
                newName: "MedicalServices");

            migrationBuilder.RenameIndex(
                name: "IX_Services_SpecializationId",
                table: "MedicalServices",
                newName: "IX_MedicalServices_SpecializationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "MedicalServices",
                newName: "Services");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalServices_SpecializationId",
                table: "Services",
                newName: "IX_Services_SpecializationId");
        }
    }
}

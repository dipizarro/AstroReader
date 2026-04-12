using AstroReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstroReader.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AstroReaderDbContext))]
    [Migration("20260411123000_AddSavedChartTechnicalMetadata")]
    public partial class AddSavedChartTechnicalMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalculationEngine",
                table: "SavedCharts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Unknown");

            migrationBuilder.AddColumn<string>(
                name: "HouseSystemCode",
                table: "SavedCharts",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculationEngine",
                table: "SavedCharts");

            migrationBuilder.DropColumn(
                name: "HouseSystemCode",
                table: "SavedCharts");
        }
    }
}

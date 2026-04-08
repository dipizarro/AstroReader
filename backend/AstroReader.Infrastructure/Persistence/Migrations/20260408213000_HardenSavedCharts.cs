using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstroReader.Infrastructure.Persistence.Migrations
{
    public partial class HardenSavedCharts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResultSnapshotJson",
                table: "SavedCharts",
                newName: "CalculatedChartJson");

            migrationBuilder.AddColumn<int>(
                name: "SnapshotVersion",
                table: "SavedCharts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "TimezoneIana",
                table: "SavedCharts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SnapshotVersion",
                table: "SavedCharts");

            migrationBuilder.DropColumn(
                name: "TimezoneIana",
                table: "SavedCharts");

            migrationBuilder.RenameColumn(
                name: "CalculatedChartJson",
                table: "SavedCharts",
                newName: "ResultSnapshotJson");
        }
    }
}

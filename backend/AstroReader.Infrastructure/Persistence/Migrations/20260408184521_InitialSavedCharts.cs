using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstroReader.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSavedCharts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedCharts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProfileName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PlaceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BirthTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    TimezoneOffsetMinutes = table.Column<short>(type: "smallint", nullable: false),
                    BirthInstantUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    SunSign = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MoonSign = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AscendantSign = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ResultSnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedCharts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedCharts_CreatedAtUtc",
                table: "SavedCharts",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCharts_UserId",
                table: "SavedCharts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedCharts");
        }
    }
}

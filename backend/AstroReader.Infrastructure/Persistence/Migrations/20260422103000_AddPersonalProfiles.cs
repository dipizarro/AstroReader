using System;
using AstroReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstroReader.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AstroReaderDbContext))]
    [Migration("20260422103000_AddPersonalProfiles")]
    public partial class AddPersonalProfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonalProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SavedChartId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BirthTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    BirthInstantUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BirthPlace = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    TimezoneOffsetMinutes = table.Column<short>(type: "smallint", nullable: false),
                    SelfPerceptionFocus = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    CurrentChallenge = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    DesiredInsight = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    SelfDescription = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalProfiles_SavedCharts_SavedChartId",
                        column: x => x.SavedChartId,
                        principalTable: "SavedCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonalProfiles_CreatedAtUtc",
                table: "PersonalProfiles",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalProfiles_SavedChartId",
                table: "PersonalProfiles",
                column: "SavedChartId",
                unique: true,
                filter: "[SavedChartId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalProfiles_UserId",
                table: "PersonalProfiles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonalProfiles");
        }
    }
}

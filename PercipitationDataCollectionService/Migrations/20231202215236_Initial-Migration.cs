using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrecipitationService.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Measurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PrecipitationMm = table.Column<float>(type: "REAL", nullable: true),
                    Coverage = table.Column<float>(type: "REAL", nullable: true),
                    Snowfall = table.Column<float>(type: "REAL", nullable: true),
                    SnowDepth = table.Column<float>(type: "REAL", nullable: true),
                    SevereRisk = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurements", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Measurements");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Encounters.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitEncounters4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequiredParticipants",
                schema: "encounters",
                table: "Encounters",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EncounterProgresses",
                schema: "encounters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EncounterId = table.Column<long>(type: "bigint", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EnteredPhotoRadiusAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncounterProgresses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncounterProgresses",
                schema: "encounters");

            migrationBuilder.DropColumn(
                name: "RequiredParticipants",
                schema: "encounters",
                table: "Encounters");
        }
    }
}

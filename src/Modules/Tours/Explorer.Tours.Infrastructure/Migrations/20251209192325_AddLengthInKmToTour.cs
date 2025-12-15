using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Tours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLengthInKmToTour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LengthInKm",
                schema: "tours",
                table: "Tours",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "tours",
                table: "TourProblems",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "IsSolved",
                schema: "tours",
                table: "TourProblems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolveDue",
                schema: "tours",
                table: "TourProblems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "tours",
                table: "TourProblems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "_comments",
                schema: "tours",
                table: "TourProblems",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AnnualAwards",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    VotingStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VotingEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualAwards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monuments",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    YearOfCreation = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicPointRequests",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    KeyPointIndex = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AdminComment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicPointRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourDurations",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransportType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DurationInMinutes = table.Column<int>(type: "integer", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourDurations", x => x.Id);
                    table.CheckConstraint("CK_TourDuration_Minutes_Positive", "\"DurationInMinutes\" > 0");
                    table.ForeignKey(
                        name: "FK_TourDurations_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tours",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourEquipment",
                schema: "tours",
                columns: table => new
                {
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourEquipment", x => new { x.TourId, x.EquipmentId });
                    table.ForeignKey(
                        name: "FK_TourEquipment_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "tours",
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourEquipment_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tours",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourDurations_TourId",
                schema: "tours",
                table: "TourDurations",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourEquipment_EquipmentId",
                schema: "tours",
                table: "TourEquipment",
                column: "EquipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnualAwards",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Monuments",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "PublicPointRequests",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "TourDurations",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "TourEquipment",
                schema: "tours");

            migrationBuilder.DropColumn(
                name: "LengthInKm",
                schema: "tours",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "IsSolved",
                schema: "tours",
                table: "TourProblems");

            migrationBuilder.DropColumn(
                name: "ResolveDue",
                schema: "tours",
                table: "TourProblems");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "tours",
                table: "TourProblems");

            migrationBuilder.DropColumn(
                name: "_comments",
                schema: "tours",
                table: "TourProblems");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "tours",
                table: "TourProblems",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);
        }
    }
}

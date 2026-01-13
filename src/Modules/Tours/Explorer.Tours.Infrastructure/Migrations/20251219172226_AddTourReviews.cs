using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Tours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTourReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeyPointUnlockTimes",
                schema: "tours",
                table: "TourExecutions",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivity",
                schema: "tours",
                table: "TourExecutions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "TourReviews",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    TourExecutionId = table.Column<long>(type: "bigint", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TourProgressPercentage = table.Column<double>(type: "double precision", nullable: false),
                    ImageUrls = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourReviews_TourExecutions_TourExecutionId",
                        column: x => x.TourExecutionId,
                        principalSchema: "tours",
                        principalTable: "TourExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TourReviews_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tours",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourReviews_CreatedAt",
                schema: "tours",
                table: "TourReviews",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TourReviews_TourExecutionId",
                schema: "tours",
                table: "TourReviews",
                column: "TourExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_TourReviews_TourId",
                schema: "tours",
                table: "TourReviews",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourReviews_TouristId",
                schema: "tours",
                table: "TourReviews",
                column: "TouristId");

            migrationBuilder.CreateIndex(
                name: "IX_TourReviews_TouristId_TourId",
                schema: "tours",
                table: "TourReviews",
                columns: new[] { "TouristId", "TourId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourReviews",
                schema: "tours");

            migrationBuilder.DropColumn(
                name: "KeyPointUnlockTimes",
                schema: "tours",
                table: "TourExecutions");

            migrationBuilder.DropColumn(
                name: "LastActivity",
                schema: "tours",
                table: "TourExecutions");
        }
    }
}

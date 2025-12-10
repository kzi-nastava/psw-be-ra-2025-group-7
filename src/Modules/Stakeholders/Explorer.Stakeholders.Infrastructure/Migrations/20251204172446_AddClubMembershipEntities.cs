using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Stakeholders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClubMembershipEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Clubs",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ImageUrls = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClubInvitations",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClubId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubInvitations_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalSchema: "stakeholders",
                        principalTable: "Clubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClubJoinRequests",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClubId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubJoinRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubJoinRequests_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalSchema: "stakeholders",
                        principalTable: "Clubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClubMembers",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClubId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubMembers_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalSchema: "stakeholders",
                        principalTable: "Clubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubInvitations_ClubId",
                schema: "stakeholders",
                table: "ClubInvitations",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubJoinRequests_ClubId",
                schema: "stakeholders",
                table: "ClubJoinRequests",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubMembers_ClubId",
                schema: "stakeholders",
                table: "ClubMembers",
                column: "ClubId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubInvitations",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "ClubJoinRequests",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "ClubMembers",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "TourPreferences",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Clubs",
                schema: "stakeholders");

            
        }
    }
}

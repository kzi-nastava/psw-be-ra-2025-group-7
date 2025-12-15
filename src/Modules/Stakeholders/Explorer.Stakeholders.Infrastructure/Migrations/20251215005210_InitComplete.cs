using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Stakeholders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitComplete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "stakeholders");

            migrationBuilder.CreateTable(
                name: "Accounts",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClubMessages",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClubId = table.Column<long>(type: "bigint", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResourceId = table.Column<long>(type: "bigint", nullable: true),
                    ResourceType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubMessages", x => x.Id);
                });

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
                name: "FollowerMessages",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResourceId = table.Column<long>(type: "bigint", nullable: true),
                    ResourceType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowerMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Followers",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FollowerId = table.Column<long>(type: "bigint", nullable: false),
                    FollowedId = table.Column<long>(type: "bigint", nullable: false),
                    FollowedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SentByUserId = table.Column<long>(type: "bigint", nullable: false),
                    SentToUserId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourPreferences",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    PreferredDifficulty = table.Column<int>(type: "integer", nullable: false),
                    WalkingRating = table.Column<int>(type: "integer", nullable: false),
                    BicycleRating = table.Column<int>(type: "integer", nullable: false),
                    BoatRating = table.Column<int>(type: "integer", nullable: false),
                    CarRating = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPreferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
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
                    Status = table.Column<int>(type: "integer", nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Status = table.Column<int>(type: "integer", nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "People",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.ForeignKey(
                        name: "FK_People_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "stakeholders",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    ProfilePicture = table.Column<string>(type: "text", nullable: true),
                    Biography = table.Column<string>(type: "text", nullable: true),
                    Motto = table.Column<string>(type: "text", nullable: true),
                    CurrentLatitude = table.Column<double>(type: "double precision", nullable: true),
                    CurrentLongitude = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "stakeholders",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ResourceId = table.Column<long>(type: "bigint", nullable: true),
                    ResourceType = table.Column<int>(type: "integer", nullable: true),
                    SourceFollowerMessageId = table.Column<long>(type: "bigint", nullable: true),
                    SourceClubMessageId = table.Column<long>(type: "bigint", nullable: true),
                    ClubId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_ClubMessages_SourceClubMessageId",
                        column: x => x.SourceClubMessageId,
                        principalSchema: "stakeholders",
                        principalTable: "ClubMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalSchema: "stakeholders",
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_FollowerMessages_SourceFollowerMessageId",
                        column: x => x.SourceFollowerMessageId,
                        principalSchema: "stakeholders",
                        principalTable: "FollowerMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_People_UserId",
                        column: x => x.UserId,
                        principalSchema: "stakeholders",
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_People_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "stakeholders",
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_ClubMessages_AuthorId",
                schema: "stakeholders",
                table: "ClubMessages",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubMessages_ClubId",
                schema: "stakeholders",
                table: "ClubMessages",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubMessages_CreatedAt",
                schema: "stakeholders",
                table: "ClubMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerMessages_AuthorId",
                schema: "stakeholders",
                table: "FollowerMessages",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerMessages_CreatedAt",
                schema: "stakeholders",
                table: "FollowerMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowedId",
                schema: "stakeholders",
                table: "Followers",
                column: "FollowedId");

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowerId",
                schema: "stakeholders",
                table: "Followers",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowerId_FollowedId",
                schema: "stakeholders",
                table: "Followers",
                columns: new[] { "FollowerId", "FollowedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentByUserId_SentToUserId_Id",
                schema: "stakeholders",
                table: "Messages",
                columns: new[] { "SentByUserId", "SentToUserId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ClubId",
                schema: "stakeholders",
                table: "Notifications",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                schema: "stakeholders",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SourceClubMessageId",
                schema: "stakeholders",
                table: "Notifications",
                column: "SourceClubMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SourceFollowerMessageId",
                schema: "stakeholders",
                table: "Notifications",
                column: "SourceFollowerMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead",
                schema: "stakeholders",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_People_UserId",
                schema: "stakeholders",
                table: "People",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PersonId",
                schema: "stakeholders",
                table: "Reviews",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                schema: "stakeholders",
                table: "UserProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "stakeholders",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts",
                schema: "stakeholders");

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
                name: "Followers",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "TourPreferences",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "UserProfiles",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "ClubMessages",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Clubs",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "FollowerMessages",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "People",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "stakeholders");
        }
    }
}

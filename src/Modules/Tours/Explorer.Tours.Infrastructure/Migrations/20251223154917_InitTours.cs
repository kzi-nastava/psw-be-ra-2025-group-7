using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Tours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitTours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tours");

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
                name: "EnhancedReviews",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    OverallRating = table.Column<int>(type: "integer", nullable: false),
                    GuideQuality = table.Column<int>(type: "integer", nullable: false),
                    ValueForMoney = table.Column<int>(type: "integer", nullable: false),
                    RouteScenery = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    GroupSize = table.Column<int>(type: "integer", nullable: false),
                    TextReview = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnhancedReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facility",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.Id);
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
                name: "Notifications",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Preview = table.Column<string>(type: "text", nullable: false),
                    ProblemId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
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
                name: "Quizzes",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TouristEquipment",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouristEquipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourJournals",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourJournals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourProblems",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<int>(type: "integer", nullable: false),
                    TouristId = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    TimeReported = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolveDue = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsSolved = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    _comments = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourProblems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnhancedReviewCons",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnhancedReviewId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnhancedReviewCons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnhancedReviewCons_EnhancedReviews_EnhancedReviewId",
                        column: x => x.EnhancedReviewId,
                        principalSchema: "tours",
                        principalTable: "EnhancedReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnhancedReviewHelpfulVotes",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnhancedReviewId = table.Column<long>(type: "bigint", nullable: false),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    VotedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnhancedReviewHelpfulVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnhancedReviewHelpfulVotes_EnhancedReviews_EnhancedReviewId",
                        column: x => x.EnhancedReviewId,
                        principalSchema: "tours",
                        principalTable: "EnhancedReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnhancedReviewImages",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnhancedReviewId = table.Column<long>(type: "bigint", nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnhancedReviewImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnhancedReviewImages_EnhancedReviews_EnhancedReviewId",
                        column: x => x.EnhancedReviewId,
                        principalSchema: "tours",
                        principalTable: "EnhancedReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnhancedReviewPros",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnhancedReviewId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnhancedReviewPros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnhancedReviewPros_EnhancedReviews_EnhancedReviewId",
                        column: x => x.EnhancedReviewId,
                        principalSchema: "tours",
                        principalTable: "EnhancedReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnhancedReviewTags",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnhancedReviewId = table.Column<long>(type: "bigint", nullable: false),
                    Tag = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnhancedReviewTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnhancedReviewTags_EnhancedReviews_EnhancedReviewId",
                        column: x => x.EnhancedReviewId,
                        principalSchema: "tours",
                        principalTable: "EnhancedReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "text", nullable: false),
                    AllowsMultipleCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    QuizId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalSchema: "tours",
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    TourName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ShoppingCartId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_ShoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalSchema: "tours",
                        principalTable: "ShoppingCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyPoints",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Secret = table.Column<string>(type: "text", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyPoints_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tours",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "TourPurchaseTokens",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPurchaseTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourPurchaseTokens_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tours",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Option",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    Feedback = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    QuestionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Option", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Option_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "tours",
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnhancedReviewCons_EnhancedReviewId",
                schema: "tours",
                table: "EnhancedReviewCons",
                column: "EnhancedReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_EnhancedReviewHelpfulVotes_EnhancedReviewId_TouristId",
                schema: "tours",
                table: "EnhancedReviewHelpfulVotes",
                columns: new[] { "EnhancedReviewId", "TouristId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnhancedReviewImages_EnhancedReviewId",
                schema: "tours",
                table: "EnhancedReviewImages",
                column: "EnhancedReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_EnhancedReviewPros_EnhancedReviewId",
                schema: "tours",
                table: "EnhancedReviewPros",
                column: "EnhancedReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_EnhancedReviews_TourId_TouristId",
                schema: "tours",
                table: "EnhancedReviews",
                columns: new[] { "TourId", "TouristId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnhancedReviewTags_EnhancedReviewId",
                schema: "tours",
                table: "EnhancedReviewTags",
                column: "EnhancedReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyPoints_TourId",
                schema: "tours",
                table: "KeyPoints",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Option_QuestionId",
                schema: "tours",
                table: "Option",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ShoppingCartId",
                schema: "tours",
                table: "OrderItems",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizId",
                schema: "tours",
                table: "Questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_TouristId",
                schema: "tours",
                table: "ShoppingCarts",
                column: "TouristId",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_TourPurchaseTokens_TourId",
                schema: "tours",
                table: "TourPurchaseTokens",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPurchaseTokens_UserId",
                schema: "tours",
                table: "TourPurchaseTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPurchaseTokens_UserId_TourId",
                schema: "tours",
                table: "TourPurchaseTokens",
                columns: new[] { "UserId", "TourId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnualAwards",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "EnhancedReviewCons",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "EnhancedReviewHelpfulVotes",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "EnhancedReviewImages",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "EnhancedReviewPros",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "EnhancedReviewTags",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Facility",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "KeyPoints",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Monuments",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Option",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "OrderItems",
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

            migrationBuilder.DropTable(
                name: "TouristEquipment",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "TourJournals",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "TourProblems",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "TourPurchaseTokens",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "EnhancedReviews",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Questions",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "ShoppingCarts",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Equipment",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Tours",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Quizzes",
                schema: "tours");
        }
    }
}

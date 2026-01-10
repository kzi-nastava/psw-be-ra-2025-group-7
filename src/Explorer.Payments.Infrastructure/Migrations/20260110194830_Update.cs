using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BundlePurchases",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    BundleId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundlePurchases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bundles",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bundles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    DiscountPercentage = table.Column<int>(type: "integer", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentNotifications",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRecords",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: true),
                    BundleId = table.Column<long>(type: "bigint", nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CouponCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseNotifications",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BundleItems",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    BundleId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BundleItems_Bundles_BundleId",
                        column: x => x.BundleId,
                        principalSchema: "payments",
                        principalTable: "Bundles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_BundleId_TourId",
                schema: "payments",
                table: "BundleItems",
                columns: new[] { "BundleId", "TourId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundlePurchases_TouristId_BundleId",
                schema: "payments",
                table: "BundlePurchases",
                columns: new[] { "TouristId", "BundleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bundles_AuthorId",
                schema: "payments",
                table: "Bundles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_AuthorId",
                schema: "payments",
                table: "Coupons",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                schema: "payments",
                table: "Coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentNotifications_UserId",
                schema: "payments",
                table: "PaymentNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRecords_TouristId",
                schema: "payments",
                table: "PaymentRecords",
                column: "TouristId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseNotifications_TouristId",
                schema: "payments",
                table: "PurchaseNotifications",
                column: "TouristId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BundleItems",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "BundlePurchases",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "Coupons",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "PaymentNotifications",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "PaymentRecords",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "PurchaseNotifications",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "Bundles",
                schema: "payments");
        }
    }
}

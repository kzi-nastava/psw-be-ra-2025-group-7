using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Tours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitTours4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "ShoppingCarts",
                schema: "tours");

            migrationBuilder.AddColumn<long>(
                name: "EncounterId",
                schema: "tours",
                table: "KeyPoints",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                schema: "tours",
                table: "KeyPoints",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncounterId",
                schema: "tours",
                table: "KeyPoints");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                schema: "tours",
                table: "KeyPoints");

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TouristId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ShoppingCartId = table.Column<long>(type: "bigint", nullable: true),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    TourName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ShoppingCartId",
                schema: "tours",
                table: "OrderItems",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_TouristId",
                schema: "tours",
                table: "ShoppingCarts",
                column: "TouristId",
                unique: true);
        }
    }
}

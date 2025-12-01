using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Explorer.Tours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTourPurchaseTokenNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddForeignKey(
                name: "FK_TourPurchaseTokens_Tours_TourId",
                schema: "tours",
                table: "TourPurchaseTokens",
                column: "TourId",
                principalSchema: "tours",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourPurchaseTokens_Tours_TourId",
                schema: "tours",
                table: "TourPurchaseTokens");

            migrationBuilder.DropIndex(
                name: "IX_TourPurchaseTokens_TourId",
                schema: "tours",
                table: "TourPurchaseTokens");

            migrationBuilder.DropIndex(
                name: "IX_TourPurchaseTokens_UserId",
                schema: "tours",
                table: "TourPurchaseTokens");
        }
    }
}

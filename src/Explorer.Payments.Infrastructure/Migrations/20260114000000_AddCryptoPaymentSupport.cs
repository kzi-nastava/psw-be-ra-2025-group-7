using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCryptoPaymentSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add SolanaWalletAddress column to Wallets table
            migrationBuilder.AddColumn<string>(
                name: "SolanaWalletAddress",
                schema: "payments",
                table: "Wallets",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            // Create index on SolanaWalletAddress
            migrationBuilder.CreateIndex(
                name: "IX_Wallets_SolanaWalletAddress",
                schema: "payments",
                table: "Wallets",
                column: "SolanaWalletAddress");

            // Create CryptoDepositRequests table
            migrationBuilder.CreateTable(
                name: "CryptoDepositRequests",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CryptoAmount = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    CoinsAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BlockchainExplorerUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SenderWalletAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoDepositRequests", x => x.Id);
                });

            // Create indexes on CryptoDepositRequests
            migrationBuilder.CreateIndex(
                name: "IX_CryptoDepositRequests_UserId",
                schema: "payments",
                table: "CryptoDepositRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoDepositRequests_TransactionId",
                schema: "payments",
                table: "CryptoDepositRequests",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CryptoDepositRequests_Status",
                schema: "payments",
                table: "CryptoDepositRequests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop CryptoDepositRequests table
            migrationBuilder.DropTable(
                name: "CryptoDepositRequests",
                schema: "payments");

            // Drop index on SolanaWalletAddress
            migrationBuilder.DropIndex(
                name: "IX_Wallets_SolanaWalletAddress",
                schema: "payments",
                table: "Wallets");

            // Drop SolanaWalletAddress column from Wallets table
            migrationBuilder.DropColumn(
                name: "SolanaWalletAddress",
                schema: "payments",
                table: "Wallets");
        }
    }
}

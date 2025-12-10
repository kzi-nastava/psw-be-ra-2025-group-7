using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Explorer.Stakeholders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClubStatusAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubInvitations_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubJoinRequests_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubJoinRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubMembers_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubMembers");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "stakeholders",
                table: "ClubJoinRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "stakeholders",
                table: "ClubInvitations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubInvitations_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubInvitations",
                column: "ClubId",
                principalSchema: "stakeholders",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubJoinRequests_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubJoinRequests",
                column: "ClubId",
                principalSchema: "stakeholders",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubMembers_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubMembers",
                column: "ClubId",
                principalSchema: "stakeholders",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubInvitations_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubJoinRequests_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubJoinRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubMembers_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubMembers");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "stakeholders",
                table: "ClubJoinRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "stakeholders",
                table: "ClubInvitations");

            migrationBuilder.AddForeignKey(
                name: "FK_ClubInvitations_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubInvitations",
                column: "ClubId",
                principalSchema: "stakeholders",
                principalTable: "Clubs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClubJoinRequests_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubJoinRequests",
                column: "ClubId",
                principalSchema: "stakeholders",
                principalTable: "Clubs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClubMembers_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubMembers",
                column: "ClubId",
                principalSchema: "stakeholders",
                principalTable: "Clubs",
                principalColumn: "Id");
        }
    }
}

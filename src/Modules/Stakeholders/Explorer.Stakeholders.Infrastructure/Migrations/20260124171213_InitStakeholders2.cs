using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Explorer.Stakeholders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitStakeholders2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                schema: "stakeholders",
                table: "UserProfiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "XP",
                schema: "stakeholders",
                table: "UserProfiles",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                schema: "stakeholders",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "XP",
                schema: "stakeholders",
                table: "UserProfiles");
        }
    }
}

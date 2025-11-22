using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Explorer.Tours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_User_AuthorId",
                schema: "tours",
                table: "Quizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                schema: "tours",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "tours",
                newName: "Users",
                newSchema: "tours");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                schema: "tours",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Users_AuthorId",
                schema: "tours",
                table: "Quizzes",
                column: "AuthorId",
                principalSchema: "tours",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Users_AuthorId",
                schema: "tours",
                table: "Quizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                schema: "tours",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "tours",
                newName: "User",
                newSchema: "tours");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                schema: "tours",
                table: "User",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_User_AuthorId",
                schema: "tours",
                table: "Quizzes",
                column: "AuthorId",
                principalSchema: "tours",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Explorer.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogVotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                schema: "blog",
                table: "BlogComments");

            migrationBuilder.RenameColumn(
                name: "BlogId",
                schema: "blog",
                table: "BlogComments",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "blog",
                table: "BlogComments",
                newName: "BlogId");

            migrationBuilder.AddColumn<long>(
                name: "AuthorId",
                schema: "blog",
                table: "BlogComments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}

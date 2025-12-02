using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Explorer.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogLifecycleColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                schema: "blog",
                table: "BlogComments",
                newName: "LastModifiedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModifiedAt",
                schema: "blog",
                table: "BlogComments",
                newName: "LastEditedAt");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrowBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class _008_Remove_BlogUserId_Foreign_Key : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogUserId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "BlogUserId",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlogUserId",
                table: "Tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BlogUserId",
                table: "Posts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

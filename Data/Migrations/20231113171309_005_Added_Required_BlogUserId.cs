using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechTalkBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class _005_Added_Required_BlogUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlogUserId",
                table: "Posts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogUserId",
                table: "Posts");
        }
    }
}

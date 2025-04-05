using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechTalkBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class _007_Remove_BlogUserID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogUserId",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlogUserId",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechTalkBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class _006_Added_Required_BlogUserId_To_All_Models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlogUserId",
                table: "Tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BlogUserId",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogUserId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "BlogUserId",
                table: "Comments");
        }
    }
}

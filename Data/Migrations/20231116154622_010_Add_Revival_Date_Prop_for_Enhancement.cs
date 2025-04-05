using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrowBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class _010_Add_Revival_Date_Prop_for_Enhancement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RevivalDate",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevivalDate",
                table: "Posts");
        }
    }
}

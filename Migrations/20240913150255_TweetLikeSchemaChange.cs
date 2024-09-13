using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YApi.Migrations
{
    /// <inheritdoc />
    public partial class TweetLikeSchemaChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d3ae55c8-b68e-4083-ba64-4eacf93d123d");

            migrationBuilder.AddColumn<List<string>>(
                name: "Likes",
                table: "Tweets",
                type: "text[]",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "86fb8a2d-59e8-4f9b-a7ad-02d30df463bc", null, "User", "USER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "86fb8a2d-59e8-4f9b-a7ad-02d30df463bc");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Tweets");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d3ae55c8-b68e-4083-ba64-4eacf93d123d", null, "User", "USER" });
        }
    }
}

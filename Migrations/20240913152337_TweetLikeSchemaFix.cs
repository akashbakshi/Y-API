using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YApi.Migrations
{
    /// <inheritdoc />
    public partial class TweetLikeSchemaFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "86fb8a2d-59e8-4f9b-a7ad-02d30df463bc");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Tweets");

            migrationBuilder.AddColumn<long>(
                name: "TweetId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "93153282-5bc1-4c6a-9031-059b7ce0843f", null, "User", "USER" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TweetId",
                table: "AspNetUsers",
                column: "TweetId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Tweets_TweetId",
                table: "AspNetUsers",
                column: "TweetId",
                principalTable: "Tweets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Tweets_TweetId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TweetId",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "93153282-5bc1-4c6a-9031-059b7ce0843f");

            migrationBuilder.DropColumn(
                name: "TweetId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<List<string>>(
                name: "Likes",
                table: "Tweets",
                type: "text[]",
                nullable: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "86fb8a2d-59e8-4f9b-a7ad-02d30df463bc", null, "User", "USER" });
        }
    }
}

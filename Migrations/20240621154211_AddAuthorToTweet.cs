using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorToTweet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "99a48690-c86b-4a95-9a37-021f27bc79b4");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Tweets",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "014587d9-7dbe-4818-8b6d-b408c94c7c1d", null, "User", "USER" });

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_AuthorId",
                table: "Tweets",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tweets_AspNetUsers_AuthorId",
                table: "Tweets",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweets_AspNetUsers_AuthorId",
                table: "Tweets");

            migrationBuilder.DropIndex(
                name: "IX_Tweets_AuthorId",
                table: "Tweets");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "014587d9-7dbe-4818-8b6d-b408c94c7c1d");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Tweets");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "99a48690-c86b-4a95-9a37-021f27bc79b4", null, "User", "USER" });
        }
    }
}

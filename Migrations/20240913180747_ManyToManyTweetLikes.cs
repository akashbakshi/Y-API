using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YApi.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyTweetLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Tweets_TweetId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tweets_AspNetUsers_AuthorId",
                table: "Tweets");

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

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Tweets",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AppUserTweet",
                columns: table => new
                {
                    LikedTweetsId = table.Column<long>(type: "bigint", nullable: false),
                    LikesId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserTweet", x => new { x.LikedTweetsId, x.LikesId });
                    table.ForeignKey(
                        name: "FK_AppUserTweet_AspNetUsers_LikesId",
                        column: x => x.LikesId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserTweet_Tweets_LikedTweetsId",
                        column: x => x.LikedTweetsId,
                        principalTable: "Tweets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a15d4c89-c7ea-4403-a347-b25dece58e27", null, "User", "USER" });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserTweet_LikesId",
                table: "AppUserTweet",
                column: "LikesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tweets_AspNetUsers_AuthorId",
                table: "Tweets",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweets_AspNetUsers_AuthorId",
                table: "Tweets");

            migrationBuilder.DropTable(
                name: "AppUserTweet");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a15d4c89-c7ea-4403-a347-b25dece58e27");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Tweets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Tweets_AspNetUsers_AuthorId",
                table: "Tweets",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

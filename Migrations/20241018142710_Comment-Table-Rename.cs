using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YApi.Migrations
{
    /// <inheritdoc />
    public partial class CommentTableRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_CommentedById",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Tweets_TweetId",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6ce5a58a-de5e-4f4e-b7ae-0bcc08330dda");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_TweetId",
                table: "Comments",
                newName: "IX_Comments_TweetId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_CommentedById",
                table: "Comments",
                newName: "IX_Comments_CommentedById");

            migrationBuilder.AlterColumn<string>(
                name: "CommentedById",
                table: "Comments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0ac2efbc-6382-4258-88c7-8087f54af9e9", null, "User", "USER" });

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CommentedById",
                table: "Comments",
                column: "CommentedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tweets_TweetId",
                table: "Comments",
                column: "TweetId",
                principalTable: "Tweets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CommentedById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tweets_TweetId",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0ac2efbc-6382-4258-88c7-8087f54af9e9");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TweetId",
                table: "Comment",
                newName: "IX_Comment_TweetId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_CommentedById",
                table: "Comment",
                newName: "IX_Comment_CommentedById");

            migrationBuilder.AlterColumn<string>(
                name: "CommentedById",
                table: "Comment",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6ce5a58a-de5e-4f4e-b7ae-0bcc08330dda", null, "User", "USER" });

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_CommentedById",
                table: "Comment",
                column: "CommentedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Tweets_TweetId",
                table: "Comment",
                column: "TweetId",
                principalTable: "Tweets",
                principalColumn: "Id");
        }
    }
}

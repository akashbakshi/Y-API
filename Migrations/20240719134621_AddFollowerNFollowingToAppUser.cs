using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowerNFollowingToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "014587d9-7dbe-4818-8b6d-b408c94c7c1d");

            migrationBuilder.CreateTable(
                name: "AppUserAppUser",
                columns: table => new
                {
                    FollowersId = table.Column<string>(type: "text", nullable: false),
                    FollowingId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserAppUser", x => new { x.FollowersId, x.FollowingId });
                    table.ForeignKey(
                        name: "FK_AppUserAppUser_AspNetUsers_FollowersId",
                        column: x => x.FollowersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserAppUser_AspNetUsers_FollowingId",
                        column: x => x.FollowingId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d3ae55c8-b68e-4083-ba64-4eacf93d123d", null, "User", "USER" });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserAppUser_FollowingId",
                table: "AppUserAppUser",
                column: "FollowingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserAppUser");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d3ae55c8-b68e-4083-ba64-4eacf93d123d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "014587d9-7dbe-4818-8b6d-b408c94c7c1d", null, "User", "USER" });
        }
    }
}

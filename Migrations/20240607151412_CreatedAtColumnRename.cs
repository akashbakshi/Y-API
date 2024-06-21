using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YApi.Migrations
{
    /// <inheritdoc />
    public partial class CreatedAtColumnRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "Tweets",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Tweets",
                newName: "CreateAt");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Users.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Group",
                table: "Users");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                schema: "Users",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AvatarThumbnail",
                schema: "Users",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                schema: "Users",
                table: "User");

            migrationBuilder.DropColumn(
                name: "AvatarThumbnail",
                schema: "Users",
                table: "User");
        }
    }
}

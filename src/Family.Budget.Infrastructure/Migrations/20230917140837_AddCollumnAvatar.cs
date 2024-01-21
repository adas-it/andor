using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family.Budget.Infrastructure.Migrations
{
    public partial class AddCollumnAvatar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                schema: "budget",
                table: "Category",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                schema: "budget",
                table: "Category");
        }
    }
}

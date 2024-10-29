using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderCategoryAndSubCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultOrder",
                schema: "Engagement",
                table: "SubCategory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultOrder",
                schema: "Engagement",
                table: "Category",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "Engagement",
                table: "AccountSubCategory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "Engagement",
                table: "AccountCategory",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultOrder",
                schema: "Engagement",
                table: "SubCategory");

            migrationBuilder.DropColumn(
                name: "DefaultOrder",
                schema: "Engagement",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "Engagement",
                table: "AccountSubCategory");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "Engagement",
                table: "AccountCategory");
        }
    }
}

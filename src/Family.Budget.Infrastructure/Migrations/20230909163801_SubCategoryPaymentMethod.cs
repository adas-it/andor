using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family.Budget.Infrastructure.Migrations
{
    public partial class SubCategoryPaymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DefaultPaymentMethodId",
                schema: "budget",
                table: "SubCategory",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_DefaultPaymentMethodId",
                schema: "budget",
                table: "SubCategory",
                column: "DefaultPaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategory_PaymentMethod_DefaultPaymentMethodId",
                schema: "budget",
                table: "SubCategory",
                column: "DefaultPaymentMethodId",
                principalSchema: "budget",
                principalTable: "PaymentMethod",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCategory_PaymentMethod_DefaultPaymentMethodId",
                schema: "budget",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_SubCategory_DefaultPaymentMethodId",
                schema: "budget",
                table: "SubCategory");

            migrationBuilder.DropColumn(
                name: "DefaultPaymentMethodId",
                schema: "budget",
                table: "SubCategory");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family.Budget.Infrastructure.Migrations
{
    public partial class AddIsDeletedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialMovement_PaymentMethod_PaymentMethodId",
                schema: "budget",
                table: "FinancialMovement");

            migrationBuilder.DropTable(
                name: "SubCategoryWeek",
                schema: "budget");

            migrationBuilder.DropTable(
                name: "FinancialSummary",
                schema: "budget");

            migrationBuilder.AlterColumn<Guid>(
                name: "PaymentMethodId",
                schema: "budget",
                table: "FinancialMovement",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "budget",
                table: "FinancialMovement",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialMovement_PaymentMethod_PaymentMethodId",
                schema: "budget",
                table: "FinancialMovement",
                column: "PaymentMethodId",
                principalSchema: "budget",
                principalTable: "PaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialMovement_PaymentMethod_PaymentMethodId",
                schema: "budget",
                table: "FinancialMovement");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "budget",
                table: "FinancialMovement");

            migrationBuilder.AlterColumn<Guid>(
                name: "PaymentMethodId",
                schema: "budget",
                table: "FinancialMovement",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "FinancialSummary",
                schema: "budget",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialSummary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubCategoryWeek",
                schema: "budget",
                columns: table => new
                {
                    FinancialSummaryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Week1 = table.Column<decimal>(type: "numeric", nullable: false),
                    Week2 = table.Column<decimal>(type: "numeric", nullable: false),
                    Week3 = table.Column<decimal>(type: "numeric", nullable: false),
                    Week4 = table.Column<decimal>(type: "numeric", nullable: false),
                    Week5 = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategoryWeek", x => new { x.FinancialSummaryId, x.SubCategoryId });
                    table.ForeignKey(
                        name: "FK_SubCategoryWeek_FinancialSummary_FinancialSummaryId",
                        column: x => x.FinancialSummaryId,
                        principalSchema: "budget",
                        principalTable: "FinancialSummary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubCategoryWeek_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalSchema: "budget",
                        principalTable: "SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoryWeek_SubCategoryId",
                schema: "budget",
                table: "SubCategoryWeek",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialMovement_PaymentMethod_PaymentMethodId",
                schema: "budget",
                table: "FinancialMovement",
                column: "PaymentMethodId",
                principalSchema: "budget",
                principalTable: "PaymentMethod",
                principalColumn: "Id");
        }
    }
}

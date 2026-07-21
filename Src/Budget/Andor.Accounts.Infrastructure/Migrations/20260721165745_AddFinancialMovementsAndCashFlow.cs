using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Accounts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialMovementsAndCashFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashFlow",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    PeriodKey = table.Column<int>(type: "int", nullable: false),
                    FinalBalancePreviousMonth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthRevenues = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ForecastUpcomingRevenues = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Expenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ForecastExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashFlowAppliedMovement",
                schema: "Accounts",
                columns: table => new
                {
                    FinancialMovementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashFlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppliedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlowAppliedMovement", x => x.FinancialMovementId);
                });

            migrationBuilder.CreateTable(
                name: "FinancialMovement",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SubCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialMovement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialMovement_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Accounts",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinancialMovement_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "Accounts",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinancialMovement_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalSchema: "Accounts",
                        principalTable: "SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashFlow_AccountId_PeriodKey",
                schema: "Accounts",
                table: "CashFlow",
                columns: new[] { "AccountId", "PeriodKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialMovement_AccountId",
                schema: "Accounts",
                table: "FinancialMovement",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialMovement_PaymentMethodId",
                schema: "Accounts",
                table: "FinancialMovement",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialMovement_SubCategoryId",
                schema: "Accounts",
                table: "FinancialMovement",
                column: "SubCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashFlow",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "CashFlowAppliedMovement",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "FinancialMovement",
                schema: "Accounts");
        }
    }
}

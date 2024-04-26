using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CashFlowConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashFlow",
                schema: "Engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    FinalBalancePreviousMonth = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MonthRevenues = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ForecastUpcomingRevenues = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Expenses = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ForecastExpenses = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AccountBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashFlow_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Engagement",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashFlow_AccountId_Year_Month",
                schema: "Engagement",
                table: "CashFlow",
                columns: new[] { "AccountId", "Year", "Month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashFlow",
                schema: "Engagement");
        }
    }
}

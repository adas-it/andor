using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Accounts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCashFlowAppliedMovementAndConsolidateFinancialMovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashFlowAppliedMovement",
                schema: "Accounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashFlowAppliedMovement",
                schema: "Accounts",
                columns: table => new
                {
                    FinancialMovementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppliedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CashFlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlowAppliedMovement", x => x.FinancialMovementId);
                });
        }
    }
}

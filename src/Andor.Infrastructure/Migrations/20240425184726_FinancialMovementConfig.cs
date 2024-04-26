using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinancialMovementConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialMovement",
                schema: "Engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    SubCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialMovement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialMovement_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Engagement",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancialMovement_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "Engagement",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinancialMovement_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalSchema: "Engagement",
                        principalTable: "SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialMovement_AccountId",
                schema: "Engagement",
                table: "FinancialMovement",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialMovement_PaymentMethodId",
                schema: "Engagement",
                table: "FinancialMovement",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialMovement_SubCategoryId",
                schema: "Engagement",
                table: "FinancialMovement",
                column: "SubCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialMovement",
                schema: "Engagement");
        }
    }
}

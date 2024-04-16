using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AccountDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permission_Recipient_RecipientId",
                schema: "Communication",
                table: "Permission");

            migrationBuilder.DropForeignKey(
                name: "FK_Template_Rule_RuleId",
                schema: "Communication",
                table: "Template");

            migrationBuilder.EnsureSchema(
                name: "Engagement");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "Communication",
                table: "Template",
                type: "character varying(2500)",
                maxLength: 2500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "Communication",
                table: "Template",
                type: "character varying(70)",
                maxLength: 70,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ContentLanguage",
                schema: "Communication",
                table: "Template",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Administration",
                table: "Configuration",
                type: "character varying(70)",
                maxLength: 70,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Administration",
                table: "Configuration",
                type: "character varying(70)",
                maxLength: 70,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "Engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeactivationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                schema: "Engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    Iso = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                schema: "Administration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    Iso = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                schema: "Engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeactivationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                schema: "Engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    Description = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    FirstMovement = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastMovement = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "Engagement",
                        principalTable: "Currency",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubCategory",
                schema: "Engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeactivationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    DefaultPaymentMethodId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Engagement",
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubCategory_PaymentMethod_DefaultPaymentMethodId",
                        column: x => x.DefaultPaymentMethodId,
                        principalSchema: "Engagement",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountCategory",
                schema: "Engagement",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCategory", x => new { x.AccountId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_AccountCategory_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Engagement",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Engagement",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountPaymentMethod",
                schema: "Engagement",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPaymentMethod", x => new { x.AccountId, x.PaymentMethodId });
                    table.ForeignKey(
                        name: "FK_AccountPaymentMethod_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Engagement",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountPaymentMethod_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "Engagement",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountUser",
                schema: "Engagement",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountUser", x => new { x.AccountId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AccountUser_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Engagement",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invite",
                schema: "Engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invite_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Engagement",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountSubCategory",
                schema: "Engagement",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubCategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSubCategory", x => new { x.AccountId, x.SubCategoryId });
                    table.ForeignKey(
                        name: "FK_AccountSubCategory_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Engagement",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountSubCategory_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalSchema: "Engagement",
                        principalTable: "SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_CurrencyId",
                schema: "Engagement",
                table: "Account",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCategory_CategoryId",
                schema: "Engagement",
                table: "AccountCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPaymentMethod_PaymentMethodId",
                schema: "Engagement",
                table: "AccountPaymentMethod",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSubCategory_SubCategoryId",
                schema: "Engagement",
                table: "AccountSubCategory",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Invite_AccountId",
                schema: "Engagement",
                table: "Invite",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryId",
                schema: "Engagement",
                table: "SubCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_DefaultPaymentMethodId",
                schema: "Engagement",
                table: "SubCategory",
                column: "DefaultPaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_Recipient_RecipientId",
                schema: "Communication",
                table: "Permission",
                column: "RecipientId",
                principalSchema: "Communication",
                principalTable: "Recipient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Template_Rule_RuleId",
                schema: "Communication",
                table: "Template",
                column: "RuleId",
                principalSchema: "Communication",
                principalTable: "Rule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permission_Recipient_RecipientId",
                schema: "Communication",
                table: "Permission");

            migrationBuilder.DropForeignKey(
                name: "FK_Template_Rule_RuleId",
                schema: "Communication",
                table: "Template");

            migrationBuilder.DropTable(
                name: "AccountCategory",
                schema: "Engagement");

            migrationBuilder.DropTable(
                name: "AccountPaymentMethod",
                schema: "Engagement");

            migrationBuilder.DropTable(
                name: "AccountSubCategory",
                schema: "Engagement");

            migrationBuilder.DropTable(
                name: "AccountUser",
                schema: "Engagement");

            migrationBuilder.DropTable(
                name: "Invite",
                schema: "Engagement");

            migrationBuilder.DropTable(
                name: "Language",
                schema: "Administration");

            migrationBuilder.DropTable(
                name: "SubCategory",
                schema: "Engagement");

            migrationBuilder.DropTable(
                name: "Account",
                schema: "Engagement");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "Engagement");

            migrationBuilder.DropTable(
                name: "PaymentMethod",
                schema: "Engagement");

            migrationBuilder.DropTable(
                name: "Currency",
                schema: "Engagement");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "Communication",
                table: "Template",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2500)",
                oldMaxLength: 2500);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "Communication",
                table: "Template",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(70)",
                oldMaxLength: 70);

            migrationBuilder.AlterColumn<string>(
                name: "ContentLanguage",
                schema: "Communication",
                table: "Template",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Administration",
                table: "Configuration",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(70)",
                oldMaxLength: 70);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Administration",
                table: "Configuration",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(70)",
                oldMaxLength: 70);

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_Recipient_RecipientId",
                schema: "Communication",
                table: "Permission",
                column: "RecipientId",
                principalSchema: "Communication",
                principalTable: "Recipient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Template_Rule_RuleId",
                schema: "Communication",
                table: "Template",
                column: "RuleId",
                principalSchema: "Communication",
                principalTable: "Rule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

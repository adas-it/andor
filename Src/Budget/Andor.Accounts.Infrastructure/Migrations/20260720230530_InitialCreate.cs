using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Accounts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Accounts");

            migrationBuilder.EnsureSchema(
                name: "Outbox");

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Owner = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Iso = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "Outbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ProcessedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    TargetQueue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Owner = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "Accounts",
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubCategory",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Owner = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultPaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Accounts",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubCategory_PaymentMethod_DefaultPaymentMethodId",
                        column: x => x.DefaultPaymentMethodId,
                        principalSchema: "Accounts",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountCategory",
                schema: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCategory", x => new { x.AccountId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_AccountCategory_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Accounts",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Accounts",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountPaymentMethod",
                schema: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPaymentMethod", x => new { x.AccountId, x.PaymentMethodId });
                    table.ForeignKey(
                        name: "FK_AccountPaymentMethod_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Accounts",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPaymentMethod_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "Accounts",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountUser",
                schema: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionType = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountUser", x => new { x.AccountId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AccountUser_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Accounts",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invite",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Permission = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invite_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Accounts",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountSubCategory",
                schema: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSubCategory", x => new { x.AccountId, x.SubCategoryId });
                    table.ForeignKey(
                        name: "FK_AccountSubCategory_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Accounts",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountSubCategory_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalSchema: "Accounts",
                        principalTable: "SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_CurrencyId",
                schema: "Accounts",
                table: "Account",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_IsDeleted",
                schema: "Accounts",
                table: "Account",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCategory_CategoryId",
                schema: "Accounts",
                table: "AccountCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPaymentMethod_PaymentMethodId",
                schema: "Accounts",
                table: "AccountPaymentMethod",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSubCategory_SubCategoryId",
                schema: "Accounts",
                table: "AccountSubCategory",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_IsDeleted",
                schema: "Accounts",
                table: "Category",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Invite_AccountId",
                schema: "Accounts",
                table: "Invite",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedOn_OccurredOn",
                schema: "Outbox",
                table: "OutboxMessages",
                columns: new[] { "ProcessedOn", "OccurredOn" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_IsDeleted",
                schema: "Accounts",
                table: "PaymentMethod",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryId",
                schema: "Accounts",
                table: "SubCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_DefaultPaymentMethodId",
                schema: "Accounts",
                table: "SubCategory",
                column: "DefaultPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_IsDeleted",
                schema: "Accounts",
                table: "SubCategory",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountCategory",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "AccountPaymentMethod",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "AccountSubCategory",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "AccountUser",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "Invite",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "Outbox");

            migrationBuilder.DropTable(
                name: "SubCategory",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "Account",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "PaymentMethod",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "Currency",
                schema: "Accounts");
        }
    }
}

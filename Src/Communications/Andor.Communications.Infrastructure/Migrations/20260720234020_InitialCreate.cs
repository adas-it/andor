using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Communications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CommunicationsOutbox");

            migrationBuilder.EnsureSchema(
                name: "Communication");

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "CommunicationsOutbox",
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
                name: "Rule",
                schema: "Communication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Template",
                schema: "Communication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Partner = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Template", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Template_Rule_RuleId",
                        column: x => x.RuleId,
                        principalSchema: "Communication",
                        principalTable: "Rule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedOn_OccurredOn",
                schema: "CommunicationsOutbox",
                table: "OutboxMessages",
                columns: new[] { "ProcessedOn", "OccurredOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Template_RuleId",
                schema: "Communication",
                table: "Template",
                column: "RuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "CommunicationsOutbox");

            migrationBuilder.DropTable(
                name: "Template",
                schema: "Communication");

            migrationBuilder.DropTable(
                name: "Rule",
                schema: "Communication");
        }
    }
}

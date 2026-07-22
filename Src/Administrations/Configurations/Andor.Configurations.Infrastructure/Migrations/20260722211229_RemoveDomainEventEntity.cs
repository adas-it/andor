using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Configurations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDomainEventEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DomainEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DomainEvent_Configuration_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalSchema: "Administration",
                        principalTable: "Configuration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvent_ConfigurationId",
                table: "DomainEvent",
                column: "ConfigurationId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePipelineStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FinalDate",
                schema: "Andor",
                table: "Configuration",
                newName: "ExpireDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Andor",
                table: "Configuration",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Andor",
                table: "Configuration");

            migrationBuilder.RenameColumn(
                name: "ExpireDate",
                schema: "Andor",
                table: "Configuration",
                newName: "FinalDate");
        }
    }
}

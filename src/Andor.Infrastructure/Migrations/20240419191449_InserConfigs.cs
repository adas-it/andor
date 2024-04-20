using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InserConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    INSERT INTO ""Administration"".""Configuration"" (""Id"",""Name"",""Value"",""Description"", ""StartDate"", ""ExpireDate"", ""CreatedBy"", ""CreatedAt"")
                    VALUES ('1691cc32-0110-4218-8d6a-e40764cd7e22', 'defaultLocation:country', '04fe6203-8eac-4c39-bb38-ae251194e7ee','defaultLocation:country', NOW(), NULL, 'System', NOW());

                    INSERT INTO ""Administration"".""Language"" (""Id"", ""Name"", ""Iso"", ""Symbol"")
                    VALUES ('f1daaf05-a8f2-44f0-9ed7-011490ce8e9f', 'English', 'USA', 'USA');

                    UPDATE ""Administration"".""Configuration"" SET ""Value"" = 'f1daaf05-a8f2-44f0-9ed7-011490ce8e9f'
                    WHERE ""Id"" = '8b01ade8-0467-4352-af66-4af750030e00';
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

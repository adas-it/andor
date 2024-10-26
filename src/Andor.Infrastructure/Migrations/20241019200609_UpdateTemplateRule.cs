using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTemplateRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Communication"".""Rule"" SET ""Name"" = 'wellcome_email'
                WHERE ""Id"" = '3fae1937-2742-42fe-8370-3d974bccab73';

                UPDATE ""Communication"".""Template"" SET ""Value"" = 'Hello <name> Its so good to have you with us'
                WHERE ""RuleId"" = '3fae1937-2742-42fe-8370-3d974bccab73';

                INSERT INTO ""Communication"".""Rule"" (""Id"", ""Name"", ""Type"", ""CreatedAt"")
                VALUES ('b8b02fe9-81a6-4ed2-9080-cf0a97599128', 'invite_to_create_account',1,NOW());

                INSERT INTO ""Communication"".""Rule"" (""Id"", ""Name"", ""Type"", ""CreatedAt"")
                VALUES ('25c8b193-b0eb-4f60-910f-d5e9e7135403', 'invite_to_join_account',1,NOW());

                INSERT INTO ""Communication"".""Template"" ( ""Id"",""Value"",""ContentLanguage"",""Title"",""Partner"",""CreatedAt"",""RuleId"")
                VALUES ('fbd29d3c-bd8b-4ace-bb7f-8be8870bd580', 'Hello <name> <br /> Your friend <inviting_name> has invite you to join your App', 'en', 'Title', 1, NOW(), 'b8b02fe9-81a6-4ed2-9080-cf0a97599128');

                INSERT INTO ""Communication"".""Template"" ( ""Id"",""Value"",""ContentLanguage"",""Title"",""Partner"",""CreatedAt"",""RuleId"")
                VALUES ('145537ae-d432-4e72-87ef-a8241ac5b570', 'Hello <name> <br /> You had been invited to Join <account_name>', 'en', 'Title', 1, NOW(), '25c8b193-b0eb-4f60-910f-d5e9e7135403');

                INSERT INTO ""Administration"".""Configuration""
                (""Id"", ""Name"", ""Value"", ""Description"", ""StartDate"", ""ExpireDate"", ""CreatedAt"", ""CreatedBy"")
                VALUES
                ('18ffd761-c53a-42ef-9ad5-1e05ee64f81b', 'invite_to_join_account', '25c8b193-b0eb-4f60-910f-d5e9e7135403', 'register_email', NOW(), null, NOW(), 'System'),
                ('f305bef3-c340-4926-9dc5-c83037a9607c', 'invite_to_create_account', 'b8b02fe9-81a6-4ed2-9080-cf0a97599128', 'register_email', NOW(), null, NOW(), 'System');
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

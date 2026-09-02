using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Communications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialRulesAndTemplates : Migration
    {
        private static readonly Guid OnboardingVerificationCodeRuleId = new("acb860a5-1af6-4b03-afae-e290dfcac7d4");
        private static readonly Guid WelcomeRuleId = new("42b2aa27-82e9-4d20-8e88-afbf25e2c721");

        private static readonly Guid OnboardingVerificationCodeTemplateId = new("a382b4e7-db3d-49a8-a15b-c258e86cd6d0");
        private static readonly Guid WelcomeTemplateId = new("d5b1f6c2-8a34-4e79-9c1b-2f7e0a4d6b83");

        // Fixed timestamps so the migration is deterministic across environments.
        private static readonly DateTime RuleCreatedAt = new(2026, 9, 2, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime TemplateCreatedAt = new(2026, 8, 31, 12, 7, 30, 510, DateTimeKind.Utc);

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Communication",
                table: "Rule",
                columns: new[] { "Id", "Name", "Type", "CreatedAt" },
                values: new object[,]
                {
                    { OnboardingVerificationCodeRuleId, "onboarding-verification-code", 1, RuleCreatedAt },
                    { WelcomeRuleId, "wellcome", 1, RuleCreatedAt }
                });

            migrationBuilder.InsertData(
                schema: "Communication",
                table: "Template",
                columns: new[] { "Id", "Value", "ContentLanguage", "Title", "Subject", "Partner", "IsDefault", "CreatedAt", "RuleId" },
                values: new object[,]
                {
                    {
                        OnboardingVerificationCodeTemplateId,
                        "<h1>Hello <name>!</h1><br>Your code is <code>",
                        "en",
                        "wellcome",
                        "Welcome Email",
                        1,
                        true,
                        TemplateCreatedAt,
                        OnboardingVerificationCodeRuleId
                    },
                    {
                        WelcomeTemplateId,
                        "<h1>Welcome <name>!</h1><br>We're glad to have you on board.",
                        "en",
                        "wellcome",
                        "Welcome Email",
                        1,
                        true,
                        TemplateCreatedAt,
                        WelcomeRuleId
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Communication",
                table: "Template",
                keyColumn: "Id",
                keyValues: new object[] { OnboardingVerificationCodeTemplateId, WelcomeTemplateId });

            migrationBuilder.DeleteData(
                schema: "Communication",
                table: "Rule",
                keyColumn: "Id",
                keyValues: new object[] { OnboardingVerificationCodeRuleId, WelcomeRuleId });
        }
    }
}

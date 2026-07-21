using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Accounts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedGenericAccountingTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO [Accounts].[Currency] ([Id], [Name], [Iso], [Symbol])
                VALUES ('01994953-f745-73f4-85b3-03b5aad12591', 'US Dollar', 'USD', '$');
            ");

            // PaymentMethod.Type is assigned so it matches the Type of the SubCategories that default to it
            // (Account.AddTemplateSubCategory/SubCategory.SetDefaultPaymentMethod require the same MovementType) --
            // the reverse of the numbering used by the legacy 'Engagement' schema this data was adapted from.
            migrationBuilder.Sql(@"
                INSERT INTO [Accounts].[PaymentMethod] ([Id], [Owner], [Name], [Description], [Type])
                VALUES
                ('b05246de-8fc0-481a-80be-9197f6db3fb2', NULL, 'payment_method_a_cash', '', 2),
                ('71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6', NULL, 'payment_method_b_debit_card', '', 2),
                ('45e87b93-f001-44b4-a7a6-a099618a3042', NULL, 'payment_method_c_transfer', '', 2),
                ('ffad05f8-5748-498c-8002-1d1624107fc7', NULL, 'payment_method_d_automatic_debit', '', 2),
                ('5d44c865-5e56-4f12-926c-6c97a44cc2a9', NULL, 'payment_method_e_withheld_at_source', '', 2),
                ('b72a24e3-7c5f-4790-9a3b-959ddf3c8315', NULL, 'payment_method_f_credit_card_billthismonth', '', 2),
                ('93cef651-35c5-4486-81e4-a61962695b81', NULL, 'payment_method_g_credit_card_nextmonth', '', 2),
                ('d463630d-308b-4b88-a764-cb8592ac93d1', NULL, 'payment_method_transfer', '', 1),
                ('a2bf0c1f-941d-41e0-9dc1-578e7c232e2f', NULL, 'payment_method_cash', '', 1);
            ");

            migrationBuilder.Sql(@"
                INSERT INTO [Accounts].[Category] ([Id], [Owner], [Name], [Description], [Type])
                VALUES
                ('cc1b00df-79fc-4058-915e-81c9eb2ba76e', NULL, 'feeding', 'feeding', 2),
                ('271dd7e2-80c9-4d66-94f5-75e74dc90bc8', NULL, 'habitation', 'habitation', 2),
                ('97487a15-cc62-4452-8743-0492c8bcf70e', NULL, 'education', '', 2),
                ('b276a770-4a78-4a2d-aa5f-02ea0f6b9e63', NULL, 'communication', '', 2),
                ('4550f2fc-c1aa-4caf-9e93-934e783885ac', NULL, 'health', '', 2),
                ('f615eb79-4510-49a1-b695-4602e9010519', NULL, 'transport', '', 2),
                ('2a266c4f-ad87-4f77-825b-1945b503a393', NULL, 'personal', '', 2),
                ('e863073c-6d4e-47c0-9c3e-bb1e7accd7f0', NULL, 'leisure', '', 2),
                ('c7588d95-44a9-4199-95e5-775f5ff2cec2', NULL, 'financial_services', '', 2),
                ('6bb017a1-7e65-4d38-a7bb-347e25c656f3', NULL, 'financial_income', 'Financial Income', 1);
            ");

            migrationBuilder.Sql(@"
                INSERT INTO [Accounts].[SubCategory] ([Id], [Owner], [Name], [Description], [CategoryId], [DefaultPaymentMethodId])
                VALUES
                ('c24acdb1-2897-48cf-bb9e-c5531493817b', NULL, 'supermarket', '', 'cc1b00df-79fc-4058-915e-81c9eb2ba76e', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('5e211458-23b9-4039-9dc2-276831b4898e', NULL, 'fair-greengrocer', '', 'cc1b00df-79fc-4058-915e-81c9eb2ba76e', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('355d81b0-e61c-4364-aa02-f1f45083c14c', NULL, 'bakery', '', 'cc1b00df-79fc-4058-915e-81c9eb2ba76e', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('2a6083f0-46e0-4020-8c11-0574fb233b04', NULL, 'food-other', '', 'cc1b00df-79fc-4058-915e-81c9eb2ba76e', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('099f8f85-830c-4711-bedf-3f8bcb6e608f', NULL, 'rent', '', '271dd7e2-80c9-4d66-94f5-75e74dc90bc8', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('1370e6ba-3454-45cd-a8ef-00a514062281', NULL, 'gas', '', '271dd7e2-80c9-4d66-94f5-75e74dc90bc8', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('d8938ce6-0906-4b44-9576-8cbdec4dae18', NULL, 'water', '', '271dd7e2-80c9-4d66-94f5-75e74dc90bc8', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('c5dbe44f-a726-4433-8177-c1e09298d73f', NULL, 'laundry', '', '271dd7e2-80c9-4d66-94f5-75e74dc90bc8', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('ecf967cb-0d8a-4401-8c33-667c2926d8ff', NULL, 'electricity', '', '271dd7e2-80c9-4d66-94f5-75e74dc90bc8', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('183fff27-adbc-4ca2-9769-03c2d62bf5e2', NULL, 'others', '', '271dd7e2-80c9-4d66-94f5-75e74dc90bc8', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('cb064858-5fdf-417f-9c0e-926f0cddfa90', NULL, 'property-taxes', '', '271dd7e2-80c9-4d66-94f5-75e74dc90bc8', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('5a800f23-1de0-41d3-b7fe-f198d7101f5d', NULL, 'house-makeover', '', '271dd7e2-80c9-4d66-94f5-75e74dc90bc8', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('934831fc-3a1a-4e76-b83f-3eab2a8442b4', NULL, 'monthly-payment', '', '97487a15-cc62-4452-8743-0492c8bcf70e', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('bb9db1f0-765b-4c8b-a582-ffc18eecadb1', NULL, 'school supplies', '', '97487a15-cc62-4452-8743-0492c8bcf70e', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('a5f40ccb-5d9b-45c3-bd0d-5bcfefe21fde', NULL, 'other courses', '', '97487a15-cc62-4452-8743-0492c8bcf70e', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('343c37c6-ffab-4676-9fc9-baa680821907', NULL, 'school bus', '', '97487a15-cc62-4452-8743-0492c8bcf70e', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('3a9220f1-b88f-45b0-9f7a-125024734156', NULL, 'cell phone', '', 'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('b18efdd9-0471-498c-97a9-117cc7d6f5b1', NULL, 'netflix', '', 'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('07d2886b-755e-41f0-bba5-666b758c0a4b', NULL, 'internet', '', 'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('3c1da1cc-f42a-4f47-af46-2d047936c303', NULL, 'Cable-tv', '', 'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('e324efc4-137b-4de7-b413-193c980a3e84', NULL, 'academy', '', '4550f2fc-c1aa-4caf-9e93-934e783885ac', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('8b953444-b56b-4f7a-a18d-b6172c166c56', NULL, 'medicines', '', '4550f2fc-c1aa-4caf-9e93-934e783885ac', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('cf7689ab-41cd-492f-9a01-fecaa82f4cb2', NULL, 'health-insurance', '', '4550f2fc-c1aa-4caf-9e93-934e783885ac', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('5142df27-edc6-4fe9-8530-a8ef08f13f6a', NULL, 'perfumes', '', '4550f2fc-c1aa-4caf-9e93-934e783885ac', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('ce01c625-c112-43c3-a990-3c157367dae8', NULL, 'bath', '', '4550f2fc-c1aa-4caf-9e93-934e783885ac', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('2b11bc58-655a-4be9-9fd8-225ed2b13bbf', NULL, 'others', '', 'f615eb79-4510-49a1-b695-4602e9010519', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('2a4d6017-1a3b-4d33-9bbd-076b49183260', NULL, 'taxi', '', 'f615eb79-4510-49a1-b695-4602e9010519', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('966f5091-2b89-45a3-b2d8-0f2fe99512e8', NULL, 'fuel', '', 'f615eb79-4510-49a1-b695-4602e9010519', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('ceeb5d7a-17fc-4d5a-9dd2-037381802506', NULL, 'parking', '', 'f615eb79-4510-49a1-b695-4602e9010519', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('969902e0-3052-4d40-abe1-beaf6cc9d5ee', NULL, 'safe', '', 'f615eb79-4510-49a1-b695-4602e9010519', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('8ba87af7-ea9b-4033-b160-ee215e710f8d', NULL, 'maintenance', '', 'f615eb79-4510-49a1-b695-4602e9010519', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('32b4643d-163c-41d4-9bc4-a8b1e2c0eaa1', NULL, 'subway', '', 'f615eb79-4510-49a1-b695-4602e9010519', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('7c49a988-d266-4955-be5e-bb58c5d05318', NULL, 'toll', '', 'f615eb79-4510-49a1-b695-4602e9010519', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('32783b35-b2ba-4f4a-ad74-5082292bdd56', NULL, 'vehicle-circulation-tax', '', 'f615eb79-4510-49a1-b695-4602e9010519', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('d00e7053-4383-4d85-9dce-797d4baa34fb', NULL, 'clothing-footwear', '', '2a266c4f-ad87-4f77-825b-1945b503a393', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('8aeb1ed6-d042-49aa-8311-022f09f43e08', NULL, 'hairdresser-manicure', '', '2a266c4f-ad87-4f77-825b-1945b503a393', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('d6cfe9d5-a18d-4256-9840-91ae498b27b6', NULL, 'manicure', '', '2a266c4f-ad87-4f77-825b-1945b503a393', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('507dc46d-1180-4b91-b49e-3c5c9b771468', NULL, 'gifts', '', '2a266c4f-ad87-4f77-825b-1945b503a393', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('55cbe641-e644-4b91-8fc1-0c5e4ec07c0a', NULL, 'others', '', '2a266c4f-ad87-4f77-825b-1945b503a393', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('f44d9413-5245-4d53-b725-fbba130654b2', NULL, 'cinema-theater', '', 'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('8f84ab7a-e2ee-4233-95a3-2554e9aa7a7d', NULL, 'books-magazines-cds', '', 'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('e68506a2-f500-48f8-a915-f48026d574f4', NULL, 'transportation-costs', '', 'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('f65f4f8f-2558-4bff-a06e-e382937f93f6', NULL, 'trips', '', 'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('b46e4020-f460-4070-9e92-e8c9f9774c87', NULL, 'restaurants-bars', '', 'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('1b56ca8b-ba30-476f-919d-ec7ba970cc71', NULL, 'loans', '', 'c7588d95-44a9-4199-95e5-775f5ff2cec2', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('7f7007ca-2739-440b-89c1-1a8a65a8e95f', NULL, 'insurance', '', 'c7588d95-44a9-4199-95e5-775f5ff2cec2', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('b331e327-2950-4271-aeb3-fadad062858f', NULL, 'private-pension', '', 'c7588d95-44a9-4199-95e5-775f5ff2cec2', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('603399c7-0960-4063-b595-b8c5289730ef', NULL, 'interest-overdraft-check', '', 'c7588d95-44a9-4199-95e5-775f5ff2cec2', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('735e7e23-3cd4-4c06-aee2-2ad7ae5bcef4', NULL, 'bank-fees', '', 'c7588d95-44a9-4199-95e5-775f5ff2cec2', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('b66aad18-5f6d-467f-a43b-4383b2fe3015', NULL, 'social-security', '', 'c7588d95-44a9-4199-95e5-775f5ff2cec2', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('36266214-f43d-4470-839d-ecd58380d0ef', NULL, 'credit-card-bill', '', 'c7588d95-44a9-4199-95e5-775f5ff2cec2', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('210212d5-4229-4a29-9501-401a089edb34', NULL, 'personal-income-tax', '', 'c7588d95-44a9-4199-95e5-775f5ff2cec2', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('687f3df5-baad-45db-8ba0-a958fbce1913', NULL, 'investments', '', 'c7588d95-44a9-4199-95e5-775f5ff2cec2', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6'),
                ('172fcd0d-4f01-4650-8e17-407aca64709d', NULL, 'salary-advance', '', '6bb017a1-7e65-4d38-a7bb-347e25c656f3', 'd463630d-308b-4b88-a764-cb8592ac93d1'),
                ('b09fc2ec-380f-4cea-bf1b-fca58ef7b5cc', NULL, 'vacation', '', '6bb017a1-7e65-4d38-a7bb-347e25c656f3', 'd463630d-308b-4b88-a764-cb8592ac93d1'),
                ('0a06fd0c-3a7d-4bbd-a553-8d6a5ba1347c', NULL, '13th-salary', '', '6bb017a1-7e65-4d38-a7bb-347e25c656f3', 'd463630d-308b-4b88-a764-cb8592ac93d1'),
                ('9ca9083f-ed80-40ec-bc33-596a35674455', NULL, 'food-allowance', '', '6bb017a1-7e65-4d38-a7bb-347e25c656f3', 'd463630d-308b-4b88-a764-cb8592ac93d1'),
                ('967374c1-9c77-4d22-9b0a-aeea12d4984d', NULL, 'extra-income', '', '6bb017a1-7e65-4d38-a7bb-347e25c656f3', 'd463630d-308b-4b88-a764-cb8592ac93d1');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [Accounts].[SubCategory] WHERE [CategoryId] IN (
                    'cc1b00df-79fc-4058-915e-81c9eb2ba76e', '271dd7e2-80c9-4d66-94f5-75e74dc90bc8',
                    '97487a15-cc62-4452-8743-0492c8bcf70e', 'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63',
                    '4550f2fc-c1aa-4caf-9e93-934e783885ac', 'f615eb79-4510-49a1-b695-4602e9010519',
                    '2a266c4f-ad87-4f77-825b-1945b503a393', 'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0',
                    'c7588d95-44a9-4199-95e5-775f5ff2cec2', '6bb017a1-7e65-4d38-a7bb-347e25c656f3'
                );
            ");

            migrationBuilder.Sql(@"
                DELETE FROM [Accounts].[Category] WHERE [Id] IN (
                    'cc1b00df-79fc-4058-915e-81c9eb2ba76e', '271dd7e2-80c9-4d66-94f5-75e74dc90bc8',
                    '97487a15-cc62-4452-8743-0492c8bcf70e', 'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63',
                    '4550f2fc-c1aa-4caf-9e93-934e783885ac', 'f615eb79-4510-49a1-b695-4602e9010519',
                    '2a266c4f-ad87-4f77-825b-1945b503a393', 'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0',
                    'c7588d95-44a9-4199-95e5-775f5ff2cec2', '6bb017a1-7e65-4d38-a7bb-347e25c656f3'
                );
            ");

            migrationBuilder.Sql(@"
                DELETE FROM [Accounts].[PaymentMethod] WHERE [Id] IN (
                    'b05246de-8fc0-481a-80be-9197f6db3fb2', '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6',
                    '45e87b93-f001-44b4-a7a6-a099618a3042', 'ffad05f8-5748-498c-8002-1d1624107fc7',
                    '5d44c865-5e56-4f12-926c-6c97a44cc2a9', 'b72a24e3-7c5f-4790-9a3b-959ddf3c8315',
                    '93cef651-35c5-4486-81e4-a61962695b81', 'd463630d-308b-4b88-a764-cb8592ac93d1',
                    'a2bf0c1f-941d-41e0-9dc1-578e7c232e2f'
                );
            ");

            migrationBuilder.Sql(@"
                DELETE FROM [Accounts].[Currency] WHERE [Id] = '01994953-f745-73f4-85b3-03b5aad12591';
            ");
        }
    }
}

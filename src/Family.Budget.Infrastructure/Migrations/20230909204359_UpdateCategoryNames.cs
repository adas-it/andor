using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family.Budget.Infrastructure.Migrations
{
    public partial class UpdateCategoryNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'payment_method_a_cash' WHERE ""Id"" = 'b05246de-8fc0-481a-80be-9197f6db3fb2';
UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'payment_method_b_debit_card' WHERE ""Id"" = '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6';
UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'payment_method_c_transfer' WHERE ""Id"" = '45e87b93-f001-44b4-a7a6-a099618a3042';
UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'payment_method_d_automatic_debit' WHERE ""Id"" = 'ffad05f8-5748-498c-8002-1d1624107fc7';
UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'payment_method_e_withheld_at_source' WHERE ""Id"" = '5d44c865-5e56-4f12-926c-6c97a44cc2a9';
UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'payment_method_f_credit_card_billthismonth' WHERE ""Id"" = 'b72a24e3-7c5f-4790-9a3b-959ddf3c8315';
UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'payment_method_g_credit_card_nextmonth' WHERE ""Id"" = '93cef651-35c5-4486-81e4-a61962695b81';
UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'payment_method_transfer' WHERE ""Id"" = 'd463630d-308b-4b88-a764-cb8592ac93d1';
UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'payment_method_cash' WHERE ""Id"" = 'a2bf0c1f-941d-41e0-9dc1-578e7c232e2f';

UPDATE ""budget"".""Category"" SET ""Name"" = 'feeding' WHERE ""Id"" = 'cc1b00df-79fc-4058-915e-81c9eb2ba76e';
UPDATE ""budget"".""Category"" SET ""Name"" = 'habitation' WHERE ""Id"" = '271dd7e2-80c9-4d66-94f5-75e74dc90bc8';
UPDATE ""budget"".""Category"" SET ""Name"" = 'education' WHERE ""Id"" = '97487a15-cc62-4452-8743-0492c8bcf70e';
UPDATE ""budget"".""Category"" SET ""Name"" = 'communication' WHERE ""Id"" = 'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63';
UPDATE ""budget"".""Category"" SET ""Name"" = 'health' WHERE ""Id"" = '4550f2fc-c1aa-4caf-9e93-934e783885ac';
UPDATE ""budget"".""Category"" SET ""Name"" = 'transport' WHERE ""Id"" = 'f615eb79-4510-49a1-b695-4602e9010519';
UPDATE ""budget"".""Category"" SET ""Name"" = 'personal' WHERE ""Id"" = '2a266c4f-ad87-4f77-825b-1945b503a393';
UPDATE ""budget"".""Category"" SET ""Name"" = 'leisure' WHERE ""Id"" = 'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0';
UPDATE ""budget"".""Category"" SET ""Name"" = 'financial_services' WHERE ""Id"" = 'c7588d95-44a9-4199-95e5-775f5ff2cec2';
UPDATE ""budget"".""Category"" SET ""Name"" = 'financial_income' WHERE ""Id"" = '6bb017a1-7e65-4d38-a7bb-347e25c656f3';

UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'supermarket' WHERE ""Id"" = 'c24acdb1-2897-48cf-bb9e-c5531493817b';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'fair-greengrocer' WHERE ""Id"" = '5e211458-23b9-4039-9dc2-276831b4898e';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'bakery' WHERE ""Id"" = '355d81b0-e61c-4364-aa02-f1f45083c14c';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'food-other' WHERE ""Id"" = '2a6083f0-46e0-4020-8c11-0574fb233b04';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'rent' WHERE ""Id"" = '099f8f85-830c-4711-bedf-3f8bcb6e608f';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'gas' WHERE ""Id"" = '1370e6ba-3454-45cd-a8ef-00a514062281';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'water' WHERE ""Id"" = 'd8938ce6-0906-4b44-9576-8cbdec4dae18';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'laundry' WHERE ""Id"" = 'c5dbe44f-a726-4433-8177-c1e09298d73f';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'electricity' WHERE ""Id"" = 'ecf967cb-0d8a-4401-8c33-667c2926d8ff';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'others' WHERE ""Id"" = '183fff27-adbc-4ca2-9769-03c2d62bf5e2';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'property-taxes' WHERE ""Id"" = 'cb064858-5fdf-417f-9c0e-926f0cddfa90';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'house-makeover' WHERE ""Id"" = '5a800f23-1de0-41d3-b7fe-f198d7101f5d';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'monthly-payment' WHERE ""Id"" = '934831fc-3a1a-4e76-b83f-3eab2a8442b4';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'school supplies' WHERE ""Id"" = 'bb9db1f0-765b-4c8b-a582-ffc18eecadb1';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'other courses' WHERE ""Id"" = 'a5f40ccb-5d9b-45c3-bd0d-5bcfefe21fde';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'school bus' WHERE ""Id"" = '343c37c6-ffab-4676-9fc9-baa680821907';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'cell phone' WHERE ""Id"" = '3a9220f1-b88f-45b0-9f7a-125024734156';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'netflix' WHERE ""Id"" = 'b18efdd9-0471-498c-97a9-117cc7d6f5b1';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'internet' WHERE ""Id"" = '07d2886b-755e-41f0-bba5-666b758c0a4b';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'Cable-tv' WHERE ""Id"" = '3c1da1cc-f42a-4f47-af46-2d047936c303';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'academy' WHERE ""Id"" = 'e324efc4-137b-4de7-b413-193c980a3e84';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'medicines' WHERE ""Id"" = '8b953444-b56b-4f7a-a18d-b6172c166c56';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'health-insurance' WHERE ""Id"" = 'cf7689ab-41cd-492f-9a01-fecaa82f4cb2';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'perfumes' WHERE ""Id"" = '5142df27-edc6-4fe9-8530-a8ef08f13f6a';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'bath' WHERE ""Id"" = 'ce01c625-c112-43c3-a990-3c157367dae8';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'others' WHERE ""Id"" = '2b11bc58-655a-4be9-9fd8-225ed2b13bbf';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'taxi' WHERE ""Id"" = '2a4d6017-1a3b-4d33-9bbd-076b49183260';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'fuel' WHERE ""Id"" = '966f5091-2b89-45a3-b2d8-0f2fe99512e8';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'parking' WHERE ""Id"" = 'ceeb5d7a-17fc-4d5a-9dd2-037381802506';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'safe' WHERE ""Id"" = '969902e0-3052-4d40-abe1-beaf6cc9d5ee';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'maintenance' WHERE ""Id"" = '8ba87af7-ea9b-4033-b160-ee215e710f8d';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'subway' WHERE ""Id"" = '32b4643d-163c-41d4-9bc4-a8b1e2c0eaa1';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'toll' WHERE ""Id"" = '7c49a988-d266-4955-be5e-bb58c5d05318';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'vehicle-circulation-tax' WHERE ""Id"" = '32783b35-b2ba-4f4a-ad74-5082292bdd56';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'clothing-footwear' WHERE ""Id"" = 'd00e7053-4383-4d85-9dce-797d4baa34fb';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'hairdresser-manicure' WHERE ""Id"" = '8aeb1ed6-d042-49aa-8311-022f09f43e08';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'manicure' WHERE ""Id"" = 'd6cfe9d5-a18d-4256-9840-91ae498b27b6';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'gifts' WHERE ""Id"" = '507dc46d-1180-4b91-b49e-3c5c9b771468';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'others' WHERE ""Id"" = '55cbe641-e644-4b91-8fc1-0c5e4ec07c0a';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'cinema-theater' WHERE ""Id"" = 'f44d9413-5245-4d53-b725-fbba130654b2';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'transportation-costs' WHERE ""Id"" = 'e68506a2-f500-48f8-a915-f48026d574f4';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'books-magazines-cds' WHERE ""Id"" = '8f84ab7a-e2ee-4233-95a3-2554e9aa7a7d';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'trips' WHERE ""Id"" = 'f65f4f8f-2558-4bff-a06e-e382937f93f6';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'restaurants-bars' WHERE ""Id"" = 'b46e4020-f460-4070-9e92-e8c9f9774c87';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'loans' WHERE ""Id"" = '1b56ca8b-ba30-476f-919d-ec7ba970cc71';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'insurance' WHERE ""Id"" = '7f7007ca-2739-440b-89c1-1a8a65a8e95f';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'private-pension' WHERE ""Id"" = 'b331e327-2950-4271-aeb3-fadad062858f';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'interest-overdraft-check' WHERE ""Id"" = '603399c7-0960-4063-b595-b8c5289730ef';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'bank-fees' WHERE ""Id"" = '735e7e23-3cd4-4c06-aee2-2ad7ae5bcef4';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'social-security' WHERE ""Id"" = 'b66aad18-5f6d-467f-a43b-4383b2fe3015';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'credit-card-bill' WHERE ""Id"" = '36266214-f43d-4470-839d-ecd58380d0ef';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'personal-income-tax' WHERE ""Id"" = '210212d5-4229-4a29-9501-401a089edb34';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'investments' WHERE ""Id"" = '687f3df5-baad-45db-8ba0-a958fbce1913';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'salary-advance' WHERE ""Id"" = '172fcd0d-4f01-4650-8e17-407aca64709d';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'vacation' WHERE ""Id"" = 'b09fc2ec-380f-4cea-bf1b-fca58ef7b5cc';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = '13th-salary' WHERE ""Id"" = '0a06fd0c-3a7d-4bbd-a553-8d6a5ba1347c';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'food-allowance' WHERE ""Id"" = '9ca9083f-ed80-40ec-bc33-596a35674455';
UPDATE ""budget"".""SubCategory"" SET ""Name"" = 'extra-income' WHERE ""Id"" = '967374c1-9c77-4d22-9b0a-aeea12d4984d';
            ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryAndSubCategoryOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Engagement"".""Category"" SET ""DefaultOrder"" = 1 WHERE ""Id"" = 'cc1b00df-79fc-4058-915e-81c9eb2ba76e';
                UPDATE ""Engagement"".""Category"" SET ""DefaultOrder"" = 2 WHERE ""Id"" = '271dd7e2-80c9-4d66-94f5-75e74dc90bc8';
                UPDATE ""Engagement"".""Category"" SET ""DefaultOrder"" = 3 WHERE ""Id"" = '97487a15-cc62-4452-8743-0492c8bcf70e';
                UPDATE ""Engagement"".""Category"" SET ""DefaultOrder"" = 4 WHERE ""Id"" = 'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63';
                UPDATE ""Engagement"".""Category"" SET ""DefaultOrder"" = 5 WHERE ""Id"" = '4550f2fc-c1aa-4caf-9e93-934e783885ac';
                UPDATE ""Engagement"".""Category"" SET ""DefaultOrder"" = 6 WHERE ""Id"" = 'f615eb79-4510-49a1-b695-4602e9010519';
                UPDATE ""Engagement"".""Category"" SET ""DefaultOrder"" = 7 WHERE ""Id"" = '2a266c4f-ad87-4f77-825b-1945b503a393';
                UPDATE ""Engagement"".""Category"" SET ""DefaultOrder"" = 8 WHERE ""Id"" = 'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0';
                UPDATE ""Engagement"".""Category"" SET ""DefaultOrder"" = 9 WHERE ""Id"" = 'c7588d95-44a9-4199-95e5-775f5ff2cec2';
                UPDATE ""Engagement"".""Category"" SET ""DefaultOrder"" = 1 WHERE ""Id"" = '6bb017a1-7e65-4d38-a7bb-347e25c656f3';

                UPDATE ""Engagement"".""AccountCategory"" AS ""A""
                SET ""Order"" = ""B"".""DefaultOrder""
                FROM ""Engagement"".""Category"" AS ""B""
                WHERE ""A"".""CategoryId"" = ""B"".""Id"";

                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 1 WHERE ""Id"" = '172fcd0d-4f01-4650-8e17-407aca64709d';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 2 WHERE ""Id"" = 'b09fc2ec-380f-4cea-bf1b-fca58ef7b5cc';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 3 WHERE ""Id"" = '0a06fd0c-3a7d-4bbd-a553-8d6a5ba1347c';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 4 WHERE ""Id"" = '9ca9083f-ed80-40ec-bc33-596a35674455';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 5 WHERE ""Id"" = '967374c1-9c77-4d22-9b0a-aeea12d4984d';

                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 1 WHERE ""Id"" = 'c24acdb1-2897-48cf-bb9e-c5531493817b';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 2 WHERE ""Id"" = '5e211458-23b9-4039-9dc2-276831b4898e';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 3 WHERE ""Id"" = '355d81b0-e61c-4364-aa02-f1f45083c14c';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 4 WHERE ""Id"" = '2a6083f0-46e0-4020-8c11-0574fb233b04';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 5 WHERE ""Id"" = '099f8f85-830c-4711-bedf-3f8bcb6e608f';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 6 WHERE ""Id"" = '1370e6ba-3454-45cd-a8ef-00a514062281';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 7 WHERE ""Id"" = 'd8938ce6-0906-4b44-9576-8cbdec4dae18';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 8 WHERE ""Id"" = 'c5dbe44f-a726-4433-8177-c1e09298d73f';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 9 WHERE ""Id"" = 'ecf967cb-0d8a-4401-8c33-667c2926d8ff';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 10 WHERE ""Id"" = '183fff27-adbc-4ca2-9769-03c2d62bf5e2';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 11 WHERE ""Id"" = 'cb064858-5fdf-417f-9c0e-926f0cddfa90';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 12 WHERE ""Id"" = '5a800f23-1de0-41d3-b7fe-f198d7101f5d';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 13 WHERE ""Id"" = '934831fc-3a1a-4e76-b83f-3eab2a8442b4';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 14 WHERE ""Id"" = 'bb9db1f0-765b-4c8b-a582-ffc18eecadb1';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 15 WHERE ""Id"" = 'a5f40ccb-5d9b-45c3-bd0d-5bcfefe21fde';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 16 WHERE ""Id"" = '343c37c6-ffab-4676-9fc9-baa680821907';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 17 WHERE ""Id"" = '3a9220f1-b88f-45b0-9f7a-125024734156';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 18 WHERE ""Id"" = 'b18efdd9-0471-498c-97a9-117cc7d6f5b1';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 19 WHERE ""Id"" = '07d2886b-755e-41f0-bba5-666b758c0a4b';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 20 WHERE ""Id"" = '3c1da1cc-f42a-4f47-af46-2d047936c303';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 21 WHERE ""Id"" = 'e324efc4-137b-4de7-b413-193c980a3e84';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 22 WHERE ""Id"" = '8b953444-b56b-4f7a-a18d-b6172c166c56';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 23 WHERE ""Id"" = 'cf7689ab-41cd-492f-9a01-fecaa82f4cb2';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 24 WHERE ""Id"" = '5142df27-edc6-4fe9-8530-a8ef08f13f6a';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 25 WHERE ""Id"" = 'ce01c625-c112-43c3-a990-3c157367dae8';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 26 WHERE ""Id"" = '2b11bc58-655a-4be9-9fd8-225ed2b13bbf';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 27 WHERE ""Id"" = '2a4d6017-1a3b-4d33-9bbd-076b49183260';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 28 WHERE ""Id"" = '966f5091-2b89-45a3-b2d8-0f2fe99512e8';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 29 WHERE ""Id"" = 'ceeb5d7a-17fc-4d5a-9dd2-037381802506';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 30 WHERE ""Id"" = '969902e0-3052-4d40-abe1-beaf6cc9d5ee';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 31 WHERE ""Id"" = '8ba87af7-ea9b-4033-b160-ee215e710f8d';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 32 WHERE ""Id"" = '32b4643d-163c-41d4-9bc4-a8b1e2c0eaa1';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 33 WHERE ""Id"" = '7c49a988-d266-4955-be5e-bb58c5d05318';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 34 WHERE ""Id"" = '32783b35-b2ba-4f4a-ad74-5082292bdd56';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 35 WHERE ""Id"" = 'd00e7053-4383-4d85-9dce-797d4baa34fb';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 36 WHERE ""Id"" = '8aeb1ed6-d042-49aa-8311-022f09f43e08';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 37 WHERE ""Id"" = 'd6cfe9d5-a18d-4256-9840-91ae498b27b6';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 38 WHERE ""Id"" = '507dc46d-1180-4b91-b49e-3c5c9b771468';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 39 WHERE ""Id"" = '55cbe641-e644-4b91-8fc1-0c5e4ec07c0a';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 40 WHERE ""Id"" = 'f44d9413-5245-4d53-b725-fbba130654b2';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 41 WHERE ""Id"" = 'e68506a2-f500-48f8-a915-f48026d574f4';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 42 WHERE ""Id"" = '8f84ab7a-e2ee-4233-95a3-2554e9aa7a7d';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 43 WHERE ""Id"" = 'f65f4f8f-2558-4bff-a06e-e382937f93f6';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 44 WHERE ""Id"" = 'b46e4020-f460-4070-9e92-e8c9f9774c87';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 45 WHERE ""Id"" = '1b56ca8b-ba30-476f-919d-ec7ba970cc71';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 46 WHERE ""Id"" = '7f7007ca-2739-440b-89c1-1a8a65a8e95f';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 47 WHERE ""Id"" = 'b331e327-2950-4271-aeb3-fadad062858f';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 48 WHERE ""Id"" = '603399c7-0960-4063-b595-b8c5289730ef';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 49 WHERE ""Id"" = '735e7e23-3cd4-4c06-aee2-2ad7ae5bcef4';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 50 WHERE ""Id"" = 'b66aad18-5f6d-467f-a43b-4383b2fe3015';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 51 WHERE ""Id"" = '36266214-f43d-4470-839d-ecd58380d0ef';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 52 WHERE ""Id"" = '210212d5-4229-4a29-9501-401a089edb34';
                UPDATE ""Engagement"".""SubCategory"" SET ""DefaultOrder"" = 53 WHERE ""Id"" = '687f3df5-baad-45db-8ba0-a958fbce1913';

                UPDATE ""Engagement"".""AccountSubCategory"" AS ""A""
                SET ""Order"" = ""B"".""DefaultOrder""
                FROM ""Engagement"".""SubCategory"" AS ""B""
                WHERE ""A"".""SubCategoryId"" = ""B"".""Id"";
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

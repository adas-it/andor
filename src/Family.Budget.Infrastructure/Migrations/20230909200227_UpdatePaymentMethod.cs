using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family.Budget.Infrastructure.Migrations
{
    public partial class UpdatePaymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""budget"".""AccountPaymentMethod""
                WHERE ""AccountPaymentMethod"".""PaymentMethodId"" IN ('0d49c9c6-ee52-4670-8654-bf0d819e0b80','4e8263ee-0769-486f-ac80-a1804e28f729','3a32cefe-aeb7-465a-a63f-883346df8639');

                DELETE FROM ""budget"".""PaymentMethod""
                WHERE ""PaymentMethod"".""Id"" IN ('0d49c9c6-ee52-4670-8654-bf0d819e0b80','4e8263ee-0769-486f-ac80-a1804e28f729','3a32cefe-aeb7-465a-a63f-883346df8639');

                UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'Transfer'
                WHERE ""Id"" = 'd463630d-308b-4b88-a764-cb8592ac93d1';

                UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'Cash'
                WHERE ""Id"" = 'a2bf0c1f-941d-41e0-9dc1-578e7c232e2f';

                UPDATE ""budget"".""SubCategory"" SET ""DefaultPaymentMethodId"" = '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6';

                UPDATE ""budget"".""SubCategory"" SET ""DefaultPaymentMethodId"" = 'd463630d-308b-4b88-a764-cb8592ac93d1'
                WHERE ""Id"" IN ('172fcd0d-4f01-4650-8e17-407aca64709d',
                'b09fc2ec-380f-4cea-bf1b-fca58ef7b5cc',
                '0a06fd0c-3a7d-4bbd-a553-8d6a5ba1347c',
                '9ca9083f-ed80-40ec-bc33-596a35674455',
                '967374c1-9c77-4d22-9b0a-aeea12d4984d');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family.Budget.Infrastructure.Migrations
{
    public partial class DeleteUnusedPaymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'Transfer'
                    WHERE ""Id"" = 'd463630d-308b-4b88-a764-cb8592ac93d1';

                    UPDATE ""budget"".""PaymentMethod"" SET ""Name"" = 'Cash'
                    WHERE ""Id"" = 'a2bf0c1f-941d-41e0-9dc1-578e7c232e2f';

                    DELETE FROM ""budget"".""PaymentMethod""
                    WHERE ""PaymentMethod"".""Id"" IN ('0d49c9c6-ee52-4670-8654-bf0d819e0b80','4e8263ee-0769-486f-ac80-a1804e28f729','3a32cefe-aeb7-465a-a63f-883346df8639');

                    DELETE FROM ""budget"".""AccountPaymentMethod""
                    WHERE ""AccountPaymentMethod"".""PaymentMethodId"" IN ('0d49c9c6-ee52-4670-8654-bf0d819e0b80','4e8263ee-0769-486f-ac80-a1804e28f729','3a32cefe-aeb7-465a-a63f-883346df8639');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

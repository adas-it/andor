﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentMethodOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Engagement"".""PaymentMethod"" SET ""DefaultOrder"" = 1 WHERE ""Id"" = 'd463630d-308b-4b88-a764-cb8592ac93d1';
                UPDATE ""Engagement"".""PaymentMethod"" SET ""DefaultOrder"" = 2 WHERE ""Id"" = 'a2bf0c1f-941d-41e0-9dc1-578e7c232e2f';

                UPDATE ""Engagement"".""PaymentMethod"" SET ""DefaultOrder"" = 1 WHERE ""Id"" = 'b05246de-8fc0-481a-80be-9197f6db3fb2';
                UPDATE ""Engagement"".""PaymentMethod"" SET ""DefaultOrder"" = 2 WHERE ""Id"" = '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6';
                UPDATE ""Engagement"".""PaymentMethod"" SET ""DefaultOrder"" = 3 WHERE ""Id"" = '45e87b93-f001-44b4-a7a6-a099618a3042';
                UPDATE ""Engagement"".""PaymentMethod"" SET ""DefaultOrder"" = 4 WHERE ""Id"" = 'ffad05f8-5748-498c-8002-1d1624107fc7';
                UPDATE ""Engagement"".""PaymentMethod"" SET ""DefaultOrder"" = 5 WHERE ""Id"" = '5d44c865-5e56-4f12-926c-6c97a44cc2a9';
                UPDATE ""Engagement"".""PaymentMethod"" SET ""DefaultOrder"" = 6 WHERE ""Id"" = 'b72a24e3-7c5f-4790-9a3b-959ddf3c8315';
                UPDATE ""Engagement"".""PaymentMethod"" SET ""DefaultOrder"" = 7 WHERE ""Id"" = '93cef651-35c5-4486-81e4-a61962695b81';

                UPDATE ""Engagement"".""AccountPaymentMethod"" AS ""A""
                SET ""Order"" = ""B"".""DefaultOrder""
                FROM ""Engagement"".""PaymentMethod"" AS ""B""
                WHERE ""A"".""PaymentMethodId"" = ""B"".""Id"";
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

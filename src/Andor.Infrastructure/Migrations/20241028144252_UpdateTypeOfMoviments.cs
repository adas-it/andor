using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTypeOfMoviments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId" },
                principalTable: "InboxState",
                principalColumns: new[] { "MessageId", "ConsumerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessage_OutboxState_OutboxId",
                table: "OutboxMessage",
                column: "OutboxId",
                principalTable: "OutboxState",
                principalColumn: "OutboxId");


            migrationBuilder.Sql(@"
                UPDATE ""Engagement"".""PaymentMethod"" SET ""Name"" = 'payment_method_transfer' , ""Type"" = 1
                WHERE ""PaymentMethod"".""Id"" = 'd463630d-308b-4b88-a764-cb8592ac93d1';

                UPDATE ""Engagement"".""PaymentMethod"" SET ""Name"" = 'payment_method_cash' , ""Type"" = 1
                WHERE ""PaymentMethod"".""Id"" = 'a2bf0c1f-941d-41e0-9dc1-578e7c232e2f';

                UPDATE ""Engagement"".""PaymentMethod"" SET ""Type"" = 2
                WHERE ""PaymentMethod"".""Id"" IN ('b05246de-8fc0-481a-80be-9197f6db3fb2',
                '71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6',
                '45e87b93-f001-44b4-a7a6-a099618a3042',
                'ffad05f8-5748-498c-8002-1d1624107fc7',
                '5d44c865-5e56-4f12-926c-6c97a44cc2a9',
                'b72a24e3-7c5f-4790-9a3b-959ddf3c8315',
                '93cef651-35c5-4486-81e4-a61962695b81');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessage_OutboxState_OutboxId",
                table: "OutboxMessage");
        }
    }
}

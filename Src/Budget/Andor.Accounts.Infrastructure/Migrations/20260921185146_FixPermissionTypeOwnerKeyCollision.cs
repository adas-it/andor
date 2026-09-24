using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Accounts.Infrastructure.Migrations
{
    /// <summary>
    /// Data-only fix for the <c>PermissionType</c> key collision: <c>Editor</c> and <c>Owner</c>
    /// used to share key 2 (see Andor.Accounts.Domain.PermissionTypes.PermissionType), so
    /// Enumeration.GetByKey(2) always resolved to Editor, silently downgrading every persisted
    /// Owner on reload. Owner now has key 3. No path in Application ever persisted an Editor
    /// before this release (LinkMember/InviteMember were never reachable from a Command/Actor),
    /// so every existing row with PermissionType = 2 is, in practice, an Owner created by
    /// Account.NewAsync (always Order = 1). Re-point those rows at the new Owner key 3 instead of
    /// leaving them to be misread as Editor.
    /// </summary>
    public partial class FixPermissionTypeOwnerKeyCollision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [Accounts].[AccountUser]
                SET [PermissionType] = 3
                WHERE [PermissionType] = 2;
                """);

            migrationBuilder.Sql(
                """
                UPDATE [Accounts].[Invite]
                SET [Permission] = 3
                WHERE [Permission] = 2;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [Accounts].[AccountUser]
                SET [PermissionType] = 2
                WHERE [PermissionType] = 3;
                """);

            migrationBuilder.Sql(
                """
                UPDATE [Accounts].[Invite]
                SET [Permission] = 2
                WHERE [Permission] = 3;
                """);
        }
    }
}

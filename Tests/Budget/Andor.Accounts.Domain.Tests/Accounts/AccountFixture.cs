using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Tests.Currencies;
using Andor.Accounts.Domain.Users;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Moq;

namespace Andor.Accounts.Domain.Tests.Accounts;

/// <summary>
/// Fixture for creating Account instances for testing purposes.
/// </summary>
internal static class AccountFixture
{
    /// <summary>
    /// Creates a valid account with a single owner member.
    /// </summary>
    /// <param name="validatorMock">Optional validator mock. If not provided, a default one will be created.</param>
    /// <param name="accountId">Optional account ID. If not provided, a new one will be generated.</param>
    /// <param name="name">Optional account name. If not provided, a valid name will be generated.</param>
    /// <param name="description">Optional account description. If not provided, a valid description will be generated.</param>
    /// <param name="currency">Optional currency. If not provided, USD will be used.</param>
    /// <param name="ownerUserId">Optional owner user ID. If not provided, a new one will be generated.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the domain result and the created account.</returns>
    public static async Task<(DomainResult result, Account? account)> CreateValidAccountAsync(
        Mock<IAccountValidator>? validatorMock = null,
        AccountId? accountId = null,
        string? name = null,
        string? description = null,
        Currency? currency = null,
        Guid? ownerUserId = null,
        CancellationToken cancellationToken = default)
    {
        var validator = validatorMock ?? CreateDefaultValidator();

        var result = await Account.NewAsync(
            accountId ?? AccountId.New(),
            name ?? GeneralFixture.GetValidName(),
            description ?? GeneralFixture.GetValidDescription(),
            currency ?? CurrencyFixture.GetUsdCurrency(),
            ownerUserId ?? Guid.NewGuid(),
            validator.Object,
            cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates a valid account with multiple members.
    /// </summary>
    /// <param name="validatorMock">Optional validator mock. If not provided, a default one will be created.</param>
    /// <param name="additionalMembers">Additional members to add to the account with their permission types.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the account and the owner user ID.</returns>
    public static async Task<(Account account, Guid ownerUserId)> CreateAccountWithMembersAsync(
        Mock<IAccountValidator>? validatorMock = null,
        IEnumerable<(Guid userId, PermissionType permission)>? additionalMembers = null,
        CancellationToken cancellationToken = default)
    {
        var (result, account) = await CreateValidAccountAsync(validatorMock, cancellationToken: cancellationToken);

        if (result.IsFailure || account == null)
        {
            throw new InvalidOperationException("Failed to create valid account for test.");
        }

        var ownerUserId = account.Members.First().UserId;

        if (additionalMembers != null)
        {
            foreach (var (userId, permission) in additionalMembers)
            {
                var user = new User { Id = userId };
                var linkResult = account.LinkMember(user, permission, ownerUserId);

                if (linkResult.IsFailure)
                {
                    throw new InvalidOperationException($"Failed to link member with permission {permission}.");
                }
            }
        }

        return (account, ownerUserId);
    }

    /// <summary>
    /// Creates a valid account with an additional member of specific permission type.
    /// </summary>
    /// <param name="validatorMock">Optional validator mock. If not provided, a default one will be created.</param>
    /// <param name="additionalMemberPermission">The permission type for the additional member.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the account, owner user ID, and additional member user ID.</returns>
    public static async Task<(Account account, Guid ownerUserId, Guid additionalMemberId)> CreateAccountWithAdditionalMemberAsync(
        PermissionType additionalMemberPermission,
        Mock<IAccountValidator>? validatorMock = null,
        CancellationToken cancellationToken = default)
    {
        var additionalMemberId = Guid.NewGuid();
        var members = new[] { (additionalMemberId, additionalMemberPermission) };

        var (account, ownerUserId) = await CreateAccountWithMembersAsync(
            validatorMock, 
            members, 
            cancellationToken);

        return (account, ownerUserId, additionalMemberId);
    }

    /// <summary>
    /// Creates a default validator mock that returns success for validation.
    /// </summary>
    /// <returns>A configured mock validator.</returns>
    public static Mock<IAccountValidator> CreateDefaultValidator()
    {
        var validatorMock = new Mock<IAccountValidator>();
        validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());
        return validatorMock;
    }

    /// <summary>
    /// Creates a validator mock that returns validation failures.
    /// </summary>
    /// <param name="notifications">The list of notifications to return.</param>
    /// <returns>A configured mock validator that fails validation.</returns>
    public static Mock<IAccountValidator> CreateFailingValidator(List<Notification> notifications)
    {
        var validatorMock = new Mock<IAccountValidator>();
        validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);
        return validatorMock;
    }
}

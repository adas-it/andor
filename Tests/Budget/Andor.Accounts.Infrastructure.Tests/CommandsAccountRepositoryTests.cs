using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Infrastructure.Context;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Andor.Accounts.Infrastructure.Tests;

/// <summary>
/// Exercises <see cref="CommandsAccountRepository"/> against a real EF Core model (InMemory
/// provider) instead of mocking <see cref="AccountsContext"/>, so mapping/config mistakes
/// (missing Includes, wrong Upsert semantics, soft-delete query filters) surface here rather
/// than only in production. Every test opens its own uniquely named database, and read-back
/// assertions use a *separate* context/repository instance to mirror how the app actually
/// works: each Akka command handler resolves a fresh DI scope (see AreaActor/AccountActor's
/// Loading state in CLAUDE.md) rather than reusing the context that wrote the data.
/// </summary>
public class CommandsAccountRepositoryTests
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    private AccountsContext CreateContext()
        => new(InMemoryDbContextOptionsFactory.Create<AccountsContext>(_databaseName));

    [Fact]
    public async Task GetByIdAsync_WhenAccountDoesNotExist_ReturnsNull()
    {
        var repository = new CommandsAccountRepository(CreateContext());

        var result = await repository.GetByIdAsync(AccountId.New(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task PersistAsync_WhenAccountIsNew_InsertsAggregateWithOwnerMemberAndCurrency()
    {
        var (_, account) = await CreateValidAccountAsync();
        var ownerId = account!.Members.Single().UserId;

        await new CommandsAccountRepository(CreateContext()).PersistAsync(account, CancellationToken.None);

        var persisted = await new CommandsAccountRepository(CreateContext())
            .GetByIdAsync(account.Id, CancellationToken.None);

        persisted.Should().NotBeNull();
        persisted!.Name.Should().Be(account.Name);
        persisted.Currency.Iso.Should().Be("USD");
        persisted.Members.Should().ContainSingle(m => m.UserId == ownerId);
    }

    [Fact]
    public async Task GetByIdAsync_LoadsCategoriesPaymentMethodsAndInvitesThroughIncludes()
    {
        var (_, account) = await CreateValidAccountAsync();
        var ownerId = account!.Members.Single().UserId;

        account.CreateCustomCategory(GeneralFixture.GetValidName(), GeneralFixture.GetValidDescription(), MovementType.MoneyDeposit, ownerId);
        account.CreateCustomPaymentMethod(GeneralFixture.GetValidName(), GeneralFixture.GetValidDescription(), MovementType.MoneyDeposit, ownerId);
        account.InviteMemberByUser(Guid.NewGuid(), PermissionType.Viewer, ownerId);

        await new CommandsAccountRepository(CreateContext()).PersistAsync(account, CancellationToken.None);

        var persisted = await new CommandsAccountRepository(CreateContext())
            .GetByIdAsync(account.Id, CancellationToken.None);

        persisted!.Categories.Should().ContainSingle();
        persisted.PaymentMethods.Should().ContainSingle();
        persisted.Invites.Should().ContainSingle();
    }

    [Fact]
    public async Task PersistAsync_WhenAccountAlreadyExists_UpdatesInPlaceInsteadOfInsertingDuplicate()
    {
        var (_, account) = await CreateValidAccountAsync();
        var ownerId = account!.Members.Single().UserId;
        await new CommandsAccountRepository(CreateContext()).PersistAsync(account, CancellationToken.None);

        // Simulates a second command hitting the same aggregate: a fresh scope loads it,
        // mutates it, and saves it back — the same shape as the ManagerActor->Actor pattern.
        var secondScopeContext = CreateContext();
        var secondScopeRepository = new CommandsAccountRepository(secondScopeContext);
        var loadedAccount = await secondScopeRepository.GetByIdAsync(account!.Id, CancellationToken.None);
        loadedAccount!.SoftDelete(ownerId);
        await secondScopeRepository.PersistAsync(loadedAccount, CancellationToken.None);

        await using var verifyContext = CreateContext();
        var rowCount = await verifyContext.Set<Account>()
            .IgnoreQueryFilters()
            .CountAsync(x => x.Id == account.Id);

        rowCount.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAccountIsSoftDeleted_IsHiddenByTheGlobalQueryFilter()
    {
        var (_, account) = await CreateValidAccountAsync();
        var ownerId = account!.Members.Single().UserId;
        account.SoftDelete(ownerId);

        await new CommandsAccountRepository(CreateContext()).PersistAsync(account, CancellationToken.None);

        var persisted = await new CommandsAccountRepository(CreateContext())
            .GetByIdAsync(account.Id, CancellationToken.None);

        persisted.Should().BeNull();
    }

    private static async Task<(DomainResult result, Account? account)> CreateValidAccountAsync()
    {
        var validatorMock = new Mock<IAccountValidator>();
        validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        var (_, currency) = Currency.New(GeneralFixture.GetValidName(), "USD", "$");

        return await Account.NewAsync(
            AccountId.New(),
            GeneralFixture.GetValidName(),
            GeneralFixture.GetValidDescription(),
            currency!,
            Guid.NewGuid(),
            validatorMock.Object,
            CancellationToken.None);
    }
}

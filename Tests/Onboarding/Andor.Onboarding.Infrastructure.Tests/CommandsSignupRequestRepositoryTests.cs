using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Domain;
using Andor.Onboarding.Domain.ValueObjects;
using Andor.Onboarding.Infrastructure.Context;
using Andor.TestsUtil;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Andor.Onboarding.Infrastructure.Tests;

/// <summary>
/// Exercises <see cref="CommandsSignupRequestRepository"/> against a real EF Core model
/// (InMemory provider) instead of mocking <see cref="OnboardingContext"/>, following the same
/// pattern as <c>Andor.Accounts.Infrastructure.Tests</c> and
/// <c>Andor.Communications.Infrastructure.Tests</c>. Each test uses its own uniquely named
/// database, and read-back assertions go through a separate context/repository instance to
/// mirror the fresh-DI-scope-per-command pattern used by the Akka.NET actors in production.
/// </summary>
public class CommandsSignupRequestRepositoryTests
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    private OnboardingContext CreateContext()
        => new(InMemoryDbContextOptionsFactory.Create<OnboardingContext>(_databaseName));

    [Fact]
    public async Task GetByIdAsync_WhenSignupRequestDoesNotExist_ReturnsNull()
    {
        var repository = new CommandsSignupRequestRepository(CreateContext());

        var result = await repository.GetByIdAsync(SignupRequestId.New(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task PersistAsync_WhenSignupRequestIsNew_InsertsIt()
    {
        var request = await CreateValidSignupRequestAsync("new.signup@andor.test");

        await new CommandsSignupRequestRepository(CreateContext()).PersistAsync(request, CancellationToken.None);

        var persisted = await new CommandsSignupRequestRepository(CreateContext())
            .GetByIdAsync(request.Id, CancellationToken.None);

        persisted.Should().NotBeNull();
        persisted!.Email.Should().Be(request.Email);
        persisted.VerificationCode.Should().Be(request.VerificationCode);
        persisted.IsVerified.Should().BeFalse();
    }

    [Fact]
    public async Task GetByEmailAsync_WhenMultipleRequestsShareEmail_ReturnsTheMostRecentOne()
    {
        const string email = "duplicate.signup@andor.test";
        var repository = new CommandsSignupRequestRepository(CreateContext());

        var first = await CreateValidSignupRequestAsync(email);
        await repository.PersistAsync(first, CancellationToken.None);

        await Task.Delay(25);

        var second = await CreateValidSignupRequestAsync(email);
        await repository.PersistAsync(second, CancellationToken.None);

        var mostRecent = await new CommandsSignupRequestRepository(CreateContext())
            .GetByEmailAsync(email, CancellationToken.None);

        mostRecent.Should().NotBeNull();
        mostRecent!.Id.Should().Be(second.Id);
    }

    [Fact]
    public async Task PersistAsync_WhenSignupRequestAlreadyExists_UpdatesInPlaceInsteadOfInsertingDuplicate()
    {
        var request = await CreateValidSignupRequestAsync("verify.me@andor.test");
        await new CommandsSignupRequestRepository(CreateContext()).PersistAsync(request, CancellationToken.None);

        // Simulates a second command hitting the same aggregate through a fresh scope.
        var secondScopeContext = CreateContext();
        var secondScopeRepository = new CommandsSignupRequestRepository(secondScopeContext);
        var loadedRequest = await secondScopeRepository.GetByIdAsync(request.Id, CancellationToken.None);
        loadedRequest!.Verify(loadedRequest.VerificationCode, "hashed-password");
        await secondScopeRepository.PersistAsync(loadedRequest, CancellationToken.None);

        await using var verifyContext = CreateContext();
        var rowCount = await verifyContext.SignupRequest.CountAsync(x => x.Id == request.Id);
        rowCount.Should().Be(1);

        var persisted = await new CommandsSignupRequestRepository(CreateContext())
            .GetByIdAsync(request.Id, CancellationToken.None);
        persisted!.IsVerified.Should().BeTrue();
    }

    private static async Task<SignupRequest> CreateValidSignupRequestAsync(string email)
    {
        var validatorMock = new Mock<IOnboardingValidator>();
        validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<SignupRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        var (_, request) = await SignupRequest.NewAsync(
            SignupRequestId.New(),
            GeneralFixture.GetValidName(),
            email,
            validatorMock.Object,
            CancellationToken.None);

        return request!;
    }
}

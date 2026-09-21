using Andor.Communications.Domain;
using Andor.Communications.Domain.ValueObjects;
using Andor.Communications.Infrastructure.Context;
using Andor.TestsUtil;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using CommunicationType = Andor.Communications.Domain.ValueObjects.Type;

namespace Andor.Communications.Infrastructure.Tests;

/// <summary>
/// Exercises <see cref="CommandsRuleRepository"/> against a real EF Core model (InMemory
/// provider) rather than mocking <see cref="CommunicationContext"/>, so the repository's
/// Include/Upsert wiring is verified the same way as <c>Andor.Accounts.Infrastructure.Tests</c>.
/// Each test uses its own uniquely named database, and read-back assertions go through a
/// separate context/repository instance to mirror the fresh-DI-scope-per-command pattern used
/// by the Akka.NET actors in production. Tests that aren't specifically about caching give each
/// repository instance its own fresh <see cref="IMemoryCache"/> so they keep exercising the
/// database round-trip; the cache-specific tests below share one cache across instances instead.
/// </summary>
public class CommandsRuleRepositoryTests
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    private CommunicationContext CreateContext()
        => new(InMemoryDbContextOptionsFactory.Create<CommunicationContext>(_databaseName));

    private static IMemoryCache CreateCache() => new MemoryCache(new MemoryCacheOptions());

    [Fact]
    public async Task GetByIdAsync_WhenRuleDoesNotExist_ReturnsNull()
    {
        var repository = new CommandsRuleRepository(CreateContext(), CreateCache());

        var result = await repository.GetByIdAsync(RuleId.New(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task PersistAsync_WhenRuleIsNew_InsertsRule()
    {
        var rule = await CreateValidRuleAsync();

        await new CommandsRuleRepository(CreateContext(), CreateCache()).PersistAsync(rule, CancellationToken.None);

        var persisted = await new CommandsRuleRepository(CreateContext(), CreateCache())
            .GetByIdAsync(rule.Id, CancellationToken.None);

        persisted.Should().NotBeNull();
        persisted!.Name.Should().Be(rule.Name);
        persisted.Type.Should().Be(CommunicationType.Information);
    }

    [Fact]
    public async Task GetByIdAsync_LoadsTemplatesThroughInclude()
    {
        var rule = await CreateValidRuleAsync();
        var (_, template) = Template.New("Body", "en-US", "Welcome", "Welcome", Partner.InHouse, rule, false);
        rule.Templates.Add(template!);

        await new CommandsRuleRepository(CreateContext(), CreateCache()).PersistAsync(rule, CancellationToken.None);

        var persisted = await new CommandsRuleRepository(CreateContext(), CreateCache())
            .GetByIdAsync(rule.Id, CancellationToken.None);

        persisted!.Templates.Should().ContainSingle(t => t.Title == "Welcome");
    }

    [Fact]
    public async Task PersistAsync_WhenRuleAlreadyExists_UpdatesInPlaceInsteadOfInsertingDuplicate()
    {
        var rule = await CreateValidRuleAsync();
        await new CommandsRuleRepository(CreateContext(), CreateCache()).PersistAsync(rule, CancellationToken.None);

        // Simulates a second command hitting the same aggregate through a fresh scope.
        var secondScopeContext = CreateContext();
        var secondScopeRepository = new CommandsRuleRepository(secondScopeContext, CreateCache());
        var loadedRule = await secondScopeRepository.GetByIdAsync(rule.Id, CancellationToken.None);
        var (_, template) = Template.New("Body", "en-US", "Welcome", "Welcome", Partner.InHouse, loadedRule!, false);
        loadedRule!.Templates.Add(template!);
        await secondScopeRepository.PersistAsync(loadedRule, CancellationToken.None);

        await using var verifyContext = CreateContext();
        var rowCount = await verifyContext.Rule.CountAsync(x => x.Id == rule.Id);

        rowCount.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCalledTwiceWithSameCache_ReturnsCachedInstanceWithoutRequeryingDatabase()
    {
        var rule = await CreateValidRuleAsync();
        var cache = CreateCache();

        await new CommandsRuleRepository(CreateContext(), cache).PersistAsync(rule, CancellationToken.None);

        var repository = new CommandsRuleRepository(CreateContext(), cache);
        var first = await repository.GetByIdAsync(rule.Id, CancellationToken.None);

        // Removing the row straight through a fresh context proves the second call below is
        // served from cache rather than re-querying the (now empty) database.
        await using (var directContext = CreateContext())
        {
            directContext.Rule.Remove(directContext.Rule.Single(x => x.Id == rule.Id));
            await directContext.SaveChangesAsync(CancellationToken.None);
        }

        var second = await repository.GetByIdAsync(rule.Id, CancellationToken.None);

        second.Should().BeSameAs(first);
    }

    [Fact]
    public async Task PersistAsync_RemovesTheCachedEntryForThatRule()
    {
        var rule = await CreateValidRuleAsync();
        var cache = CreateCache();
        var repository = new CommandsRuleRepository(CreateContext(), cache);
        var cacheKey = $"Rule:{rule.Id.Value}";

        await repository.PersistAsync(rule, CancellationToken.None);
        _ = await repository.GetByIdAsync(rule.Id, CancellationToken.None);

        cache.TryGetValue(cacheKey, out _).Should().BeTrue("the read above should have populated the cache");

        await repository.PersistAsync(rule, CancellationToken.None);

        cache.TryGetValue(cacheKey, out _).Should().BeFalse("persisting should invalidate the stale cache entry");
    }

    private static async Task<Rule> CreateValidRuleAsync()
    {
        var (_, rule) = await Rule.NewAsync(
            RuleId.New(),
            "Weekly digest",
            CommunicationType.Information,
            new List<Template>(),
            skipValidations: true,
            userId: Guid.NewGuid(),
            validator: new Mock<IRuleValidator>().Object,
            CancellationToken.None);

        return rule!;
    }
}

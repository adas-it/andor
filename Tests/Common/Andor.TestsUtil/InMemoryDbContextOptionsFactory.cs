using Microsoft.EntityFrameworkCore;

namespace Andor.TestsUtil;

/// <summary>
/// Builds isolated EF Core InMemory options for infrastructure/repository tests. Each call
/// (or each test, when no name is supplied) gets its own database instance so tests never
/// see state left behind by another test.
/// </summary>
public static class InMemoryDbContextOptionsFactory
{
    public static DbContextOptions<TContext> Create<TContext>(string? databaseName = null)
        where TContext : DbContext
        => new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;
}

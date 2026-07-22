namespace Andor.ComponentTests.Common;

/// <summary>
/// Identity impersonated by <see cref="TestAuthHandler"/>. Group names must match whatever the
/// module under test seeded into its authorization repository (e.g. Configurations seeds
/// "Administrador"/"Administrador Senior").
/// </summary>
public sealed record TestUser(Guid Id, string Group, string Tenant = "TenantA")
{
    public static TestUser Default { get; } = new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Administrador Senior");
}

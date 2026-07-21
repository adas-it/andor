using Andor.Accounts.Binder.Application;
using Andor.Accounts.Binder.Infrastructure;
using Andor.Accounts.Domain.Currencies;
using Andor.Accounts.Domain.Currencies.Repositories;
using Andor.Accounts.RestApi;
using Andor.Foundation.Application;
using Andor.Foundation.Domain.ValuesObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Accounts.Binder;

public static class AccountsExtensions
{
    public static WebApplicationBuilder UseAccounts(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        _ = builder.Services.AddScoped<ITenantService, TenantService>();

        _ = builder.Services.UseApi()
            .WithAccountsApplication()
            .WithAccountsInfrastructure(configuration);

        return builder;
    }

    public static Task ApplyAccountsMigrationsAsync(this WebApplication app)
        => app.Services.ApplyAccountsMigrationsAsync();

    /// <summary>
    /// Seeds the default currency ("BRL") used when an account is created without an explicit
    /// one (e.g. auto-created from the signup flow). Placeholder until currencies get a real
    /// admin surface.
    /// </summary>
    public static async Task SeedDefaultCurrencyAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<ICommandsCurrencyRepository>();

        var existing = await repository.GetByIsoAsync("BRL", CancellationToken.None);

        if (existing != null)
        {
            return;
        }

        var (result, currency) = Currency.New(new Name("Real Brasileiro"), "BRL", "R$");

        if (result.IsSuccess && currency != null)
        {
            await repository.PersistAsync(currency, CancellationToken.None);
        }
    }
}

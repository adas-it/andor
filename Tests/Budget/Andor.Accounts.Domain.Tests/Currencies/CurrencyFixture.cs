using Andor.Accounts.Domain.Currencies;

namespace Andor.Accounts.Domain.Tests.Currencies;

internal static class CurrencyFixture
{
    /// <summary>
    /// Creates a valid USD currency for testing.
    /// </summary>
    public static Currency GetUsdCurrency()
        => CreateCurrency("USD", "US Dollar", "$");

    /// <summary>
    /// Creates a valid EUR currency for testing.
    /// </summary>
    public static Currency GetEurCurrency()
        => CreateCurrency("EUR", "Euro", "€");

    /// <summary>
    /// Creates a valid BRL currency for testing.
    /// </summary>
    public static Currency GetBrlCurrency()
        => CreateCurrency("BRL", "Brazilian Real", "R$");

    /// <summary>
    /// Creates a valid GBP currency for testing.
    /// </summary>
    public static Currency GetGbpCurrency()
        => CreateCurrency("GBP", "British Pound", "£");

    /// <summary>
    /// Creates a custom currency with specified values.
    /// </summary>
    /// <param name="code">The currency code (e.g., "USD").</param>
    /// <param name="name">The currency name (e.g., "US Dollar").</param>
    /// <param name="symbol">The currency symbol (e.g., "$").</param>
    /// <returns>A valid Currency instance.</returns>
    public static Currency CreateCurrency(string code, string name, string symbol)
    {
        var (result, currency) = Currency.New(code, name, symbol);

        if (result.IsFailure || currency == null)
        {
            throw new InvalidOperationException(
                $"Failed to create currency: {string.Join(", ", result.Errors.Select(e => e.Message))}");
        }

        return currency;
    }
}

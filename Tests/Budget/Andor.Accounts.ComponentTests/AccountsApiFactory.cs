using Andor.Accounts.Infrastructure.Context;
using Andor.ComponentTests.Common;

namespace Andor.Accounts.ComponentTests;

public sealed class AccountsApiFactory : ComponentTestWebApplicationFactory<Program, AccountsContext>
{
}

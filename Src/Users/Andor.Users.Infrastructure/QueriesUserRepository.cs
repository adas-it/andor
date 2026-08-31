using Andor.Users.Application.Interfaces;
using Andor.Users.Infrastructure.Context;
using Foundation.Infrastructure;
using Andor.Users.Domain.Users;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.Infrastructure;

public class QueriesUserRepository(UserContext context) :
    QueryHelper<User, UserId>(context), IQueriesUserRepository
{
}

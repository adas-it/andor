using Andor.Foundation.Domain.Validation;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.Domain.Users;

public interface IUserValidator : IDefaultValidator<User, UserId>
{
}

using Andor.Foundation.Domain.Validation;
using Users.Users.ValueObjects;

namespace Users.Users;

public interface IUserValidator : IDefaultValidator<User, UserId>
{
}

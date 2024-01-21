namespace Family.Budget.Application.@Common.Interfaces;

using Family.Budget.Domain.Entities.Users;
using Family.Budget.Domain.Entities.Users.ValueObject;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IKeycloackService
{
    Task<User> CreateUser(string Username,
        string Email,
        string FirstName,
        string LastName,
        string Password,
        LocationInfos DefaultLanguage,
        string? Avatar,
        CancellationToken cancellationToken);

    Task<List<User>?> GetUserByEmail(string email, CancellationToken cancellation);
    Task<List<User>?> GetUserByUserName(string username, CancellationToken cancellation);
    Task<User?> GetUserByUserId(Guid userId, CancellationToken cancellation);
}
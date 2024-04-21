using Andor.Domain.Communications.Users;
using Andor.Domain.Communications.Users.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Communications.Repositories;

public interface ICommandsRecipientRepository : ICommandRepository<Recipient, RecipientId>
{
}
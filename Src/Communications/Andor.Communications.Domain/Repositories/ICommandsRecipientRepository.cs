using Andor.Communications.Domain.Users;
using Andor.Communications.Domain.Users.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;

namespace Andor.Communications.Domain.Repositories;

public interface ICommandsRecipientRepository :
    ICommandRepository<Recipient, RecipientId>
{
}

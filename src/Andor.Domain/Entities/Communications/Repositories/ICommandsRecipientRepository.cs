using Andor.Domain.Entities.Communications.Users;
using Andor.Domain.Entities.Communications.Users.ValueObjects;
using Andor.Domain.SeedWork.Repository.CommandRepository;

namespace Andor.Domain.Entities.Communications.Repositories;

public interface ICommandsRecipientRepository : ICommandRepository<Recipient, RecipientId>
{
}
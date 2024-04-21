using Andor.Domain.Communications;
using Andor.Domain.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Communications.Repositories;

public interface ICommandsTemplateRepository : ICommandRepository<Template, TemplateId>
{
}
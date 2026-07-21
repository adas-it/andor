using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;

namespace Andor.Accounts.Domain.Categories.Repositories;

public interface ICommandsCategoryRepository : ICommandRepository<Category, CategoryId>
{
    Task<IReadOnlyList<Category>> GetTemplatesAsync(CancellationToken cancellationToken);
}

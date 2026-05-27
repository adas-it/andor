using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;

namespace Andor.Accounts.Domain.SubCategories.Repositories;

public interface ICommandsSubCategoryRepository : ICommandRepository<SubCategory, SubCategoryId>
{
}

using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.SubCategories.Repositories;

public interface ICommandsSubCategoryRepository : ICommandRepository<SubCategory, SubCategoryId>
{
}

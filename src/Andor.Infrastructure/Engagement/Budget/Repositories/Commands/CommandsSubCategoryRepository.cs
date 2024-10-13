using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Commands;

public class CommandsSubCategoryRepository(PrincipalContext context) :
    CommandsBaseRepository<SubCategory, SubCategoryId>(context),
    ICommandsSubCategoryRepository
{
}

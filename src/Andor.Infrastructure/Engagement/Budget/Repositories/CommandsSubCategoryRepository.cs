using Andor.Domain.Engagement.Budget.Entities.SubCategories;
using Andor.Domain.Engagement.Budget.Entities.SubCategories.ValueObjects;
using Andor.Domain.Onboarding.Registrations.Repositories;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class CommandsSubCategoryRepository(PrincipalContext context) :
    CommandsBaseRepository<SubCategory, SubCategoryId>(context),
    ICommandsSubCategoryRepository
{
}

using Andor.Domain.Engagement.Budget.Entities.SubCategories;
using Andor.Domain.Engagement.Budget.Entities.SubCategories.ValueObjects;
using Andor.Domain.SeedWork.Repository.CommandRepository;

namespace Andor.Domain.Onboarding.Registrations.Repositories;

public interface ICommandsSubCategoryRepository : ICommandRepository<SubCategory, SubCategoryId>
{
}

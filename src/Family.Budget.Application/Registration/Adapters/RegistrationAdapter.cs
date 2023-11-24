namespace Family.Budget.Application.Registration.Adapters;
using Family.Budget.Application.Dto.Registrations.Responses;
using Family.Budget.Domain.Entities.Registrations;

public static class RegistrationAdapter
{
    public static RegistrationOutput MapDtoFromDomain(this Registration entity)
        => new(entity.FirstName, entity.LastName, entity.Email);
}

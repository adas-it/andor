namespace Family.Budget.Application.PaymentMethod.Adapters;
using Family.Budget.Application.Dto.PaymentMethods.Responses;
using Family.Budget.Domain.Entities.PaymentMethods;

public static class PaymentMethodAdapter
{
    public static PaymentMethodOutput MapDtoFromDomain(this PaymentMethod item)
        => new(item.Id, item.Name, item.Description, item.StartDate, item.DeactivationDate);
}

using Andor.Application.Communications.Interfaces;
using Andor.Communications.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Application.Communications.Services.Manager;

public interface IPartnerManager
{
    IPartner GetPartnerHandler(Partner partner);
}

// Keyed by Partner.Key so adding a new channel is just another AddKeyedScoped<IPartner, T>(...)
// registration — this manager doesn't need to change to route to it.
public class PartnerManager(IServiceProvider serviceProvider) : IPartnerManager
{
    public IPartner GetPartnerHandler(Partner partner)
        => serviceProvider.GetKeyedService<IPartner>(partner.Key)
            ?? throw new ArgumentOutOfRangeException(nameof(partner), $"Not expected partner value: {partner}");
}
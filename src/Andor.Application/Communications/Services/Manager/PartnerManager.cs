using Andor.Application.Communications.Interfaces;
using Andor.Domain.Communications.ValueObjects;

namespace Andor.Application.Communications.Services.Manager;

public interface IPartnerManager
{
    IPartner GetPartnerHandler(Partner partner);
}

public class PartnerManager : IPartnerManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IPartner _partners;

    public PartnerManager(IServiceProvider serviceProvider,
        IPartner partners)
    {
        _serviceProvider = serviceProvider;
        _partners = partners;
    }

    public IPartner GetPartnerHandler(Partner partner)
    {
        if (partner == Partner.InHouse)
        {
            return _partners;
        }

        throw new ArgumentOutOfRangeException(nameof(partner), $"Not expected partner value: {partner}");
    }
}
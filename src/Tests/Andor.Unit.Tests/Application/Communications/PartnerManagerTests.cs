using Andor.Application.Communications.Interfaces;
using Andor.Application.Communications.Services.Manager;
using Andor.Domain.Entities.Communications.ValueObjects;
using NSubstitute;

namespace Andor.Unit.Tests.Application.Communications;

public class PartnerManagerTests
{
    [Fact]
    public void GetPartnerHandler_UnexpectedPartner_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var partner = Substitute.For<IPartner>();
        var partnerManager = new PartnerManager(serviceProvider, partner);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => partnerManager.GetPartnerHandler(Partner.Undefined));
    }
}
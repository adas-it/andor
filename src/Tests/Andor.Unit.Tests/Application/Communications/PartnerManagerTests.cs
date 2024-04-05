using Andor.Application.Communications.Interfaces;
using Andor.Application.Communications.Services.Manager;
using Andor.Application.Communications.Services.PartnerHandler;
using Andor.Domain.Entities.Communications.ValueObjects;
using NSubstitute;

namespace Andor.Unit.Tests.Application.Communications;

public class PartnerManagerTests
{
    [Fact]
    public void GetPartnerHandler_InHouse_ReturnsInHousePartner()
    {
        // Arrange
        var inHousePartner = Substitute.For<InHousePartner>(Substitute.For<ISMTP>());
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(InHousePartner)).Returns(inHousePartner);
        var partnerManager = new PartnerManager(serviceProvider);

        // Act
        var result = partnerManager.GetPartnerHandler(Partner.InHouse);

        // Assert
        Assert.IsType<InHousePartner>(result);
    }

    [Fact]
    public void GetPartnerHandler_UnexpectedPartner_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var partnerManager = new PartnerManager(serviceProvider);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => partnerManager.GetPartnerHandler(Partner.Undefined));
    }
}
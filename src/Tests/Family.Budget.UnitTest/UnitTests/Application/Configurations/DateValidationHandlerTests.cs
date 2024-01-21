namespace Family.Budget.UnitTest.UnitTests.Application.Configurations;

using Family.Budget.Application.Administrations.Services;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Admin;
using Family.Budget.Domain.Entities.Admin.Repository;
using Family.Budget.UnitTest.UnitTests.Domain.Configurations;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ConfigurationsErrors = Budget.Application.Dto.Configurations.Errors;

[Collection(nameof(ConfigurationTestFixture))]
public class DateValidationServicesTests
{
    private readonly ConfigurationTestFixture _fixture;
    private readonly Mock<IConfigurationRepository> _configurationMock;
    private readonly Mock<Notifier> _notifier;

    public DateValidationServicesTests(ConfigurationTestFixture fixture)
    {
        _fixture = fixture;
        _configurationMock = new Mock<IConfigurationRepository>();
        _notifier = new Mock<Notifier>();
    }

    [Fact(DisplayName = nameof(HandleDatesWithEmptyDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithEmptyDatabaseAsync()
    {
        var validData = _fixture.GetValidConfiguration();

        var app = new DateValidationServices(_configurationMock.Object, _notifier.Object);

        //Act
        await app.Handle(validData, CancellationToken.None);

        //Assert
        _notifier.Object.Erros.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(HandleDatesWithSameNameClosedBeforeDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithSameNameClosedBeforeDatabaseAsync()
    {
        //close befor
        var validData = _fixture.GetValidConfiguration();

        var beforeConfig = Configuration.New(validData.Name, validData.Value, validData.Description, validData.StartDate.AddDays(-30), validData.StartDate.AddDays(-10),
            Guid.NewGuid().ToString());

        _configurationMock.Setup(x => x.GetByName(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Configuration>()
        {
            beforeConfig
        });

        var app = new DateValidationServices(_configurationMock.Object, _notifier.Object);

        //Act
        await app.Handle(validData, CancellationToken.None);

        //Assert
        _notifier.Object.Erros.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(HandleDatesWithSameNameStartsAfterDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithSameNameStartsAfterDatabaseAsync()
    {
        //open after
        var validData = _fixture.GetValidConfiguration();

        var beforeConfig = Configuration.New(validData.Name, validData.Value, validData.Description, validData.FinalDate.Value.AddDays(10), validData.FinalDate.Value.AddDays(20),
            Guid.NewGuid().ToString());

        _configurationMock.Setup(x => x.GetByName(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Configuration>()
        {
            beforeConfig
        });

        var app = new DateValidationServices(_configurationMock.Object, _notifier.Object);

        //Act
        await app.Handle(validData, CancellationToken.None);

        //Assert
        _notifier.Object.Erros.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(HandleDatesWithSameNameOpeningDuringDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithSameNameOpeningDuringDatabaseAsync()
    {
        //open during
        var validData = _fixture.GetValidConfiguration();

        var beforeConfig = Configuration.New(validData.Name, validData.Value, validData.Description, validData.StartDate.AddDays(-3), validData.StartDate.AddDays(1),
            Guid.NewGuid().ToString());

        _configurationMock.Setup(x => x.GetByName(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Configuration>()
        {
            beforeConfig
        });

        var app = new DateValidationServices(_configurationMock.Object, _notifier.Object);

        //Act
        await app.Handle(validData, CancellationToken.None);

        //Assert
        _notifier.Object.Erros.Should().HaveCount(1);
        _notifier.Object.Erros[0].Code.Should().Be(ConfigurationsErrors.ConfigurationErrors.ThereWillCurrentConfigurationStartDate().Code);
        _notifier.Object.Erros[0].Message.Should().Be(ConfigurationsErrors.ConfigurationErrors.ThereWillCurrentConfigurationStartDate().Message);
    }

    [Fact(DisplayName = nameof(HandleDatesWithSameNameClosingDuringDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithSameNameClosingDuringDatabaseAsync()
    {
        //close during
        var validData = _fixture.GetValidConfiguration();

        var beforeConfig = Configuration.New(validData.Name, validData.Value, validData.Description, validData.StartDate.AddDays(3), validData.FinalDate.Value.AddDays(1),
            Guid.NewGuid().ToString());

        _configurationMock.Setup(x => x.GetByName(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Configuration>()
        {
            beforeConfig
        });

        var app = new DateValidationServices(_configurationMock.Object, _notifier.Object);

        //Act
        await app.Handle(validData, CancellationToken.None);

        //Assert
        _notifier.Object.Erros.Should().HaveCount(1);
        _notifier.Object.Erros[0].Code.Should().Be(ConfigurationsErrors.ConfigurationErrors.ThereWillCurrentConfigurationEndDate().Code);
        _notifier.Object.Erros[0].Message.Should().Be(ConfigurationsErrors.ConfigurationErrors.ThereWillCurrentConfigurationEndDate().Message);
    }

    [Fact(DisplayName = nameof(HandleDatesWithSameNameDuringDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithSameNameDuringDatabaseAsync()
    {
        //open and close during - inside
        var validData = _fixture.GetValidConfiguration();

        var beforeConfig = Configuration.New(validData.Name, validData.Value, validData.Description, validData.StartDate.AddDays(-3), validData.FinalDate.Value.AddDays(3),
            Guid.NewGuid().ToString());

        _configurationMock.Setup(x => x.GetByName(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Configuration>()
        {
            beforeConfig
        });

        var app = new DateValidationServices(_configurationMock.Object, _notifier.Object);

        //Act
        await app.Handle(validData, CancellationToken.None);

        //Assert
        _notifier.Object.Erros.Should().HaveCount(2);
        _notifier.Object.Erros[0].Code.Should().Be(ConfigurationsErrors.ConfigurationErrors.ThereWillCurrentConfigurationStartDate().Code);
        _notifier.Object.Erros[0].Message.Should().Be(ConfigurationsErrors.ConfigurationErrors.ThereWillCurrentConfigurationStartDate().Message);

        _notifier.Object.Erros[1].Code.Should().Be(ConfigurationsErrors.ConfigurationErrors.ThereWillCurrentConfigurationEndDate().Code);
        _notifier.Object.Erros[1].Message.Should().Be(ConfigurationsErrors.ConfigurationErrors.ThereWillCurrentConfigurationEndDate().Message);
    }
}

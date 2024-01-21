namespace Family.Budget.UnitTest.UnitTests.Application.Configurations;

using Family.Budget.Application.Administrations.Commands;
using Family.Budget.Application.Administrations.Services;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Models.Errors;
using Family.Budget.Application.Models;
using Family.Budget.Application.Models.Authorization;
using Family.Budget.Domain.Common;
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
using ConfigurationsErrors = Family.Budget.Application.Dto.Configurations.Errors;

[Collection(nameof(ConfigurationTestFixture))]
public class RegisterConfigurationCommandTests
{
    private readonly ConfigurationTestFixture fixture;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<IConfigurationRepository> configurationMock;
    private readonly Mock<IDateValidationServices> dateValidationMock;
    private readonly Mock<Notifier> notifier;
    private readonly Mock<ICurrentUserService> currentUserServiceMock;

    public RegisterConfigurationCommandTests(ConfigurationTestFixture fixture)
    {
        this.fixture = fixture;
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.configurationMock = new Mock<IConfigurationRepository>();
        this.notifier = new Mock<Notifier>();
        this.dateValidationMock = new Mock<IDateValidationServices>();
        this.currentUserServiceMock = new Mock<ICurrentUserService>();
    }

    //[Fact(DisplayName = nameof(HandleRegisterConfigurationCommand_HappyPath_Async))]
    //[Trait("Domain", "Configuration - RegisterConfigurationCommand")]
    public async Task HandleRegisterConfigurationCommand_HappyPath_Async()
    {
        var validData = fixture.GetValidConfiguration();

        var item = new RegisterConfigurationCommand()
        {
            Description = validData.Description,
            FinalDate = validData.FinalDate,
            Name = validData.Name,
            StartDate = validData.StartDate,
            Value = validData.Value,
        };

        configurationMock.Setup(x => x.GetByName(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Configuration>()
        {
            validData
        });

        var app = GetApp();

        //Act
        var datetimeBefore = DateTime.UtcNow;

        var result = await app.Handle(item, CancellationToken.None);

        var datetimeAfter = DateTime.UtcNow.AddSeconds(1);

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        dateValidationMock.Verify(x => x.Handle(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()), Times.Once());
        configurationMock.Verify(x => x.Insert(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()), Times.Once());
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    //[Fact(DisplayName = nameof(HandleRegisterConfigurationCommand_WarningInitalDateBeforeToday_Async))]
    //[Trait("Domain", "Configuration - RegisterConfigurationCommand")]
    public async Task HandleRegisterConfigurationCommand_WarningInitalDateBeforeToday_Async()
    {
        var validData = fixture.GetValidConfiguration();

        var item = new RegisterConfigurationCommand()
        {
            Description = validData.Description,
            FinalDate = validData.FinalDate,
            Name = validData.Name,
            StartDate = DateTimeOffset.UtcNow.AddDays(-2),
            Value = validData.Value,
        };

        configurationMock.Setup(x => x.GetByName(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Configuration>()
        {
            validData
        });

        var app = GetApp();

        //Act
        var datetimeBefore = DateTime.UtcNow;

        var result = await app.Handle(item, CancellationToken.None);

        var datetimeAfter = DateTime.UtcNow.AddSeconds(1);

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        (result.StartDate > datetimeBefore).Should().BeTrue();

        notifier.Object.Erros.Should().BeEmpty();

        notifier.Object.Warnings.Should().HaveCount(1);
        notifier.Object.Warnings[0].Code.Should().Be(ConfigurationsErrors.ConfigurationErrors.StartDateCannotBeBeforeToUtcNow().Code);
        notifier.Object.Warnings[0].Message.Should().Be(ConfigurationsErrors.ConfigurationErrors.StartDateCannotBeBeforeToUtcNow().Message);

        dateValidationMock.Verify(x => x.Handle(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()), Times.Once());
        configurationMock.Verify(x => x.Insert(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()), Times.Once());
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    //[Fact(DisplayName = nameof(HandleRegisterConfigurationCommand_ErrorFinalDateBeforeToday_Async))]
    //[Trait("Domain", "Configuration - RegisterConfigurationCommand")]
    public async Task HandleRegisterConfigurationCommand_ErrorFinalDateBeforeToday_Async()
    {
        var validData = fixture.GetValidConfiguration();

        var item = new RegisterConfigurationCommand()
        {
            Description = validData.Description,
            FinalDate = DateTimeOffset.UtcNow.AddDays(-2),
            Name = validData.Name,
            StartDate = DateTimeOffset.UtcNow.AddDays(-20),
            Value = validData.Value,
        };

        var app = GetApp();

        //Act
        var result = await app.Handle(item, CancellationToken.None);

        //Assert
        result.Should().BeNull();

        notifier.Object.Erros.Should().HaveCount(1);
        notifier.Object.Erros[0].Code.Should().Be(ConfigurationsErrors.ConfigurationErrors.EndDateCannotBeBeforeToToday().Code);
        notifier.Object.Erros[0].Message.Should().Be(ConfigurationsErrors.ConfigurationErrors.EndDateCannotBeBeforeToToday().Message);
    }

    //[Fact(DisplayName = nameof(HandleRegisterConfigurationCommand_ErrorDomainValidation_Async))]
    //[Trait("Domain", "Configuration - RegisterConfigurationCommand")]
    public async Task HandleRegisterConfigurationCommand_ErrorDomainValidation_Async()
    {
        var validData = fixture.GetValidConfiguration();

        var item = new RegisterConfigurationCommand()
        {
            Description = validData.Description,
            FinalDate = DateTimeOffset.UtcNow.AddDays(-2),
            Name = fixture.GetStringRigthSize(150,1000),
            StartDate = DateTimeOffset.UtcNow.AddDays(-20),
            Value = validData.Value,
        };

        var app = GetApp();

        //Act
        var result = await app.Handle(item, CancellationToken.None);

        //Assert
        result.Should().BeNull();

        notifier.Object.Erros.Should().HaveCount(1);
        notifier.Object.Erros[0].Code.Should().Be(Errors.Validation().Code);
        notifier.Object.Erros[0].Message.Should().Be(Errors.Validation().Message);
        notifier.Object.Erros[0].InnerMessage.Should().Be(DefaultsErrorsMessages.BetweenLength.GetMessage(nameof(RegisterConfigurationCommand.Name), 3, 100));
    }

    private RegisterConfigurationCommandHandler GetApp()
    {
        return new RegisterConfigurationCommandHandler(
            configurationMock.Object, 
            unitOfWorkMock.Object, 
            notifier.Object, 
            dateValidationMock.Object,
            currentUserServiceMock.Object);
    }
}

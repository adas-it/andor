namespace Family.Budget.UnitTest.UnitTests.Application.Configurations;

using Family.Budget.Application.Administrations.Commands;
using Family.Budget.Application.Administrations.Services;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Models.Errors;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Common;
using Family.Budget.Domain.Entities.Admin;
using Family.Budget.Domain.Entities.Admin.Repository;
using Family.Budget.UnitTest.UnitTests.Domain.Configurations;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ConfigurationsErrors = Family.Budget.Application.Dto.Configurations.Errors;

[Collection(nameof(ConfigurationTestFixture))]
public class ChangeConfigurationCommandTests
{
    private readonly ConfigurationTestFixture fixture;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<IConfigurationRepository> configurationMock;
    private readonly Mock<IDateValidationServices> dateValidationMock;
    private readonly Mock<Notifier> notifier;

    public ChangeConfigurationCommandTests(ConfigurationTestFixture fixture)
    {
        this.fixture = fixture;
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.configurationMock = new Mock<IConfigurationRepository>();
        this.notifier = new Mock<Notifier>();
        this.dateValidationMock = new Mock<IDateValidationServices>();
    }

    [Fact(DisplayName = nameof(HandleChangeConfigurationCommand_HappyPath_Async))]
    [Trait("Domain", "Configuration - ChangeConfigurationCommand")]
    public async Task HandleChangeConfigurationCommand_HappyPath_Async()
    {
        //Arrange
        var entity = new ModifyConfigurationCommand
        {
            Id = Guid.NewGuid(),
            Name = fixture.GetStringRigthSize(5, 100),
            Value = fixture.GetStringRigthSize(5, 100),
            Description = fixture.GetStringRigthSize(5, 1000),
            StartDate = DateTimeOffset.UtcNow.AddDays(10),
            FinalDate = DateTimeOffset.UtcNow.AddDays(30)
        };

        var app = new ChangeConfigurationCommandHandler(configurationMock.Object,
            unitOfWorkMock.Object,
            dateValidationMock.Object,
            notifier.Object);

        configurationMock.Setup(x => x.GetById(It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.GetValidConfiguration());

        //Act
        await app.Handle(entity, CancellationToken.None);

        //Assert
        notifier.Object.Erros.Should().BeEmpty();
        notifier.Object.Warnings.Should().BeEmpty();

        configurationMock.Verify(x => x.GetById(It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        configurationMock.Verify(x => x.Update(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        dateValidationMock.Verify(x => x.Handle(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact(DisplayName = nameof(HandleChangeConfigurationCommand_NotFound_Async))]
    [Trait("Domain", "Configuration - ChangeConfigurationCommand")]
    public async Task HandleChangeConfigurationCommand_NotFound_Async()
    {
        //Arrange
        var entity = new ModifyConfigurationCommand();

        var app = new ChangeConfigurationCommandHandler(configurationMock.Object,
            unitOfWorkMock.Object,
            dateValidationMock.Object,
            notifier.Object);

        //Act
        var ret = await app.Handle(entity, CancellationToken.None);

        //Assert
        ret.Should().BeNull();

        notifier.Object.Erros.Should().HaveCount(1);
        notifier.Object.Erros[0].Code.Should().Be(ConfigurationsErrors.ConfigurationErrors.ConfigurationNotFound().Code);
        notifier.Object.Erros[0].Message.Should().Be(ConfigurationsErrors.ConfigurationErrors.ConfigurationNotFound().Message);
        notifier.Object.Warnings.Should().BeEmpty();

        configurationMock.Verify(x => x.GetById(It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        configurationMock.Verify(x => x.Update(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        dateValidationMock.Verify(x => x.Handle(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Never());
    }

    //Found - Set to invalid domain
    //[Fact(DisplayName = nameof(HandleChangeConfigurationCommand_SetToInvalidDomain_Async))]
    //[Trait("Domain", "Configuration - ChangeConfigurationCommand")]
    public async Task HandleChangeConfigurationCommand_SetToInvalidDomain_Async()
    {
        //Arrange
        ModifyConfigurationCommand? entity = new ()
        {
            Id = Guid.NewGuid(),
            Name = fixture.GetStringRigthSize(1, 2),
            Value = fixture.GetStringRigthSize(5, 100),
            Description = fixture.GetStringRigthSize(5, 1000),
            StartDate = DateTimeOffset.UtcNow.AddDays(10),
            FinalDate = DateTimeOffset.UtcNow.AddDays(30)
        };

        var app = new ChangeConfigurationCommandHandler(configurationMock.Object,
            unitOfWorkMock.Object,
            dateValidationMock.Object,
            notifier.Object);

        configurationMock.Setup(x => x.GetById(It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.GetValidConfiguration());

        //Act
        var ret = await app.Handle(entity, CancellationToken.None);

        //Assert
        ret.Should().BeNull();

        notifier.Object.Erros.Should().HaveCount(1);
        notifier.Object.Erros[0].Code.Should().Be(Errors.Validation().Code);
        notifier.Object.Erros[0].Message.Should().Be(Errors.Validation().Message);
        notifier.Object.Erros[0].InnerMessage.Should().Be(DefaultsErrorsMessages.BetweenLength.GetMessage(nameof(Configuration.Name), 3, 100));
        notifier.Object.Warnings.Should().BeEmpty();

        configurationMock.Verify(x => x.GetById(It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()),
            Times.Once());

        configurationMock.Verify(x => x.Update(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        dateValidationMock.Verify(x => x.Handle(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Never());
    }

    //Error - Found Closed (as closed final date has been passed)- Allow to Update only description
    [Fact(DisplayName = nameof(HandleChangeConfigurationCommand_SetToInvalidDomain_Async))]
    [Trait("Domain", "Configuration - ChangeConfigurationCommand")]
    public async Task HandleChangeConfigurationCommand_FoundClosedNotAllowToUpdateNameValueAndDates_Async()
    {
        //Arrange
        var entity = new ModifyConfigurationCommand
        {
            Name = fixture.GetStringRigthSize(3, 100),
            Value = fixture.GetStringRigthSize(5, 100),
            Description = fixture.GetStringRigthSize(5, 1000),
            StartDate = DateTimeOffset.UtcNow.AddDays(10),
            FinalDate = DateTimeOffset.UtcNow.AddDays(30)
        };

        var app = new ChangeConfigurationCommandHandler(configurationMock.Object,
            unitOfWorkMock.Object,
            dateValidationMock.Object,
            notifier.Object);

        var inDatabase = Configuration.New(
            fixture.GetStringRigthSize(5, 100),
            fixture.GetStringRigthSize(5, 300),
            fixture.GetStringRigthSize(5, 1000),
            DateTimeOffset.UtcNow.AddMonths(-2),
            DateTimeOffset.UtcNow.AddMonths(-1),
            Guid.NewGuid().ToString());

        entity.Id = inDatabase.Id;

        configurationMock.Setup(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(inDatabase);

        //Act
        var ret = await app.Handle(entity, CancellationToken.None);

        //Assert
        ret.Should().BeNull();

        notifier.Object.Erros.Should().HaveCount(1);
        notifier.Object.Erros[0].Code.Should().Be(ConfigurationsErrors.ConfigurationErrors.OnlyDescriptionAreAvaliableToChangedOnClosedConfiguration().Code);
        notifier.Object.Erros[0].Message.Should().Be(ConfigurationsErrors.ConfigurationErrors.OnlyDescriptionAreAvaliableToChangedOnClosedConfiguration().Message);
        notifier.Object.Warnings.Should().BeEmpty();

        configurationMock.Verify(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()),
            Times.Once());

        configurationMock.Verify(x => x.Update(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        dateValidationMock.Verify(x => x.Handle(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Never());
    }

    //Success - Found Closed (as closed final date has been passed)- Allow to Update only description
    //[Fact(DisplayName = nameof(HandleChangeConfigurationCommand_SetToInvalidDomain_Async))]
    //[Trait("Domain", "Configuration - ChangeConfigurationCommand")]
    public async Task HandleChangeConfigurationCommand_FoundClosedAllowToUpdateDescription_Async()
    {
        //Arrange
        var entity = new ModifyConfigurationCommand
        {
            Description = fixture.GetStringRigthSize(5, 1000)
        };

        var app = new ChangeConfigurationCommandHandler(configurationMock.Object,
            unitOfWorkMock.Object,
            dateValidationMock.Object,
            notifier.Object);

        var inDatabase = Configuration.New(
            fixture.GetStringRigthSize(5, 100),
            fixture.GetStringRigthSize(5, 300),
            fixture.GetStringRigthSize(5, 1000),
            DateTimeOffset.UtcNow.AddMonths(-2),
            DateTimeOffset.UtcNow.AddMonths(-1),
            Guid.NewGuid().ToString());

        entity.Id = inDatabase.Id;
        entity.Name = inDatabase.Name;
        entity.Value = inDatabase.Value;
        entity.StartDate = inDatabase.StartDate;
        entity.FinalDate = inDatabase.FinalDate;

        configurationMock.Setup(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(inDatabase);

        //Act
        var datetimeBefore = DateTimeOffset.UtcNow;
        Thread.Sleep(1);

        var ret = await app.Handle(entity, CancellationToken.None);

        //Assert
        ret.Should().NotBeNull();

        notifier.Object.Erros.Should().BeEmpty();
        notifier.Object.Warnings.Should().BeEmpty();

        configurationMock.Verify(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()),
            Times.Once());

        dateValidationMock.Verify(x => x.Handle(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Once());
    }

    //Found In Course - When Updates description, the descripition and lastUpdate receives the update
    //[Fact(DisplayName = nameof(HandleChangeConfigurationCommand_FoundInCourseAllowToUpdateDescription_Async))]
    //[Trait("Domain", "Configuration - ChangeConfigurationCommand")]
    public async Task HandleChangeConfigurationCommand_FoundInCourseAllowToUpdateDescription_Async()
    {
        //Arrange
        var entity = new ModifyConfigurationCommand
        {
            Description = fixture.GetStringRigthSize(5, 1000)
        };

        var app = new ChangeConfigurationCommandHandler(configurationMock.Object,
            unitOfWorkMock.Object,
            dateValidationMock.Object,
            notifier.Object);

        var inDatabase = Configuration.New(
            fixture.GetStringRigthSize(5, 100),
            fixture.GetStringRigthSize(5, 300),
            fixture.GetStringRigthSize(5, 1000),
            DateTimeOffset.UtcNow.AddMonths(-2),
            DateTimeOffset.UtcNow.AddMonths(1),
            Guid.NewGuid().ToString());

        entity.Id = inDatabase.Id;
        entity.Name = inDatabase.Name;
        entity.Value = inDatabase.Value;
        entity.StartDate = inDatabase.StartDate;
        entity.FinalDate = inDatabase.FinalDate;

        configurationMock.Setup(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(inDatabase);

        //Act
        var datetimeBefore = DateTimeOffset.UtcNow;
        Thread.Sleep(1);

        var ret = await app.Handle(entity, CancellationToken.None);

        //Assert
        ret.Should().NotBeNull();

        ret.Id.Should().Be(inDatabase.Id);

        notifier.Object.Erros.Should().BeEmpty();
        notifier.Object.Warnings.Should().BeEmpty();

        configurationMock.Verify(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()),
            Times.Once());

        configurationMock.Verify(x => x.Insert(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Never());

        dateValidationMock.Verify(x => x.Handle(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Once());
    }

    //Error - Found In Course - When Updates Final Date. Is Not allowes to update before today (Warning set to UtcNow) update field LastUpdate
    //[Fact(DisplayName = nameof(HandleChangeConfigurationCommand_FoundInCourseChangeFinalDateToBeforeToday_Async))]
    //[Trait("Domain", "Configuration - ChangeConfigurationCommand")]
    public async Task HandleChangeConfigurationCommand_FoundInCourseChangeFinalDateToBeforeToday_Async()
    {
        //Arrange
        var entity = new ModifyConfigurationCommand
        {
            Description = fixture.GetStringRigthSize(5, 1000),
            Name = fixture.GetStringRigthSize(3, 100)
        };

        var app = new ChangeConfigurationCommandHandler(configurationMock.Object,
            unitOfWorkMock.Object,
            dateValidationMock.Object,
            notifier.Object);

        var inDatabase = Configuration.New(
            fixture.GetStringRigthSize(5, 100),
            fixture.GetStringRigthSize(5, 300),
            fixture.GetStringRigthSize(5, 1000),
            DateTimeOffset.UtcNow.AddMonths(-2),
            DateTimeOffset.UtcNow.AddMonths(1),
            Guid.NewGuid().ToString());

        entity.Id = inDatabase.Id;
        entity.Name = inDatabase.Name;
        entity.Value = inDatabase.Value;
        entity.StartDate = inDatabase.StartDate;
        entity.FinalDate = DateTimeOffset.UtcNow.AddDays(-1);

        configurationMock.Setup(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(inDatabase);


        //Act
        var datetimeBefore = DateTimeOffset.UtcNow;
        Thread.Sleep(1);

        var ret = await app.Handle(entity, CancellationToken.None);

        Thread.Sleep(1);
        var datetimeAfter = DateTimeOffset.UtcNow;

        //Assert
        ret.Should().NotBeNull();

        ret.Id.Should().Be(inDatabase.Id);

        notifier.Object.Erros.Should().BeEmpty();
        notifier.Object.Warnings.Should().HaveCount(1);

        notifier.Object.Warnings[0].Code.Should().Be(ConfigurationsErrors.ConfigurationErrors.ItsNotAllowedToChangeFinalDatetoBeforeToday().Code);
        notifier.Object.Warnings[0].Message.Should().Be(ConfigurationsErrors.ConfigurationErrors.ItsNotAllowedToChangeFinalDatetoBeforeToday().Message);

        configurationMock.Verify(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()),
            Times.Once());

        dateValidationMock.Verify(x => x.Handle(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Once());
    }

    //Success - Found In Course - When Updates Final Date. Is Not allowes to update before today (Warning set to UtcNow) update field LastUpdate
    //[Fact(DisplayName = nameof(HandleChangeConfigurationCommand_FoundInCourseChangeFinalDate_Async))]
    //[Trait("Domain", "Configuration - ChangeConfigurationCommand")]
    public async Task HandleChangeConfigurationCommand_FoundInCourseChangeFinalDate_Async()
    {
        //Arrange
        var entity = new ModifyConfigurationCommand
        {
            Description = fixture.GetStringRigthSize(5, 1000),
            Name = fixture.GetStringRigthSize(3, 100)
        };

        var app = new ChangeConfigurationCommandHandler(configurationMock.Object,
            unitOfWorkMock.Object,
            dateValidationMock.Object,
            notifier.Object);

        var inDatabase = Configuration.New(
            fixture.GetStringRigthSize(5, 100),
            fixture.GetStringRigthSize(5, 300),
            fixture.GetStringRigthSize(5, 1000),
            DateTimeOffset.UtcNow.AddMonths(-2),
            DateTimeOffset.UtcNow.AddMonths(1),
            Guid.NewGuid().ToString());

        entity.Id = inDatabase.Id;
        entity.Name = inDatabase.Name;
        entity.Value = inDatabase.Value;
        entity.StartDate = inDatabase.StartDate;
        entity.FinalDate = DateTimeOffset.UtcNow.AddDays(1);

        configurationMock.Setup(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(inDatabase);

        //Act
        var datetimeBefore = DateTimeOffset.UtcNow;
        Thread.Sleep(1);

        var ret = await app.Handle(entity, CancellationToken.None);

        Thread.Sleep(1);
        var datetimeAfter = DateTimeOffset.UtcNow;

        //Assert
        ret.Should().NotBeNull();

        ret.Id.Should().Be(inDatabase.Id);

        notifier.Object.Erros.Should().BeEmpty();
        notifier.Object.Warnings.Should().BeEmpty();

        configurationMock.Verify(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()),
            Times.Once());

        dateValidationMock.Verify(x => x.Handle(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact(DisplayName = nameof(HandleChangeConfigurationCommand_FoundInCourseChangeInitalDateOnConfigInCourse_Async))]
    [Trait("Domain", "Configuration - ChangeConfigurationCommand")]
    public async Task HandleChangeConfigurationCommand_FoundInCourseChangeInitalDateOnConfigInCourse_Async()
    {
        //Arrange
        var entity = new ModifyConfigurationCommand
        {
            Description = fixture.GetStringRigthSize(5, 1000),
            Name = fixture.GetStringRigthSize(3, 100)
        };

        var app = new ChangeConfigurationCommandHandler(configurationMock.Object,
            unitOfWorkMock.Object,
            dateValidationMock.Object,
            notifier.Object);

        var inDatabase = Configuration.New(
            fixture.GetStringRigthSize(5, 100),
            fixture.GetStringRigthSize(5, 300),
            fixture.GetStringRigthSize(5, 1000),
            DateTimeOffset.UtcNow.AddMonths(-2),
            DateTimeOffset.UtcNow.AddMonths(1),
            Guid.NewGuid().ToString());

        entity.Id = inDatabase.Id;
        entity.Name = inDatabase.Name;
        entity.Value = inDatabase.Value;
        entity.StartDate = DateTimeOffset.UtcNow.AddMonths(-1);
        entity.FinalDate = inDatabase.FinalDate;

        configurationMock.Setup(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(inDatabase);


        //Act
        var datetimeBefore = DateTimeOffset.UtcNow;
        Thread.Sleep(1);

        var ret = await app.Handle(entity, CancellationToken.None);

        Thread.Sleep(1);
        var datetimeAfter = DateTimeOffset.UtcNow;

        //Assert
        ret.Should().BeNull();

        notifier.Object.Erros.Should().HaveCount(1);
        notifier.Object.Erros[0].Code.Should().Be(ConfigurationsErrors.ConfigurationErrors.ItsNotAllowedToChangeInitialDate().Code);
        notifier.Object.Erros[0].Message.Should().Be(ConfigurationsErrors.ConfigurationErrors.ItsNotAllowedToChangeInitialDate().Message);

        notifier.Object.Warnings.Should().BeEmpty();

        configurationMock.Verify(x => x.GetById(entity.Id,
            It.IsAny<CancellationToken>()),
            Times.Once());

        dateValidationMock.Verify(x => x.Handle(It.IsAny<Configuration>(),
            It.IsAny<CancellationToken>()),
            Times.Never());
    }
}

namespace Family.Budget.UnitTest.UnitTests.Application.Configurations;
using System;
using System.Threading;
using System.Threading.Tasks;
using ConfigurationsErrors = Family.Budget.Application.Dto.Configurations.Errors;
using Family.Budget.Application.Models;
using Family.Budget.UnitTest.UnitTests.Domain.Configurations;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using Xunit;
using Family.Budget.Domain.Entities.Admin;
using Family.Budget.Application.Dto.Models.Errors;
using Family.Budget.Domain.Entities.Admin.Repository;
using Family.Budget.Application.Administrations.Commands;

[Collection(nameof(ConfigurationTestFixture))]
public class ModifyConfigurationCommandTests
{
    private readonly ConfigurationTestFixture fixture;
    private readonly Mock<IConfigurationRepository> configurationMock;
    private readonly Mock<IMediator> mediator;
    private readonly Mock<Notifier> notifier;

    public ModifyConfigurationCommandTests(ConfigurationTestFixture fixture)
    {
        this.fixture = fixture;
        this.configurationMock = new Mock<IConfigurationRepository>();
        this.notifier = new Mock<Notifier>();
        this.mediator = new Mock<IMediator>();
    }

    // Only Allow to Update Value, Description, StartDate, FinalDate, Name
    [Fact(DisplayName = nameof(HandleModifyConfigurationCommand_AllowedPathsToPatch_Async))]
    [Trait("Domain", "Configuration - ModifyConfigurationCommand")]
    public async Task HandleModifyConfigurationCommand_AllowedPathsToPatch_Async()
    {
        //Arrange
        var configurationPatch = new JsonPatchDocument<ModifyConfigurationCommand>();
        configurationPatch.Replace(x => x.Name, fixture.GetStringRigthSize(3,100));
        configurationPatch.Replace(x => x.Value, fixture.GetStringRigthSize(3, 100));
        configurationPatch.Replace(x => x.Description, fixture.GetStringRigthSize(5, 1000));
        configurationPatch.Replace(x => x.StartDate, DateTimeOffset.UtcNow.AddMonths(1));
        configurationPatch.Replace(x => x.FinalDate, DateTimeOffset.UtcNow.AddMonths(2));

        var app = new ModifyConfigurationCommandHandler(configurationMock.Object, notifier.Object, mediator.Object);

        configurationMock.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.GetValidConfiguration());

        var entity = new PatchConfiguration(Guid.NewGuid(), configurationPatch);
        
        //Act
        await app.Handle(entity, CancellationToken.None);

        //Assert
        notifier.Object.Erros.Should().BeEmpty();
        notifier.Object.Warnings.Should().BeEmpty();

        configurationMock.Verify(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once());
        mediator.Verify(x => x.Send(It.IsAny<ModifyConfigurationCommand>(), It.IsAny<CancellationToken>()), Times.Once());

        configurationMock.Verify(x => x.Update(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact(DisplayName = nameof(HandleModifyConfigurationCommand_NotAllowedPathsToPatch_Async))]
    [Trait("Domain", "Configuration - ModifyConfigurationCommand")]
    public async Task HandleModifyConfigurationCommand_NotAllowedPathsToPatch_Async()
    {
        //Arrange
        var configurationPatch = new JsonPatchDocument<ModifyConfigurationCommand>();
        configurationPatch.Replace(x => x.Id, Guid.NewGuid());

        var app = new ModifyConfigurationCommandHandler(configurationMock.Object, notifier.Object, mediator.Object);

        var entity = new PatchConfiguration(Guid.NewGuid(), configurationPatch);

        //Act
        var ret = await app.Handle(entity, CancellationToken.None);

        //Assert
        ret.Should().BeNull();

        notifier.Object.Erros.Should().HaveCount(1);

        notifier.Object.Erros[0].Code.Should().Be(Errors.InvalidPathOnPatch().Code);
        notifier.Object.Erros[0].Message.Should().Be(Errors.InvalidPathOnPatch().Message);

        notifier.Object.Warnings.Should().BeEmpty();

        configurationMock.Verify(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        mediator.Verify(x => x.Send(It.IsAny<ModifyConfigurationCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        configurationMock.Verify(x => x.Update(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    //Not Found - Error
    [Fact(DisplayName = nameof(HandleModifyConfigurationCommand_NotFound_Async))]
    [Trait("Domain", "Configuration - ModifyConfigurationCommand")]
    public async Task HandleModifyConfigurationCommand_NotFound_Async()
    {
        //Arrange
        var configurationPatch = new JsonPatchDocument<ModifyConfigurationCommand>();
        configurationPatch.Replace(x => x.Name, fixture.GetStringRigthSize(3, 100));

        var app = new ModifyConfigurationCommandHandler(configurationMock.Object, notifier.Object, mediator.Object);

        var entity = new PatchConfiguration(Guid.NewGuid(), configurationPatch);

        //Act
        var ret = await app.Handle(entity, CancellationToken.None);

        //Assert
        ret.Should().BeNull();

        notifier.Object.Erros.Should().HaveCount(1);

        notifier.Object.Erros[0].Code.Should().Be(ConfigurationsErrors.ConfigurationErrors.ConfigurationNotFound().Code);
        notifier.Object.Erros[0].Message.Should().Be(ConfigurationsErrors.ConfigurationErrors.ConfigurationNotFound().Message);

        notifier.Object.Warnings.Should().BeEmpty();

        configurationMock.Verify(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once());
        mediator.Verify(x => x.Send(It.IsAny<ModifyConfigurationCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        configurationMock.Verify(x => x.Update(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()), Times.Never());
    }
}

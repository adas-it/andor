﻿using Andor.Application.Administrations.Configurations.Commands.ModifyConfigurations;
using Andor.Application.Administrations.Configurations.Services;
using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Common.ApplicationsErrors;
using Andor.Domain.Administrations.Configurations;
using Andor.Domain.Administrations.Configurations.Repository;
using Andor.Domain.Administrations.Configurations.ValueObjects;
using Andor.TestsUtil;
using Andor.Unit.Tests.Domain.Entities.Admin.Configurations;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using NSubstitute;

namespace Andor.Unit.Tests.Application.Administrations.Configurations.Commands.ModifyConfigurations;


[Collection(nameof(ConfigurationTestFixture))]
public class ModifyConfigurationCommandHandlerTests
{
    private readonly ICommandsConfigurationRepository _configurationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfigurationServices _configurationServices;
    private readonly ConfigurationTestFixture _fixture;

    public ModifyConfigurationCommandHandlerTests(ConfigurationTestFixture fixture)
    {
        _configurationRepository = Substitute.For<ICommandsConfigurationRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _configurationServices = Substitute.For<IConfigurationServices>();
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(HandleModifyConfigurationCommand_AllowedPathsToPatch_Async))]
    [Trait("Domain", "Configuration - ModifyConfigurationCommandHandler")]
    public async Task HandleModifyConfigurationCommand_AllowedPathsToPatch_Async()
    {
        //Arrange

        var newConfig = ConfigurationFixture.GetValidConfiguration(ConfigurationState.Awaiting);

        var configurationPatch = new JsonPatchDocument<Andor.Application.Dto.Administrations.Configurations.Requests.BaseConfiguration>();
        configurationPatch.Replace(x => x.Name, newConfig.Name);
        configurationPatch.Replace(x => x.Value, newConfig.Value);
        configurationPatch.Replace(x => x.Description, newConfig.Description);
        configurationPatch.Replace(x => x.StartDate, newConfig.StartDate);
        configurationPatch.Replace(x => x.ExpireDate, newConfig.ExpireDate);

        var app = GetApp();

        var config = ConfigurationFixture.GetValidConfiguration(ConfigurationState.Awaiting);

        _configurationRepository.GetByIdAsync(Arg.Is<ConfigurationId>(id => id == config.Id),
            Arg.Any<CancellationToken>()).Returns(config);

        var entity = new ModifyConfigurationCommand(config.Id, configurationPatch);

        //Act
        var _notifier = await app.Handle(entity, CancellationToken.None);

        //Assert
        _notifier.Errors.Should().BeEmpty();
        await _configurationServices.Received(1).Handle(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        await _configurationRepository.Received(1).UpdateAsync(
            Arg.Is<Configuration>(config => config.Name == newConfig.Name &&
            config.Value == newConfig.Value &&
            config.Description == newConfig.Description &&
            config.StartDate == newConfig.StartDate &&
            config.ExpireDate == newConfig.ExpireDate), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = nameof(HandleModifyConfigurationCommandHandler_NotFoundAsync))]
    [Trait("Domain", "Configuration - ModifyConfigurationCommandHandler")]
    public async Task HandleModifyConfigurationCommandHandler_NotFoundAsync()
    {
        var app = GetApp();

        var config = ConfigurationFixture.GetValidConfiguration(ConfigurationState.Awaiting);

        var newName = ConfigurationFixture.GetValidName();

        var command = GetCommand(config.Id, newName);

        var _notifier = await app.Handle(command, CancellationToken.None);

        _notifier.Errors.Should().NotBeEmpty();
        _notifier.Errors.Should().Contain(x => x.Code == ConfigurationErrorCodes.NotFound);
        await _configurationServices.Received(0).Handle(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(0).CommitAsync(Arg.Any<CancellationToken>());
        await _configurationRepository.Received(0).UpdateAsync(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
    }


    [Fact(DisplayName = nameof(HandleModifyConfigurationCommandHandler_NotFoundAsync))]
    [Trait("Domain", "Configuration - ModifyConfigurationCommandHandler")]
    public async Task HandleModifyConfigurationCommandHandler_NameInvalidAsync()
    {
        var app = GetApp();

        var config = ConfigurationFixture.GetValidConfiguration(ConfigurationState.Awaiting);

        _configurationRepository.GetByIdAsync(Arg.Is<ConfigurationId>(id => id == config.Id),
            Arg.Any<CancellationToken>()).Returns(config);

        var command = GetCommand(config.Id, "");

        var _notifier = await app.Handle(command, CancellationToken.None);

        _notifier.Errors.Should().NotBeEmpty();
        _notifier.Errors.Should().Contain(x => x.Code == ConfigurationErrorCodes.ConfigurationsValidation);
        await _configurationServices.Received(0).Handle(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(0).CommitAsync(Arg.Any<CancellationToken>());
        await _configurationRepository.Received(0).UpdateAsync(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
    }

    private ModifyConfigurationCommand GetCommand(ConfigurationId Id, string? name)
    {
        var configurationPatch = new JsonPatchDocument<Andor.Application.Dto.Administrations.Configurations.Requests.BaseConfiguration>();
        configurationPatch.Replace(x => x.Name, name ?? ConfigurationFixture.GetValidName());

        return new(Id, configurationPatch);
    }

    private ModifyConfigurationCommandHandler GetApp()
        => new(_configurationRepository,
                _unitOfWork,
                _configurationServices);
}

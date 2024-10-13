using Andor.Application.Administrations.Configurations.Errors;
using Andor.Application.Administrations.Configurations.Services;
using Andor.Application.Common.Attributes;
using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Administrations.Configurations.Responses;
using Andor.Application.Dto.Common.Responses;
using Andor.Domain.Administrations.Configurations;
using Andor.Domain.Administrations.Configurations.Repository;
using FluentValidation;
using Mapster;
using MediatR;
using _dto = Andor.Application.Dto.Administrations.Configurations.Requests;

namespace Andor.Application.Administrations.Configurations.Commands.RegisterConfigurations;
public record RegisterConfigurationCommand(_dto.BaseConfiguration BaseConfiguration)
    : BaseConfiguration(BaseConfiguration), IRequest<ApplicationResult<ConfigurationOutput>>;

public class RegisterConfigurationCommandValidator : AbstractValidator<RegisterConfigurationCommand>
{
    public RegisterConfigurationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);
    }
}

public class RegisterConfigurationCommandHandler(ICommandsConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    IConfigurationServices configurationServices,
    ICurrentUserService userService) : IRequestHandler<RegisterConfigurationCommand, ApplicationResult<ConfigurationOutput>>
{
    [Transaction]
    [Log]
    public async Task<ApplicationResult<ConfigurationOutput>> Handle(RegisterConfigurationCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ConfigurationOutput>.Success();

        var (result, config) = Configuration.New(request.Name,
            request.Value,
            request.Description,
            request.StartDate,
            request.ExpireDate,
            userService.User.UserId.ToString());

        if (result.IsFailure || config is null)
        {
            await HandleConfigurationResult.HandleResultConfiguration(result, response);
            return response;
        }

        response = await configurationServices.Handle(config!, cancellationToken);

        if (response.IsFailure)
        {
            return response;
        }

        await repository.InsertAsync(config!, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return config!.Adapt<ConfigurationOutput>();
    }
}
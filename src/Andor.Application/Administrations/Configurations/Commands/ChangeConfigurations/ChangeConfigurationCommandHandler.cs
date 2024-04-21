using Andor.Application.Administrations.Configurations.Errors;
using Andor.Application.Administrations.Configurations.Services;
using Andor.Application.Common.Attributes;
using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Administrations.Configurations.Responses;
using Andor.Application.Dto.Common.Responses;
using Andor.Domain.Administrations.Configurations.Repository;
using Andor.Domain.Administrations.Configurations.ValueObjects;
using FluentValidation;
using Mapster;
using MediatR;
using _dto = Andor.Application.Dto.Administrations.Configurations.Requests;

namespace Andor.Application.Administrations.Configurations.Commands.ChangeConfiguration;

public record ChangeConfigurationCommand(ConfigurationId Id, _dto.BaseConfiguration BaseConfiguration)
    : BaseConfiguration(BaseConfiguration), IRequest<ApplicationResult<ConfigurationOutput>>;

public class ChangeConfigurationCommandValidator : AbstractValidator<ChangeConfigurationCommand>
{
    public ChangeConfigurationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);
    }
}

public class ChangeConfigurationCommandHandler(ICommandsConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    IConfigurationServices configurationServices) : IRequestHandler<ChangeConfigurationCommand, ApplicationResult<ConfigurationOutput>>
{
    [Transaction]
    [Log]
    public async Task<ApplicationResult<ConfigurationOutput>> Handle(ChangeConfigurationCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ConfigurationOutput>.Success();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            response.AddError(Dto.Common.ApplicationsErrors.Errors.ConfigurationNotFound());
            return response;
        }

        var result = entity.Update(request.Name,
            request.Value,
            request.Description,
            request.StartDate,
            request.ExpireDate);

        if (result.IsFailure)
        {
            await HandleConfigurationResult.HandleResultConfiguration(result, response);
            return response;
        }

        await configurationServices.Handle(entity, cancellationToken);

        if (response.IsFailure)
        {
            return response;
        }

        await repository.UpdateAsync(entity, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return entity.Adapt<ConfigurationOutput>();
    }
}
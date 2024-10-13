using Andor.Application.Administrations.Configurations.Errors;
using Andor.Application.Common.Attributes;
using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Domain.Administrations.Configurations.Repository;
using Andor.Domain.Administrations.Configurations.ValueObjects;
using FluentValidation;
using MediatR;

namespace Andor.Application.Administrations.Configurations.Commands.RemoveConfigurations;

public record RemoveConfigurationCommand(ConfigurationId Id) : IRequest<ApplicationResult<object>>;

public class RemoveConfigurationCommandValidator : AbstractValidator<RemoveConfigurationCommand>
{
    public RemoveConfigurationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);
    }
}

public class RemoveConfigurationCommandHandler(ICommandsConfigurationRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveConfigurationCommand, ApplicationResult<object>>
{
    private readonly ICommandsConfigurationRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [Transaction]
    [Log]
    public async Task<ApplicationResult<object>> Handle(RemoveConfigurationCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<object>.Success();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            response.AddWarnings(Dto.Common.ApplicationsErrors.Errors.ConfigurationNotFound());
            return response;
        }

        var result = entity.Delete();

        await HandleConfigurationResult.HandleResultConfiguration(result, response);

        if (response.IsFailure)
        {
            return response;
        }

        await _repository.UpdateAsync(entity, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
        return response;
    }
}



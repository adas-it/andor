using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.ApplicationsErrors;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Onboarding.Registrations.Responses;
using Andor.Domain.Entities.Onboarding.Registrations.Repositories;
using FluentValidation;
using MediatR;
using System.Net.Mail;

namespace Andor.Application.Onboarding.Registrations.Commands;

public record ResubmitCheckCodeCommand(string Email) : IRequest<ApplicationResult<RegistrationOutput>>;

public class CoachRegistrationResubmitEmailInputValidator : AbstractValidator<ResubmitCheckCodeCommand>
{
    public CoachRegistrationResubmitEmailInputValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField)
            .EmailAddress()
            .WithMessage(ValidationConstant.InvalidParameter);
    }
}

public class ResubmitCheckCodeCommandHandler
    : IRequestHandler<ResubmitCheckCodeCommand, ApplicationResult<RegistrationOutput>>
{
    private readonly ICommandsRegistrationRepository _repository;
    private readonly IQueriesRegistrationRepository _queriesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResubmitCheckCodeCommandHandler(ICommandsRegistrationRepository repository,
        IUnitOfWork unitOfWork,
        IQueriesRegistrationRepository queriesRepository)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _queriesRepository = queriesRepository;
    }

    public async Task<ApplicationResult<RegistrationOutput>> Handle(ResubmitCheckCodeCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<RegistrationOutput>.Success();

        var item = await _queriesRepository.GetByEmailAsync(new MailAddress(request.Email), cancellationToken);

        if (item == null)
        {
            response.AddError(Errors.RegistrationNotFound());

            return response;
        }

        if (item.IsComplete())
        {
            response.AddError(Errors.EmailInUse());

            return response;
        }

        item.SetNewCode();

        await _repository.UpdateAsync(item, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return response;
    }
}

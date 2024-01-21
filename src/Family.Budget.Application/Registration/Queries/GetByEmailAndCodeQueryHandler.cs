using Family.Budget.Application.Dto.Registrations.Errors;
using Family.Budget.Application.Dto.Registrations.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Application.Registration.Adapters;
using Family.Budget.Domain.Entities.Registrations.Repository;
using MediatR;

namespace Family.Budget.Application.Registration.Queries;

public class GetByEmailAndCodeQuery : IRequest<RegistrationOutput>
{
    public string Email { get; private set; }
    public string Code { get; private set; }
    public GetByEmailAndCodeQuery(string email, string code)
    {
        Email = email;
        Code = code;
    }
}

public class GetByEmailAndCodeQueryHandler : BaseCommands, IRequestHandler<GetByEmailAndCodeQuery, RegistrationOutput>
{
    public IRegistrationRepository _repository;

    public GetByEmailAndCodeQueryHandler(IRegistrationRepository repository,
        Notifier notifier) : base(notifier)
    {
        _repository = repository;
    }

    public async Task<RegistrationOutput> Handle(GetByEmailAndCodeQuery request, CancellationToken cancellationToken)
    {
        var registration = await _repository.GetByEmail(request.Email, cancellationToken);

        if (registration is null || registration.IsTheRightCheckCode(request.Code) is false)
        {
            _notifier.Warnings.Add(Errors.RegistrationNotFound());

            return null!;
        }

        return registration.MapDtoFromDomain();
    }
}

namespace Family.Budget.Application.Users.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Users.Errors;
using Family.Budget.Application.Dto.Users.Requests;
using Family.Budget.Application.Dto.Users.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Admin.Repository;
using Family.Budget.Domain.Entities.Currencies.Repository;
using Family.Budget.Domain.Entities.Registrations.Repository;
using Family.Budget.Domain.Entities.Users.ValueObject;
using MediatR;

public record RegisterUserCommand : BaseUser, IRequest<UserOutput>
{
    public string Password { get; set; }
    public string Code { get; set; }
}

public class RegisterUserCommandHandler : BaseCommands, IRequestHandler<RegisterUserCommand, UserOutput>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IKeycloackService _service;
    private readonly IRegistrationRepository _repository;
    private readonly IConfigurationRepository _configRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IMediator _mediator;

    public RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        Notifier notifier,
        IRegistrationRepository repository,
        IKeycloackService service,
        IMediator mediator,
        IConfigurationRepository configRepository,
        ICurrencyRepository currencyRepository) : base(notifier)
    {
        this.unitOfWork = unitOfWork;
        _service = service;
        _repository = repository;
        _mediator = mediator;
        _configRepository = configRepository;
        _currencyRepository = currencyRepository;
    }

    public async Task<UserOutput> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var registration = await _repository.GetByEmail(request.Email, cancellationToken);

        if (registration == null)
        {
            _notifier.Erros.Add(Errors.UserNotFound());
            return null!;
        }

        if (!registration.IsTheRightCheckCode(request.Code))
        {
            _notifier.Erros.Add(Errors.WrongCode());
            return null!;
        }

        var userEmail = await _service.GetUserByEmail(request.Email, cancellationToken);

        if (userEmail != null)
        {
            _notifier.Erros.Add(Errors.EmailInUse());
            return null!;
        }

        if (string.IsNullOrEmpty(request.UserName))
        {
            request.UserName = request.Email;
        }

        var defaultLocationLanguage = await _configRepository.GetByNameActive("defaultLocation:language", cancellationToken);
        var defaultLocationCurrency = await _configRepository.GetByNameActive("defaultLocation:currency", cancellationToken);

        var currency = await _currencyRepository.GetById(Guid.Parse(defaultLocationCurrency!.Value), cancellationToken);

        var locationInfo = LocationInfos.New(defaultLocationLanguage!.Value, currency);

        var item = await _service.CreateUser(request.UserName,
            request.Email, request.FirstName, request.LastName, request.Password,
            locationInfo, request.Avatar, cancellationToken);

        await _repository.Delete(registration, cancellationToken);

        foreach (var domainEvent in item.Events)
            await _mediator.Publish(domainEvent);

        await unitOfWork.CommitAsync(cancellationToken);

        return new UserOutput(item.Id, item.UserName, item.Enabled, item.EmailVerified,
            item.FirstName, item.LastName, item.Email, item.Avatar, item.LocationInfos.PreferedLanguage,
            item.AcceptedPrivateData, item.AcceptedPrivateData);
    }
}
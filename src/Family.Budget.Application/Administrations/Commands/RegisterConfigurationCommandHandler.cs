namespace Family.Budget.Application.Administrations.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Administrations.Services;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Configurations.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Application.Models.Authorization;
using Family.Budget.Domain.Entities.Admin;
using Family.Budget.Domain.Entities.Admin.Repository;
using Mapster;
using MediatR;

public record RegisterConfigurationCommand : BaseConfiguration, IRequest<ConfigurationOutput>
{
}

public class RegisterConfigurationCommandHandler : BaseCommands, IRequestHandler<RegisterConfigurationCommand, ConfigurationOutput>
{
    private readonly IConfigurationRepository repository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IDateValidationServices _configurationServices;
    private readonly ICurrentUserService _userService;

    public RegisterConfigurationCommandHandler(IConfigurationRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier,
        IDateValidationServices configurationServices,
        ICurrentUserService userService) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
        _configurationServices = configurationServices;
        _userService = userService;
    }

    public async Task<ConfigurationOutput> Handle(RegisterConfigurationCommand request, CancellationToken cancellationToken)
    {
        var item = Configuration.New(request.Name,
            request.Value,
            request.Description,
            request.StartDate,
            request.FinalDate,
            _userService.User.UserId.ToString());

        await _configurationServices.Handle(item, cancellationToken);

        if (_notifier.Erros.Any())
        {
            return null!;
        }

        await repository.Insert(item, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return item.Adapt<ConfigurationOutput>();
    }
}
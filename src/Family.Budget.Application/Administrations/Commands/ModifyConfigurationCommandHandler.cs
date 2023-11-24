namespace Family.Budget.Application.Administrations.Commands;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using Family.Budget.Application.Dto.Configurations.Errors;
using Family.Budget.Application.Dto.Configurations.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Admin;
using Family.Budget.Domain.Entities.Admin.Repository;
using Family.Budget.Application.Common.Extensions;

public class PatchConfiguration : IRequest<ConfigurationOutput>
{
    public JsonPatchDocument<ModifyConfigurationCommand> PatchDocument { get; set; }
    public Guid Id { get; set; }

    public PatchConfiguration(Guid Id, JsonPatchDocument<ModifyConfigurationCommand> PatchDocument)
    {
        this.PatchDocument = PatchDocument;
        this.Id = Id;
    }
}

public class ModifyConfigurationCommandHandler : IRequestHandler<PatchConfiguration, ConfigurationOutput>
{
    private readonly IConfigurationRepository repository;
    private readonly Notifier notifier;
    private readonly IMediator mediator;

    public ModifyConfigurationCommandHandler(IConfigurationRepository repository,
        Notifier notifier,
        IMediator mediator)
    {
        this.repository = repository;
        this.notifier = notifier;
        this.mediator = mediator;
    }

    public async Task<ConfigurationOutput> Handle(PatchConfiguration command, CancellationToken cancellationToken)
    {
        try
        {
            command.PatchDocument.Validate(
                   OperationType.Replace,
                   new List<string> { $"/{nameof(Configuration.Name)}",
                    $"/{nameof(Configuration.Value)}",
                    $"/{nameof(Configuration.Description)}",
                    $"/{nameof(Configuration.StartDate)}",
                    $"/{nameof(Configuration.FinalDate)}" }
                   );
        }
        catch (BusinessException ex)
        {
            notifier.Erros.Add(ex.ErrorCode);
            return null!;
        }

        var entity = await repository.GetById(command.Id, cancellationToken);

        if (entity == null)
        {
            notifier.Erros.Add(ConfigurationErrors.ConfigurationNotFound());
            return null!;
        }

        var oldItem = new ModifyConfigurationCommand(
            entity.Id,
            entity.Name,
            entity.Value,
            entity.Description,
            entity.StartDate,
            entity.FinalDate);

        command.PatchDocument.ApplyTo(oldItem);

        var ret = await mediator.Send(oldItem, cancellationToken);

        return ret;
    }
}

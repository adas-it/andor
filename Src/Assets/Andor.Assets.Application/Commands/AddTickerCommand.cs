using Andor.Assets.Domain.Investments.Areas.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Assets.Application.Commands;

public record AddTickerCommand(AreaId Id, Name Name, ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<AreaId>;

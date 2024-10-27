using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.FinancialMovements.Response;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using MediatR;

namespace Andor.Application.Engagement.Budget.FinancialMovements.Queries;

public record ListFinancialMovementsQuery
    : PaginatedListInput, IRequest<ApplicationResult<ListFinancialMovementsOutput>>
{
    public int Type { get; set; }
    public Year Year { get; set; } = DateTime.Today.Year;
    public Month Month { get; set; } = DateTime.Today.Month;
    public Domain.Engagement.Budget.Accounts.Accounts.ValueObjects.AccountId AccountId { get; set; }
}

public class ListFinancialMovementsQueryHandler(IQueriesFinancialMovementRepository _repository)
    : IRequestHandler<ListFinancialMovementsQuery, ApplicationResult<ListFinancialMovementsOutput>>
{
    public async Task<ApplicationResult<ListFinancialMovementsOutput>> Handle(ListFinancialMovementsQuery request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ListFinancialMovementsOutput>.Success();

        var movementType = request.Type == MovementType.MoneyDeposit.Key ? MovementType.MoneyDeposit : MovementType.MoneySpending;
        try
        {
            var searchOutput = await _repository.SearchOutputAsync(
                new SearchInputMovement(
                    request.Page,
                    request.PerPage,
                    request.Search,
                    request.Sort ?? nameof(FinancialMovement.Date),
                    (Domain.SeedWork.Repositories.ResearchableRepository.SearchOrder)request.Dir,
                    movementType,
                    request.AccountId,
                    request.Year,
                    request.Month
                ),
                cancellationToken
            );

            var output = new ListFinancialMovementsOutput(
                searchOutput.CurrentPage,
                searchOutput.PerPage,
                searchOutput.Total,
                searchOutput.Items.ToList()
            );

            response.SetData(output);

            return response;

        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

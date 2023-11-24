namespace Family.Budget.Application.FinancialMovements.Commands;

using Family.Budget.Application.Dto.FinancialMovements.Responses;
using MediatR;


public record ModifyFinancialMovementCommand : Dto.FinancialMovements.Requests.BaseFinancialMovement, IRequest<FinancialMovementOutput>
{
    public Guid Id { get; set; }
}

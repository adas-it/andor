namespace Family.Budget.Application._Common.Behaviours;

using Family.Budget.Application._Common.Exceptions;
using Family.Budget.Application.Dto.Accounts.Errors;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using Family.Budget.Application.Dto.Models.Errors;
using Family.Budget.Domain.Exceptions;
using MediatR;

public class RequestExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{


    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            var response = next();
            return response;
        }
        catch (InvalidDomainException ex)
        {
            throw new ValidationException(HanddlerInvalidDomainExceptionException(ex));
        }
    }

    protected List<ErrorModel> HanddlerInvalidDomainExceptionException(InvalidDomainException ex)
    {
        var ret = new List<ErrorModel>();
        var errors = ex.Message.Split(";");

        foreach (var item in errors)
        {
            ErrorModel message;

            var err = item.Split(":");

            message = GetErrors().GetValueOrDefault(int.Parse(err[0]), Errors.Generic());

            message.ChangeInnerMessage(err[1]);

            ret.Add(message);
        }

        return ret;
    }

    protected Dictionary<int, ErrorModel> GetErrors()
        => new() { 
            { 
                Domain.Entities.Admin.ConfigurationsErrorsCodes.Validation.Value, Errors.Validation()
            }
        };
}
using Family.Budget.Domain.Common.ValuesObjects;

namespace Family.Budget.Domain.Exceptions;

public sealed class InvalidDomainException : Exception
{
    public int Code { get; init; }

    public InvalidDomainException(string? message, DomainErrorCode code) : base(message)
    {
        Code = code.Value;
    }
}


namespace Family.Budget.Domain.Validation;
internal static class ValidationConstant
{
    public const string RequiredParameter = "The field {PropertyName} are required.";

    public const string MaxLengthExceeded = "The field {PropertyName} has to have length between {MinLength} and {MaxLength}. ";
}
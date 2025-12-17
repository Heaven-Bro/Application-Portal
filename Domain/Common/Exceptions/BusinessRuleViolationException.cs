namespace Domain.Common.Exceptions;

public sealed class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string rule)
        : base($"Business rule violation: {rule}")
    {
    }
}

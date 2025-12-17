namespace Domain.Common.Exceptions;

public sealed class InvalidStateTransitionException : DomainException
{
    public InvalidStateTransitionException(string currentState, string attemptedAction)
        : base($"Cannot perform '{attemptedAction}' when in '{currentState}' state.")
    {
    }
}

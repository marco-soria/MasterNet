namespace MasterNet.Application.Core;

public sealed class ValidationException(
    IEnumerable<ValidationError> errors
    ) : Exception
{
    public IEnumerable<ValidationError> Errors { get; } = errors;

}
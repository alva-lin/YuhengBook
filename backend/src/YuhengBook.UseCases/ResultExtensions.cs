namespace YuhengBook.UseCases;

public static class ResultExtensions
{
    public static Result WithError(this Result result, string propertyName, string errorMessage,
        string? errorCode = null, ValidationSeverity severity = ValidationSeverity.Error)
    {
        result.ValidationErrors.Add(
            new ValidationError(propertyName, errorMessage, errorCode ?? string.Empty, severity)
        );
        return result;
    }

    public static Result WithError(this Result result, ValidationError error)
    {
        result.ValidationErrors.Add(error);
        return result;
    }

    public static Result WithErrors(this Result result, params ValidationError[] errors)
    {
        foreach (var error in errors)
        {
            result.ValidationErrors.Add(error);
        }

        return result;
    }
}

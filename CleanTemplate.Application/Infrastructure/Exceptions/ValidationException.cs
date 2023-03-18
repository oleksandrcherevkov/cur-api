using FluentValidation.Results;

namespace CleanTemplate.Application.Infrastructure.Exceptions;
public class ValidationException : Exception
{
    public ValidationException(string message = "One or more validation failures have occurred.")
        : base(message)
    {
        Failures = new Dictionary<string, string[]>();
    }

    public ValidationException(string propertyName, string[] propertyFailures)
        : this()
    {
        Failures.Add(propertyName, propertyFailures);
    }

    public ValidationException(List<ValidationFailure> failures)
        : this()
    {
        var propertyNames = failures
            .Select(e => e.PropertyName)
            .Distinct();

        foreach (var propertyName in propertyNames)
        {
            var propertyFailures = failures
                .Where(e => e.PropertyName == propertyName)
                .Select(e => e.ErrorMessage)
                .ToArray();

            Failures.Add(propertyName, propertyFailures);
        }
    }

    public IDictionary<string, string[]> Failures { get; }
}

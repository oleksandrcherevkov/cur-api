namespace CleanTemplate.WebApi.Infrastructure.ExceptionsHandling;

public class ValidationExceptionInfo : ExceptionInfo
{
    public IDictionary<string, string[]> Errors { get; set; }
}

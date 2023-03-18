namespace CleanTemplate.Application.Infrastructure.Exceptions;
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}
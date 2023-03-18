namespace CleanTemplate.WebApi.Infrastructure.UserContexts.Extensions;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomUserContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserContextMiddleware>();
    }
}

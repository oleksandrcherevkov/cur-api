using CleanTemplate.WebApi.Infrastructure.Jwt.Options;

namespace CleanTemplate.WebApi.Infrastructure.Jwt.Extentions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtGenerator(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Section));
        services.AddSingleton<IJwtGenerator, JwtGenerator>();
        return services;
    }
}
using CleanTemplate.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanTemplate.WebApi.Infrastructure.Data.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> SetupDatabaseAsync(this IApplicationBuilder appBuilder)
        {
            using (var scope = appBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<MyDbContext>();
                await context.Database.MigrateAsync();
            }

            return appBuilder;
        }
    }
}

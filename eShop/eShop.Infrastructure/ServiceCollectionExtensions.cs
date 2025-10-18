using eShop.Infrastructure.Identity;
using eShop.Infrastructure.Persistence.Contexts;
using eShop.Infrastructure.Persistence.DbInitializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddDatabase(configuration)
                .AddIdentityServices(configuration)
                .AddPermissions()
                .AddJwtAuthentication(configuration);
        }

        internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddDbContext<ApplicationDbContext>(options => options
                    .UseSqlServer(config.GetConnectionString("DefaultConnection"), builder =>
                    {
                        builder.MigrationsHistoryTable("Migrations", "EFCore");
                        builder.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: new TimeSpan(0, 0, 0, 100), errorNumbersToAdd: [1]);
                    }))
                .AddTransient<ApplicationDbSeeder>();
        }

        public static async Task<IApplicationBuilder> UseInfrastructureAsync(this IApplicationBuilder app)
        {

            //  Run migration & seeder at startup
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>();
                await seeder.SeedApplicationDatabaseAsync();
            }


            return app
                .UseAuthentication()
                .UseCurrentUser()
                .UseAuthorization();
        }

    }
}

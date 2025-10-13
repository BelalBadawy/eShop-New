using eShop.Application.Features.Roles;
using eShop.Application.Features.Token;
using eShop.Application.Models.JWT;
using eShop.Infrastructure.Identity;
using eShop.Infrastructure.Identity.Permissions;
using eShop.Infrastructure.Identity.Services;
using eShop.Infrastructure.Persistence.Contexts;
using eShop.Infrastructure.Persistence.DbInitializers;
using IdentityService.Application.Features.Users;
using Microsoft.AspNetCore.Authorization;
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
                .AddIdentityServices()
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



        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            return app
                .UseAuthentication()
                .UseCurrentUser()
                .UseAuthorization();
        }

    }
}

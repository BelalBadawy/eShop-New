using eShop.Application.Behaviours;
using eShop.Application.Helpers;
using Microsoft.Extensions.Configuration;

namespace eShop.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Add services for FluentValidation auto-validation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddDataProtection();

            // Register IdProtector
            services.AddSingleton<IdProtector>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>)); ;

            services.AddMapster();

            return services;
        }
    }
}

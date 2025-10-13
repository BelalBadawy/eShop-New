using Asp.Versioning;
using eShop.Infrastructure.Identity.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace eShop.API
{
    public static class StartUp
    {
        internal static IServiceCollection AddApiVersioningConfig(this IServiceCollection services)
        {
            return services.AddApiVersioning(options =>
                 {
                     options.DefaultApiVersion = new ApiVersion(1, 0);
                     options.AssumeDefaultVersionWhenUnspecified = true;
                     options.ReportApiVersions = true;
                 })
                 .AddApiExplorer(options =>
                 {
                     options.GroupNameFormat = "'v'VVV";
                     options.SubstituteApiVersionInUrl = true;
                 }).Services;
        }



    }
}

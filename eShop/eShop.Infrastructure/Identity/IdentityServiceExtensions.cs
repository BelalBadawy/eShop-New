using System.Net;
using eShop.Application.Features.Roles;
using eShop.Application.Features.Token;
using eShop.Application.Interfaces;
using eShop.Application.Models.JWT;
using eShop.Infrastructure.Identity.Models;
using eShop.Infrastructure.Identity.Permissions;
using eShop.Infrastructure.Identity.Services;
using eShop.Infrastructure.Persistence.Contexts;
using IdentityService.Application.Features.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using eShop.Application.Models;
using eShop.Infrastructure.Identity.Constants;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace eShop.Infrastructure.Identity
{
    internal static class IdentityServiceExtensions
    {
        internal static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            return services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    // Configure password requirements
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    // Require unique email addresses
                    options.User.RequireUniqueEmail = true;
                })
                // Use Entity Framework Core for identity storage
                .AddEntityFrameworkStores<ApplicationDbContext>()
                // Add default token providers for password reset, email confirmation, etc.
                .AddDefaultTokenProviders()
                .Services
                .AddTransient<IUserService, UserService>()
                .AddTransient<IRoleService, RoleService>()
                .AddTransient<ITokenService, TokenService>()
                .AddScoped<CurrentUserMiddleware>()
                .AddScoped<ICurrentUserService, CurrentUserService>(); ;
        }

        internal static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CurrentUserMiddleware>();
        }

        internal static IServiceCollection AddPermissions(this IServiceCollection services)
        {
            services
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            return services;
        }

        internal static JwtConfiguration GetTokenSettings(this IServiceCollection services, IConfiguration config)
        {
            var tokenSettingsConfig = config.GetSection(nameof(JwtConfiguration));
            services.Configure<JwtConfiguration>(tokenSettingsConfig);

            return tokenSettingsConfig.Get<JwtConfiguration>();
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration
                .GetSection("JwtConfiguration")
                .Get<JwtConfiguration>();

            var secret = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            services
                .AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    };
                    bearer.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("Token has expired"));
                                return context.Response.WriteAsync(result);
                            }
                            else
                            {
                                if (!context.Response.HasStarted)
                                {
                                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    context.Response.ContentType = "application/json";
                                    var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("An unhandled error has occurred"));
                                    return context.Response.WriteAsync(result);
                                }
                                return Task.CompletedTask;
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized."));
                                return context.Response.WriteAsync(result);
                            }
                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resource."));
                            return context.Response.WriteAsync(result);
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                foreach (var prop in typeof(AppPermissions).GetNestedTypes()
                    .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                    {
                        options.AddPolicy(propertyValue.ToString(), policy => policy
                            .RequireClaim(AppClaim.Permission, propertyValue.ToString()));
                    }
                }
            });

            return services;
        }
    }
}

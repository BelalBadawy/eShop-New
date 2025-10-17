using eShop.Application.Features.Roles;
using eShop.Application.Features.Token;
using eShop.Application.Interfaces;
using eShop.Application.Models;
using eShop.Application.Models.JWT;
using eShop.Infrastructure.Identity.Constants;
using eShop.Infrastructure.Identity.Models;
using eShop.Infrastructure.Identity.Permissions;
using eShop.Infrastructure.Identity.Services;
using eShop.Infrastructure.Persistence.Contexts;
using eShop.Infrastructure.Persistence.DbInitializers;
using IdentityService.Application.Features.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace eShop.Infrastructure.Identity
{
    internal static class IdentityServiceExtensions
    {
        internal static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
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
                    options.SignIn.RequireConfirmedEmail = true;

                    // Lockout settings
                    options.Lockout = new LockoutOptions
                    {
                        // How long the user will be locked out after exceeding failed attempts
                        DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15),

                        // Number of failed attempts before lockout
                        MaxFailedAccessAttempts = 5,

                        // Should new users also be subject to lockout?
                        AllowedForNewUsers = true
                    };

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
                .AddScoped<ICurrentUserService, CurrentUserService>()
                .AddTransient<IdentityDbSeeder>()
                .Configure<JwtConfiguration>(config.GetSection("JwtConfiguration")); ;
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

        //public static IServiceCollection AddJwtAuthenticationWORKING(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var jwtSettings = configuration
        //        .GetSection("JwtConfiguration")
        //        .Get<JwtConfiguration>();

        //    // It's good practice to ensure the settings are loaded correctly
        //    if (jwtSettings == null)
        //    {
        //        throw new InvalidOperationException("JwtConfiguration section is not configured in appsettings.json");
        //    }

        //    var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

        //    services
        //        .AddAuthentication(options =>
        //        {
        //            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //        })
        //        .AddJwtBearer(options =>
        //        {
        //            options.RequireHttpsMetadata = false; // Set to true in production environments
        //            options.SaveToken = true;
        //            options.TokenValidationParameters = new TokenValidationParameters
        //            {
        //                ValidateIssuerSigningKey = true,
        //                IssuerSigningKey = new SymmetricSecurityKey(key),
        //                ValidateIssuer = true,
        //                ValidIssuer = jwtSettings.Issuer,
        //                ValidateAudience = true,
        //                ValidAudience = jwtSettings.Audience,
        //                ValidateLifetime = true,
        //                ClockSkew = TimeSpan.Zero,
        //                RoleClaimType = ClaimTypes.Role
        //            };

        //            options.Events = new JwtBearerEvents
        //            {
        //                // This event is triggered when the token is fundamentally invalid (e.g., bad signature, expired).
        //                // Its purpose is for LOGGING. Do not write the response here.
        //                OnAuthenticationFailed = context =>
        //                {
        //                    // Log the error here if you have a logger instance.
        //                    // e.g., _logger.LogError(context.Exception, "Authentication failed.");
        //                    return Task.CompletedTask;
        //                },

        //                // This event is triggered when authentication is required but fails or is missing.
        //                // This is the CORRECT place to write a custom 401 Unauthorized response.
        //                OnChallenge = context =>
        //                {
        //                    // HandleResponse() prevents the default authentication challenge logic from running.
        //                    // This is critical to avoid conflicts and the "response has already started" error.
        //                    context.HandleResponse();

        //                    // A safeguard, though HandleResponse should prevent this.
        //                    if (context.Response.HasStarted)
        //                    {
        //                        return Task.CompletedTask;
        //                    }

        //                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //                    context.Response.ContentType = "application/json";

        //                    var message = "You are not authorized.";

        //                    // FIX: Explicitly declare the type as 'string' to resolve the CS0815 error.
        //                    //  string result = JsonSerializer.Serialize(ResponseWrapper.Fail(message));
        //                    var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized."));

        //                    return context.Response.WriteAsync(result);
        //                },

        //                // This event is triggered when the user is authenticated but lacks the required permissions.
        //                // This is the CORRECT place to write a custom 403 Forbidden response.
        //                OnForbidden = context =>
        //                {
        //                    if (context.Response.HasStarted)
        //                    {
        //                        return Task.CompletedTask;
        //                    }

        //                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
        //                    context.Response.ContentType = "application/json";

        //                    // FIX: Explicitly declare the type as 'string' here as well for consistency.
        //                    // string result = JsonSerializer.Serialize(ResponseWrapper.Fail("You are not authorized to access this resource."));
        //                    var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resource."));

        //                    return context.Response.WriteAsync(result);
        //                }
        //            };
        //        });

        //    // Authorization policies based on permissions
        //    services.AddAuthorization(options =>
        //    {
        //        foreach (var prop in typeof(AppPermissions)
        //            .GetNestedTypes()
        //            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        //        {
        //            var value = prop.GetValue(null)?.ToString();
        //            if (!string.IsNullOrWhiteSpace(value))
        //            {
        //                options.AddPolicy(value, policy =>
        //                    policy.RequireClaim(AppClaim.Permission, value));
        //            }
        //        }
        //    });

        //    return services;
        //}


        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration
                .GetSection("JwtConfiguration")
                .Get<JwtConfiguration>();

            // It's good practice to ensure the settings are loaded correctly
            if (jwtSettings == null)
            {
                throw new InvalidOperationException("JwtConfiguration section is not configured in appsettings.json");
            }

            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

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
                      ValidateLifetime = true,
                      ValidIssuer = jwtSettings.Issuer,
                      ValidAudience = jwtSettings.Audience,
                      RoleClaimType = ClaimTypes.Role,
                      ClockSkew = TimeSpan.Zero,
                      IssuerSigningKey = new SymmetricSecurityKey(key)
                  };

                  bearer.Events = new JwtBearerEvents
                  {
                      OnAuthenticationFailed = context =>
                      {
                          if (context.Exception is SecurityTokenExpiredException)
                          {
                              context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                              context.Response.ContentType = "application/json";
                              var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("The Token is expired."));
                              return context.Response.WriteAsync(result);
                          }
                          else
                          {
                              context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                              context.Response.ContentType = "application/json";
                              var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("An unhandled error has occurred."));
                              return context.Response.WriteAsync(result);
                          }
                      },
                      OnChallenge = context =>
                      {
                          context.HandleResponse();
                          if (!context.Response.HasStarted)
                          {
                              context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                              context.Response.ContentType = "application/json";
                              var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not Authorized."));
                              return context.Response.WriteAsync(result);
                          }

                          return Task.CompletedTask;
                      },
                      OnForbidden = context =>
                      {
                          context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                          context.Response.ContentType = "application/json";
                          var result = JsonConvert
                          .SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resource."));
                          return context.Response.WriteAsync(result);
                      }
                  };
              });

            // Authorization policies based on permissions
            services.AddAuthorization(options =>
            {
                foreach (var prop in typeof(AppPermissions)
                    .GetNestedTypes()
                    .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var value = prop.GetValue(null)?.ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        options.AddPolicy(value, policy =>
                            policy.RequireClaim(AppClaim.Permission, value));
                    }
                }
            });

            return services;
        }
    }
}

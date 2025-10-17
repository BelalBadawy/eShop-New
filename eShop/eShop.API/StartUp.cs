using Asp.Versioning;
using eShop.Infrastructure.Identity.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;

namespace eShop.API
{
    public static class StartUp
    {
        internal static IServiceCollection AddApiVersioningConfig(this IServiceCollection services)
        {
            services
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;

                    //  This line triggers OnStarting (disable it)
                    options.ReportApiVersions = false;
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            return services;
        }

        //internal static IServiceCollection AddSwagger(this IServiceCollection services)
        //{
        //    //return services.AddSwaggerGen(options =>
        //    //{

        //    //    options.MapType<byte[]>(() => new Microsoft.OpenApi.Models.OpenApiSchema
        //    //    {
        //    //        Type = "string",
        //    //        Format = "base64"
        //    //    });

        //    //    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyTemplate", Version = "v1" });

        //    //    var securitySchema = new OpenApiSecurityScheme
        //    //    {
        //    //        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        //    //        Name = "Authorization",
        //    //        In = ParameterLocation.Header,
        //    //        Type = SecuritySchemeType.Http,
        //    //        Scheme = "bearer",
        //    //        Reference = new OpenApiReference
        //    //        {
        //    //            Type = ReferenceType.SecurityScheme,
        //    //            Id = "Bearer"
        //    //        }
        //    //    };

        //    //    options.AddSecurityDefinition("Bearer", securitySchema);

        //    //    var securityRequirement = new OpenApiSecurityRequirement
        //    //    {
        //    //        { securitySchema, new[] { "Bearer" } }
        //    //    };

        //    //    options.AddSecurityRequirement(securityRequirement);


        //    //});
        //}

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Include XML comments (for controller/action/method summaries)
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }

                // Map byte[] to base64 string (as you already had)
                options.MapType<byte[]>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "base64"
                });

                // Optionally, map some common types (DateOnly, TimeOnly, etc.)
                // options.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
                // options.MapType<TimeOnly>(() => new OpenApiSchema { Type = "string", Format = "time" });

                // Define API version(s) if using versioning
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MyTemplate API",
                    Version = "v1",
                    Description = "MyTemplate API — version 1",
                    Contact = new OpenApiContact
                    {
                        Name = "Team Name",
                        Email = "team@example.com",
                        Url = new Uri("https://example.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                // Add security scheme for JWT Bearer tokens
                var securitySchema = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                options.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new[] { "Bearer" }
                    }
                };
                options.AddSecurityRequirement(securityRequirement);

                // Optionally turn on annotations (to allow decorating actions with [SwaggerOperation], [SwaggerResponse], etc.)
                //  options.EnableAnnotations();

                // Optionally, apply document filters or operation filters
                // e.g. add a filter that sets a default 500 response, or handles “problem details” globally:
                //  options.DocumentFilter<GlobalErrorResponseDocumentFilter>();

                // If using API versioning, you might add custom doc selection:
                // options.DocInclusionPredicate((docName, apiDesc) =>
                // {
                //     // include only controllers whose version matches docName
                //     ...
                // });

                // Tag by controller or group actions as you like
                options.TagActionsBy(api =>
                {
                    // e.g. group by controller name
                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    return new[] { controllerActionDescriptor?.ControllerName ?? api.GroupName };
                });

            });

            return services;
        }



        public static IServiceCollection AddCorsAllowAll(this IServiceCollection services)
        {
            // Add CORS policy
            return services.AddCors(options =>
                                                 {
                                                     options.AddPolicy("AllowAll", policy =>
                                                     {
                                                         policy
                                                             .AllowAnyOrigin()
                                                             .AllowAnyMethod()
                                                             .AllowAnyHeader();
                                                     });
                                                 });
        }




        public static IApplicationBuilder UseCorsAllowAll(this IApplicationBuilder app)
        {
            return app.UseCors("AllowAll");
        }
    }
}

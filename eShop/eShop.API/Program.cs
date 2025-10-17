
using eShop.Application;
using eShop.Infrastructure;
using Newtonsoft.Json;
using SmartStore.API.Middlewares;

namespace eShop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddControllers()
                .AddNewtonsoftJson(opt =>
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            builder.Services.AddCustomSwagger();
            builder.Services.AddCorsAllowAll();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddOpenApi();
            builder.Services.AddApiVersioningConfig();
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            // Configure the middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
            }

            app.UseHttpsRedirection();

            // Routing first
            app.UseRouting();

            // CORS before authentication
            app.UseCorsAllowAll();

            // Authentication and authorization before Swagger
            //app.UseAuthentication();
            //app.UseAuthorization();
            app.UseInfrastructureAsync().GetAwaiter().GetResult();



            app.UseSwaggerAuthorized();
            // Swagger after auth
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                c.RoutePrefix = "swagger";
            });

            // Controllers last
            app.MapControllers();

            app.Run();
        }
    }
}

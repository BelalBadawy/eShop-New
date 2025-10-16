
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

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                // options.Filters.Add(typeof(LogUserActivitiesAttribute));
            }).AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);


            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddApiVersioningConfig();

            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddCorsAllowAll();

            builder.Services.AddCustomSwagger();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseSwaggerAuthorized();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyTemplate API v1");
                // optionally serve UI at root: c.RoutePrefix = string.Empty;
            });


            app.UseCorsAllowAll();
            app.UseHttpsRedirection();

            app.UseInfrastructureAsync().GetAwaiter().GetResult(); ;

            app.MapControllers();


            app.Run();
        }
    }
}

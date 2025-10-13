
using eShop.Application;
using eShop.Infrastructure;
using Newtonsoft.Json;

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


            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseInfrastructure();

            app.MapControllers();


            app.Run();
        }
    }
}

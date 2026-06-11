using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using UShort.Data;
using UShort.Data.Configurations;

namespace UShort.Api.Configurations;

internal static class ProgramExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void ConfigureDatabase()
        {
            var configuration = builder.Configuration;
            var services = builder.Services;

            (services, configuration).AddDbContext();
        }
    }

    extension(IServiceCollection services)
    {
        public void ConfigureServices()
        {
            services.AddMemoryCache();
            services.AddCustomAuthorizeMethod();
            services.AddOpenApi();
            services.AddCors();
        }
    }

    extension(WebApplication app)
    {
        public void UseConfigurations()
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .WithExposedHeaders("Location");
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
        }

        public async Task ExecuteMigrations()
        {
            using var scope = app.Services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<UShortDbContext>();
            await context.Database.MigrateAsync();
        }
    }
}
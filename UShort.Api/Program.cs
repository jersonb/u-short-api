using UShort.Api.Configurations;
using UShort.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureDatabase();

var services = builder.Services;
services.ConfigureServices();

var app = builder.Build();

app.UseConfigurations();

await app.ExecuteMigrations();

app.AddAuthEndpoints();

app.AddShortUrlEndpoints();

await app.RunAsync();
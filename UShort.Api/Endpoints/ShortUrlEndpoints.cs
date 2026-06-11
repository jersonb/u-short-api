using Microsoft.EntityFrameworkCore;
using UShort.Api.Configurations;
using UShort.Data;
using UShort.Data.Entities;

namespace UShort.Api.Endpoints;

public static class ShortUrlEndpoints
{
    extension(WebApplication app)
    {
        public void AddShortUrlEndpoints()
        {
            app.MapGroup("/api/short/")
                .AddPost(app.Logger)
                .AddGet(app.Logger)
                .AddGetByCode(app.Logger);
        }
    }

    extension(RouteGroupBuilder route)
    {
        internal RouteGroupBuilder AddPost(ILogger logger)
        {
            route.MapPost("", async (ShortUrlCreateRequest request, HttpContext httpContext, UShortDbContext context, CancellationToken cancellationToken) =>
            {
                try
                {
                    var userId = httpContext.LoggedInUserId;

                    var shortUrl = new ShortUrl
                    {
                        Code = request.Code,
                        Description = request.Description,
                        Url = request.Url,
                        UshortUserId = userId,
                        CreatedtAt = DateTimeOffset.UtcNow,
                    };

                    context.ShortUrls.Add(shortUrl);
                    await context.SaveChangesAsync(cancellationToken);
                    return Results.Created(string.Empty, shortUrl);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error: ");
                    return Results.BadRequest();
                }
            }).RequireAuthorization()
            .WithName("CreateShortUrl");

            return route;
        }

        internal RouteGroupBuilder AddGet(ILogger logger)
        {
            route.MapGet("", async (UShortDbContext context, HttpContext httpContext, CancellationToken cancellationToken) =>
            {
                try
                {
                    var userId = httpContext.LoggedInUserId;

                    var list = context.ShortUrls
                        .Where(s => s.UshortUser.Id == userId)
                        .Select(s => new ShortUrlItemResponse
                        {
                            Code = s.Code,
                            Date = s.CreatedtAt,
                            Description = s.Description,
                            Url = s.Url,
                            Id = s.ShortUrlId,
                        });

                    return Results.Ok(list);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error: ");
                    return Results.BadRequest();
                }
            }).RequireAuthorization()
            .WithName("GetListShortUrl");
            return route;
        }

        internal RouteGroupBuilder AddGetByCode(ILogger logger)
        {
            route.MapGet("{code}/redirect", async (string code, UShortDbContext context, CancellationToken cancellationToken) =>
            {
                try
                {
                    var uShort = await context.ShortUrls.SingleOrDefaultAsync(s => s.Code == code, cancellationToken);

                    if (uShort is null)
                    {
                        return Results.NotFound();
                    }
                    return Results.Ok(uShort.Url);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error: ");
                    return Results.BadRequest();
                }
            }).WithName("GetByCode");
            return route;
        }
    }
}

record ShortUrlCreateRequest(string Code, string Description, string Url);

record ShortUrlItemResponse
{
    public Guid Id { get; init; }
    public required string Code { get; init; }
    public DateTimeOffset Date { get; init; }
    public required string Url { get; init; }
    public required string Description { get; init; }
}
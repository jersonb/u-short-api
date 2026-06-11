using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using UShort.Data;

namespace UShort.Api.Configurations;

public static class AuthorizationConfiguration
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCustomAuthorizeMethod()
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = UserByHeaderAuthenticationOptions.DefaultScheme;
                options.DefaultChallengeScheme = UserByHeaderAuthenticationOptions.DefaultScheme;
            })
            .AddScheme<UserByHeaderAuthenticationOptions, ApiKeyAuthenticationHandler>(
                UserByHeaderAuthenticationOptions.DefaultScheme, null);

            services.AddAuthorization();
            return services;
        }
    }

    extension(HttpContext httpContext)
    {
        public int LoggedInUserId =>
            int.TryParse(httpContext.User.FindFirst(ClaimTypes.UserData)?.Value, out int userId)
            ? userId
            : throw new AuthenticationFailureException("");
    }
}

public class UserByHeaderAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "UserIdHeader";
    public string HeaderName { get; set; } = "X-user-id";
}

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<UserByHeaderAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IMemoryCache memoryCache,
    UShortDbContext context)
    : AuthenticationHandler<UserByHeaderAuthenticationOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var extractedApiKey))
        {
            return AuthenticateResult.Fail("Faltou se autenticar!");
        }

        if (!Guid.TryParse(extractedApiKey, out Guid userId))
        {
            return AuthenticateResult.Fail("Autenticação falhou!");
        }

        var user = await memoryCache.GetOrCreateAsync(userId.ToString(), async (entry) =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return await context.UshortUsers.SingleAsync(u => u.UserId == userId, CancellationToken.None);
        });

        if (user == null)
        {
            return AuthenticateResult.Fail("Autenticação falhou!");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.UserData, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Email.ToString()),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
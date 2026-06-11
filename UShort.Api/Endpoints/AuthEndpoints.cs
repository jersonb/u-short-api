using Microsoft.EntityFrameworkCore;
using UShort.Data;
using UShort.Data.Entities;

namespace UShort.Api.Endpoints;

public static class AuthEndpoints
{
    extension(WebApplication app)
    {
        public void AddAuthEndpoints()
        {
            app.Login();
        }

        private void Login()
        {
            app.MapPost("/api/auth/login", async (AuthRequest request, UShortDbContext context, CancellationToken cancellationToken) =>
            {
                try
                {
                    var user = await context.UshortUsers
                    .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

                    if (user is null)
                    {
                        user = new UshortUser
                        {
                            Email = request.Email,
                        };

                        context.UshortUsers.Add(user);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    return Results.Ok(new { token = user.UserId });
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error: ");
                    return Results.BadRequest();
                }
            }).WithName("AuthLogin");
        }
    }
}

record AuthRequest(string Email);
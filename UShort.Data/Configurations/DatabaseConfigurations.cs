using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UShort.Data.Configurations;

public static class DatabaseConfigurations
{
    extension((IServiceCollection Services, IConfiguration Configuration) i)
    {
        public void AddDbContext()
        {
            i.Services.AddNpgsql<UShortDbContext>(i.Configuration.GetConnectionString("Postgres"));
        }
    }
}
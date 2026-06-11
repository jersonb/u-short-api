using Microsoft.EntityFrameworkCore;
using UShort.Data.Entities;

namespace UShort.Data;

public class UShortDbContext(DbContextOptions<UShortDbContext> options) : DbContext(options)
{
    public DbSet<UshortUser> UshortUsers { get; set; }
    public DbSet<ShortUrl> ShortUrls { get; set; }
}
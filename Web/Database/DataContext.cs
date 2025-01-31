using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web.Configuration;
using Web.Exceptions;
using Web.Features.Users;

namespace Web.Database;

public class DataContext(DbContextOptions options, IOptions<AppOptions> qqProjectNameOptions) : DbContext(options)
{
    private readonly AppOptions appOptions = qqProjectNameOptions.Value;
    public DbSet<User> Users { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (string.IsNullOrWhiteSpace(appOptions.ConnectionString))
        {
            throw new ConnectionStringNotProvidedException();
        }

        optionsBuilder.UseSqlServer(appOptions.ConnectionString, x => x.UseNodaTime());
    }
}

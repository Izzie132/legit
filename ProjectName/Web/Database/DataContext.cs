using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web.Configuration;
using Web.Features.User;

namespace Web.Database;

public class DataContext(DbContextOptions options, IOptions<ProjectNameOptions> projectNameOptions)
    : DbContext(options)
{
    private readonly ProjectNameOptions projectNameOptions = projectNameOptions.Value;
    public DbSet<User> Users { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (string.IsNullOrWhiteSpace(projectNameOptions.ConnectionString))
        {
            throw new ConnectionStringNotProvidedException();
        }

        optionsBuilder.UseSqlServer(projectNameOptions.ConnectionString);
    }
}

public class ConnectionStringNotProvidedException : Exception
{
    public ConnectionStringNotProvidedException()
        : base(
            $"Could not determine connection string - please ensure the "
                + $"'{ProjectNameOptions.ConfigurationKey}:{nameof(ProjectNameOptions.ConnectionString)}' "
                + $"config value is set"
        ) { }
}

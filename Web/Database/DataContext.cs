using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web.Configuration;
using Web.Exceptions;
using Web.Features.Users;

namespace Web.Database;

public class DataContext(DbContextOptions options, IOptions<ProjectNameOptions> projectNameOptions) : DbContext(options)
{
    private readonly ProjectNameOptions projectNameOptions = projectNameOptions.Value;
    public DbSet<User> Users { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (string.IsNullOrWhiteSpace(projectNameOptions.ConnectionString))
        {
            throw new ConnectionStringNotProvidedException();
        }

        optionsBuilder.UseSqlServer(projectNameOptions.ConnectionString, x => x.UseNodaTime());
    }
}

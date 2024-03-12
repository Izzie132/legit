using Web.Features.Users;

namespace DataSeeder;

public static class DataSeederContext
{
#pragma warning disable CA2227 // (Collection properties should be read only) This is fine for a data seeder that's only used in a limited way
    public static ICollection<User>? Users { get; set; }
#pragma warning restore CA2227
}

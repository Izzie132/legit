using Web.Features.User;

namespace DataSeeder;

public static class DataSeederContext
{
    public static ICollection<User>? Users { get; set; }
}

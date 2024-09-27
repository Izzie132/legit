using Bogus;
using Builders.Features.User;
using Web.Features.Users;

namespace DataSeeder.Features.Users;

public static class TestUsers
{
    public static void BuildAndSeedUsers()
    {
        DataSeeder.IfVerbose(Console.Write, "Building Users ...");
        var seedUsers = GetSeedUsers(DataSeeder.DataSeederModeAmounts);
        DataSeeder.IfVerbose(DataSeeder.WriteLineElapsedTime, DataSeeder.Stopwatch.ElapsedMilliseconds);
        DataSeeder.Stopwatch.Restart();

        DataSeeder.Insert(seedUsers.ToList());
    }

    private static List<User> GetSeedUsers(DataSeederModeAmounts dataSeederModeAmounts)
    {
        // Only create this once to avoid the overhead of creating one per builder
        var faker = new Faker { Random = new Randomizer(348972347) };

        var testUsers = Enumerable
            .Range(0, dataSeederModeAmounts.MaxUsers)
            .Select(_ => new UserBuilder(faker).Build())
            .ToList();

        DataSeederContext.Users = testUsers;

        return testUsers;
    }
}

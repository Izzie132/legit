namespace DataSeeder;

public enum DataSeedMode
{
    None,
    Minimal,
    Full,
    Load,
}

public record DataSeederModeAmounts(int MaxUsers)
{
    public static readonly DataSeederModeAmounts Minimal = new(MaxUsers: 0);

    public static readonly DataSeederModeAmounts Full = new(MaxUsers: 10);

    public static readonly DataSeederModeAmounts Load = new(MaxUsers: 1000);
};

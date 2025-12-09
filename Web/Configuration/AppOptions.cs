namespace Web.Configuration;

public class AppOptions
{
    public const string ConfigurationKey = "Legit";

    public string ConnectionString { get; set; } = string.Empty;
    public string SecretMessagePassword { get; set; } = string.Empty;
}

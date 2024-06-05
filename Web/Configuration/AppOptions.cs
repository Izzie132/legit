namespace Web.Configuration;

public class AppOptions
{
    public const string ConfigurationKey = "QQProjectName";

    public string ConnectionString { get; set; } = string.Empty;
    public string SecretMessagePassword { get; set; } = string.Empty;
}

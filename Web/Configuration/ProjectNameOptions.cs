namespace Web.Configuration;

public class ProjectNameOptions
{
    public const string ConfigurationKey = "ProjectName";

    public string ConnectionString { get; set; } = string.Empty;
    public string SecretMessagePassword { get; set; } = string.Empty;
}

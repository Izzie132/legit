namespace Web.Configuration.Extensions;

public static class OptionsConfigExtensions
{
    public static void ConfigureOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<AppOptions>(configuration.GetSection(key: AppOptions.ConfigurationKey));
        services.Configure<ExceptionOptions>(configuration.GetSection(key: ExceptionOptions.ConfigurationKey));
    }
}

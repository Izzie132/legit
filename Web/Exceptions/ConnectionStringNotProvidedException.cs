using Web.Configuration;

namespace Web.Exceptions;

public class ConnectionStringNotProvidedException : Exception
{
    public ConnectionStringNotProvidedException()
        : base(
            $"Could not determine connection string - please ensure the "
                + $"'{AppOptions.ConfigurationKey}:{nameof(AppOptions.ConnectionString)}' "
                + $"config value is set"
        ) { }
}

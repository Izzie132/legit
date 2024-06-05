using Web.Configuration;

namespace Web.Exceptions;

public class ConnectionStringNotProvidedException : Exception
{
    public ConnectionStringNotProvidedException()
        : base(
            $"Could not determine connection string - please ensure the "
                + $"'{ProjectNameOptions.ConfigurationKey}:{nameof(ProjectNameOptions.ConnectionString)}' "
                + $"config value is set"
        ) { }
}

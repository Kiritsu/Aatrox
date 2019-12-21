using Aatrox.Data;
using Microsoft.Extensions.Options;

namespace Aatrox.Core.Providers
{
    public sealed class DatabaseConfigurationProvider : IDatabaseConfigurationProvider
    {
        private readonly DatabaseConfiguration _databaseConfiguration;

        public DatabaseConfigurationProvider(IOptions<DatabaseConfiguration> config)
        {
            _databaseConfiguration = config.Value;
        }

        public DatabaseConfiguration GetConfiguration()
        {
            return _databaseConfiguration;
        }
    }
}

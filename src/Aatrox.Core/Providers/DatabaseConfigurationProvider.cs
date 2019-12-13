﻿using Aatrox.Core.Configurations;
using Aatrox.Data;
using Microsoft.Extensions.Options;

namespace Aatrox.Core.Providers
{
    public sealed class DatabaseConfigurationProvider : IDatabaseConfigurationProvider
    {
        private readonly IDatabaseConfiguration _databaseConfiguration;

        public DatabaseConfigurationProvider(IOptions<DatabaseConfiguration> config)
        {
            _databaseConfiguration = config.Value;
        }

        public IDatabaseConfiguration GetConfiguration()
        {
            return _databaseConfiguration;
        }
    }
}

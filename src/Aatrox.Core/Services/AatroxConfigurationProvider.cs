using Aatrox.Core.Configurations;
using Aatrox.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Aatrox.Core.Services
{
    public sealed class AatroxConfigurationProvider : IAatroxConfigurationProvider
    {
        private readonly AatroxConfiguration _configuration;

        public AatroxConfigurationProvider(IOptions<AatroxConfiguration> config)
        {
            _configuration = config.Value;
        }

        public AatroxConfiguration GetConfiguration()
        {
            return _configuration;
        }
    }
}

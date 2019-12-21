using Aatrox.Core.Configurations;
using Microsoft.Extensions.Options;

namespace Aatrox.Core.Providers
{
    public sealed class AatroxConfigurationProvider
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

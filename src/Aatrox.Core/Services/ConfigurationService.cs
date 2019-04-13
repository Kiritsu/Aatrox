using System.IO;
using Newtonsoft.Json;

namespace Aatrox.Core.Services
{
    public sealed class ConfigurationService
    {
        public string Token { get; set; }

        public static ConfigurationService Setup()
        {
            if (!File.Exists("credentials.json"))
            {
                throw new FileNotFoundException("The configuration file was not found.");
            }

            return JsonConvert.DeserializeObject<ConfigurationService>(File.ReadAllText("credentials.json"));
        }
    }
}

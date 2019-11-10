using System.IO;
using Aatrox.Core.Configurations;
using Aatrox.Data;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json.Linq;

namespace Aatrox
{
    public sealed class DesignTimeAatroxContextFactory : IDesignTimeDbContextFactory<AatroxDbContext>
    {
        public AatroxDbContext CreateDbContext(string[] args)
        {
            return new AatroxDbContext(new ConnectionStringProvider(new DesignTimeDatabaseConfigurationProvider()));
        }
    }

    public sealed class DesignTimeDatabaseConfigurationProvider : IDatabaseConfigurationProvider
    {
        public IDatabaseConfiguration GetConfiguration()
        {
            var config = JObject.Parse(File.ReadAllText("credentials.json"))["Database"];

            return new DatabaseConfiguration
            {
                Host = config["Host"].Value<string>(),
                Port = config["Port"].Value<int>(),
                Database = config["Database"].Value<string>(),
                Username = config["Username"].Value<string>(),
                Password = config["Password"].Value<string>(),
            };
        }
    }
}

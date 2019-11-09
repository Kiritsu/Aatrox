using Aatrox.Data;

namespace Aatrox.Core.Configurations
{
    public sealed class DatabaseConfiguration : IDatabaseConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

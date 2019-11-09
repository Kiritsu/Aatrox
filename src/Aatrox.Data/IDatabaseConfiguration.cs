namespace Aatrox.Data
{
    public interface IDatabaseConfiguration
    {
        string Host { get; set; }
        int Port { get; set; }
        string Database { get; set; }
        string Username { get; set; }
        string Password { get; set; }
    }
}

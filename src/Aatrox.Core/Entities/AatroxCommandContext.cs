using System;
using System.Threading.Tasks;
using Aatrox.Data;
using Disqord;
using Disqord.Events;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Aatrox.Core.Entities
{
    public sealed class AatroxCommandContext : CommandContext, IDisposable
    {
        public CommandService Commands { get; }
        public DiscordClient Client { get; }
        public CachedGuild Guild { get; }
        public IMessageChannel Channel { get; }
        public CachedUser User { get; }
        public CachedMember Member { get; }
        public CachedMessage Message { get; }
        public CachedUser Aatrox => Client.CurrentUser;

        public string Prefix { get; set; }

        private readonly IUnitOfWork _database;
        private DatabaseCommandContext _databaseContext;

        public DatabaseCommandContext DatabaseContext
        { 
            get
            {
                if (!_databaseContext.IsReady)
                {
                    throw new InvalidOperationException("The database context is not ready. Please make sure it's prepared.");
                }

                return _databaseContext;
            }
        }

        public EventArgs EventArgs { get; }

        public AatroxCommandContext(MessageReceivedEventArgs e, IServiceProvider services) : base(services)
        {
            Commands = services.GetRequiredService<CommandService>();

            Client = e.Client as DiscordClient;
            Guild = (e.Message.Channel as CachedGuildChannel).Guild;
            Channel = e.Message.Channel;
            User = e.Message.Author;
            Member = e.Message.Author as CachedMember;
            Message = e.Message;

            EventArgs = e;

            _database = AatroxDbContextManager.CreateContext();
        }

        public AatroxCommandContext(MessageUpdatedEventArgs e, IServiceProvider services) : base(services)
        {
            Commands = services.GetRequiredService<CommandService>();

            Client = e.Client as DiscordClient;
            Guild = (e.NewMessage.Channel as CachedGuildChannel).Guild;
            Channel = e.NewMessage.Channel;
            User = e.NewMessage.Author;
            Member = e.NewMessage.Author as CachedMember;
            Message = e.NewMessage;

            EventArgs = e;

            _database = AatroxDbContextManager.CreateContext();
        }

        public Task PrepareAsync()
        {
            _databaseContext = new DatabaseCommandContext(this, _database);
            return _databaseContext.PrepareAsync();
        }
        
        public Task EndAsync()
        {
            return _database.SaveChangesAsync();
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}

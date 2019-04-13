using System;
using System.Threading.Tasks;
using Aatrox.Core.Services;
using Aatrox.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Aatrox.Core.Entities
{
    public sealed class DiscordCommandContext : CommandContext, IDisposable
    {
        public IServiceProvider Services { get; }
        public CommandService Commands { get; }
        public InternationalizationService I18n { get; }

        public DiscordClient Client { get; }
        public DiscordGuild Guild { get; }
        public DiscordChannel Channel { get; }
        public DiscordUser User { get; }
        public DiscordMember Member { get; }
        public DiscordMessage Message { get; }
        public DiscordUser Aatrox => Client.CurrentUser;

        public string Prefix { get; set; }

        private readonly IUnitOfWork _database;
        private DatabaseContext _databaseContext;

        public DatabaseContext DatabaseContext
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

        public DiscordCommandContext(MessageCreateEventArgs e, IServiceProvider services)
        {
            Services = services;
            Commands = Services.GetRequiredService<CommandService>();
            I18n = Services.GetRequiredService<InternationalizationService>();

            Client = e.Client;
            Guild = e.Guild;
            Channel = e.Channel;
            User = e.Author;
            Member = e.Author as DiscordMember;
            Message = e.Message;

            EventArgs = e;

            _database = AatroxDbContextManager.CreateContext();
        }

        public DiscordCommandContext(MessageUpdateEventArgs e, IServiceProvider services)
        {
            Services = services;
            Commands = Services.GetRequiredService<CommandService>();
            I18n = Services.GetRequiredService<InternationalizationService>();

            Client = e.Client;
            Guild = e.Guild;
            Channel = e.Channel;
            User = e.Author;
            Member = e.Author as DiscordMember;
            Message = e.Message;

            EventArgs = e;

            _database = AatroxDbContextManager.CreateContext();
        }

        public Task PrepareAsync()
        {
            _databaseContext = new DatabaseContext(this, _database);
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

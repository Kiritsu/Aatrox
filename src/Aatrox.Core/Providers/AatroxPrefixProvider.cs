using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Data;
using Aatrox.Data.Repositories;
using Disqord;
using Disqord.Bot.Prefixes;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Aatrox.Core.Providers
{
    public sealed class AatroxPrefixProvider : IPrefixProvider
    {
        private readonly IServiceProvider _services;

        public AatroxPrefixProvider(IServiceProvider services)
        {
            _services = services;
        }

        public async ValueTask<IEnumerable<IPrefix>> GetPrefixesAsync(CachedUserMessage message)
        {
            var strPrefixes = new List<string>()
            {
                $"<@{message.Guild.CurrentMember.Id}> ",
                $"<@!{message.Guild.CurrentMember.Id}> ",
                "Aa!"
            };

            await using var db = _services.GetRequiredService<AatroxDbContext>();
            var repository = db.RequestRepository<GuildRepository>();

            var guild = await repository.GetOrAddAsync(message.Guild.Id);

            strPrefixes.AddRange(guild.Prefixes);

            return strPrefixes.Select(x => new AatroxPrefix(x));
        }
    }

    public class AatroxPrefix : IPrefix
    {
        public readonly string _prefix;

        public AatroxPrefix(string prefix)
        {
            _prefix = prefix;
        }

        public bool TryFind(CachedUserMessage message, out string output)
        {
            return CommandUtilities.HasPrefix(message.Content, _prefix,
                StringComparison.OrdinalIgnoreCase, out output);
        }

        public override string ToString()
        {
            return _prefix;
        }
    }
}

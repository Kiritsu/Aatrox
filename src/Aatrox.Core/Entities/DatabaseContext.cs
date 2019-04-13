using System.Threading.Tasks;
using Aatrox.Data;
using Aatrox.Data.Entities;
using Aatrox.Data.Repositories.Interfaces;

namespace Aatrox.Core.Entities
{
    public class DatabaseContext
    {
        private readonly DiscordCommandContext _ctx;
        private readonly IUnitOfWork _uow;

        private readonly IGuildRepository _guilds;
        
        public bool IsReady { get; private set; }

        public GuildEntity Guild { get; private set; }

        public DatabaseContext(DiscordCommandContext ctx, IUnitOfWork uow)
        {
            _ctx = ctx;
            _uow = uow;
            _guilds = _uow.RequestRepository<IGuildRepository>();
        }

        public async Task PrepareAsync()
        {
            Guild = await _guilds.GetOrAddAsync(_ctx.Guild.Id);

            IsReady = true;
        }

        public async Task UpdateGuildAsync()
        {
            await _guilds.UpdateAsync(Guild);
        }
    }
}

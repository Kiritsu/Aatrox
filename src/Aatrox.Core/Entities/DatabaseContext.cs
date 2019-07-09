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
        private readonly IUserRepository _users;
        
        public bool IsReady { get; private set; }

        public GuildEntity Guild { get; private set; }
        public UserEntity User { get; private set; }

        public DatabaseContext(DiscordCommandContext ctx, IUnitOfWork uow)
        {
            _ctx = ctx;
            _uow = uow;
            _guilds = _uow.RequestRepository<IGuildRepository>();
            _users = _uow.RequestRepository<IUserRepository>();
        }

        public async Task PrepareAsync()
        {
            Guild = await _guilds.GetOrAddAsync(_ctx.Guild.Id);
            User = await _users.GetOrAddAsync(_ctx.User.Id);

            IsReady = true;
        }

        public async Task UpdateGuildAsync()
        {
            await _guilds.UpdateAsync(Guild);
        }

        public async Task UpdateUserAsync()
        {
            await _users.UpdateAsync(User);
        }
    }
}

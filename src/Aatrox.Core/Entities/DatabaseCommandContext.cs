using System.Threading.Tasks;
using Aatrox.Data;
using Aatrox.Data.Entities;
using Aatrox.Data.Repositories.Interfaces;

namespace Aatrox.Core.Entities
{
    public class DatabaseCommandContext
    {
        private readonly AatroxCommandContext _ctx;

        private readonly IGuildRepository _guilds;
        private readonly IUserRepository _users;
        
        public bool IsReady { get; private set; }

        public GuildEntity Guild { get; private set; }
        public UserEntity User { get; private set; }

        public DatabaseCommandContext(AatroxCommandContext ctx, IUnitOfWork uow)
        {
            _ctx = ctx;
            _guilds = uow.RequestRepository<IGuildRepository>();
            _users = uow.RequestRepository<IUserRepository>();
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

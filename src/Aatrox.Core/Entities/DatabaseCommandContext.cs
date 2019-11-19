using System.Threading.Tasks;
using Aatrox.Data;
using Aatrox.Data.Entities;
using Aatrox.Data.Repositories.Interfaces;

namespace Aatrox.Core.Entities
{
    public sealed class DatabaseCommandContext
    {
        private readonly AatroxCommandContext _ctx;

        private readonly IGetOrAddRepository<GuildEntity> _guilds;
        private readonly IGetOrAddRepository<UserEntity> _users;
        private readonly IGetOrAddRepository<LeagueUserEntity> _leagueUsers;
        private readonly IGetOrAddRepository<OsuUserEntity> _osuUsers;

        public bool IsReady { get; private set; }

        public GuildEntity Guild { get; private set; }
        public UserEntity User { get; private set; }
        public LeagueUserEntity LeagueUser { get; private set; }
        public OsuUserEntity OsuUser { get; private set; }

        public AatroxDbContext Database { get; }

        public DatabaseCommandContext(AatroxCommandContext ctx, AatroxDbContext context)
        {
            _ctx = ctx;
            _guilds = context.RequestRepository<IGetOrAddRepository<GuildEntity>>();
            _users = context.RequestRepository<IGetOrAddRepository<UserEntity>>();
            _leagueUsers = context.RequestRepository<IGetOrAddRepository<LeagueUserEntity>>();
            _osuUsers = context.RequestRepository<IGetOrAddRepository<OsuUserEntity>>();
            Database = context;
        }

        public async Task PrepareAsync()
        {
            LeagueUser = await _leagueUsers.GetOrAddAsync(_ctx.User.Id);
            Guild = await _guilds.GetOrAddAsync(_ctx.Guild.Id);
            User = await _users.GetOrAddAsync(_ctx.User.Id);
            OsuUser = await _osuUsers.GetOrAddAsync(_ctx.User.Id);

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

        public async Task UpdateLeagueUserAsync()
        {
            await _leagueUsers.UpdateAsync(User.LeagueProfile);
        }

        public async Task UpdateOsuUserAsync()
        {
            await _osuUsers.UpdateAsync(User.OsuProfile);
        }
    }
}

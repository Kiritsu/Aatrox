using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;

namespace Aatrox.Core.Entities
{
    public class DiscordModuleBase : ModuleBase<DiscordCommandContext>
    {
        public DatabaseContext DbContext => Context.DatabaseContext;

        public Task<DiscordMessage> RespondLocalizedAsync(string key, params object[] parameters)
        {
            return Context.Channel.SendMessageAsync(Context.I18n.GetLocalization(key, parameters: parameters));
        }
    }
}

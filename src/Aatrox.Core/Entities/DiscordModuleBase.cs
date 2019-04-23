﻿using System.Threading.Tasks;
using Aatrox.Core.Helpers;
using Aatrox.Core.Services;
using DSharpPlus.Entities;
using Qmmands;

namespace Aatrox.Core.Entities
{
    public class DiscordModuleBase : ModuleBase<DiscordCommandContext>
    {
        public DatabaseContext DbContext => Context.DatabaseContext;
        public InternationalizationService I18n => Context.I18n;

        public Task<DiscordMessage> RespondLocalizedAsync(string key, params object[] parameters)
        {
            var localization = I18n.GetLocalization(key, parameters: parameters);

            return Context.Channel.SendMessageAsync(localization);
        }

        public Task<DiscordMessage> RepondEmbedLocalizedAsync(string key, params object[] parameters)
        {
            var localization = I18n.GetLocalization(key, parameters: parameters);
            var embed = EmbedHelper.New(Context, localization);

            return Context.Channel.SendMessageAsync(embed: embed);
        }

        public Task<DiscordMessage> RespondAsync(string message)
        {
            return Context.Channel.SendMessageAsync(message);
        }

        public Task<DiscordMessage> RespondEmbedAsync(string message)
        {
            var embed = EmbedHelper.New(Context, message);

            return Context.Channel.SendMessageAsync(embed: embed);
        }

        public Task<DiscordMessage> RespondAsync(string message, DiscordEmbed embed)
        {
            return Context.Channel.SendMessageAsync(message, embed: embed);
        }

        public Task<DiscordMessage> RespondAsync(DiscordEmbed embed)
        {
            return Context.Channel.SendMessageAsync(embed: embed);
        }
    }
}

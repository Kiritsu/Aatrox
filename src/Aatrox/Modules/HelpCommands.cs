using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aatrox.Checks;
using Aatrox.Core.Configurations;
using Aatrox.Core.Entities;
using Aatrox.Core.Extensions;
using Aatrox.Core.Interfaces;
using Aatrox.Core.Services;
using Disqord;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Help"), Hidden]
    public sealed class HelpCommands : AatroxModuleBase
    {
        private readonly ICommandService _commands;
        private readonly AatroxConfiguration _configuration;

        public HelpCommands(ICommandService commands, IAatroxConfigurationProvider configuration)
        {
            _commands = commands;
            _configuration = configuration.GetConfiguration();
        }

        [Command("Help"), Hidden]
        [Description("Shows the different commands and modules usages.")]
        public async Task HelpAsync()
        {
            var modules = await Task.WhenAll(_commands.GetAllModules().Where(x => !x.Attributes.Any(y => y is HiddenAttribute)).Select(async x =>
            {
                var result = await x.RunChecksAsync(Context);
                return result.IsSuccessful ? x : null;
            }).Where(x => x != null));

            var commands = (await Task.WhenAll(_commands.GetAllCommands().Where(x => !x.Attributes.Any(y => y is HiddenAttribute)).Select(async x =>
            {
                var result = await x.RunChecksAsync(Context);
                return result.IsSuccessful ? x : null;
            }))).Where(x => x != null && x.Module.Parent is null).DistinctBy(x => x.Name).ToArray();

            var prefixes = DbContext.Guild.Prefixes.Select(x => $"`{x}`").Append("`Aa!`");
            var embed = new LocalEmbedBuilder
            {
                Color = _configuration.EmbedColor,
                Title = "Help",
                Description = InternationalizationService.GetLocalization("help_description", DbContext.User.Language, Context.Prefix, string.Join(", ", prefixes)),
                Footer = new LocalEmbedFooterBuilder
                {
                    Text = InternationalizationService.GetLocalization("help_footer", DbContext.User.Language, modules.Length, commands.Length)
                }
            };

            embed.AddField("Modules", string.Join(", ", modules.Select(x => $"`{x.Name}`")));
            embed.AddField("Commands", string.Join(", ", commands.Select(x => $"`{x.Name}`")));

            await RespondAsync(embed.Build());
        }

        [Command("Help"), Hidden]
        [Description("Shows the different commands and modules usages.")]
        public Task HelpAsync([Remainder, Description("Command or module.")] string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return HelpAsync();
            }

            var matchingCommands = _commands.FindCommands(command);
            if (matchingCommands.Count == 0)
            {
                var typoFix = command.Levenshtein(_commands);
                if (!string.IsNullOrWhiteSpace(typoFix))
                {
                    matchingCommands = _commands.FindCommands(typoFix);
                }
            }

            LocalEmbedBuilder embed;
            if (matchingCommands.Count == 0) //Could be a module.
            {
                var matchingModule = _commands.TopLevelModules.FirstOrDefault(x => x.Name.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (matchingModule is null)
                {
                    matchingModule = _commands.TopLevelModules.FirstOrDefault(x => x.Name.Equals(command.Levenshtein(_commands), StringComparison.OrdinalIgnoreCase));
                }

                var modules = _commands.GetAllModules();
                if (matchingModule is null) //Look for nested modules
                {
                    matchingModule = modules.FirstOrDefault(x => x.FullAliases.Any(y => y.Equals(command, StringComparison.OrdinalIgnoreCase)));
                }

                if (matchingModule is null) //Look for nested modules with levenshtein
                {
                    matchingModule = modules.FirstOrDefault(x => x.FullAliases.Any(y => y.Equals(command.Levenshtein(modules.Select(x => x.FullAliases.FirstOrDefault()).ToList()), StringComparison.OrdinalIgnoreCase)));
                }

                if (matchingModule is null) //Look for submodule but without complete module path
                {
                    matchingModule = modules.FirstOrDefault(x => x.Name.Equals(command, StringComparison.OrdinalIgnoreCase));
                }

                if (matchingModule is null) //Look for submodule but without complete module path with levenshtein
                {
                    matchingModule = modules.FirstOrDefault(x => x.Name.Equals(command.Levenshtein(modules.Select(x => x.Name).ToList()), StringComparison.OrdinalIgnoreCase));
                }

                if (matchingModule is null) //Remove last input
                {
                    var cmdArgs = command.Split(' ').ToList();
                    cmdArgs.RemoveAt(cmdArgs.Count - 1);

                    return HelpAsync(string.Join(' ', cmdArgs));
                }

                embed = new LocalEmbedBuilder
                {
                    Color = _configuration.EmbedColor,
                    Title = "Help",
                    Description = InternationalizationService.GetLocalization("help_module_description"),
                    Footer = new LocalEmbedFooterBuilder
                    {
                        Text = InternationalizationService.GetLocalization("help_module_footer", DbContext.User.Language, matchingModule.Commands.Count)
                    }
                };

                if (matchingModule.Submodules.Count > 0)
                {
                    embed.AddField("Modules", string.Join(", ", matchingModule.Submodules.DistinctBy(x => x.Name).Select(x => $"`{x.Name}`")));
                }

                if (matchingModule.Commands.Count > 0)
                {
                    embed.AddField("Commands", string.Join(", ", matchingModule.Commands.DistinctBy(x => x.Name).Select(x => $"`{x.Name}`")));
                }

                var moduleChecks = CommandUtilities.EnumerateAllChecks(matchingModule).Cast<AatroxCheckBaseAttribute>().ToArray();
                if (moduleChecks.Length > 0)
                {
                    embed.AddField("Requirements", string.Join("\n", moduleChecks.Select(x => $"`- {x.Name}{x.Details}`")));
                }

                return RespondAsync(embed.Build());
            }

            embed = new LocalEmbedBuilder
            {
                Color = _configuration.EmbedColor,
                Title = "Help"
            };

            var builder = new StringBuilder();
            foreach (var cmd in matchingCommands)
            {
                builder.AppendLine($"**{cmd.Command.Description ?? "Undocumented."}**");
                builder.AppendLine($"`{Context.Prefix}{cmd.Command.Name} {string.Join(" ", cmd.Command.Parameters.Select(x => $"[{x.Name}]"))}`".ToLowerInvariant());
                foreach (var param in cmd.Command.Parameters)
                {
                    builder.AppendLine($"`[{param.Name}]`: {param.Description ?? "Undocumented."}");
                }
                builder.AppendLine();
            }

            embed.AddField("Usages", builder.ToString());

            var defaultCmd = matchingCommands.FirstOrDefault().Command;

            var checks = CommandUtilities.EnumerateAllChecks(defaultCmd.Module).Cast<AatroxCheckBaseAttribute>().ToArray();
            if (checks.Length > 0)
            {
                embed.AddField($"Module Requirements", string.Join("\n", checks.Select(x => $"- `{x.Name}{x.Details}`")));
            }

            if (defaultCmd.Checks.Count > 0)
            {
                embed.AddField($"Command Requirements", string.Join("\n", defaultCmd.Checks.Cast<AatroxCheckBaseAttribute>().Select(x => $"- `{x.Name}{x.Details}`")));
            }

            return RespondAsync(embed: embed.Build());
        }
    }
}

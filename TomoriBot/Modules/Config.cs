using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TomoriBot.Core.Guilds;

namespace TomoriBot.Modules
{
	public class Config : ModuleBase<SocketCommandContext>
	{
		[Command("modenable")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task ModuleEnable(string moduleName)
		{
			var guild = Servers.GetServer(Context.Guild);
			bool success = true;

			switch (moduleName.ToLower())
			{
				case "moderation":
					guild.EnableModerationModule = true;
					break;
				case "profile":
				case "profiles":
					guild.EnableProfilesModule = true;
					break;
				case "economy":
					guild.EnableEconomyModule = true;
					break;
				case "fun":
					guild.EnableFunModule = true;
					break;
				case "nsfw":
					guild.EnableNsfwModule = true;
					break;
				default:
					success = false;
					await Context.Channel.SendMessageAsync("Unknown module! Please type help and then a command to get help");
					break;
			}

			Servers.SaveServers();

			if (success)
				await Context.Channel.SendMessageAsync($"Enabled {moduleName} module!");
		}
		[Command("moddisable")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task ModuleDisable(string moduleName)
		{
			var guild = Servers.GetServer(Context.Guild);
			bool success = true;

			switch (moduleName.ToLower())
			{
				case "moderation":
					guild.EnableModerationModule = false;
					break;
				case "profile":
				case "profiles":
					guild.EnableProfilesModule = false;
					break;
				case "economy":
					guild.EnableEconomyModule = false;
					break;
				case "fun":
					guild.EnableFunModule = false;
					break;
				case "nsfw":
					guild.EnableNsfwModule = false;
					break;
				default:
					success = false;
					await Context.Channel.SendMessageAsync("Unknown module! Please type help and then a command to get help");
					break;
			}

			Servers.SaveServers();

			if (success)
				await Context.Channel.SendMessageAsync($"Disabled {moduleName} module!");
		}

		[Command("lmods")]
		public async Task ListModules()
		{
			var embed = new EmbedBuilder();
			var guild = Servers.GetServer(Context.Guild);
			var msg = $"Moderation: **{guild.EnableModerationModule}**\n" +
			          $"Profiles: **{guild.EnableProfilesModule}**\n" +
			          $"Economy: **{guild.EnableEconomyModule}**\n" +
			          $"Fun: **{guild.EnableFunModule}**\n" +
			          $"Nsfw: **{guild.EnableNsfwModule}**";

			embed.Color = Color.Gold;
			embed.Title = "Command Modules";
			embed.Description = msg;

			await Context.Channel.SendMessageAsync("", embed: embed);
		}

		[Command("enablelvlupmsg")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task EnableLevelUpMessage()
		{
			Servers.GetServer(Context.Guild).EnableLevelUpMessages = true;
			await Context.Channel.SendMessageAsync($"Enabled level-up messages!");
		}
		[Command("disablelvlupmsg")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task DisableLevelUpMessage()
		{
			Servers.GetServer(Context.Guild).EnableLevelUpMessages = false;
			await Context.Channel.SendMessageAsync($"Disabled level-up messages!");
		}
	}
}

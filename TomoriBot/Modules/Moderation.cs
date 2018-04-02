using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using static TomoriBot.Utilities;

namespace TomoriBot.Modules
{
	public class Moderation : ModuleBase<SocketCommandContext>
	{
		[Command("nickname")]
		[Alias("nick")]
		[RequireUserPermission(GuildPermission.ManageNicknames)]
		public async Task Nickname(SocketGuildUser user, [Remainder]string newName)
		{
			await Context.Guild.GetUser(user.Id).ModifyAsync(x => { x.Nickname = newName; });
		}

		[Command("kick")]
		[RequireUserPermission(GuildPermission.KickMembers)]
		public async Task Kick(SocketGuildUser user)
		{
			await user.KickAsync();
			await Context.Channel.SendMessageAsync($"{GetNickname(user)} ({user.Id}) has been kicked!");
		}
		[Command("kick")]
		[RequireUserPermission(GuildPermission.KickMembers)]
		public async Task Kick(SocketGuildUser user, [Remainder]string reason)
		{
			await user.KickAsync(reason);
			await Context.Channel.SendMessageAsync($"{GetNickname(user)} ({user.Id}) has been kicked!");
		}

		[Command("ban")]
		[RequireUserPermission(GuildPermission.BanMembers)]
		public async Task Ban(SocketGuildUser user)
		{
			await Context.Guild.AddBanAsync(user);
			await Context.Channel.SendMessageAsync($"{GetNickname(user)} ({user.Id}) has been banned!");
		}
		[Command("ban")]
		[RequireUserPermission(GuildPermission.BanMembers)]
		public async Task Ban(SocketGuildUser user, [Remainder]string reason)
		{
			await Context.Guild.AddBanAsync(user, reason: reason);
			await Context.Channel.SendMessageAsync($"{GetNickname(user)} ({user.Id}) has been banned!");
		}
	}
}

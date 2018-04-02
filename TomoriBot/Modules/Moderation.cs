using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using static TomoriBot.Global;

namespace TomoriBot.Modules
{
	public class Moderation : ModuleBase<SocketCommandContext>
	{
		[Command("nick")]
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


		[Command("bean")]
		[RequireUserPermission(GuildPermission.ManageRoles)]
		public async Task Bean(SocketGuildUser user)
		{
			var ds = new DataStorage<string, ulong>("Storage/IDStorage.json");

			var beanedRoleId = ds.GetPair("BeanedRoleID");
			var role = Context.Guild.Roles.FirstOrDefault(x => x.Id == beanedRoleId);

			await (user as IGuildUser).AddRoleAsync(role);

			await Context.Channel.SendMessageAsync($"{GetNickname(user)} was beaned!");
		}

		[Command("unbean")]
		[RequireUserPermission(GuildPermission.ManageRoles)]
		public async Task Unbean(SocketGuildUser user)
		{
			var ds = new DataStorage<string, ulong>("Storage/IDStorage.json");

			var beanedRoleId = ds.GetPair("BeanedRoleID");
			var role = Context.Guild.Roles.FirstOrDefault(x => x.Id == beanedRoleId);

			await (user as IGuildUser).RemoveRoleAsync(role);

			await Context.Channel.SendMessageAsync($"{GetNickname(user)} was unbeaned!");
		}
	}
}

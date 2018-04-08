using System;
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
			if (!CheckModerationEnabled(Context).Result) return;

			await Context.Guild.GetUser(user.Id).ModifyAsync(x => { x.Nickname = newName; });
		}

		[Command("kick")]
		[RequireUserPermission(GuildPermission.KickMembers)]
		public async Task Kick(SocketGuildUser user)
		{
			if (!CheckModerationEnabled(Context).Result) return;

			await user.KickAsync();
			await Context.Channel.SendMessageAsync($"{GetNickname(user)} ({user.Id}) has been kicked!");
		}
		[Command("kick")]
		[RequireUserPermission(GuildPermission.KickMembers)]
		public async Task Kick(SocketGuildUser user, [Remainder]string reason)
		{
			if (!CheckModerationEnabled(Context).Result) return;

			await user.KickAsync(reason);
			await Context.Channel.SendMessageAsync($"{GetNickname(user)} ({user.Id}) has been kicked!");
		}

		[Command("ban")]
		[RequireUserPermission(GuildPermission.BanMembers)]
		public async Task Ban(SocketGuildUser user)
		{
			if (!CheckModerationEnabled(Context).Result) return;

			await Context.Guild.AddBanAsync(user);
			await Context.Channel.SendMessageAsync($"{GetNickname(user)} ({user.Id}) has been banned!");
		}
		[Command("ban")]
		[RequireUserPermission(GuildPermission.BanMembers)]
		public async Task Ban(SocketGuildUser user, [Remainder]string reason)
		{
			if (!CheckModerationEnabled(Context).Result) return;

			await Context.Guild.AddBanAsync(user, reason: reason);
			await Context.Channel.SendMessageAsync($"{GetNickname(user)} ({user.Id}) has been banned!");
		}

		[Command("mute")]
		[RequireUserPermission(ChannelPermission.ManagePermissions)]
		public async Task Mute(SocketGuildUser user)
		{
			if (!CheckModerationEnabled(Context).Result) return;

			var mutedRole = InitializeMute();

			// .Result can cause deadlock
			await user.AddRoleAsync(mutedRole.Result);
		}
		private async Task<IRole> InitializeMute()
		{
			var ds = new DataStorage<string, ulong>("Storage/IDStorage.json");

			var roles = from r in Context.Guild.Roles
				where r.Name == "Muted"
				select r;

			IRole mutedRole;
			OverwritePermissions perms = new OverwritePermissions(sendMessages: PermValue.Deny, readMessages: PermValue.Allow, addReactions: PermValue.Deny);
			if (!roles.Any())
			{
				var mutedPerms = new GuildPermissions();
				mutedPerms.Modify(sendMessages: false);
				mutedRole = await Context.Guild.CreateRoleAsync("Muted", mutedPerms, Color.LightGrey);
				ds.SetPair("MutedRole", mutedRole.Id);
			}
			else mutedRole = Context.Guild.GetRole(ds.GetPair("MutedRole"));

			foreach (var channel in Context.Guild.TextChannels)
			{
				foreach (var overwrite in channel.PermissionOverwrites)
				{
					// If the muted role already exists and has proper perms in the channel permissions, skip it
					if (overwrite.TargetId == mutedRole?.Id
					    && overwrite.Permissions.Equals(perms)) goto next;
				}
				await channel.AddPermissionOverwriteAsync(mutedRole, perms);
				next:;
			}

			return mutedRole;
		}
		[Command("unmute")]
		[RequireUserPermission(ChannelPermission.ManagePermissions)]
		public async Task Unmute(SocketGuildUser user)
		{
			if (!CheckModerationEnabled(Context).Result) return;

			var ds = new DataStorage<string, ulong>("Storage/IDStorage.json");
			var mutedRole = Context.Guild.GetRole(ds.GetPair("MutedRole"));

			await user.RemoveRoleAsync(Context.Guild.Roles.First(r => r.Id == mutedRole?.Id));
		}

		//[Command("bean")]
		//[RequireUserPermission(GuildPermission.ManageRoles)]
		//public async Task Bean(SocketGuildUser user)
		//{
		//	var ds = new DataStorage<string, ulong>("Storage/IDStorage.json");

		//	var beanedRoleId = ds.GetPair("BeanedRoleID");
		//	var role = Context.Guild.Roles.FirstOrDefault(x => x.Id == beanedRoleId);

		//	await (user as IGuildUser).AddRoleAsync(role);

		//	await Context.Channel.SendMessageAsync($"{GetNickname(user)} was beaned!");
		//}

		//[Command("unbean")]
		//[RequireUserPermission(GuildPermission.ManageRoles)]
		//public async Task Unbean(SocketGuildUser user)
		//{
		//	var ds = new DataStorage<string, ulong>("Storage/IDStorage.json");

		//	var beanedRoleId = ds.GetPair("BeanedRoleID");
		//	var role = Context.Guild.Roles.FirstOrDefault(x => x.Id == beanedRoleId);

		//	await (user as IGuildUser).RemoveRoleAsync(role);

		//	await Context.Channel.SendMessageAsync($"{GetNickname(user)} was unbeaned!");
		//}
	}
}

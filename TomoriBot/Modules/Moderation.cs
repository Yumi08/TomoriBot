﻿using System;
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

		[Command("mute")]
		[RequireUserPermission(ChannelPermission.ManagePermissions)]
		public async Task Mute(SocketGuildUser user)
		{
			await InitializeMute();

			var ds = new DataStorage<string, ulong>("Storage/IDStorage.json");
			var mutedRole = Context.Guild.GetRole(ds.GetPair("MutedRole"));

			new SocketCommandContext(CommandHandler.Client, Context.Message);
			await user.AddRoleAsync(Context.Guild.Roles.First(r => r.Id == mutedRole?.Id));
		}
		private async Task InitializeMute()
		{
			var ds = new DataStorage<string, ulong>("Storage/IDStorage.json");

			var roles = from r in Context.Guild.Roles
				where r.Name == "Muted"
				select r;

			IRole mutedRole;
			OverwritePermissions perms = new OverwritePermissions(sendMessages: PermValue.Deny, readMessages: PermValue.Allow);
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
				if (channel == null) continue;

				foreach (var overwrite in channel.PermissionOverwrites)
				{
					if (overwrite.TargetId == mutedRole?.Id) goto next;
				}
				await channel.AddPermissionOverwriteAsync(mutedRole, perms);
				next:;
				Console.WriteLine("End of InitializeMute()");
			}
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

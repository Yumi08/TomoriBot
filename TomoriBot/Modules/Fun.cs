using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using TomoriBot.Core;
using TomoriBot.Core.UserProfiles;

namespace TomoriBot.Modules
{
	public class Fun : ModuleBase<SocketCommandContext>
	{
		[Command("pick")]
		[Alias("choose")]
		public async Task Choose([Remainder]string input)
		{
			var args = input.Split(' ');

			await Context.Channel.SendMessageAsync(args[Global.R.Next(args.Length)]);
		}

		[Command("tag")]
		public async Task Tag(SocketUser user, [Remainder]string value)
		{
			var userAccount = UserAccounts.GetAccount(Context.User);

			userAccount.AddTag(user.Id, value);

			await Context.Channel.SendMessageAsync($"Tagged {user.Username} with \"{value}\"!");
		}

		[Command("tag")]
		public async Task Tag(SocketUser user)
		{
			var userAccount = UserAccounts.GetAccount(Context.User);

			var tag = userAccount.GetTag(user.Id, out bool success);

			if (!success)
				await Context.Channel.SendMessageAsync("No tag exists for specified user!");
			else
				await Context.Channel.SendMessageAsync(tag);
		}

		[Command("tags")]
		public async Task Tags()
		{
			var u = Context.Message.Author;
			var userAccount = UserAccounts.GetAccount(Context.User);
			var m = "";

			foreach (KeyValuePair<ulong, string> entry in userAccount.Tags)
			{
				m += $"<@{entry.Key}> - {entry.Value}\n";
			}

			await Discord.UserExtensions.SendMessageAsync(u, m);
		}

		[Command("x")]
		public async Task X([Remainder] string input)
		{
			await Context.Channel.SendMessageAsync($"{Context.User.Username} doubts that {input}!");
		}
	}
}

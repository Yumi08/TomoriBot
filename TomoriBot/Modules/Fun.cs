using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using TomoriBot.Core.UserProfiles;
using static TomoriBot.Utilities;

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
			var guildUser = (SocketGuildUser) user;

			userAccount.AddTag(user.Id, value);

			await Context.Channel.SendMessageAsync($"Tagged {GetNickname(guildUser)} with \"{value}\"!");
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
			var guildContextUser = (SocketGuildUser) Context.User;

			await Context.Channel.SendMessageAsync($"{GetNickname(guildContextUser)} doubts that {input}!");
		}

		[Command("rps")]
		[Alias("rockpaperscissors")]
		public async Task Rps(SocketUser user)
		{
			var guildContextUser = (SocketGuildUser) Context.User;
			var guildUser = (SocketGuildUser) user;

			string[] hands = {":fist:", ":hand_splayed:", ":v:"};
			int user1Hand = Global.R.Next(3);
			int user2Hand = Global.R.Next(3);
			SocketGuildUser winner;

			var m = await Context.Channel.SendMessageAsync($"Throwing... [{GetNickname(guildContextUser)} - {hands[1]}]" +
			                                               $" vs [{GetNickname(guildUser)} - {hands[1]}]");
			await Task.Delay(2000);

			if (user1Hand == user2Hand)
			{
				await m.ModifyAsync(msg => msg.Content = $"It's a tie!\n" +
				                                         $"[{GetNickname(guildContextUser)} - {hands[user1Hand]}]" +
				                                         $" vs [{GetNickname(guildUser)} - {hands[user2Hand]}]");
				return;
			}

			if (user1Hand == 0 && user2Hand == 1) winner = guildUser;
			else if (user1Hand == 1 && user2Hand == 2) winner = guildUser;
			else if (user1Hand == 2 && user2Hand == 0) winner = guildUser;
			else winner = guildContextUser;

			await m.ModifyAsync(msg => msg.Content = $"{GetNickname(winner)} won!\n" +
													 $"[{GetNickname(guildContextUser)} - {hands[user1Hand]}]" +
			                                         $" vs [{GetNickname(guildUser)} - {hands[user2Hand]}]");
		}

		[Command("iq")]
		[Alias("intelligence")]
		public async Task Intelligence()
		{
			var guildUser = (SocketGuildUser) Context.User;

			ushort iq = 0;
			while (Global.R.Next(101) != 0) iq++;

			await Context.Channel.SendMessageAsync($"{GetNickname(guildUser)} has an IQ of {iq}!");
		}
	}
}

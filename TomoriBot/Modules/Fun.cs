using System;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
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
		public async Task Tag(SocketGuildUser user, [Remainder]string value)
		{
			var userAccount = UserAccounts.GetAccount(Context.User);

			userAccount.AddTag(user.Id, value);

			await Context.Channel.SendMessageAsync($"Tagged {GetNickname(user)} with \"{value}\"!");
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

		[Command("cleartag")]
		[Alias("removetag")]
		public async Task ClearTag(SocketGuildUser user)
		{
			var userAccount = UserAccounts.GetAccount(Context.User);

			userAccount.RemoveTag(user.Id);

			await Context.Channel.SendMessageAsync($"Removed tag for {GetNickname(user)}");
		}

		[Command("tags")]
		public async Task Tags()
		{
			var u = Context.Message.Author;
			var userAccount = UserAccounts.GetAccount(Context.User);
			var m = "";

			foreachBegin:
			foreach (KeyValuePair<ulong, string> entry in userAccount.Tags)
			{
				string userName;
				try
				{
					userName = GetNickname(Context.Guild.GetUser(entry.Key));
				}
				catch (NullReferenceException)
				{
					userAccount.RemoveTag(entry.Key);
					// SLOW SOLUTION TO PROBLEM (try to use for loop).
					goto foreachBegin;
				}

				m += $"{userName} - {entry.Value}\n";
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
		public async Task Rps(SocketGuildUser user)
		{
			if (user == Context.User)
			{
				await Context.Channel.SendMessageAsync("You can't play with yourself! ~~that sounds so lewd~~");
				return;
			}

			var guildContextUser = (SocketGuildUser) Context.User;

			string[] hands = {":fist:", ":hand_splayed:", ":v:"};
			int user1Hand = Global.R.Next(3);
			int user2Hand = Global.R.Next(3);
			SocketGuildUser winner;

			var m = await Context.Channel.SendMessageAsync($"Throwing... [{GetNickname(guildContextUser)} - {hands[1]}]" +
			                                               $" vs [{GetNickname(user)} - {hands[1]}]");
			await Task.Delay(2000);

			if (user1Hand == user2Hand)
			{
				await m.ModifyAsync(msg => msg.Content = $"It's a tie!\n" +
				                                         $"[{GetNickname(guildContextUser)} - {hands[user1Hand]}]" +
				                                         $" vs [{GetNickname(user)} - {hands[user2Hand]}]");
				return;
			}

			if (user1Hand == 0 && user2Hand == 1) winner = user;
			else if (user1Hand == 1 && user2Hand == 2) winner = user;
			else if (user1Hand == 2 && user2Hand == 0) winner = user;
			else winner = guildContextUser;

			await m.ModifyAsync(msg => msg.Content = $"{GetNickname(winner)} won!\n" +
													 $"[{GetNickname(guildContextUser)} - {hands[user1Hand]}]" +
			                                         $" vs [{GetNickname(user)} - {hands[user2Hand]}]");
		}

		[Command("iq")]
		[Alias("intelligence")]
		public async Task Intelligence()
		{
			var guildContextUser = (SocketGuildUser) Context.User;
			var userAccount = UserAccounts.GetAccount(Context.User);

			if (userAccount.Iq == 0)
			{
				ushort iq = (ushort) Global.R.Next(1, 200);
				userAccount.Iq = iq;
				await Context.Channel.SendMessageAsync($"{GetNickname(guildContextUser)} has an IQ of {iq}!");
			}
			else await Context.Channel.SendMessageAsync($"{GetNickname(guildContextUser)} has an IQ of {userAccount.Iq}!");
		}

		[Command("iq")]
		[Alias("intelligence")]
		public async Task Intelligence(SocketGuildUser user)
		{
			var userAccount = UserAccounts.GetAccount(user);

			if (userAccount.Iq == 0)
			{
				ushort iq = (ushort) Global.R.Next(1, 200);
				userAccount.Iq = iq;
				await Context.Channel.SendMessageAsync($"{GetNickname(user)} has an IQ of {iq}!");
			}
			else await Context.Channel.SendMessageAsync($"{GetNickname(user)} has an IQ of {userAccount.Iq}!");
		}

		[Command("smartest")]
		public async Task Smartest()
		{
			var accList = UserAccounts.GetAccountList();
			int maxIq = accList.Max(t => t.Iq);
			var smartest = from a in accList
				where a.Iq == maxIq
				select a;

			string msg = "";
			foreach (var userAccount in smartest.ToList())
			{
				msg += $"- {GetNickname(Context.Guild.GetUser(userAccount.Id))}\n";
			}

			msg += $"With an IQ of {maxIq}";

			await Context.Channel.SendMessageAsync(msg);
		}
	}
}

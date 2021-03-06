﻿using System;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Newtonsoft.Json;
using TomoriBot.Core.UserProfiles;
using static TomoriBot.Global;

namespace TomoriBot.Modules
{
	public class Fun : ModuleBase<SocketCommandContext>
	{
		[Command("pick")]
		[Alias("choose")]
		public async Task Choose([Remainder] string input)
		{
			if (!CheckFunEnabled(Context).Result) return;

			var args = input.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

			var m = await Context.Channel.SendMessageAsync("Hmm.....");

			await Task.Delay(1000);

			await m.ModifyAsync(x => x.Content = args[R.Next(args.Length)]);
		}

		[Command("rate")]
		public async Task Rate([Remainder] string input)
		{
			var ds = new DataStorage<string, ushort>("Storage/RateStringStorage.json");
			ushort value = 0;

			if (!ds.PairExists(input.ToLower()))
			{
				value = (ushort)R.Next(11);
				ds.SetPair(input.ToLower(), value);
			}
			else
			{
				value = ds.GetPair(input.ToLower());
			}

			await Context.Channel.SendMessageAsync($"I give {input} a {value}/10!");
		}

		[Command("rate")]
		public async Task Rate(SocketGuildUser targetUser)
		{
			var ds = new DataStorage<ulong, ushort>("Storage/RateUserStorage.json");
			ushort value = 0;

			if (!ds.PairExists(targetUser.Id))
			{
				value = (ushort)R.Next(11);
				ds.SetPair(targetUser.Id, value);
			}
			else
			{
				value = ds.GetPair(targetUser.Id);
			}

			await Context.Channel.SendMessageAsync($"I give {GetNickname(targetUser)} a {value}/10!");
		}

		[Command("amt")]
		public async Task Amt(SocketGuildUser targetUser, [Remainder] string thing)
		{
			var ds = new DataStorage<AmtObject>("Storage/AmtStorage.json");
			var amt = new AmtObject { User = targetUser, Thing = thing.ToLower() };
			ushort percentage = 0;

			var listOfMatching = from a in ds.ReturnList()
				where a.Equals(amt)
				select a;
			bool exists = listOfMatching.Any();

			// If the object doesn't already exist in storage
			if (!exists)
			{
				percentage = (ushort)R.Next(101);
				amt.Value = percentage;
				ds.Add(amt);
			}

			var obj = from a in ds.ReturnList()
				where a.Equals(amt)
				select a.Value;
			percentage = obj.FirstOrDefault();

			await Context.Channel.SendMessageAsync($"{GetNickname(targetUser)} is {percentage}% {thing}");
		}

		private struct AmtObject
		{
			/// <summary>
			/// The target user
			/// </summary>
			public SocketGuildUser User { get; set; }

			/// <summary>
			/// The thing that the user is an amount of
			/// </summary>
			public string Thing { get; set; }

			/// <summary>
			/// The amount the user is of a thing
			/// </summary>
			public ushort Value { get; set; }

			public bool Equals(AmtObject obj)
			{
				return User == obj.User && Thing == obj.Thing;
			}
		}

		[Command("tag")]
		public async Task Tag(SocketGuildUser user, [Remainder] string value)
		{
			if (!CheckFunEnabled(Context).Result) return;

			var userAccount = UserAccounts.GetAccount(Context.User);

			userAccount.AddTag(user.Id, value);

			await Context.Channel.SendMessageAsync($"Tagged {GetNickname(user)} with \"{value}\"!");
		}

		[Command("tag")]
		public async Task Tag(SocketUser user)
		{
			if (!CheckFunEnabled(Context).Result) return;

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
			if (!CheckFunEnabled(Context).Result) return;

			var userAccount = UserAccounts.GetAccount(Context.User);

			userAccount.RemoveTag(user.Id);

			await Context.Channel.SendMessageAsync($"Removed tag for {GetNickname(user)}");
		}

		[Command("tags")]
		public async Task Tags()
		{
			if (!CheckFunEnabled(Context).Result) return;

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
			if (!CheckFunEnabled(Context).Result) return;

			var guildContextUser = (SocketGuildUser) Context.User;

			await Context.Channel.SendMessageAsync($"{GetNickname(guildContextUser)} doubts that {input}!");
		}

		[Command("rps")]
		[Alias("rockpaperscissors")]
		public async Task Rps(SocketGuildUser user)
		{
			if (!CheckFunEnabled(Context).Result) return;

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
			if (!CheckFunEnabled(Context).Result) return;

			var guildContextUser = (SocketGuildUser) Context.User;
			var userAccount = UserAccounts.GetAccount(Context.User);

			if (userAccount.Iq == 0)
			{
				ushort iq = (ushort) Global.R.Next(1, 200);
				userAccount.Iq = iq;
				await Context.Channel.SendMessageAsync($"{GetNickname(guildContextUser)} has an IQ of {iq}!");

				UserAccounts.SaveAccounts();
			}
			else await Context.Channel.SendMessageAsync($"{GetNickname(guildContextUser)} has an IQ of {userAccount.Iq}!");
		}

		[Command("iq")]
		[Alias("intelligence")]
		public async Task Intelligence(SocketGuildUser user)
		{
			if (!CheckFunEnabled(Context).Result) return;

			var userAccount = UserAccounts.GetAccount(user);

			if (userAccount.Iq == 0)
			{
				ushort iq = (ushort) Global.R.Next(1, 200);
				userAccount.Iq = iq;
				await Context.Channel.SendMessageAsync($"{GetNickname(user)} has an IQ of {iq}!");
			}
			else await Context.Channel.SendMessageAsync($"{GetNickname(user)} has an IQ of {userAccount.Iq}!");
		}

		[Command("picklesize")]
		public async Task PickleSize()
		{
			if (!CheckFunEnabled(Context).Result) return;

			var userAccount = UserAccounts.GetAccount(Context.User);

			if (userAccount.PickleSize.Equals(0f))
			{
				userAccount.PickleSize = (float) Math.Round(NextFloat(0.01f, 12f), 2);
				await Context.Channel.SendMessageAsync(
					$"{GetNickname((SocketGuildUser) Context.User)} has a pickle size of {userAccount.PickleSize}in!");

				UserAccounts.SaveAccounts();
			}
			else
				await Context.Channel.SendMessageAsync(
					$"{GetNickname((SocketGuildUser) Context.User)} has a pickle size of {userAccount.PickleSize}in!");
		}

		[Command("picklesize")]
		public async Task PickleSize(SocketGuildUser user)
		{
			if (!CheckFunEnabled(Context).Result) return;

			var userAccount = UserAccounts.GetAccount(user);
			if (userAccount.PickleSize.Equals(0f))
			{
				userAccount.PickleSize = (float) Math.Round(NextFloat(0.01f, 12f), 2);
				await Context.Channel.SendMessageAsync(
					$"{GetNickname(user)} has a pickle size of {userAccount.PickleSize}in!");

				UserAccounts.SaveAccounts();
			}
			else
				await Context.Channel.SendMessageAsync(
					$"{GetNickname(user)} has a pickle size of {userAccount.PickleSize}in!");
		}

		[Command("smartest")]
		public async Task Smartest()
		{
			if (!CheckFunEnabled(Context).Result) return;

			var accList = UserAccounts.GetAccountList();

			var accEnumerable = from a in accList
				where Context.Guild.GetUser(a.Id) != null
				select a;

			var guildAccList = accEnumerable.ToList().OrderByDescending(o => o.Iq).ToList();

			var embed = new EmbedBuilder
			{
				Color = Color.Green
			};
			for (var i = 0; i < 5; i++)
			{
				embed.Description +=
					$"{1 + i}. {GetNickname(Context.Guild.GetUser(guildAccList[i].Id))} - {guildAccList[i].Iq} IQ\n";
			}

			await Context.Channel.SendMessageAsync("", embed: embed);
		}

		[Command("dumbest")]
		public async Task Dumbest()
		{
			if (!CheckFunEnabled(Context).Result) return;

			var accList = UserAccounts.GetAccountList();

			var accEnumerable = from a in accList
				where Context.Guild.GetUser(a.Id) != null && a.Iq != 0
				select a;

			var guildAccList = accEnumerable.ToList().OrderBy(o => o.Iq).ToList();

			var embed = new EmbedBuilder
			{
				Color = Color.Green
			};
			for (var i = 0; i < 5; i++)
			{
				embed.Description +=
					$"{1 + i}. {GetNickname(Context.Guild.GetUser(guildAccList[i].Id))} - {guildAccList[i].Iq} IQ\n";
			}

			await Context.Channel.SendMessageAsync("", embed: embed);
		}

		private class FishEmote : IWeighted
		{
			public int Weight { get; set; }
			public string Name { get; set; }
		}

		//private readonly SemaphoreSlim _sem = new SemaphoreSlim(1, 1);

		//[Command("resetstat")]
		//public async Task ResetStat([Remainder] string stat)
		//{
		//	if (!CheckFunEnabled(Context).Result) return;

		//	switch (stat.ToLower())
		//	{
		//		case "iq":
		//		case "intelligence":
		//			await Context.Channel.SendMessageAsync("Are you sure you want to reset your IQ for ¥1000?");
		//			await WaitForInput(Context, UserAccount.Stat.Iq);
		//			break;

		//		case "pickle":
		//		case "picklesize":
		//			await Context.Channel.SendMessageAsync("Are you sure you want to reset your pickle size for ¥1000?");
		//			await WaitForInput(Context, UserAccount.Stat.Picklesize);
		//			break;

		//		default:
		//			await Context.Channel.SendMessageAsync("Unknown stat! See help for usage.");
		//			return;
		//	}
		//}

		//public async Task WaitForInput(SocketCommandContext context, UserAccount.Stat stat)
		//{
		//	var userAccount = UserAccounts.GetAccount(context.User);

		//	string receivedMsg = null;
		//	CommandHandler.MessageReceived += (sender, e) =>
		//	{
		//		receivedMsg = e.Msg.Content;
		//	};

		//	await Task.Delay(5000);
		//	if (receivedMsg == null)
		//	{
		//		await Context.Channel.SendMessageAsync("Cancelling command!");
		//		return;
		//	}

		//	if (receivedMsg.ToLower() == "y")
		//	{
		//		userAccount.PickleSize = (float)Math.Round(NextFloat(0.01f, 12f), 2);
		//		switch (stat)
		//		{
		//			case UserAccount.Stat.Iq:
		//				ushort iq = (ushort) R.Next(1, 200);
		//				userAccount.Iq = iq;
		//				await Context.Channel.SendMessageAsync(
		//					$"{GetNickname((SocketGuildUser)context.User)} has an IQ of {userAccount.Iq}!");
		//				break;
		//			case UserAccount.Stat.Picklesize:
		//				userAccount.PickleSize = (float)Math.Round(NextFloat(0.01f, 12f), 2);
		//				await Context.Channel.SendMessageAsync(
		//					$"{GetNickname((SocketGuildUser)context.User)} has a pickle size of {userAccount.PickleSize}in!");
		//				break;
		//		}

		//		userAccount.Yen -= 1000;
		//		UserAccounts.SaveAccounts();
		//	}
		//}

		[Command("fishy")]
		public async Task Fishy()
		{
			if (!CheckFunEnabled(Context).Result) return;

			var fishEmotes = new List<FishEmote>()
			{
				new FishEmote {Name = ":octopus:", Weight = 5},
				new FishEmote {Name = ":crab:", Weight = 20},
				new FishEmote {Name = ":fish:", Weight = 70},
				new FishEmote {Name = ":tropical_fish:", Weight = 50},
				new FishEmote {Name = ":blowfish:", Weight = 30},
				new FishEmote {Name = ":shark:", Weight = 10},
				new FishEmote {Name = ":shrimp:", Weight = 20},
				new FishEmote {Name = ":dolphin:", Weight = 3},
				new FishEmote {Name = ":whale:", Weight = 1},
				new FishEmote {Name = ":squid:", Weight = 15},
				new FishEmote {Name = ":whale2:", Weight = 1}
			};

			FishEmote randomEmote = WeightedRandomization.Choose(fishEmotes);

			await Context.Channel.SendMessageAsync(randomEmote.Name);
		}

		/// <summary>
		/// Gets a random neko image from the internet
		/// </summary>
		/// <returns></returns>
		[Command("neko")]
		public async Task Neko()
		{
			if (!CheckFunEnabled(Context).Result) return;

			string json;
			using (var client = new WebClient())
			{
				json = client.DownloadString("https://nekos.life/api/neko");
			}

			var _object = JsonConvert.DeserializeObject<dynamic>(json);
			await Context.Channel.SendMessageAsync(_object.neko.ToString());
		}

		//[Command("spamkanna")]
		//public Task SpamKanna(string input)
		//{
		//	if (input == "true") Global.SpamKanna = true;
		//	else Global.SpamKanna = false;

		//	return Task.CompletedTask;
		//}
	}
}

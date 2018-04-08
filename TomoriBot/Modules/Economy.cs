using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TomoriBot.Core;
using TomoriBot.Core.Guilds;
using TomoriBot.Core.UserProfiles;
using static TomoriBot.Global;

namespace TomoriBot.Modules
{
	public class Economy : ModuleBase<SocketCommandContext>
	{
		[Command("transfer")]
		[Alias("give")]
		public async Task Transfer(SocketGuildUser recipient, uint amt)
		{
			if (!CheckEconomyEnabled(Context).Result) return;

			var callerAccount = UserAccounts.GetAccount(Context.User);

			if (await CheckEnoughMoney(amt, callerAccount)) return;

			if (Context.User.Id == recipient.Id)
			{
				await Context.Channel.SendMessageAsync("Silly! You can't send money to yourself!");
				return;
			}

			var recipientAccount = UserAccounts.GetAccount(recipient);

			callerAccount.Yen -= amt;
			recipientAccount.Yen += amt;

			await Context.Channel.SendMessageAsync($"{GetNickname(recipient)} has been given ¥{amt} by {GetNickname((SocketGuildUser)Context.User)}!");
		}

		[Command("bet")]
		public async Task Bet(uint amt)
		{
			if (!CheckEconomyEnabled(Context).Result) return;

			var userAccount = UserAccounts.GetAccount(Context.User);

			if (await CheckEnoughMoney(amt, userAccount)) return;


			userAccount.Yen -= amt;

			if (Global.R.Next(2) == 1)
			{
				await Context.Channel.SendMessageAsync($"Congratulations! You just won ¥{amt * 2}!");
				userAccount.Yen += amt * 2;
				return;
			}

			await Context.Channel.SendMessageAsync("Oh no! I'm afraid you lost...");
		}

		[Command("dump")]
		[Alias("trash")]
		public async Task Dump(uint amt)
		{
			if (!CheckEconomyEnabled(Context).Result) return;

			var userAccount = UserAccounts.GetAccount(Context.User);

			if (await CheckEnoughMoney(amt, userAccount)) return;

			userAccount.Yen -= amt;

			await Context.Channel.SendMessageAsync($"Threw away ¥{amt}!");
		}

		[Command("bury")]
		public async Task Bury(uint amt)
		{
			if (!CheckEconomyEnabled(Context).Result) return;

			var userAccount = UserAccounts.GetAccount(Context.User);

			if (await CheckEnoughMoney(amt, userAccount)) return;

			var server = Servers.GetServer(Context.Guild);
			server.Buried += amt;

			userAccount.Yen -= amt;

			// Sends and deletes the message
			var m = await Context.Channel.SendMessageAsync($"Buried ¥{amt}!");
			await Task.Delay(1200);
			await m.DeleteAsync();
			await Context.Message.DeleteAsync();

			Servers.SaveServers();
		}

		[Command("unbury")]
		public async Task Unbury()
		{
			if (!CheckEconomyEnabled(Context).Result) return;

			var userAccount = UserAccounts.GetAccount(Context.User);

			var server = Servers.GetServer(Context.Guild);
			var buried = server.Buried;

			if (buried == 0)
			{
				await Context.Channel.SendMessageAsync("There is nothing buried!");
				return;
			}

			userAccount.Yen += buried;
			await Context.Channel.SendMessageAsync($"{GetNickname((SocketGuildUser)Context.User)} unburied ¥{buried}!");
			server.Buried = 0;

			Servers.SaveServers();
		}

		// TIMING SYSTEM DOESN'T WORK, HOUR GOES UP INSTEAD OF DOWN.
		[Command("daily")]
		public async Task Daily()
		{
			if (!CheckEconomyEnabled(Context).Result) return;

			var userAccount = UserAccounts.GetAccount(Context.User);

			if (Math.Abs(DateTime.Today.Day - userAccount.PreviousDailyTime.Day) < 1)
			{
				await Context.Channel.SendMessageAsync(
					$"Please wait {Math.Abs(24 - DateTime.Now.Hour)} more hours!");
				return;
			}

			if (Math.Abs(DateTime.Now.Day - userAccount.PreviousDailyTime.Day) > 1)
			{
				userAccount.PreviousDailyAmount = 500;
				userAccount.DailyStreak = 0;
			}

			if (userAccount.PreviousDailyAmount == 0) userAccount.PreviousDailyAmount = 500;

			uint amt = userAccount.PreviousDailyAmount += userAccount.PreviousDailyAmount / 35;
			userAccount.Yen += amt;
			userAccount.DailyStreak++;

			await Context.Channel.SendMessageAsync($"You received ¥{amt} for the day!\n" +
			                                       $"**Streak:** {userAccount.DailyStreak}");
			userAccount.PreviousDailyTime = DateTime.Now;
		}

		//[Command("!clearlast")]
		//[RequireUserPermission(GuildPermission.Administrator)]
		//public Task CLearlast(int days)
		//{
		//	var userAccount = UserAccounts.GetAccount(Context.User);

		//	userAccount.PreviousDailyTime = DateTime.Today.AddDays(-days);

		//	return Task.CompletedTask;
		//}

		[Command("totalyen")]
		public async Task TotalYen()
		{
			if (!CheckEconomyEnabled(Context).Result) return;

			var users = UserAccounts.GetAccountList();

			var yen = 0u;
			foreach (var user in users)
			{
				yen += user.Yen;
			}

			await Context.Channel.SendMessageAsync(
				$"¥{yen} total yen is distributed ~~equally~~ among {UserAccounts.UserAccountCount()} users.");
		}



		private async Task<bool> CheckEnoughMoney(uint amt, UserAccount userAccount)
		{
			if (amt > userAccount.Yen)
			{
				await Context.Channel.SendMessageAsync("Sorry! You don't have that much money!");
				return true;
			}

			return false;
		}
	}
}
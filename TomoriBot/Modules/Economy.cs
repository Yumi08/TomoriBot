using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TomoriBot.Core;
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
			var userAccount = UserAccounts.GetAccount(Context.User);

			if (await CheckEnoughMoney(amt, userAccount)) return;

			userAccount.Yen -= amt;

			await Context.Channel.SendMessageAsync($"Threw away ¥{amt}!");
		}

		[Command("bury")]
		public async Task Bury(uint amt)
		{
			var userAccount = UserAccounts.GetAccount(Context.User);

			if (await CheckEnoughMoney(amt, userAccount)) return;

			var ds = new DataStorage<string, uint>("Storage/DataStorage.json");
			ds.SetPair("Buried", amt + ds.GetOrCreatePair("Buried"));

			userAccount.Yen -= amt;

			// Sends and deletes the message
			var m = await Context.Channel.SendMessageAsync($"Buried ¥{amt}!");
			await Task.Delay(1200);
			await m.DeleteAsync();
			await Context.Message.DeleteAsync();
		}

		[Command("unbury")]
		public async Task Unbury()
		{
			var userAccount = UserAccounts.GetAccount(Context.User);

			var ds = new DataStorage<string,uint>("Storage/DataStorage.json");
			var buried = ds.GetOrCreatePair("Buried");

			if (buried == 0)
			{
				await Context.Channel.SendMessageAsync("There is nothing buried!");
				return;
			}

			userAccount.Yen += buried;
			await Context.Channel.SendMessageAsync($"{GetNickname((SocketGuildUser)Context.User)} unburied ¥{buried}!");
			ds.SetPair("Buried", 0);
		}

		// TIMING SYSTEM DOESN'T WORK, HOUR GOES UP INSTEAD OF DOWN.
		[Command("daily")]
		public async Task Daily()
		{
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

		[Command("!clearlast")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public Task CLearlast(int days)
		{
			var userAccount = UserAccounts.GetAccount(Context.User);

			userAccount.PreviousDailyTime = DateTime.Today.AddDays(-days);

			return Task.CompletedTask;
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
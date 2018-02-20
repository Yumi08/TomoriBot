using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TomoriBot.Core.UserProfiles;

namespace TomoriBot.Modules
{
	public class Experimental : ModuleBase<SocketCommandContext>
	{
		// TODO: GAMBLING SYSTEM

		[Command("inventory")]
		public async Task Inventory()
		{
			var account = UserAccounts.GetAccount(Context.User);
			await Context.Channel.SendMessageAsync($":potato: Potatoes : {account.Potatoes}" +
			                                       $"\n:fish: Fish : {account.Fish}");
		}

		[Command("inventory")]
		public async Task Inventory(SocketUser user)
		{
			var account = UserAccounts.GetAccount(user);
			await Context.Channel.SendMessageAsync($":potato: Potatoes : {account.Potatoes}" +
			                                       $"\n:fish: Fish : {account.Fish}");
		}

		[Command("buy")]
		public async Task Buy([Remainder] string item)
		{
			var account = UserAccounts.GetAccount(Context.User);
			uint potatoCost = 5;
			uint fishCost = 9;

			switch (item)
			{
				case "fish":
					account.Fish += 1;
					account.Yen -= fishCost;
					await Context.Channel.SendMessageAsync($":fish: You bought a fish for ¥{fishCost}!");
					break;
				case "potato":
					account.Potatoes += 1;
					account.Yen -= potatoCost;
					await Context.Channel.SendMessageAsync($":potato: You bought a potato for ¥{potatoCost}!");
					break;
				default:
					await Context.Channel.SendMessageAsync("I only sell potatoes and fishies! (buy ``potato/fish``)");
					break;
			}

			UserAccounts.SaveAccounts();
		}

		[Command("buy")]
		public async Task Buy(uint amt, [Remainder] string item)
		{
			var account = UserAccounts.GetAccount(Context.User);
			uint potatoCost = 5;
			uint fishCost = 9;

			if (amt == 0)
			{
				await Context.Channel.SendMessageAsync("You can't buy nothing!");
				return;
			}

			if (amt == 1)
			{
				Buy(item);
				return;
			}

			switch (item)
			{
				case "fish":
					account.Fish += amt;
					account.Yen -= fishCost * amt;
					await Context.Channel.SendMessageAsync($":fish: You bought {amt} fish for ¥{fishCost * amt}!");
					break;
				case "potato":
					account.Potatoes += amt;
					account.Yen -= potatoCost * amt;
					await Context.Channel.SendMessageAsync($":potato: You bought {amt} potatoes for ¥{potatoCost * amt}!");
					break;
				default:
					await Context.Channel.SendMessageAsync("I only sell potatoes and fishies! (buy ``potato/fish``)");
					break;
			}

			UserAccounts.SaveAccounts();
		}

		[Command("stats")]
		public async Task stats(SocketUser user)
		{
			var account = UserAccounts.GetAccount(user);
			var embed = new EmbedBuilder();
			embed.WithColor(220, 20, 60);
			embed.WithTitle($"{user.Username}'s Stats");
			embed.WithDescription($"XP: {account.Experience}, Level: {account.LevelNumber}, Money ¥{account.Yen}");
			await Context.Channel.SendMessageAsync("", embed: embed);
		}

		[Command("stats")]
		public async Task stats()
		{
			var account = UserAccounts.GetAccount(Context.User);
			var embed = new EmbedBuilder();
			embed.WithColor(220, 20, 60);
			embed.WithTitle($"{Context.User.Username}'s Stats");
			embed.WithDescription($"XP: {account.Experience}, Level: {account.LevelNumber}, Money ¥{account.Yen}");
			await Context.Channel.SendMessageAsync("", embed: embed);
		}

		[Command("hello")]
		public async Task Hello()
		{
			await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREET_&USERNAME_&BOTNAME",
				Context.User.Username, Config.bot.botName));
		}

		[Command("exchange")]
		public async Task Exchange(uint amt)
		{
			var account = UserAccounts.GetAccount(Context.User);
			uint _yen = (uint)(amt / 13.5);
			if (account.Experience < amt)
			{
				Context.Channel.SendMessageAsync("You don't have enough XP!");
				return;
			}
			account.Experience -= amt;
			account.Yen += _yen;
			UserAccounts.SaveAccounts();
			await Context.Channel.SendMessageAsync($"You just exchanged {amt}XP for ¥{_yen}");
		}

		[Command("exchangemax")]
		public async Task ExchangeMax()
		{
			var account = UserAccounts.GetAccount(Context.User);
			uint _yen = (uint)(account.Experience / 13.5);
			await Context.Channel.SendMessageAsync($"You just exchanged {account.Experience}XP for ¥{_yen}");
			
			account.Experience -= account.Experience;
			account.Yen += _yen;

			UserAccounts.SaveAccounts();
		}

		[Command("echo")]
		public async Task Echo([Remainder]string msg)
		{
			await Context.Channel.SendMessageAsync(msg);
		}

		[Command("hug")]
		public async Task Hug()
		{
			await Context.Channel.SendFileAsync("C:/Users/Yumi/Pictures/389F9492-2C33-4C49-9BDD-9FA92A686DCC.gif");
		}

		[Command("data")]
		public async Task GetData()
		{
			await Context.Channel.SendMessageAsync("Data contains " + DataStorage.GetPairCount() + " pairs");
		}

		[Command("addpair")]
		public void AddPair()
		{
			DataStorage.AddPairToStorage(Context.User.Username, "Name");
		}

		[Command("myXP")]
		public async Task MyXp()
		{
			var account = UserAccounts.GetAccount(Context.User);
			await Context.Channel.SendMessageAsync($"You have {account.Experience}XP.");
		}

		[Command("myMoney")]
		public async Task MyMoney()
		{
			var account = UserAccounts.GetAccount(Context.User);
			await Context.Channel.SendMessageAsync($"You have ¥{account.Yen}.");
		}
	}
}

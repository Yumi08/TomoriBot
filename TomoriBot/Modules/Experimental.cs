using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TomoriBot.Core.UserProfiles;

namespace TomoriBot.Modules
{
	public class Experimental : ModuleBase<SocketCommandContext>
	{
		[Command("eat")]
		public async Task Eat(uint amount, [Remainder] string input)
		{
			var account = UserAccounts.GetAccount(Context.User);

			Global.Food food = Global.Food.Potato;

			switch (input.ToLower())
			{
				case "potato":
				case "potatoes":
					if (account.Potatoes == 0)
					{
						await Context.Channel.SendMessageAsync("You don't have any potatoes!");
						return;
					}
					if (account.Potatoes < amount)
					{
						
						await Context.Channel.SendMessageAsync($"You only have {account.Potatoes} potatoes!");
						return;
					}

					food = Global.Food.Potato;
					account.Potatoes -= amount;
					break;

				case "fish":
					if (account.Fish == 0)
					{
						await Context.Channel.SendMessageAsync("You don't have any fish!");
						return;
					}
					if (account.Fish < amount)
					{
						
						await Context.Channel.SendMessageAsync($"You only have {account.Fish} fish!");
						return;
					}

					food = Global.Food.Fish;
					account.Fish -= amount;
					break;

				default:
					await Context.Channel.SendMessageAsync("I don't know what food that is!");
					break;
			}

			await Context.Channel.SendMessageAsync($"{Context.User.Username} just ate {amount} :{Global.FoodToString(food)}:");

			
		}

		[Command("transfer")]
		[Alias("give")]
		public async Task Transfer(uint amt, SocketUser _recipient)
		{
			var account = UserAccounts.GetAccount(Context.User);
			if (amt > account.Yen)
			{
				await Context.Channel.SendMessageAsync("You don't have that much money!");
				return;
			}

			if (_recipient.Id == Context.User.Id)
			{
				await Context.Channel.SendMessageAsync("You can't send money to yourself!");
				return;
			}

			var recipient = UserAccounts.GetAccount(_recipient);

			account.Yen -= amt;
			recipient.Yen += amt;

			await Context.Channel.SendMessageAsync($"{_recipient.Mention} has been given ¥{amt} by {Context.User.Username}!");
		}

		[Command("bet")]
		public async Task Bet(uint amt)
		{
			var account = UserAccounts.GetAccount(Context.User);
			if (amt > account.Yen)
			{
				await Context.Channel.SendMessageAsync("You don't have that much money!");
				return;
			}

			account.Yen -= amt;
			if (Global.R.Next(1, 101) < 50)
			{
				uint winnings = amt * 3;
				account.Yen += winnings;
				await Context.Channel.SendMessageAsync($"Congratulations, you won ¥{winnings - amt}! You now have ¥{account.Yen}.");
			}
			else
			{
				await Context.Channel.SendMessageAsync($"Oh no, you lost! You now have ¥{account.Yen}.");
			}

		}

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
		public async Task Stats(SocketUser user)
		{
			var account = UserAccounts.GetAccount(user);
			var embed = new EmbedBuilder();
			embed.WithColor(220, 20, 60);
			embed.WithTitle($"{user.Username}'s Stats");
			embed.WithDescription($"XP: {account.Experience}, Lvl: {account.LevelNumber}, Money ¥{account.Yen}\nTotal Messages: {account.TotalMessages}, Points: {account.Points}");
			await Context.Channel.SendMessageAsync("", embed: embed);
		}

		[Command("stats")]
		public async Task Stats()
		{
			var account = UserAccounts.GetAccount(Context.User);
			var embed = new EmbedBuilder();
			embed.WithColor(220, 20, 60);
			embed.WithTitle($"{Context.User.Username}'s Stats");
			embed.WithDescription($"XP: {account.Experience}, Lvl: {account.LevelNumber}, Money ¥{account.Yen}\nTotal Messages: {account.TotalMessages}, Points: {account.Points}");
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
			var yen = (uint)(amt / 7.5);
			if (account.Points < amt)
			{
				await Context.Channel.SendMessageAsync("You don't have enough XP!");
				return;
			}
			account.Points -= amt;
			account.Yen += yen;
			UserAccounts.SaveAccounts();
			await Context.Channel.SendMessageAsync($"You just exchanged {amt} points for ¥{yen}");
		}

		[Command("exchangemax")]
		public async Task ExchangeMax()
		{
			var account = UserAccounts.GetAccount(Context.User);
			uint yen = (uint)(account.Points / 7.5);
			await Context.Channel.SendMessageAsync($"You just exchanged {account.Points} points for ¥{yen}");
			
			account.Points -= account.Points;
			account.Yen += yen;

			UserAccounts.SaveAccounts();
		}

		//[Command("echo")]
		//public async Task Echo([Remainder]string msg)
		//{
		//	await Context.Channel.SendMessageAsync(msg);
		//}

		[Command("hug")]
		public async Task Hug()
		{
			await Context.Channel.SendFileAsync("C:/Users/Yumi/Pictures/389F9492-2C33-4C49-9BDD-9FA92A686DCC.gif");
		}

		//[Command("data")]
		//public async Task GetData()
		//{
		//	await Context.Channel.SendMessageAsync("Data contains " + DataStorage.GetPairCount() + " pairs");
		//}

		//[Command("addpair")]
		//public void AddPair()
		//{
		//	DataStorage.AddPairToStorage(Context.User.Username, "Name");
		//}
	}
}

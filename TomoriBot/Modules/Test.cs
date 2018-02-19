using System.Threading.Tasks;
using Discord.Commands;
using TomoriBot.Core.UserProfiles;

namespace TomoriBot.Modules
{
	public class Test : ModuleBase<SocketCommandContext>
	{
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

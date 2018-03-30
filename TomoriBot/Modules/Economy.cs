using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using TomoriBot.Core.UserProfiles;

namespace TomoriBot.Modules
{
	public class Economy : ModuleBase<SocketCommandContext>
	{
		[Command("transfer")]
		[Alias("give")]
		public async Task Transfer(SocketUser recipient, uint amt)
		{
			var callerAccount = UserAccounts.GetAccount(Context.User);

			if (amt > callerAccount.Yen)
			{
				await Context.Channel.SendMessageAsync("Sorry! You don't have that much money!");
				return;
			}

			if (Context.User.Id == recipient.Id)
			{
				await Context.Channel.SendMessageAsync("Silly! You can't send money to yourself!");
				return;
			}

			var recipientAccount = UserAccounts.GetAccount(recipient);

			callerAccount.Yen -= amt;
			recipientAccount.Yen += amt;

			await Context.Channel.SendMessageAsync($"{recipient.Username} has been given ¥{amt} by {Context.User.Username}!");
		}
	}
}

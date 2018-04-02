using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TomoriBot.Core.UserProfiles;
using static TomoriBot.Global;

namespace TomoriBot.Modules
{
	public class Profile : ModuleBase<SocketCommandContext>
	{
		[Command("profile")]
		public async Task _Profile(SocketGuildUser user)
		{
			var userAccount = UserAccounts.GetAccount(user);

			var embed = new EmbedBuilder()
			{
				Color = new Color(220, 20, 60),
				Title = $"{GetNickname(user)}'s Stats"
			};

			embed.WithDescription($"Money: ¥{userAccount.Yen}\n" +
			                      $"Level: {userAccount.LevelNumber}\n" +
			                      $"XP: {userAccount.Experience}\n" +
			                      $"Total Messages: {userAccount.TotalMessages}");

			await Context.Channel.SendMessageAsync("", embed: embed);
		}

		[Command("profile")]
		public async Task _Profile()
		{
			var user = Context.User;

			await _Profile((SocketGuildUser)user);
		}
	}
}

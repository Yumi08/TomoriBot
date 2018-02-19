using Discord.WebSocket;
using TomoriBot.Core.UserProfiles;

namespace TomoriBot.Core.LevelingSystem
{
	internal static class Leveling
	{
		internal static async void UserSentMessage(SocketGuildUser user, SocketTextChannel channel)
		{
			var userAccount = UserAccounts.GetAccount(user);

			// Return if last message was within the last minute
			if ((Global.GetElapsedTime().TotalMinutes - userAccount.LastMessageTime) < 1) return;

			// Set the last message time to right now
			userAccount.LastMessageTime = Global.GetElapsedTime().TotalMinutes;

			uint oldLevel = userAccount.LevelNumber;

			userAccount.Experience += (uint)Global.R.Next(25, 41);
			UserAccounts.SaveAccounts();

			if (oldLevel != userAccount.LevelNumber)
			{
				await channel.SendMessageAsync(user.Username + " just leveled up to level " + userAccount.LevelNumber + "!");
			}
		}
	}
}

using Discord.WebSocket;
using TomoriBot.Core.UserProfiles;
using static TomoriBot.Global;

namespace TomoriBot.Core.LevelingSystem
{
	internal static class Leveling
	{
		internal static async void UserSentMessage(SocketGuildUser user, SocketTextChannel channel)
		{
			var userAccount = UserAccounts.GetAccount(user);

			// Add to total messages & points
			userAccount.TotalMessages++;
			OnMessageAmount(user, channel, userAccount.TotalMessages);

			// Return if last message was within the last minute
			if ((Global.GetElapsedTime().TotalMinutes - userAccount.LastMessageTime) >= 1)
			{
				// Set the last leveled message time to right now
				userAccount.LastMessageTime = Global.GetElapsedTime().TotalMinutes;

				uint oldLevel = userAccount.LevelNumber;

				userAccount.Experience += (uint) Global.R.Next(25, 41);

				if (oldLevel != userAccount.LevelNumber)
				{
					await channel.SendMessageAsync(GetNickname(user) + " just leveled up to level " + userAccount.LevelNumber + "!");
				}
			}

			UserAccounts.SaveAccounts();
		}

		private static void OnMessageAmount(SocketGuildUser user, SocketTextChannel channel, uint totalMsgs)
		{
			switch (totalMsgs)
			{
				case 100:
					NotifyTotalMessages(user, channel, totalMsgs);
					break;
				case 500:
					NotifyTotalMessages(user, channel, totalMsgs);
					break;
				case 1000:
					NotifyTotalMessages(user, channel, totalMsgs);
					break;
				case 5000:
					NotifyTotalMessages(user, channel, totalMsgs);
					break;
				case 10000:
					NotifyTotalMessages(user, channel, totalMsgs);
					break;
				case 25000:
					NotifyTotalMessages(user, channel, totalMsgs);
					break;
				case 50000:
					NotifyTotalMessages(user, channel, totalMsgs);
					break;
				case 100000:
					NotifyTotalMessages(user, channel, totalMsgs);
					break;
			}
		}

		private static async void NotifyTotalMessages(SocketGuildUser user, SocketTextChannel channel, uint totalMsgs)
		{
			await channel.SendMessageAsync($"{GetNickname(user)} has sent {totalMsgs} messages so far! Keep it up! <3");
		}
	}
}

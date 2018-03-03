using Discord.WebSocket;
using TomoriBot.Core.UserProfiles;

namespace TomoriBot.Core.LevelingSystem
{
	internal static class Leveling
	{
		internal static async void UserSentMessage(SocketGuildUser user, SocketTextChannel channel)
		{
			var userAccount = UserAccounts.GetAccount(user);

			// Add to total messages & points
			userAccount.TotalMessages++;
			userAccount.Points += 1;
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
					await channel.SendMessageAsync(user.Username + " just leveled up to level " + userAccount.LevelNumber + "!");
				}
			}

			UserAccounts.SaveAccounts();
		}

		private static void OnMessageAmount(SocketGuildUser user, SocketTextChannel channel, uint totalMsgs)
		{
			switch (totalMsgs)
			{
				case 60:
					SendMsgOMA(user, channel, totalMsgs);
					break;
				case 500:
					SendMsgOMA(user, channel, totalMsgs);
					break;
				case 1000:
					SendMsgOMA(user, channel, totalMsgs);
					break;
				case 5000:
					SendMsgOMA(user, channel, totalMsgs);
					break;
				case 10000:
					SendMsgOMA(user, channel, totalMsgs);
					break;
				case 25000:
					SendMsgOMA(user, channel, totalMsgs);
					break;
				case 50000:
					SendMsgOMA(user, channel, totalMsgs);
					break;
				case 100000:
					SendMsgOMA(user, channel, totalMsgs);
					break;
			}
		}

		private static async void SendMsgOMA(SocketGuildUser user, SocketTextChannel channel, uint totalMsgs)
		{
			await channel.SendMessageAsync($"{user.Username} has sent {totalMsgs} messages so far! Great job!");
		}
	}
}

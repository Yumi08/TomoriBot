using System;
using System.Linq;
using Discord.WebSocket;
using TomoriBot.Core.UserProfiles;
using static TomoriBot.Global;

namespace TomoriBot.Core.LevelingSystem
{
	internal static class Leveling
	{
		private static int[] _messageMilestones = new[] {100, 500, 1000, 5000, 10000, 25000, 50000, 100000};

		internal static async void UserSentMessage(SocketGuildUser user, SocketTextChannel channel)
		{
			var userAccount = UserAccounts.GetAccount(user);

			// Add to total messages & points
			userAccount.TotalMessages++;
			OnMessageAmount(user, channel, userAccount.TotalMessages);

			// Return if last message was within the last minute
			if (Math.Abs(GetElapsedTime().TotalMinutes - userAccount.PreviousMessageTime) >= 1)
			{
				// Set the last leveled message time to right now
				userAccount.PreviousMessageTime = GetElapsedTime().TotalMinutes;

				uint oldLevel = userAccount.LevelNumber;

				userAccount.Experience += (uint) R.Next(25, 41);

				if (oldLevel != userAccount.LevelNumber)
				{
					await channel.SendMessageAsync(GetNickname(user) + " just leveled up to level " + userAccount.LevelNumber + "!");
				}

				UserAccounts.SaveAccounts();
			}
		}

		private static void OnMessageAmount(SocketGuildUser user, SocketTextChannel channel, uint totalMsgs)
		{
			for (int i = 0; i < _messageMilestones.Length; i++)
			{
				if (totalMsgs ==_messageMilestones[i])
					NotifyTotalMessages(user, channel, _messageMilestones[i]);
			}
		}

		private static async void NotifyTotalMessages(SocketGuildUser user, SocketTextChannel channel, int totalMsgs)
		{
			await channel.SendMessageAsync($"{GetNickname(user)} has sent {totalMsgs} messages so far! Keep it up! <3");
		}
	}
}

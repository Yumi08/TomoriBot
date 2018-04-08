using System;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using TomoriBot.Core.Guilds;
using TomoriBot.Core.UserProfiles;
using static TomoriBot.Global;

namespace TomoriBot.Core.LevelingSystem
{
	internal static class Leveling
	{
		private static readonly int[] MessageMilestones = new[] {100, 500, 1000, 5000, 10000, 25000, 50000, 100000};

		/// <summary>
		/// Fires whenever a user sends a message that the bot can see
		/// </summary>
		/// <param name="context"></param>
		internal static async void UserSentMessage(SocketCommandContext context)
		{
			var userAccount = UserAccounts.GetAccount(context.User);

			// Add to total messages & points
			userAccount.TotalMessages++;
			OnMessageAmount(context, userAccount.TotalMessages);

			// Return if last message was within the last minute
			if (Math.Abs(GetElapsedTime().TotalMinutes - userAccount.PreviousMessageTime) >= 1)
			{
				// Set the last leveled message time to right now
				userAccount.PreviousMessageTime = GetElapsedTime().TotalMinutes;

				uint oldLevel = userAccount.LevelNumber;

				userAccount.Experience += (uint) R.Next(25, 41);

				if (oldLevel != userAccount.LevelNumber)
				{
					await context.Channel.SendMessageAsync(GetNickname((SocketGuildUser)context.User) + " just leveled up to level " + userAccount.LevelNumber + "!");
				}

				UserAccounts.SaveAccounts();
			}
		}

		/// <summary>
		/// Calls another method whenever a user hits a certain message milestone.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="totalMsgs"></param>
		private static void OnMessageAmount(SocketCommandContext context, uint totalMsgs)
		{
			for (var i = 0; i < MessageMilestones.Length; i++)
			{
				if (totalMsgs ==MessageMilestones[i])
					NotifyTotalMessages(context, MessageMilestones[i]);
			}
		}

		/// <summary>
		/// Notifies the user that they've sent x total messages, for when they hit a milestone.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="totalMsgs"></param>
		private static async void NotifyTotalMessages(SocketCommandContext context, int totalMsgs)
		{
			if (!Servers.GetServer(context.Guild).EnableLevelUpMessages) return;

			await context.Channel.SendMessageAsync($"{GetNickname((SocketGuildUser)context.User)} has sent {totalMsgs} messages so far! Keep it up! <3");
		}
	}
}

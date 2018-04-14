using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using TomoriBot.Core.Guilds;
using TomoriBot.Core.UserProfiles;
using static TomoriBot.Global;

namespace TomoriBot.Core.LevelingSystem
{
	internal static class Leveling
	{
		private static readonly int[] MessageMilestones = {100, 500, 1000, 5000, 10000, 25000, 50000, 100000};
		private static readonly uint[,] LevelMilestones = { {10, 500}, {20, 2500}, {30, 7500}, {40, 15000}, {50, 35000},
			{55, 45000}, {60, 60000}, {65, 75000}, {70, 85000}, {75, 95000}, {80, 110000}, {85, 125000}, {90, 135000}, {95, 150000}, {100, 200000}};

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
					await LevelRewards(context, userAccount);
				}

				UserAccounts.SaveAccounts();
			}
		}

		private static async Task LevelRewards(SocketCommandContext context, UserAccount userAccount)
		{
			// Use GetLength since it's a multidimensional array
			for (var x = 0; x < LevelMilestones.GetLength(0); x++)
			{
				if (userAccount.LevelNumber == LevelMilestones[x, 0])
				{
					userAccount.Yen += LevelMilestones[x, 1];
					await context.Channel.SendMessageAsync($"Congratulations on leveling up to lvl {userAccount.LevelNumber}! You received ¥{LevelMilestones[x, 1]}!");
				}
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

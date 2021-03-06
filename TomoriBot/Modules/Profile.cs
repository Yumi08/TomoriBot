﻿using System.Threading.Tasks;
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
			if (!CheckProfilesEnabled(Context).Result) return;

			var userAccount = UserAccounts.GetAccount(user);

			var embed = new EmbedBuilder()
			{
				Color = new Color(220, 20, 60),
				Title = $"{GetNickname(user)}'s Profile",
			};

			embed.AddInlineField("Money", $"¥{userAccount.Yen}");
			embed.AddInlineField("Level", userAccount.LevelNumber);
			embed.AddInlineField("XP", userAccount.Experience);
			embed.AddInlineField("Total Messages", userAccount.TotalMessages);

			//embed.WithDescription($"Money: ¥{userAccount.Yen}\n" +
			//                      $"Level: {userAccount.LevelNumber}\n" +
			//                      $"XP: {userAccount.Experience}\n" +
			//                      $"Total Messages: {userAccount.TotalMessages}");

			await Context.Channel.SendMessageAsync("", embed: embed);
		}

		[Command("profile")]
		public async Task _Profile()
		{
			if (!CheckProfilesEnabled(Context).Result) return;
			
			var user = Context.User;

			await _Profile((SocketGuildUser)user);
		}
	}
}

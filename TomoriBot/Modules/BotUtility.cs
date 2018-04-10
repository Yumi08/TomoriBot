using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TomoriBot.Core.Guilds;
using TomoriBot.Core.UserProfiles;

namespace TomoriBot.Modules
{
	/// <summary>
	/// Class containing commands for managing the bot
	/// </summary>
	public class BotUtility : ModuleBase<SocketCommandContext>
	{
		// TODO: FIX (rarely outputs negative values around 800)
		//[Command("ping")]
		public async Task Ping()
		{
			var m = await Context.Channel.SendMessageAsync("Ping...");

			await m.ModifyAsync(msg => msg.Content = $"Pong! The server latency is {m.CreatedAt.Millisecond - Context.Message.CreatedAt.Millisecond}ms!\n" +
			                                         $"And the API latency is {Context.Client.Latency}ms.");
		}

		// TODO: FIX RESTART COMMAND (does not update code, needs to rebuild project.)
		[Command("restart")]
		public async Task Restart()
		{
			if (await Global.ValidateUser(Context)) return;

			await Context.Channel.SendMessageAsync("Restarting!");

			System.Diagnostics.Process.Start(
				"C:\\Users\\Yumi\\source\\Console\\Discord\\TomoriBot\\TomoriBot\\bin\\Release\\TomoriBot.exe");
			Environment.Exit(0);
		}

		[Command("help")]
		public async Task Help()
		{
			var u = Context.Message.Author;

			foreach (var message in Global.HelpMessages)
			{
				await u.SendMessageAsync(message);
				await Task.Delay(500);
			}
		}

		[Command("help")]
		public async Task Help(string command)
		{
			var msg = FileUtils.GetAlert($"Help_{command.ToLower()}");

			if (msg == "ERR: KEY NOT FOUND.")
			{
				await Context.Channel.SendMessageAsync("No such command found!");
				return;
			}

			await Context.Channel.SendMessageAsync(msg);
		}

		/// <summary>
		/// Sets the bot's current game it's playing
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[Command("setgame")]
		public async Task SetGame([Remainder]string input)
		{
			if (await Global.ValidateUser(Context)) return;

			string gameName;
			switch (input)
			{
				/* case "%RESET%":
					// Doesn't work.
					await Context.Client.SetGameAsync(null);
					await Context.Channel.SendMessageAsync("Reset game!");
					break; */

				case "%WITHME%":
				{
					var ds = new DataStorage<string, ulong>("Storage/IDStorage.json");
					var _ownerName = Context.Guild.GetUser(ds.GetPair("BotOwner")).Username;
					var ownerName = Regex.Replace(_ownerName, @"\p{Cs}", "");
					gameName = $"with {ownerName}";
					await Context.Client.SetGameAsync(gameName);
					await Context.Channel.SendMessageAsync($"I set my game to \"{gameName}\"!");
					break;
				}
					

				case "%MIRROR%":
				{
					var ds = new DataStorage<string, ulong>("Storage/IDStorage.json");
					gameName = Context.Guild.GetUser(ds.GetPair("BotOwner")).Game?.Name;
					await Context.Client.SetGameAsync(gameName);
					await Context.Channel.SendMessageAsync($"I set my game to \"{gameName}\"!");
					break;
				}

				case "%USERS%":
				{
					int userCount = UserAccounts.UserAccountCount();
					gameName = $"{userCount} users";
					await Context.Client.SetGameAsync(gameName);
					await Context.Channel.SendMessageAsync($"I set my game to \"{gameName}\"!");
					break;
				}

				default:
				{
					gameName = input;
					await Context.Client.SetGameAsync(input);
					await Context.Channel.SendMessageAsync($"I set my game to \"{input}\".");
					break;
				}
			}

			Console.WriteLine($"Game set to \"{gameName}\".");
		}

		[Command("logout")]
		public async Task LogOut()
		{
			if (await Global.ValidateUser(Context)) return;

			await Context.Channel.SendMessageAsync("Logging out!");
			Console.WriteLine("Logging out!");
			Environment.Exit(0);
		}

		[Command("registerserver")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task RegisterServer()
		{
			if (await Global.ValidateUser(Context)) return;

			var server = Servers.GetServer(Context.Guild);
			await Context.Channel.SendMessageAsync($"Successfully registered/updated server \"{server.Name}\"!");
		}

		[Command("report")]
		public async Task ReportBug([Remainder]string input)
		{
			Console.WriteLine($"BUG REPORTED: {input}");

			var ds = new DataStorage<uint, string>("Storage/BugReportStorage.json");
			uint key = (uint)ds.GetPairCount() + 1;
			ds.SetPair(key, input);

			await Context.Channel.SendMessageAsync("Thank you so much for your feedback!");
		}

		//private static readonly uint[,] LevelMilestones = { {10, 500}, {20, 2500}, {30, 7500}, {40, 15000}, {50, 35000},
		//	{55, 45000}, {60, 60000}, {65, 75000}, {70, 85000}, {75, 95000}, {80, 110000}, {85, 125000}, {90, 135000}, {95, 150000}, {100, 200000}};

		// One-shot command after implimenting the level rewards system
		//[Command("updatelvls")]
		//public async Task UpdateLvls()
		//{
		//	foreach (var account in UserAccounts.GetAccountList())
		//	{
		//		var toAdd = 0u;

		//		for (var x = 0; x < LevelMilestones.GetLength(0); x++)
		//		{
		//			if (account.LevelNumber >= LevelMilestones[x, 0])
		//			{
		//				toAdd += LevelMilestones[x, 1];
		//			}
		//		}

		//		await Context.Client.GetUser(account.Id).SendMessageAsync($"A new level rewards system is being put into place, and it seems that you are already lvl {account.LevelNumber}!" +
		//		                                                          $" So congratulations, you get a reward of ¥{toAdd}!");
		//		account.Yen += toAdd;
		//		UserAccounts.SaveAccounts();
		//	}
		//}
	}
}

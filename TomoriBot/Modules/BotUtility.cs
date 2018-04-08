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
	}
}

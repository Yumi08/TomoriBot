using System;
using System.Threading.Tasks;
using Discord.Commands;
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
			await Discord.UserExtensions.SendMessageAsync(u, FileUtils.GetCommandHelp());
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

		[Command("setgame")]
		public async Task SetGame([Remainder]string input)
		{
			if (await Global.ValidateUser(Context)) return;

			switch (input)
			{
				case "USERS":
					int userCount = UserAccounts.UserAccountCount();
					await Context.Client.SetGameAsync($"{userCount} users");
					await Context.Channel.SendMessageAsync("I set my game to the current amount of users!");
					break;

				default:
					await Context.Client.SetGameAsync(input);
					await Context.Channel.SendMessageAsync($"My game has been set to \"{input}\".");
					break;
			}
		}
	}
}

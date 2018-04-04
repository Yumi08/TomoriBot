using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TomoriBot.Modules
{
	public class Utility : ModuleBase<SocketCommandContext>
	{
		[Command("initialize")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task Initialize()
		{
			await InitializeMute();

			await Context.Channel.SendMessageAsync("Initialized all commands!");
		}

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
			if (await ValidateUser()) return;

			await Context.Channel.SendMessageAsync("Restarting!");

			System.Diagnostics.Process.Start(
				"C:\\Users\\Yumi\\source\\Console\\Discord\\TomoriBot\\TomoriBot\\bin\\Release\\TomoriBot.exe");
			Environment.Exit(0);
		}

		[Command("help")]
		public async Task Help()
		{
			var u = Context.Message.Author;
			await Discord.UserExtensions.SendMessageAsync(u, Utilities.GetCommandHelp());
		}

		[Command("help")]
		public async Task Help(string command)
		{
			var msg = Utilities.GetAlert($"Help_{command.ToLower()}");

			if (msg == "ERR: KEY NOT FOUND.")
			{
				await Context.Channel.SendMessageAsync("No such command found!");
				return;
			}

			await Context.Channel.SendMessageAsync(msg);
		}

		private async Task InitializeMute()
		{
			var ds = new DataStorage<string, ulong>("Storage/IDStorage.json");

			var roles = from r in Context.Guild.Roles
				where r.Name == "Muted"
				select r;

			IRole mutedRole;
			OverwritePermissions perms = new OverwritePermissions(sendMessages: PermValue.Deny, readMessages: PermValue.Allow);
			if (!roles.Any())
			{
				var mutedPerms = new GuildPermissions();
				mutedPerms.Modify(sendMessages: false);
				mutedRole = await Context.Guild.CreateRoleAsync("Muted", mutedPerms, Color.LightGrey);
				ds.SetPair("MutedRole", mutedRole.Id);
			}
			else mutedRole = Context.Guild.GetRole(ds.GetPair("MutedRole"));

			foreach (var channel in Context.Guild.TextChannels)
			{
				if (channel == null) continue;

				foreach (var overwrite in channel.PermissionOverwrites)
				{
					if (overwrite.TargetId == mutedRole?.Id) goto next;
				}
				await channel.AddPermissionOverwriteAsync(mutedRole, perms);
				next:;
				Console.WriteLine("End of InitializeMute()");
			}
		}
		private async Task<bool> ValidateUser()
		{
			if (Context.User.Id != 218429853144186883)
			{
				await Context.Channel.SendMessageAsync("Wat u think ur doin <:MiyanoDead:407275770151436289>");
				return true;
			}

			return false;
		}
	}
}

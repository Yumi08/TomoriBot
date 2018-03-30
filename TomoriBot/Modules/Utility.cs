using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace TomoriBot.Modules
{
	public class Utility : ModuleBase<SocketCommandContext>
	{
		[Command("ping")]
		public async Task Ping()
		{
			var m = await Context.Channel.SendMessageAsync("Ping...");

			await m.ModifyAsync(msg => msg.Content = $"Pong! The server latency is {m.CreatedAt.Millisecond - Context.Message.CreatedAt.Millisecond}ms!\n" +
			                                         $"And the API latency is {Context.Client.Latency}ms.");
		}


		// TODO: FIX RESTART COMMAND (does not update code)
		[Obsolete("Does not work properly.")]
		//[Command("restart")]
		public async Task Restart()
		{
			if (await ValidateUser()) return;

			await Context.Channel.SendMessageAsync("Restarting!");

			System.Diagnostics.Process.Start(
				"C:\\Users\\Yumi\\source\\Console\\Discord\\TomoriBot\\TomoriBot\\bin\\Release\\TomoriBot.exe");
			Environment.Exit(0);
		}

		[Command("resetdata")]
		public async Task ResetData()
		{
			if (await ValidateUser()) return;

			var ds = new DataStorage<string, string>("DataStorage.json");

			ds.ResetData();	

			await Context.Channel.SendMessageAsync("Data pairs reset!");
		}

		[Command("setdata")]
		public async Task SetData(string key, [Remainder]string value)
		{
			if (await ValidateUser()) return;

			var ds = new DataStorage<string, string>("DataStorage.json");

			if (!ds.SetPair(key, value))
			{
				await Context.Channel.SendMessageAsync($"Updated value \"{value}\" to key \"{key}\"!");
				return;
			}

			await Context.Channel.SendMessageAsync($"Set value \"{value}\" to key \"{key}\"!");
		}

		[Command("getdata")]
		public async Task GetData(string key)
		{
			if (await ValidateUser()) return;

			var ds = new DataStorage<string, string>("DataStorage.json");

			var value = ds.GetPair(key, out bool success);

			if (success) await Context.Channel.SendMessageAsync(value);
			else await Context.Channel.SendMessageAsync("No value found for such key!");
		}

		[Command("getdata")]
		public async Task GetData()
		{
			var ds = new DataStorage<string, string>("DataStorage.json");

			await Context.Channel.SendMessageAsync($"{ds.GetPairCount()} pairs in storage!");
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

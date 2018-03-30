using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using TomoriBot.Core;

namespace TomoriBot.Modules
{
	public class Fun : ModuleBase<SocketCommandContext>
	{
		[Command("pick")]
		[Alias("choose")]
		public async Task Choose([Remainder]string input)
		{
			var args = input.Split(' ');

			await Context.Channel.SendMessageAsync(args[Global.R.Next(args.Length)]);
		}

		[Command("tag")]
		public async Task Tag(SocketUser user, [Remainder]string value)
		{
			var ds = new DataStorage<ulong, string>("UserTags.json");

			ds.SetPair(user.Id, value);

			await Context.Channel.SendMessageAsync($"Tagged {user.Username} with {value}!");
		}
	}
}

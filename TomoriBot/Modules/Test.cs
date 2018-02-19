using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace TomoriBot.Modules
{
	public class Test : ModuleBase<SocketCommandContext>
	{
		[Command("hello")]
		public async Task Hello()
		{
			await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREET_&USERNAME_&BOTNAME",
				Context.User.Username, Config.bot.botName));
		}

		[Command("echo")]
		public async Task Echo([Remainder]string msg)
		{
			await Context.Channel.SendMessageAsync(msg);
		}

		[Command("hug")]
		public async Task Hug()
		{
			await Context.Channel.SendFileAsync("C:/Users/Yumi/Pictures/389F9492-2C33-4C49-9BDD-9FA92A686DCC.gif");
		}
	}
}

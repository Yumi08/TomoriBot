using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace TomoriBot.Modules
{
	public class Moderation : ModuleBase<SocketCommandContext>
	{
		[Command("nickname")]
		[Alias("nick")]
		public async Task Nickname(SocketGuildUser user, [Remainder]string newName)
		{
			await Context.Guild.GetUser(user.Id).ModifyAsync(x => { x.Nickname = newName; });
		}
	}
}

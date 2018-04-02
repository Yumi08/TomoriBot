using System.Threading.Tasks;
using Discord.Commands;

namespace TomoriBot.Modules
{
	public class Media : ModuleBase<SocketCommandContext>
	{
		[Command("danbooru")]
		public async Task Danbooru([Remainder]string input)
		{
			string[] tags = input.Split(' ');
			if (tags.Length > 2)
			{
				await Context.Channel.SendMessageAsync("Cannot use more than 2 tags!");
				return;
			}

			string tagsString;
			if (tags.Length == 1) tagsString = tags[0];
			else if (tags.Length == 2) tagsString = $"{tags[0]}+{tags[1]}";
			else tagsString = "";

			var url = $"http://danbooru.donmai.us/posts/random?tags={tagsString}";

			await Context.Channel.SendMessageAsync(url);
		}
	}
}

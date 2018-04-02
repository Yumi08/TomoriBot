using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;

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

			var url = $"http://danbooru.donmai.us/posts/random.json?tags={tagsString}";

			string json = "";
			using (WebClient client = new WebClient())
			{
				json = client.DownloadString(url);
			}

			var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

			await Context.Channel.SendMessageAsync($"http://danbooru.donmai.us/posts/{dataObject["id"]}");
		}

		[Command("danbooruf")]
		public async Task Danbooru(ushort amt, [Remainder] string input)
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

			for (int i = 0; i < amt; i++)
			{
				var url = $"http://danbooru.donmai.us/posts/random.json?tags={tagsString}";

				string json = "";
				using (WebClient client = new WebClient())
				{
					json = client.DownloadString(url);
				}

				var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

				await Context.Channel.SendMessageAsync($"http://danbooru.donmai.us/posts/{dataObject["id"]}");
				await Task.Delay(1000);
			}

		}
	}
}

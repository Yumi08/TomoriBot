using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using static TomoriBot.Global;

namespace TomoriBot.Modules
{
	public class Nsfw : ModuleBase<SocketCommandContext>
	{
		/// <summary>
		/// Gets a random danbooru image
		/// </summary>
		/// <param name="input">Tags</param>
		/// <returns></returns>
		[Command("danbooru")]
		public async Task Danbooru([Remainder]string input)
		{
			if (!CheckNsfwModule(Context).Result) return;

			string[] tags = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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

			string json;
			using (WebClient client = new WebClient())
			{
				json = client.DownloadString(url);
			}

			var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

			await Context.Channel.SendMessageAsync($"http://danbooru.donmai.us/posts/{dataObject["id"]}");
		}

		/// <summary>
		/// Gets a formatted random danbooru image
		/// </summary>
		/// <param name="amt">Amount of images</param>
		/// <param name="input">Tags</param>
		/// <returns></returns>
		[Command("danbooruf")]
		public async Task Danbooru(ushort amt, [Remainder] string input)
		{
			if (!CheckNsfwModule(Context).Result) return;

			string[] args = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (args[0].ToLower().Contains("speed:"))
			{
				args[0] = args[0].Replace("speed:", "");
				await Danbooru(amt, args[0], Global.GetRemainder(args, 1));
				return;
			}
			if (args.Length > 2)
			{
				await Context.Channel.SendMessageAsync("Cannot use more than 2 tags!");
				return;
			}

			if (amt > 20) amt = 20;

			string tagsString;
			if (args.Length == 1) tagsString = args[0];
			else if (args.Length == 2) tagsString = $"{args[0]}+{args[1]}";
			else tagsString = "";

			var count = amt;
			for (int i = 0; i < amt; i++)
			{
				count--;

				var url = $"http://danbooru.donmai.us/posts/random.json?tags={tagsString}";

				string json = "";
				using (WebClient client = new WebClient())
				{
					json = client.DownloadString(url);
				}

				var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

				await Context.Channel.SendMessageAsync($"{count} more!");
				await Context.Channel.SendMessageAsync($"http://danbooru.donmai.us/posts/{dataObject["id"]}");
				await Task.Delay(2500);
			}
		}
		/// <summary>
		/// Gets a formatted random danbooru image
		/// </summary>
		/// <param name="amt">Amount of images</param>
		/// <param name="speed">Speed of posting the images</param>
		/// <param name="tags">Tags</param>
		/// <returns></returns>
		// This function is NOT called by discord. It's called from the first overload.
		public async Task Danbooru(ushort amt, string speed, [Remainder] string[] tags)
		{
			var speeds = new Dictionary<string, int>()
			{
				{"normal", 2000},
				{"slow", 2500},
				{"slowest", 5000}
			};

			if (tags.Length > 4)
			{
				await Context.Channel.SendMessageAsync("Cannot use more than 2 tags!");
				return;
			}

			string tagsString;
			if (tags.Length == 1) tagsString = tags[0];
			else if (tags.Length == 2) tagsString = $"{tags[0]}+{tags[1]}";
			else tagsString = "";

			var delay = speeds.ContainsKey(speed.ToLower()) ? speeds[speed.ToLower()] : 3000;
			if (amt >= 25 && delay <= 5000) amt = 25;
			if (amt >= 20 && delay <= 2500) amt = 20;
			if (amt >= 15 && delay <= 2000) amt = 15;

			var count = amt;
			for (int i = 0; i < amt; i++)
			{
				count--;

				var url = $"http://danbooru.donmai.us/posts/random.json?tags={tagsString}";

				string json;
				using (var client = new WebClient())
				{
					json = client.DownloadString(url);
				}

				var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

				await Context.Channel.SendMessageAsync($"http://danbooru.donmai.us/posts/{dataObject["id"]}");
				await Context.Channel.SendMessageAsync($"{count} more!");
				await Task.Delay(delay);
			}
		}
	}
}

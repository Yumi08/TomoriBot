﻿using System;
using System.Collections.Generic;
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

			string json;
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
			string[] args = input.Split(' ');
			if (args[0].ToLower().Contains("speed:"))
			{
				args[0] = args[0].Replace("speed:", "");
				await Danbooru(amt, args[0], Global.GetRemainder(args, 1));
			}
			if (args.Length > 2)
			{
				await Context.Channel.SendMessageAsync("Cannot use more than 2 tags!");
				return;
			}

			if (amt > 25) amt = 25;

			string tagsString;
			if (args.Length == 1) tagsString = args[0];
			else if (args.Length == 2) tagsString = $"{args[0]}+{args[1]}";
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
				await Task.Delay(2500);
			}
		}
		public async Task Danbooru(ushort amt, string speed, [Remainder] string input)
		{
			var speeds = new Dictionary<string, int>()
			{
				{"fast", 1500},
				{"normal", 3000},
				{"slow", 5000},
				{"slowest", 10000}
			};

			string[] tags = input.Split(' ');
			if (tags.Length > 2)
			{
				await Context.Channel.SendMessageAsync("Cannot use more than 2 tags!");
				return;
			}

			if (amt > 25) amt = 25;

			string tagsString;
			if (tags.Length == 1) tagsString = tags[0];
			else if (tags.Length == 2) tagsString = $"{tags[0]}+{tags[1]}";
			else tagsString = "";

			for (int i = 0; i < amt; i++)
			{
				var url = $"http://danbooru.donmai.us/posts/random.json?tags={tagsString}";

				string json;
				using (var client = new WebClient())
				{
					json = client.DownloadString(url);
				}

				var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

				await Context.Channel.SendMessageAsync($"http://danbooru.donmai.us/posts/{dataObject["id"]}");

				var delay = speeds.ContainsKey(speed.ToLower()) ? speeds[speed.ToLower()] : 3000;
				await Task.Delay(delay);
			}
		}
	}
}

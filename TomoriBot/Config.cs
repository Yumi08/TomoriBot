using System.IO;
using Newtonsoft.Json;

namespace TomoriBot
{
	class Config
	{
		private const string configFolder = "Resources";
		private const string configFile = "config.json";
		public static BotConfig bot;

		static Config()
		{
			if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);

			if (!File.Exists(configFolder + "/" + configFile))
			{
				bot = new BotConfig();
				string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
				File.WriteAllText(configFolder + "/" + configFile, json);
			}
			else
			{
				string json = File.ReadAllText(configFolder + "/" + configFile);
				bot = JsonConvert.DeserializeObject<BotConfig>(json);
			}
		}
	}

	public struct BotConfig
	{
		public string token;
		public char cmdPrefix;
		public string botName;
		public string botId;
	}
}

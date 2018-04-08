using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TomoriBot.Core.Guilds;
using TomoriBot.Core.UserProfiles;

namespace TomoriBot.Core
{
	/// <summary>
	/// Static class for handling user accounts and servers' json data
	/// </summary>
	public static class DataStorage
	{
		public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
		{
			string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
			File.WriteAllText(filePath, json);
		}

		public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
		{
			if (!File.Exists(filePath)) return null;
			string json = File.ReadAllText(filePath);
			return JsonConvert.DeserializeObject<List<UserAccount>>(json);
		}

		public static void SaveServers(IEnumerable<Server> servers, string filePath)
		{
			string json = JsonConvert.SerializeObject(servers, Formatting.Indented);
			File.WriteAllText(filePath, json);
		}

		public static IEnumerable<Server> LoadServers(string filePath)
		{
			if (!File.Exists(filePath)) return null;
			string json = File.ReadAllText(filePath);
			return JsonConvert.DeserializeObject<List<Server>>(json);
		}

		/// <summary>
		/// Check if a save exists
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static bool SaveExists(string filePath)
		{
			return File.Exists(filePath);
		}
	}
}

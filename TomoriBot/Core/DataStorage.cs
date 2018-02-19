﻿using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TomoriBot.Core.UserProfiles;

namespace TomoriBot.Core
{
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

		public static bool SaveExists(string filePath)
		{
			return File.Exists(filePath);
		}
	}
}

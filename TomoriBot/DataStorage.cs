using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TomoriBot
{
	class DataStorage
	{
		private static Dictionary<string, string> _pairs = new Dictionary<string, string>();

		public static void AddPairToStorage(string key, string value)
		{
			_pairs.Add(key, value);
			SaveData();
		}

		public static int GetPairCount()
		{
			return _pairs.Count;
		}

		static DataStorage()
		{
			if (!ValidateStorageFile("DataStorage.json")) return;
			string json = File.ReadAllText("DataStorage.json");
			_pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
		}

		public static void SaveData()
		{
			string json = JsonConvert.SerializeObject(_pairs, Formatting.Indented);
			File.WriteAllText("DataStorage.json",json);
		}

		private static bool ValidateStorageFile(string file)
		{
			if (File.Exists(file)) return true;
			File.WriteAllText(file, "");
			SaveData();
			return false;

		}
	}
}

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TomoriBot
{
	class DataStorage
	{
		private static Dictionary<string, string> _pairs = new Dictionary<string, string>();

		public static bool SetPair(string key, string value)
		{
			if (_pairs.ContainsKey(key))
			{
				_pairs[key] = value;
				SaveData();
				return false;
			}

			_pairs.Add(key, value);
			SaveData();
			return true;
		}

		public static string GetPair(string key, out bool success)
		{
			if (!_pairs.ContainsKey(key))
			{
				success = false;
				return "";
			}

			success = true;
			return _pairs[key];
		}

		public static int GetPairCount()
		{
			return _pairs.Count;
		}

		private static void _loadData()
		{
			if (!_validateStorageFile("DataStorage.json")) return;
			string json = File.ReadAllText("DataStorage.json");
			_pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
		}

		static DataStorage()
		{
			_loadData();
		}

		public static void ResetData()
		{
			File.WriteAllText("DataStorage.json", "{}");
			_loadData();
		}

		public static void SaveData()
		{
			string json = JsonConvert.SerializeObject(_pairs, Formatting.Indented);
			File.WriteAllText("DataStorage.json",json);
		}

		private static bool _validateStorageFile(string file)
		{
			if (File.Exists(file)) return true;
			File.WriteAllText(file, "");
			SaveData();
			return false;

		}
	}
}

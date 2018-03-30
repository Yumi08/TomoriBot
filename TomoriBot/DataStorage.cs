using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TomoriBot
{
	class DataStorage<TKey, TValue>
	{
		private string filePath;

		private Dictionary<TKey, TValue> _pairs = new Dictionary<TKey, TValue>();

		public bool SetPair(TKey key, TValue value)
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

		public TValue GetPair(TKey key, out bool success)
		{
			if (!_pairs.ContainsKey(key))
			{
				success = false;
				return default(TValue);
			}

			success = true;
			return _pairs[key];
		}

		public int GetPairCount()
		{
			return _pairs.Count;
		}

		private void _loadData()
		{
			if (!_validateStorageFile(filePath)) return;
			string json = File.ReadAllText(filePath);
			_pairs = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(json);
		}

		public DataStorage(string filePath)
		{
			this.filePath = filePath;
			_loadData();
		}

		public void ResetData()
		{
			File.WriteAllText(filePath, "{}");
			_loadData();
		}

		public void SaveData()
		{
			string json = JsonConvert.SerializeObject(_pairs, Formatting.Indented);
			File.WriteAllText(filePath,json);
		}

		private bool _validateStorageFile(string file)
		{
			if (File.Exists(file)) return true;
			File.WriteAllText(file, "{}");
			SaveData();
			return false;

		}
	}
}

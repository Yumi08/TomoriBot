﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TomoriBot
{
	public class DataStorage<TKey, TValue>
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

		public bool PairExists(TKey key)
		{
			return _pairs.ContainsKey(key);
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

		public TValue GetPair(TKey key)
		{
			if (!_pairs.ContainsKey(key))
			{
				return default(TValue);
			}

			return _pairs[key];
		}

		public TValue GetOrCreatePair(TKey key)
		{
			if (!_pairs.ContainsKey(key))
			{
				_pairs.Add(key, default(TValue));
				SaveData();
			}

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

	public class DataStorage<T>
	{
		public DataStorage(string filePath)
		{
			_list = new List<T>();
			this._filePath = filePath;
		}

		private List<T> _list;
		private string _filePath;

		#region Helper Methods

		public List<T> ReturnList()
		{
			return _list;
		}

		public bool Add(T addition)
		{
			var listOfMatching = from a in _list
				where a.Equals(addition)
				select a;
			bool alreadyExists = listOfMatching.Any();

			if (alreadyExists) return false;

			_list.Add(addition);
			SaveData();

			return true;
		}
		
		#endregion

		#region Json Methods

		public void SaveData()
		{
			string json = JsonConvert.SerializeObject(_list, Formatting.Indented);
			File.WriteAllText(_filePath,json);
		}

		#endregion
	}
}

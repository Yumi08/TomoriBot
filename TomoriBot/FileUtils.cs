using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace TomoriBot
{
	/// <summary>
	/// Class for getting texts contained within files
	/// </summary>
	class FileUtils
	{
		private static Dictionary<string, string> _alerts;

		static FileUtils()
		{
			string json = File.ReadAllText("SystemLang/alerts.json");
			var data = JsonConvert.DeserializeObject<dynamic>(json);
			_alerts = data.ToObject<Dictionary<string, string>>();
		}

		public static string GetAlert(string key)
		{
			return _alerts.ContainsKey(key) ? _alerts[key] : "ERR: KEY NOT FOUND.";
		}

		public static string GetFormattedAlert(string key, params object[] parameter)
		{
			if (_alerts.ContainsKey(key))
			{
				return String.Format(_alerts[key], parameter);
			}
			return "";
		}

		public static string GetCommandHelp(int i)
		{
			return File.ReadAllText($"SystemLang/commandHelp{i}.txt");
		}
	}
}
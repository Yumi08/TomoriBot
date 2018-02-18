﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace TomoriBot
{
	class Utilities
	{
		private static Dictionary<string, string> _alerts;

		static Utilities()
		{
			string json = File.ReadAllText("SystemLang/alerts.json");
			var data = JsonConvert.DeserializeObject<dynamic>(json);
			_alerts = data.ToObject<Dictionary<string, string>>();
		}

		public static string GetAlert(string key)
		{
			return _alerts.ContainsKey(key) ? _alerts[key] : "ERR: KEY NOT FOUND.";
		}
	}
}
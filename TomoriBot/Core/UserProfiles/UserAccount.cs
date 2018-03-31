using System;
using System.Collections.Generic;

namespace TomoriBot.Core.UserProfiles
{
	public class UserAccount
	{
		// Meta
		public ulong Id { get; set; }

		// Utility
		public double LastMessageTime { get; set; }

		// Stats
		public uint Yen { get; set; }

		public uint Experience { get; set; }

		public uint TotalMessages { get; set; }

		public uint LevelNumber => (uint)Math.Sqrt(Experience / 150);


		public readonly Dictionary<ulong, string> Tags = new Dictionary<ulong, string>();

		public void AddTag(ulong userId, string value)
		{
			if (Tags.ContainsKey(userId))
			{
				Tags[userId] = value;
				return;
			}

			Tags.Add(userId, value);
			UserAccounts.SaveAccounts();
		}

		public string GetTag(ulong userId, out bool success)
		{
			if (!Tags.ContainsKey(userId))
			{
				success = false;
				return "";
			}

			success = true;
			return Tags[userId];
		}
	}
}

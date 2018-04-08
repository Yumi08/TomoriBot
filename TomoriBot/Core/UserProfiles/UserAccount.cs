using System;
using System.Collections.Generic;
using Discord;

namespace TomoriBot.Core.UserProfiles
{
	public class UserAccount
	{
		// META
		public ulong Id { get; set; }

		// UTILITY
		public double PreviousMessageTime { get; set; }

		// STATS
		public uint Yen { get; set; }

		// Previous amount of money earned on $daily
		public uint PreviousDailyAmount { get; set; }

		public uint DailyStreak { get; set; }

		public DateTime PreviousDailyTime { get; set; }

		public uint Experience { get; set; }

		public uint TotalMessages { get; set; }

		public uint LevelNumber => (uint)Math.Sqrt(Experience / 50);

		public ushort Iq { get; set; }

		#region Tags
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

		public void RemoveTag(ulong userId)
		{
			Tags.Remove(userId);
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
		#endregion
	}
}

using System;

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

		public uint Points { get; set; }

		//Inventory
		public uint Fish { get; set; }

		public uint Potatoes { get; set; }
	}
}

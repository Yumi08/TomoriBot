using System;

namespace TomoriBot.Core.UserProfiles
{
	public class UserAccount
	{
		public ulong Id { get; set; }

		public uint Yen { get; set; }

		public uint Experience { get; set; }

		public uint LevelNumber => (uint)Math.Sqrt(Experience / 50);

		public double LastMessageTime { get; set; }
	}
}

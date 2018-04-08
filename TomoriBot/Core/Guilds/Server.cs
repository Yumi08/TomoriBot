namespace TomoriBot.Core.Guilds
{
	public class Server
	{
		// META
		/// <summary>
		/// The server's discord Id
		/// </summary>
		public ulong Id { get; set; }
		public string Name { get; set; }

		// SETTINGS
		public bool EnableLevelUpMessages { get; set; }

		/// <summary>
		/// The bot's command prefix
		/// </summary>
		// TODO: Impliment this, it doesn't currently do anything.
		public char Prefix { get; set; }

		public bool EnableModerationModule { get; set; }
		public bool EnableProfilesModule { get; set; }
		public bool EnableEconomyModule { get; set; }
		public bool EnableFunModule { get; set; }
		public bool EnableNsfwModule { get; set; }
	}
}

using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace TomoriBot.Core.Guilds
{
	public static class Servers
	{
		private static List<Server> _servers;

		private static string _serversFile = "Resources/servers.json";

		static Servers()
		{
			if (DataStorage.SaveExists(_serversFile))
			{
				_servers = DataStorage.LoadServers(_serversFile).ToList();
			}
			else
			{
				_servers = new List<Server>();
				SaveServers();
			}
		}

		public static int ServerCount()
		{
			return _servers.Count;
		}

		public static void SaveServers()
		{
			DataStorage.SaveServers(_servers, _serversFile);
		}

		public static Server GetServer(SocketGuild guild)
		{
			var server = GetOrCreateServer(guild);
			server.Name = guild.Name;
			return server;
		}

		private static Server GetOrCreateServer(SocketGuild guild)
		{
			var result = from a in _servers
				where a.Id == guild.Id
				select a;

			var server = result.FirstOrDefault();
			if (server == null) server = CreateServer(guild);
			return server;
		}

		private static Server CreateServer(SocketGuild guild)
		{
			var newServer = new Server()
			{
				Id = guild.Id,
				Name = guild.Name,
				Prefix = Config.bot.cmdPrefix,
				EnableEconomyModule = true,
				EnableFunModule = true,
				EnableLevelUpMessages = true,
				EnableModerationModule = true,
				EnableNsfwModule = false,
				EnableProfilesModule = true
			};

			_servers.Add(newServer);
			SaveServers();
			return newServer;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace TomoriBot
{
	class Program
	{
		private DiscordSocketClient _client;
		private CommandHandler _handler;

		static void Main(string[] args)
			=> new Program().StartAsync().GetAwaiter().GetResult();

		public async Task StartAsync()
		{
			if (string.IsNullOrEmpty(Config.bot.token)) return;
			_client = new DiscordSocketClient(new DiscordSocketConfig
			{
				LogLevel = LogSeverity.Verbose
			});
			_client.Log += Log;
			await _client.LoginAsync(TokenType.Bot, Config.bot.token);
			await _client.StartAsync();
			_handler = new CommandHandler();
			await _handler.InitializeAsync(_client);
			await Task.Delay(-1);
		}

		private async Task Log(LogMessage msg)
		{
			Console.WriteLine(msg);
		}
	}
}

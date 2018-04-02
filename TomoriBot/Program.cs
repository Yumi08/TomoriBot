using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace TomoriBot
{
	class Program
	{
		// TODO: GET RELEASE - X64 TO WORK
		// NOTE: DEBUG IS CONFIGURED AS RELEASE

		private DiscordSocketClient _client;
		private CommandHandler _handler;

		static void Main()
		{
			new Program().StartAsync().GetAwaiter().GetResult();
		}

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

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg);
			return Task.CompletedTask;
		}
	}
}

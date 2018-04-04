using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using TomoriBot.Core.LevelingSystem;

namespace TomoriBot
{
	class CommandHandler
	{
		public event EventHandler<MessageReceievedEventArgs> MessageReceived; 

		public static DiscordSocketClient Client;
		private CommandService _service;

		public async Task InitializeAsync(DiscordSocketClient client)
		{
			Client = client;
			_service = new CommandService();
			await _service.AddModulesAsync(Assembly.GetEntryAssembly());
			Client.MessageReceived += HandleCommandAsync;
		}

		private async Task HandleCommandAsync(SocketMessage s)
		{
			var msg = (SocketUserMessage) s;
			if (msg == null) return;
			var context = new SocketCommandContext(Client, msg);
			if (context.User.IsBot) return;

			if (context.IsPrivate) goto TrivialEnd;

			#region Trivial
			// Leveling up
			Leveling.UserSentMessage((SocketGuildUser)context.User, (SocketTextChannel)context.Channel);

			// Spams KannaMagik reaction if enabled (toggled by command)
			if (Global.SpamKanna) await msg.AddReactionAsync(context.Guild.Emotes.First(e => e.Id == 398211422217306123));
			#endregion

			TrivialEnd:
			var argPos = 0;
			if (!msg.HasStringPrefix(Config.bot.cmdPrefix, ref argPos))
			{
				MessageReceievedEventArgs e = new MessageReceievedEventArgs {Msg = msg};
				OnMessageReceived(e);
			}

				if ((msg.HasStringPrefix(Config.bot.cmdPrefix, ref argPos)
			|| msg.HasMentionPrefix(Client.CurrentUser, ref argPos)) && !context.IsPrivate)
			{
				var result = await _service.ExecuteAsync(context, argPos);

				if (!result.IsSuccess /*&& result.Error != CommandError.UnknownCommand*/)
				{
					Console.WriteLine(result.ErrorReason);
				}
			}
		}

		public struct MessageReceievedEventArgs
		{
			public SocketUserMessage Msg { get; set; }
		}

		protected virtual void OnMessageReceived(MessageReceievedEventArgs e)
		{
			MessageReceived?.Invoke(this, e);
		}
	}
}

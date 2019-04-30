using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sora.Features;
using Log = Sora.Logging;
using System.Reflection;
using System.Linq;
using System.Threading;
using Discord.API;
using Discord.Rest;

namespace Sora
{
	public class Bot
	{
		public static Bot Instance { get; private set; }

		public List<Feature> lFeatures;

		public static string asm_dir => Assembly.GetExecutingAssembly().Location
			.Remove(Assembly.GetExecutingAssembly().Location.Length - 8);

		[Savable] public static string sz_token = "";

		[Savable] public static string prefix = ".";

		public bool bInit = false;

		public DiscordSocketClient client;


		private void SilentStart()
		{
			client = new DiscordSocketClient();

			client.Log += LogAsync;
			client.Ready += BotStarted;
		}


		public static async Task StartBot()
		{
			if (File.Exists(Environment.CurrentDirectory + "/log.txt"))
				File.Delete(Environment.CurrentDirectory + "/log.txt");

			Instance = new Bot();
			Instance.SilentStart();
			if (sz_token == "")
			{
				Log.Log("Token is not set in config!");
				return;
			}


			await Instance.client.LoginAsync(TokenType.User, sz_token);
			await Instance.client.StartAsync();
            Instance.client.MessageReceived += Instance.MessageReceivedAsync;
			Instance.client.MessageDeleted += Instance.MessageDeletedAsync;
			Instance.client.LeftGuild += Instance.LeftGuildAsync;
			Instance.client.MessageUpdated += Instance.MessageUpdatedAsync;
		}

		private async Task MessageReceivedAsync(SocketMessage message)
		{
			try
			{
				if (!Bot.Instance.bInit) return;

				if (message.Author.Id != client.CurrentUser.Id)
				{
					lFeatures.ForEach(feat => { feat.OnMessageReceive(message); });

					var users = new List<IUser>();
					users.AddRange(message.MentionedUsers);
					if (users.Count(x => x.Id == client.CurrentUser.Id) == 1)
					{
						lFeatures.ForEach(feat => { feat.OnMentioned(message); });
					}
                    return;
				}

				if (message.Content.StartsWith(prefix))
					await Command.Run(message, message.Content.Split(' '));
                    else
                    	lFeatures.ForEach(feat => { feat.OnMessageSent(message); });
			}
			catch
			{
				await message.Channel.SendMessageAsync("Ok Not Good..");
			}
		}

		private Task MessageUpdatedAsync(Cacheable<IMessage, ulong> msgOld, SocketMessage message,
			ISocketMessageChannel channel)
		{
			try { lFeatures.ForEach(feat => { feat.OnMessageUpdated(msgOld, message, channel); }); } catch { }
			return Task.CompletedTask;
		}

		private Task LeftGuildAsync(SocketGuild arg)
		{
			try { lFeatures.ForEach(feat => { feat.OnGuildLeft(arg); }); } catch { }
			return Task.CompletedTask;
		}

		private Task MessageDeletedAsync(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
		{
			lFeatures.ForEach(feat =>
			{
				feat.OnMessageDelete(message, channel);
			});

			return Task.CompletedTask;
		}

		private Task BotStarted()
		{
			Console.WriteLine($"{client.CurrentUser} is connected!");


			lFeatures = new List<Feature>()
			{
				new Evil(),
				new Utils(),
				new RepeatMessage(),
				new ColorChanger()
			};

			bInit = true;

			return Task.CompletedTask;
		}

		private Task LogAsync(LogMessage arg)
		{
			Console.WriteLine(arg.ToString());
			return Task.CompletedTask;
		}


	}
}

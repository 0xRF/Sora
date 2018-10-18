using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sora.Features;
using Log = Sora.Logging;
using System.Reflection;
using System.Linq;

namespace Sora
{
    public class Bot
    {
        public static Bot Instance { get; private set; }

        private List<Feature> lFeatures;

        public static string asm_dir => Assembly.GetExecutingAssembly().Location
            .Remove(Assembly.GetExecutingAssembly().Location.Length - 8);

        [Savable] private static string sz_token = "";

        [Savable] public static string prefix = ".";

        public DiscordSocketClient client;

        private void SilentStart()
        {
            client = new DiscordSocketClient();

            client.Log += LogAsync;
            client.Ready += BotStarted;
            client.MessageReceived += MessageReceivedAsync;
            client.MessageDeleted += MessageDeletedAsync;
            client.LeftGuild += LeftGuildAsync;
            client.MessageUpdated += MessageUpdatedAsync;
        }


        public static async Task StartBot()
        {
            Instance = new Bot();
            Instance.SilentStart();
            if (sz_token == "")
            {
                Log.Log("Token is not set in config!");
                return;
            }


            await Instance.client.LoginAsync(TokenType.User, sz_token);
            await Instance.client.StartAsync();





        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            lFeatures.ForEach(feat => { feat.OnMessageReceive(message); });

            var users = new List<IUser>();
            users.AddRange(message.MentionedUsers);
            if (users.Count(x => x.Id == client.CurrentUser.Id) == 1)
            {
                lFeatures.ForEach(feat => { feat.OnMentioned(message); });
            }

            if (message.Author.Id != client.CurrentUser.Id)
                return;

            if (message.Content.StartsWith(prefix))
                await Command.Run(message, message.Content.Split(' '));
        }

        private Task MessageUpdatedAsync(Cacheable<IMessage, ulong> msgOld, SocketMessage message,
            ISocketMessageChannel channel)
        {

            lFeatures.ForEach(feat => { feat.OnMessageUpdated(msgOld, message, channel); });

            return Task.CompletedTask;
        }

        private Task LeftGuildAsync(SocketGuild arg)
        {
            lFeatures.ForEach(feat => { feat.OnGuildLeft(arg); });

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

        private async Task BotStarted()
        {
            Console.WriteLine($"{client.CurrentUser} is connected!");


            lFeatures = new List<Feature>()
            {
               new Evil(), new UserInfo(), new RepeatMessage()
            };

            await RepeatMessage.HandleRepeat();
            
        //    return Task.CompletedTask;
        }

        private Task LogAsync(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }


    }
}

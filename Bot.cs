using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Log = Sora.Logging;
namespace Sora
{
    public class Bot
    {
        public static Bot Instance {get;private set;}


        [Savable]
        private static string sz_token = "";

        [Savable]
        public static string prefix = ".";

        public DiscordSocketClient client;

        public static async Task StartBot()
        {
            Instance = new Bot();

            if (sz_token == "")
            {
                Log.Log("Token is not set in config!");
                return;
            }


            Instance.client = new DiscordSocketClient();

            Instance.client.Log += Instance.LogAsync;
            Instance.client.Ready += Instance.ReadyAsync;
            Instance.client.MessageReceived += Instance.MessageReceivedAsync;
    
            await Instance.client.LoginAsync(TokenType.User, sz_token);
            await Instance.client.StartAsync();
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id != client.CurrentUser.Id)
                return;

            if(message.Content.StartsWith(prefix))
            await Command.Run(message.Channel, message.Content.Split(' '));

        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        private Task LogAsync(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }


    }
}

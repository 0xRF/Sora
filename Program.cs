using System;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace Sora
{
    
    class Program
    {
        static void Main(string[] args)
                 => new Program().MainAsync().GetAwaiter().GetResult();

        [Command("yeet",3)]
        public static void Yeet()
        {
            
        }
        
        DiscordSocketClient _client; 
        public async Task MainAsync()
        {
            Command.MapCommands();
            
            
            Config.GetSavedData();
            
             _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;

            await _client.LoginAsync(TokenType.User, "");
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "!ping")
                await message.Channel.SendMessageAsync("pong!");
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        private Task LogAsync(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }
    }
}

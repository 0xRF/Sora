using System;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Log = Sora.Logging;
namespace Sora
{
    
    class Program
    {
        static void Main(string[] args)
                 => Program.EntryPoint().GetAwaiter().GetResult();
        
  
        public static async Task EntryPoint()
        {
            Command.MapCommands();    
            Config.GetSavedData();

            await Bot.StartBot();

            await Task.Delay(-1);
        }

        [Command("yeet")]
        public static void Function(string yeet, string yoot)
        {
            Console.WriteLine("YEET");

        }
    }
}

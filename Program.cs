using System;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Log = Sora.Logging;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sora
{
    
    class Program
    {
        static void Main(string[] args) => EntryPoint().GetAwaiter().GetResult();
  
        public static async Task EntryPoint()
        {
            Command.MapCommands();    
            Config.GetSavedData();

            await Bot.StartBot();

            await Task.Delay(-1);
        }

    }
}

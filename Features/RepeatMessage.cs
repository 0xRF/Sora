using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Discord;
using Discord.Net.Rest;
using Discord.API;
using Discord.Rest;
using Newtonsoft.Json.Linq;

namespace Sora.Features
{
    public class RepeatMessage : Feature
    {
        public ulong channelID;
        public string szMessage;
        public int repeatTime;

        public RepeatMessage()
        {
        }

        public RepeatMessage(ulong channelId, string mess, int repeat)
        {
            this.channelID = channelId;
            this.szMessage = mess;
            this.repeatTime = repeat;
        }

        //[Savable] 
        private static Dictionary<string, RepeatMessage> _repeatMessages =
            new Dictionary<string, RepeatMessage>();



        [Command("repeat")]
        public static Task AddMessage(SocketMessage sm, string index, string message, string szRepeat)
        {
            if (int.TryParse(szRepeat, out int repeat))
            {
                _repeatMessages.Add(index, new RepeatMessage(sm.Channel.Id, message, repeat));
            }

            return Task.CompletedTask;
        }



    }
}

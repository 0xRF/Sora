using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sora
{
    public class Feature
    {

        public virtual void OnMessageReceive(SocketMessage message){}
        public virtual void OnMessageUpdated(Cacheable<IMessage, ulong> msgOld, SocketMessage message, ISocketMessageChannel channel) { }
        public virtual void OnMessageDelete(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel) { }
        public virtual void OnGuildLeft(SocketGuild guild) { }
        public virtual void OnMentioned(SocketMessage message) { }




    }
}

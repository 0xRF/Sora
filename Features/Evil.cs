using Discord;
using Discord.Net.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Sora.Features
{
    public class Evil : Feature
    {
        private static Dictionary<ulong, IDisposable> zombieDisposable = new Dictionary<ulong, IDisposable>();
        private static List<ulong> skeletonGuilds = new List<ulong>();
        private static bool ainz = false;

        public override void OnMentioned(SocketMessage message)
        {
            message.Channel.SendFileAsync(Bot.asm_dir + "/ping.png");
        }



        [Command("kys", description: "mass dm")]
        public static async Task Kys(SocketMessage message, string url)
        {

            var users = new List<IUser>();
            users.AddRange(await message.Channel.GetUsersAsync(CacheMode.AllowDownload).Flatten());
            await message.DeleteAsync();


            string msg = "";
            ulong id = Bot.Instance.client.CurrentUser.Id;


            users.ForEach(user =>
                {
                    if (user.Id != id)
                    {
                        user.SendMessageAsync(url);
                    }
                }
            );


            var sentMsg = await message.Channel.SendMessageAsync(msg);

            await sentMsg.DeleteAsync();
        }

        [Command("shubbub", description: "silent mass mention")]
        public static async Task Shubbub(SocketMessage message)
        {

            var users = new List<IUser>();
            users.AddRange(await message.Channel.GetUsersAsync(CacheMode.AllowDownload).Flatten());

            ulong id = Bot.Instance.client.CurrentUser.Id;

            string msg = "";

            users.ForEach(user =>
            {
                if (user.Id != id)
                    msg += user.Mention + ' ';
            });

            var sentMsg = await message.Channel.SendMessageAsync(msg);
            await message.DeleteAsync();
            await sentMsg.DeleteAsync();

        }

        [Command("magic", description: "Age old trick of vanishing")]
        public static async Task Magic(SocketMessage message)
        {
            string spam = "";
            spam += '\u2800';
            for (int i = 0; i < 1998; i++)
                spam += '\n';
            spam += '\u2800';

            await message.DeleteAsync();

            await message.Channel.SendMessageAsync(spam);
            await message.Channel.SendMessageAsync(spam);
            await message.Channel.SendMessageAsync(spam);
        }

        [Command("zombie", description: "Always typing in channel")]
        public static async Task AlwaysTypingInChannel(SocketMessage message)
        {
            await message.DeleteAsync();

            if (zombieDisposable.ContainsKey(message.Channel.Id))
            {
                zombieDisposable[message.Channel.Id].Dispose();
                zombieDisposable.Remove(message.Channel.Id);
            }
            else
                zombieDisposable.Add(message.Channel.Id, message.Channel.EnterTypingState());
        }

        [Command("skeleton", description: "Always typing in guild {bad}")]
        public static async Task AlwaysTypingInGuild(SocketMessage message)
        {
            await message.DeleteAsync();

            var guild = ((SocketGuildChannel)message.Channel).Guild;

            if (skeletonGuilds.Contains(guild.Id))
            {
                foreach (var gchannel in guild.Channels)
                {
                    var channel = gchannel as ISocketMessageChannel;
                    if (channel == null)  continue;

                    if (zombieDisposable.ContainsKey(channel.Id))
                    {
                        zombieDisposable[channel.Id].Dispose();
                        zombieDisposable.Remove(channel.Id);
                    }
                }
                skeletonGuilds.Remove(guild.Id);
            }
            else
            {
                foreach (var gchannel in guild.Channels)
                {
                    var channel = gchannel as ISocketMessageChannel;
                    if (channel == null) continue;

                    if (!zombieDisposable.ContainsKey(channel.Id))
                    {
                        zombieDisposable.Add(channel.Id, channel.EnterTypingState());
                    }
                }
                skeletonGuilds.Add(guild.Id);
            }
        }
    }
}

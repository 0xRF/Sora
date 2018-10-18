using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        [Command("shubbub")]
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

        [Command("magic")]
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

        [Command("zombie")]
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

        [Command("skeleton")]
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

        [Command("ainz")]
        public static async Task AlwaysTyping(SocketMessage message)
        {
            var guilds = Bot.Instance.client.Guilds;
            await message.Channel.SendMessageAsync("Ainz mode get's rate limited, huge rip..");
            await message.DeleteAsync();
            if (!ainz)
            {
                foreach (var guild in guilds)
                {
                    var gId = guild.Id;

                    if (skeletonGuilds.Contains(gId)) continue;

                    var channels = guild.Channels;

                    foreach (var gchannel in channels)
                    {
                        var channelId = gchannel.Id;

                        if (!(gchannel is ISocketMessageChannel channel)) continue;
                        if (zombieDisposable.ContainsKey(channelId)) continue;
                      
                        zombieDisposable.Add(channelId, channel.EnterTypingState());
                        await Task.Delay(400);
                    }

                    skeletonGuilds.Add(gId);
                    await Task.Delay(5000);
                }
                await message.Channel.SendMessageAsync("Ainz mode enabled, gg?");
                ainz = true;
            }
            else
            {
                foreach (var guild in guilds)
                {
                    var gId = guild.Id;

                    if (!skeletonGuilds.Contains(gId)) continue;
                    
                    var channels = guild.Channels;
                    foreach (var gchannel in channels)
                    {
                        var channelId = gchannel.Id;

                        if (!(gchannel is ISocketMessageChannel channel)) continue;
                        if (!zombieDisposable.ContainsKey(channelId)) continue;
                        
                        zombieDisposable[channelId].Dispose();
                        zombieDisposable.Remove(channelId);
                        await Task.Delay(400);
                    }
                    skeletonGuilds.Remove(gId);
                    skeletonGuilds.Remove(gId); await Task.Delay(2000);

                }
                await message.Channel.SendMessageAsync("Ainz mode disabled, gg?");
                ainz = false;
            }

        }
    }
}

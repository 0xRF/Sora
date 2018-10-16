using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sora.Features
{
    public class Evil : Feature
    {
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


    }
}

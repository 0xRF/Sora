using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Sora.Features
{
    public class UserInfo : Feature
    {
        
        [Command("save")]
        public static async Task Save(SocketMessage sm)
        { 
            Config.SaveSettings();
            await sm.Channel.SendMessageAsync("Saved bot.");
            string cfg = File.ReadAllText(Config.sz_config_dir);
            await sm.Channel.SendMessageAsync("```" + cfg +"```");

        }
        
        [Command("av")]
        public static async Task GrabAvatar(SocketMessage sm, string msg)
        {
            IUser user = null;

            if (sm.MentionedUsers.Count == 1)
                user = sm.MentionedUsers.First();
            else
               user = await Tools.GetUser(sm.Channel, msg);

            if(user == null)
            {
                await sm.Channel.SendMessageAsync("Failed to find user");
                return;
            }

            string avUrl = user.GetAvatarUrl(ImageFormat.Auto);
            var eb = new EmbedBuilder();
            eb.WithColor(Color.Red);
            eb.WithTitle(user.Username + "'s Avatar");
            eb.WithUrl(avUrl);
            eb.WithImageUrl(avUrl);
            
            await sm.DeleteAsync();
            await sm.Channel.SendMessageAsync("", false, eb);
        }


    }
}

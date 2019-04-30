using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Sora.Features
{
	public class ColorChanger : Feature
	{
		public static string generateJsonForRole(SocketRole role, Color color) => "{\"color\": " + color.RawValue + "}";

		private static Discord.Color[] colors =
			{Color.Blue, Color.Red, Color.Teal, Color.Magenta, Color.Orange, Color.Purple};

		[Savable]
		public static bool bAutoChange = false;

		private static bool bTaskRunning = false;


		public static Task ChangeColour(SocketRole role, Color color)
		{
			var url = $"https://discordapp.com/api/v6/guilds/{role.Guild.Id}/roles/{role.Id}";
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "PATCH";
			request.Timeout = 12000;
			request.ContentType = "application/json";
			request.Headers.Add("authorization", Bot.sz_token);

			using (var streamWriter = new StreamWriter(request.GetRequestStream()))
			{
				string json = generateJsonForRole(role, color);

				streamWriter.Write(json);
				streamWriter.Flush();
				streamWriter.Close();
			}

			var httpResponse = (HttpWebResponse)request.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				var result = streamReader.ReadToEnd();
			}
			return Task.CompletedTask;
		}

		public static async Task SpamColour(SocketRole role, int amount)
		{
			bTaskRunning = true;

			int index = 0;
			for (int i = 0; i < amount; i++)
			{
				if (index < colors.Length - 1)
					index++;
				else
					index = 0;

				await ChangeColour(role, colors[index]);
				await Task.Delay(150);
			}

			bTaskRunning = false;
		}



		[Command("autocol", description: "every msg change color of role")]
		public static async Task AutoCMD(SocketMessage message)
		{
            await message.Channel.SendMessageAsync($"AutoCol {!bAutoChange}");
            bAutoChange = !bAutoChange;
		}

        private static async Task DoStuff(SocketMessage message){
            SocketRole rl = null;
			var guild = ((SocketGuildChannel)message.Channel).Guild;
			rl  = guild.Roles.Where(x => x.Members.Contains(message.Author)).ToList().OrderByDescending(x => x.Position).ElementAt(1);
                if(!bTaskRunning){
                   await SpamColour(rl, 15);
                }
        }
    
        public override void OnMessageSent(SocketMessage message){
	          Task.Run(() => DoStuff(message));   
        }


		[Command("color", description: "Spam change color of role")]
		public static async Task ColorCMD(SocketMessage message, string role, string szAmount)
		{
			SocketRole rl = null;
			var guild = ((SocketGuildChannel)message.Channel).Guild;

			var roles = guild.Roles.ToList().Where(x => Tools.FuzzyMatch(x.Name, role));
			rl = roles.First();
			if (rl == null)
			{
				await message.Channel.SendMessageAsync("Couldnt find role");
			}

			int amount;

			if (!int.TryParse(szAmount, out amount))
			{
				await message.Channel.SendMessageAsync("2nd Param is not a number");
				return;
			}

			if (!bTaskRunning)
			{
				var begin_msg = await message.Channel.SendMessageAsync("Task Began");

				await SpamColour(rl, amount);

				await begin_msg.DeleteAsync();
			}
			else
			{
                await message.Channel.SendMessageAsync("Task already running");
			}
		}

	}
}

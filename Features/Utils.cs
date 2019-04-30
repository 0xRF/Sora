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
	public class Utils : Feature
	{
		[Command("cmds", description: "Lists all commanads")]
		public static async Task Commands(SocketMessage sm)
		{
			string commands = "";
			Dictionary<Command, List<string>> aliases = new Dictionary<Command, List<string>>();
			foreach (var fCmd in Command.functionMap)
			{
				if (aliases.ContainsKey(fCmd.Value))
					aliases[fCmd.Value].Add(fCmd.Key);
				else
					aliases.Add(fCmd.Value, new List<string> { fCmd.Key });
			}

			foreach (var fCmd in aliases)
			{
				foreach (var als in fCmd.Value)
				{
					if (als != fCmd.Value.Last())
						commands += als + "|";
					else
						commands += als + "\n";
				}
			}

			var eb = new EmbedBuilder();
			eb.WithColor(Color.Red);
			eb.WithDescription(commands);
			await sm.Channel.SendMessageAsync("", false, eb);
		}


		[Command("save", description: "Save's config")]
		public static async Task Save(SocketMessage sm)
		{
			Config.SaveSettings();
			await sm.Channel.SendMessageAsync("Saved bot.");
			string cfg = File.ReadAllText(Config.sz_config_dir);
		}


		[Command("man", description: "Get Information on command")]
		public static async Task Manual(SocketMessage sm, string cmd)
		{

			if (cmd == "all")
			{

				var emb = new EmbedBuilder();
				emb.WithColor(Color.Red);
				emb.WithTitle("Man of All Cmds");

				foreach (var cm in Command.functionMap)
				{
					emb.Description += "**" + cm.Key + ":**\n";
					emb.Description += cm.Value.szDescription + "\n";
					if (cm.Value.paramaters.Count > 0)
						emb.Description += "**Params:**\n" + string.Join("\n", cm.Value.paramaters.ToArray());
                    emb.Description += "\n";
				}
				emb.Build();
				await sm.Channel.SendMessageAsync("", embed: emb);

				return;
			}

			if (!Command.functionMap.ContainsKey(cmd))
			{
				var msg = await sm.Channel.SendMessageAsync("Not a command....");
				await Task.Delay(2000);
				await sm.DeleteAsync();
				await msg.DeleteAsync();
				return;
			}
			var eb = new EmbedBuilder();
			eb.WithColor(Color.Red);
			eb.WithTitle("Description of " + cmd);
			eb.WithDescription(Command.functionMap[cmd].szDescription + "\n");

			if (Command.functionMap[cmd].paramaters.Count > 0)
				eb.Description += "**Params:**\n" + string.Join("\n", Command.functionMap[cmd].paramaters.ToArray());
			eb.Build();
			await sm.Channel.SendMessageAsync("", embed: eb);

		}



		[Command("av", description: "Grabs a user's avatar")]
		public static async Task GrabAvatar(SocketMessage sm, string user)
		{
			IUser _user = null;

			if (sm.MentionedUsers.Count == 1)
				_user = sm.MentionedUsers.First();
			else
				_user = await Tools.GetUser(sm.Channel, user);

			if (_user == null)
			{
				await sm.Channel.SendMessageAsync("Failed to find user");
				return;
			}

			string avUrl = _user.GetAvatarUrl(ImageFormat.Auto);
			var eb = new EmbedBuilder();
			eb.WithColor(Color.Red);
			eb.WithTitle(_user.Username + "'s Avatar");
			eb.WithUrl(avUrl);
			eb.WithImageUrl(avUrl);

			await sm.DeleteAsync();
			await sm.Channel.SendMessageAsync("", embed: eb);
		}


	}
}

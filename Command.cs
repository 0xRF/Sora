using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Log = Sora.Logging;
namespace Sora
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class Command : Attribute
    {
        public string szInvoke = String.Empty;
        public string szFailed = "";
        public int cParams = 0;
        public MethodInfo method;
        public bool bSilent = false;

        public Command(string cmd, bool silent = false)
        {
            szInvoke = cmd;
            bSilent = silent;
        }

        public Command(string cmd, string failed)
        {
            szInvoke = cmd;
            szFailed = failed;
            bSilent = false;
        }

        private static Dictionary<string, List<Command>> functionMap = new Dictionary<string, List<Command>>();

        private static BindingFlags _flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic |
                                            BindingFlags.Public;

        public static void MapCommands()
        {
            Assembly.GetExecutingAssembly().GetTypes().ToList().ForEach(x =>
            {
                var functions = x.GetMethods().Where(f => f.GetCustomAttribute<Command>() != null).ToList();
                
                functions.ForEach(func =>
                    func.GetCustomAttributes<Command>().ToList().ForEach(pAtt =>
                    {
                        pAtt.method = func;
                        pAtt.cParams = func.GetParameters().Length;

                        if(functionMap.ContainsKey(pAtt.szInvoke))
                        functionMap[pAtt.szInvoke].Add(pAtt);
                        else
                            functionMap.Add(pAtt.szInvoke, new List<Command>(){pAtt});
                    }));
            });
            
            Log.Log("Funcs " + functionMap.Count);
        }

        public static async Task Run(ISocketMessageChannel channel, params string[] messages)
        {
            var szCmd = messages[0].Remove(0, Bot.prefix.Length);

            if (functionMap.ContainsKey(szCmd))
            {
                foreach (var cmd in functionMap[szCmd])
                {
                    if(messages.Count() - 1 == cmd.cParams)
                    {
                        var b = messages.ToList();
                        b.RemoveAt(0);
                        cmd.method.Invoke(null, b.ToArray());
                    }
                    else
                    {

                        if(!cmd.bSilent)
                        {
                            if (cmd.szFailed == "")
                                await channel.SendMessageAsync($"Failed!, expected {cmd.cParams} got {messages.Count() - 1}");
                            else
                                await channel.SendMessageAsync(cmd.szFailed);
                        }

                    }
                }
            }
            
        }



    }
}

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
        private string szInvoke = String.Empty;
        private string szFailed = "";
        private int cParams = 0;
        private MethodInfo method;
        private bool bSilent = false;
        private bool bRequiresMessageInfo = false;
        

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

        private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic |
                                            BindingFlags.Public;

        public static void MapCommands()
        {
            Assembly.GetExecutingAssembly().GetTypes().ToList().ForEach(x =>
            {
                var functions = x.GetMethods().Where(f => f.GetCustomAttribute<Command>() != null).ToList();

                functions.ForEach(func =>
                {
                    if (func.ReturnType != typeof(Task))
                    {
                        Log.Log(func.Name + " was not a valid function");
                        return;
                    }
                    func.GetCustomAttributes<Command>().ToList().ForEach(pAtt =>
                    {
                        pAtt.method = func;
                        pAtt.cParams = func.GetParameters().Length;

                        pAtt.bRequiresMessageInfo = func.GetParameters()[0].ParameterType == typeof(SocketMessage);

                        if (functionMap.ContainsKey(pAtt.szInvoke))
                            functionMap[pAtt.szInvoke].Add(pAtt);
                        else
                            functionMap.Add(pAtt.szInvoke, new List<Command>() { pAtt });
                    });
                });
            });
            
            Log.Log("Funcs " + functionMap.Count);
        }

        public static async Task Run(SocketMessage sm, params string[] messages)
        {
            var szCmd = messages[0].Remove(0, Bot.prefix.Length);

            if (functionMap.ContainsKey(szCmd))
            {
                foreach (var cmd in functionMap[szCmd])
                {

                    if(messages.Count() - 1 == cmd.cParams || (cmd.bRequiresMessageInfo && messages.Count() - 1 == cmd.cParams - 1))
                    {
                        var b = messages.ToList();
                        b.RemoveAt(0);
                        
                        var objects = new object[cmd.cParams];
                        if (cmd.bRequiresMessageInfo)
                        {
                            objects[0] = sm;
                            for (int i = 0; i < b.Count(); i++)
                                objects[i + 1] = b[i];
                        }
                        else
                            objects = b.ToArray();
                            
                        await (Task)cmd.method.Invoke(null, objects);
                    }
                    else
                    {

                        if(!cmd.bSilent)
                        {
                            if (cmd.szFailed == "")
                                await sm.Channel.SendMessageAsync($"Failed!, expected {(cmd.bRequiresMessageInfo ? cmd.cParams - 1 : cmd.cParams)} items, got {messages.Count() - 1}");
                            else
                                await sm.Channel.SendMessageAsync(cmd.szFailed);
                        }

                    }
                }
            }
            
        }



    }
}

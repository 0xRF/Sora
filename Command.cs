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
        private string szFailed;
        
        public string szDescription;
        private int cParams = 0;
        private MethodInfo method;
        private bool bSilent = false;
        private bool bRequiresMessageInfo = false;
        
        public List<string> paramaters = new List<string>();

        public Command(string cmd, bool silent = false, string failed = "", string description = "")
        {
            szInvoke = cmd;
            bSilent = silent;
            szFailed = failed;
            szDescription = description;
        }


        public static Dictionary<string, Command> functionMap = new Dictionary<string, Command>();

        private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static |
                                                     BindingFlags.NonPublic |
                                                     BindingFlags.Public;

        public static void MapCommands()
        {
            Assembly.GetExecutingAssembly().GetTypes().ToList().ForEach(x =>
            {
                var functions = x.GetMethods().Where(f => f.GetCustomAttributes<Command>().Any()).ToList();

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

                        for(int i = pAtt.bRequiresMessageInfo ? 1 : 0 ; i < pAtt.cParams; i++){
                            pAtt.paramaters.Add(func.GetParameters()[i].Name);
                        }
                        
                        functionMap.Add(pAtt.szInvoke, pAtt);
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
                var cmd = functionMap[szCmd];


                if (messages.Count() - 1 == cmd.cParams ||
                    (cmd.bRequiresMessageInfo && messages.Count() - 1 == cmd.cParams - 1))
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

                    Task.Run(() => cmd.method.Invoke(null, objects));
                }
                else
                {

                    if (!cmd.bSilent)
                    {
                        if (cmd.szFailed == "")
                            await sm.Channel.SendMessageAsync(
                                $"Failed!, expected {(cmd.bRequiresMessageInfo ? cmd.cParams - 1 : cmd.cParams)} items, got {messages.Count() - 1}");
                        else
                            await sm.Channel.SendMessageAsync(cmd.szFailed);
                    }

                }
            }   
        }
    }
}

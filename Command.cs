using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sora
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class Command : Attribute
    {
        public string szInvoke = String.Empty;
        public int cParams;
     
        public Command(string cmd, int paramCount)
        {
            szInvoke = cmd;
            cParams = paramCount;
        }

        private static Dictionary<string, List<Tuple<int, MethodInfo>>> functionMap = new Dictionary<string, List<Tuple<int, MethodInfo>>>();

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
                        if(functionMap.ContainsKey(pAtt.szInvoke))
                        functionMap[pAtt.szInvoke].Add(new Tuple<int,MethodInfo>(pAtt.cParams, func));
                        else
                            functionMap.Add(pAtt.szInvoke, new List<Tuple<int, MethodInfo>>(){new Tuple<int,MethodInfo>(pAtt.cParams, func)});
                    }));
            });
            
            Logging.Log("Funcs " + functionMap.Count);
        }

        public static void CommandRun(params string[] objects)
        {
            
            
            
        }



    }
}

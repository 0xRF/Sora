using System;
using System.IO;

namespace Sora
{
    public class Logging
    {
        public static void Log(params object[] szlogs)
        {
            foreach (var msg in szlogs)
            {
                File.AppendAllText(Environment.CurrentDirectory +"/log.txt", msg.ToString());
                Console.WriteLine(msg.ToString());
            }
        }
    }
}
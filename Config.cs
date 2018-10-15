using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Log = Sora.Logging;
namespace Sora
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class Savable : Attribute{}
      
    public class Config
    {

        private static string sz_config_dir => Assembly.GetExecutingAssembly().Location + "/config.json";
        
        public static void GetSavedData()
        {
            Console.WriteLine(sz_config_dir);
            
            if (!File.Exists(sz_config_dir))
            {
                Log.Log("Failed to load config");
                return;
            }
        
        
        }
        
        
    }
}
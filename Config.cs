using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Log = Sora.Logging;
namespace Sora
{

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class SavableAttribute : Attribute{}
      
    public class Config
    {

        private static string sz_config_dir => Bot.asm_dir + "/config.json";
        
        public static void GetSavedData()
        {
            Console.WriteLine(sz_config_dir);
            
            if (!File.Exists(sz_config_dir))
            {
                Log.Log("Failed to load config, using defaults");
                SaveSettings();
                return;
            }
        
            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(sz_config_dir));

            if (values == null)
            {
                Log.Log("Broken Config");
                return;
            }

            values.ToList().ForEach(savaData => Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.Name == savaData.Key.Split('.')[0]).ToList()
            .ForEach(t => t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(field => Attribute.GetCustomAttribute(field, typeof(SavableAttribute)) != null)
            .Where(field => field.Name == savaData.Key.Split('.')[1]).ToList()
            .ForEach(field => field.SetValue(null, savaData.Value))));
        }


        public static void SaveSettings()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            Assembly.GetExecutingAssembly().GetTypes().ToList()
                .ForEach(type => type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => Attribute.GetCustomAttribute(field, typeof(SavableAttribute)) != null).ToList()
                .ForEach(toSave => values.Add($"{type.Name}.{toSave.Name}", toSave.GetValue(null))));

            var json = JsonConvert.SerializeObject(values, Formatting.Indented);

            File.WriteAllText(sz_config_dir, json);
        }


    }
}
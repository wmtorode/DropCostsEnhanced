using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using IRBTModUtils.Logging;
using System.IO;
using Newtonsoft.Json;
using DropCostsEnhanced.Data;

namespace DropCostsEnhanced
{
    public class DCECore
    {
        internal static DeferringLogger modLog;
        internal static Settings settings;
        internal static string modDir;

        public static void Init(string modDirectory, string settingsJSON)
        {
            
            modDir = modDirectory;
            modLog = new DeferringLogger(modDirectory, "DropCostEnhanced", "DCE", true, false);

            try
            {
                using (StreamReader reader = new StreamReader($"{modDir}/settings.json"))
                {
                    string jdata = reader.ReadToEnd();
                    settings = JsonConvert.DeserializeObject<Settings>(jdata);
                }

            }

            catch (Exception ex)
            {
                modLog.Error?.Write(ex);
            }

            var harmony = HarmonyInstance.Create("ca.jwolf.DropCostsEnhanced");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

        }
    }
}

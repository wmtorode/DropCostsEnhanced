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
using CustomComponents;

namespace DropCostsEnhanced
{
    public class DCECore
    {
        internal static DeferringLogger modLog;
        internal static Settings settings;
        internal static string modDir;
        internal static DateTime cacheValidUntil;
        internal static float cachedDifficulty;

        public static void Init(string modDirectory, string settingsJSON)
        {
            
            modDir = modDirectory;
            

            try
            {
                using (StreamReader reader = new StreamReader($"{modDir}/settings.json"))
                {
                    string jdata = reader.ReadToEnd();
                    settings = JsonConvert.DeserializeObject<Settings>(jdata);
                    settings.initHolders();
                }

            }

            catch (Exception ex)
            {
                // modLog.Error?.Write(ex);
            }
            
            modLog = new DeferringLogger(modDirectory, "DropCostEnhanced", "DCE", settings.debug, settings.trace);

            try
            {
                cachedDifficulty = 0f;
                cacheValidUntil = DateTime.UtcNow;
                CustomComponents.Registry.RegisterSimpleCustomComponents(Assembly.GetExecutingAssembly());
                DropCostManager.Instance.Initialize();
                AmmoCostManager.Instance.Initialize();
                HeatCostManager.Instance.Initialize();
                DifficultyManager.Instance.Initialize();
            }
            catch (Exception e)
            {
                modLog.Error?.Write(e);
            }

            var harmony = HarmonyInstance.Create("ca.jwolf.DropCostsEnhanced");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

        }
    }
}

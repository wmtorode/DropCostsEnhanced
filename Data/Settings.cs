using System;
using BattleTech;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DropCostsEnhanced.Data
{
    public class Settings
    {
        public bool debug = false;
        public bool trace = false;
        public bool enableDropCosts = true;
        public bool enableAmmoCosts = true;
        public bool enableHeatCosts = false;
        public float costFactor = 0.002f;
        public bool useCostByTons = false;
        public float dropCostPerTon = 500f;
        public float roundToNearist = 10000f;
        public string heatSunkStat = "CACOverrallHeatSinked";
        public List<FactionCapital> capitals = new List<FactionCapital>();
        
        [JsonConverter(typeof(StringEnumConverter))]
        public EDifficultyType diffMode = EDifficultyType.NotActive;
        
        [JsonIgnore]
        private Dictionary<string, string> factionToCapital = new Dictionary<string, string>();
        
        [JsonIgnore]
        private Dictionary<string, string> capitalToFaction = new Dictionary<string, string>();

        public void initHolders()
        {
            foreach (FactionCapital capital in capitals)
            {
                factionToCapital[capital.faction] = capital.captial;
                capitalToFaction[capital.captial] = capital.faction;
            }
        }

        public string getCapitalForFaction(string faction)
        {
            if (factionToCapital.ContainsKey(faction))
            {
                return factionToCapital[faction];
            }

            return null;
        }

        public string getFactionForCapital(string capital)
        {
            if (capitalToFaction.ContainsKey(capital))
            {
                return capitalToFaction[capital];
            }

            return FactionEnumeration.GetInvalidUnsetFactionValue().Name;
        }

        public bool isCapital(StarSystem system)
        {
            if (capitalToFaction.ContainsKey(system.Name))
            {
                return true;
            }
            return false;
        }
    }
}
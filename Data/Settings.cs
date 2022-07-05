using System;
using BattleTech;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SVGImporter;
using UnityEngine;

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
        public bool useDiffRungs = false;

        public bool useDifficultyCostScaling = false;
        public float defaultDifficultyCostModifier = 1.0f;
        public List<DifficultyScaler> difficultyCostModifiers = new List<DifficultyScaler>();

        public List<FactionCapital> capitals = new List<FactionCapital>();

        public List<DifficultyWidgetLevel> diffWidgetRungs = new List<DifficultyWidgetLevel>();
        
        [JsonConverter(typeof(StringEnumConverter))]
        public EDifficultyType diffMode = EDifficultyType.NotActive;

        public int valuePerHalfSkull = 16500000;
        public int defaultMechsToCount = 8;
        public int maxDifficulty = 25;
        public int additionalUnitCount = 4;
        public int averagedDrops = 8;
        public bool excludeFlashpointsFromDropAverage = true;

        public List<string> excludedContractTypes = new List<string>();
        public List<string> excludedContractIds = new List<string>();

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

        public bool getRungColor(int rung, out Color oColour)
        {
            oColour = Color.white;
            if (useDiffRungs)
            {
                foreach (DifficultyWidgetLevel diffRung in diffWidgetRungs)
                {
                    if (diffRung.rung == rung)
                    {
                        oColour = diffRung.GetColor();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool getRungIcon(int rung, out SVGAsset asset)
        {
            asset = null;
            if (useDiffRungs)
            {
                foreach (DifficultyWidgetLevel diffRung in diffWidgetRungs)
                {
                    if (diffRung.rung == rung)
                    {
                        asset = diffRung.GetIconAsset();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool getRungBackingIcon(int rung, out SVGAsset asset)
        {
            asset = null;
            if (useDiffRungs)
            {
                foreach (DifficultyWidgetLevel diffRung in diffWidgetRungs)
                {
                    if (diffRung.rung == rung)
                    {
                        asset = diffRung.GetIconBackingAsset();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
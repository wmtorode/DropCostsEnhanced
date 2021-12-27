using BattleTech;
using System.Collections.Generic;
using System;
using DropCostsEnhanced.Data;
using System.Linq;
using UnityEngine;

namespace DropCostsEnhanced
{
    public class DifficultyManager
    {
        private static DifficultyManager _instance;
        private StatCollection companyStats;

        private const string DropCostStatPrefix = "DCE-Drop_";
        private const string DropCostIdxStat = "DCE-IDX";
        
        public static DifficultyManager Instance
        {
            get
            {
                if (_instance == null) _instance = new DifficultyManager();
                return _instance;
            }
        }
        
        public void Initialize()
        {
            return;
        }

        public int legacyCountedMechs
        {
            get;
            private set;
        }
        
        public int legacyWorth
        {
            get;
            private set;
        }

        public int dropWorth
        {
            get;
            private set;
        }
        
        public void setCompanyStats(SimGameState simGame)
        {
            companyStats = simGame.CompanyStats;
            int legacyCost = getLegacyCompanyCosts(simGame);
            for (int i = 0; i < DCECore.settings.averagedDrops; i++)
            {
                string statName = $"{DropCostStatPrefix}{i}";
                if (!companyStats.ContainsStatistic(statName))
                {
                    DCECore.modLog.Info?.Write($"Initing Stat: {statName}, Value: {legacyCost}");
                    companyStats.AddStatistic<int>(statName, legacyCost);
                }
            }
            if (!companyStats.ContainsStatistic(DropCostIdxStat))
            {
                companyStats.AddStatistic<int>(DropCostIdxStat, 0);
            }
            
        }

        public int getCompanyCosts()
        {
            if (companyStats == null)
            {
                DCECore.modLog.Info?.Write($"CStats is null, race-condition?");
                return 0;
            }
            int totalCost = 0;
            for (int i = 0; i < DCECore.settings.averagedDrops; i++)
            {
                string statName = $"{DropCostStatPrefix}{i}";
                if (companyStats.ContainsStatistic(statName))
                {
                    totalCost += companyStats.GetValue<int>(statName);
                }
            }
            
            DCECore.modLog.Debug?.Write($"TotalCost before averaging: {totalCost}");
            totalCost /= DCECore.settings.averagedDrops;
            DCECore.modLog.Debug?.Write($"TotalCost after averaging: {totalCost}");
            dropWorth = totalCost;
            return totalCost;

        }
        
        public float getCompanyDifficulty()
        {
            float difficulty = getCompanyCosts()/ DCECore.settings.valuePerHalfSkull;
            return Mathf.Min(DCECore.settings.maxDifficulty, Mathf.Round(difficulty));
        }

        public void updateDropAverage(int cost)
        {
            int idx = companyStats.GetValue<int>(DropCostIdxStat);
            string statName = $"{DropCostStatPrefix}{idx}";
            if (companyStats.ContainsStatistic(statName))
            {
                companyStats.Set<int>(statName, cost);
            }

            idx += 1;
            idx %= DCECore.settings.averagedDrops;
            companyStats.Set<int>(DropCostIdxStat, idx);
            DCECore.modLog.Info?.Write($"Updating drop stat: {statName} to value: {cost}, new Idx: {idx}");
        }

        public int getLegacyCompanyCosts(SimGameState sim)
        {
            int totalMechWorth = 0;
            List<MechDef> mechlist = sim.ActiveMechs.Values.ToList();
            mechlist = mechlist.OrderByDescending(x => DropCostManager.Instance.CalculateMechCost(x))
                .ToList();
            int countedmechs = DCECore.settings.defaultMechsToCount;
            if (sim.CompanyStats.ContainsStatistic("BiggerDrops_AdditionalMechSlots") &&
                countedmechs > 4)
            {
                int deploySize = DCECore.settings.additionalUnitCount + 
                                 sim.CompanyStats.GetValue<int>("BiggerDrops_AdditionalMechSlots");
                countedmechs = Math.Min(deploySize, DCECore.settings.defaultMechsToCount);
            }

            if (mechlist.Count < countedmechs)
            {
                countedmechs = mechlist.Count;
            }

            for (int i = 0; i < countedmechs; i++)
            {
                totalMechWorth += Mathf.RoundToInt(DropCostManager.Instance.CalculateMechCost(mechlist[i]));
            }

            legacyCountedMechs = countedmechs;
            legacyWorth = totalMechWorth;
            return totalMechWorth;
        }

        public float getLegacyCompanyDifficulty(SimGameState simGame)
        {
            float difficulty = getLegacyCompanyCosts(simGame) / DCECore.settings.valuePerHalfSkull;
            return Mathf.Min(DCECore.settings.maxDifficulty, Mathf.Round(difficulty));
        }
    }
}
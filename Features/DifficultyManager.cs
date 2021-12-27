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
        
        public void setCompanyStats(SimGameState simGame)
        {
            companyStats = simGame.CompanyStats;
            
        }

        public int getCompanyCosts(SimGameState sim)
        {
            int totalMechWorth = 0;
            List<MechDef> mechlist = sim.ActiveMechs.Values.ToList();
            mechlist = mechlist.OrderByDescending(x => DropCostManager.Instance.CalculateMechCost(x))
                .ToList();
            int countedmechs = DCECore.settings.defaultMechsToCount;
            if (sim.CompanyStats.ContainsStatistic("BiggerDrops_AdditionalMechSlots") &&
                countedmechs > 4)
            {
                int deploySize =
                    4 + sim.CompanyStats.GetValue<int>("BiggerDrops_AdditionalMechSlots");
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

            return totalMechWorth;
        }

        public float getLegacyCompanyDifficulty(SimGameState simGame)
        {
            float difficulty = getCompanyCosts(simGame) / DCECore.settings.valuePerHalfSkull;
            return Mathf.Min(DCECore.settings.maxDifficulty, Mathf.Round(difficulty));
        }
    }
}
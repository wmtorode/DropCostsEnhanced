using BattleTech;
using System.Collections.Generic;
using System;
using DropCostsEnhanced.Data;
using System.Linq;
using BattleTech.Data;
using SVGImporter;
using UnityEngine;
using JwTweaks;
using JwTweaks.Data;
using JwTweaks.Features;

namespace DropCostsEnhanced
{
    public class DifficultyManager
    {
        private static DifficultyManager _instance;
        private StatCollection companyStats;

        private const string DropCostStatPrefix = "DCE-Drop_";
        private const string DropCostIdxStat = "DCE-IDX";
        public const string SettableDifficultyStat = "DCE-FixedDifficulty";
        private const string CurrentDifficultyStat = "DCE-CurrentDifficulty";
        private const string DceSaveBlockName = "DCE-SaveBlock";
        
        private DceSaveBlock saveBlock = new DceSaveBlock();
        
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
            var block = new JsonSaveBlock<DceSaveBlock>();
            block.Data = saveBlock;
            SaveSerializationManager.RegisterCustomSaveBlock(block, DceSaveBlockName);
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
        
        public void setCompanyStats(SimGameState simGame, bool newGame = false)
        {
            if (newGame)
            {
                saveBlock.ClearForNewGame();
            }

            int legacyCost = getLegacyCompanyCosts(simGame);
            // resize the list to the new number of drops to be averaged
            if (DCECore.settings.averagedDrops > saveBlock.previousDropCosts.Count)
            {
                var slotsToAdd = DCECore.settings.averagedDrops - saveBlock.previousDropCosts.Count;
                for (int i = 0; i < slotsToAdd; i++)
                {
                    DCECore.modLog.Info?.Write($"Initing Stat: {saveBlock.previousDropCosts.Count}, Value: {legacyCost}");
                    saveBlock.previousDropCosts.Add(legacyCost);
                }
            }

            
            companyStats = simGame.CompanyStats;
            if (!saveBlock.initialized)
            {
                DCECore.modLog.Info?.Write("Initializing SaveBlock, attempt to migrate old data");
                for (int i = 0; i < DCECore.settings.averagedDrops; i++)
                {
                    string statName = $"{DropCostStatPrefix}{i}";
                    if (companyStats.ContainsStatistic(statName))
                    {
                        var value = companyStats.GetValue<int>(statName);
                        DCECore.modLog.Info?.Write($"migrating Stat: {i}, Value: {value} to new format");
                        saveBlock.previousDropCosts[i] = value;
                    }
                }

                if (companyStats.ContainsStatistic(DropCostIdxStat))
                {
                    var value = companyStats.GetValue<int>(DropCostIdxStat);
                    DCECore.modLog.Info?.Write($"migrating Index: {DropCostIdxStat}, Value: {value} to new format");
                    saveBlock.currentIndex = value;
                }

                if (companyStats.ContainsStatistic(SettableDifficultyStat))
                {
                    var value = companyStats.GetValue<int>(SettableDifficultyStat);
                    DCECore.modLog.Info?.Write(
                        $"migrating CYA: {SettableDifficultyStat}, Value: {value} to new format");
                    saveBlock.fixedDifficulty = value;
                }
                saveBlock.initialized = true;
            }

            if (!companyStats.ContainsStatistic(CurrentDifficultyStat))
            {
                companyStats.AddStatistic<int>(CurrentDifficultyStat, 1);
            }
            
        }

        public int getCompanyCosts()
        {
            if (companyStats == null)
            {
                DCECore.modLog.Info?.Write($"CStats is null, race-condition?");
                return 0;
            }
            long totalCost = 0;
            for (int i = 0; i < DCECore.settings.averagedDrops; i++)
            {
                totalCost += saveBlock.previousDropCosts[i];
            }
            
            DCECore.modLog.Debug?.Write($"TotalCost before averaging: {totalCost}");
            totalCost /= DCECore.settings.averagedDrops;
            DCECore.modLog.Debug?.Write($"TotalCost after averaging: {totalCost}");
            dropWorth = (int)totalCost;
            return dropWorth;

        }

        public void setChooseYourOwnAdventureDifficulty(int difficulty)
        {
            saveBlock.fixedDifficulty = difficulty;
        }
        
        public void setCurrentDifficultyStat(int difficulty)
        {
            companyStats.Set(CurrentDifficultyStat, difficulty);
        }

        public float getChooseYourOwnAdventureDifficulty()
        {
            return saveBlock.fixedDifficulty;
        }
        
        public float getCompanyDifficulty()
        {
            float difficulty = getCompanyCosts()/ DCECore.settings.valuePerHalfSkull;
            return Mathf.Min(DCECore.settings.maxDifficulty, Mathf.Round(difficulty));
        }

        public void updateDropAverage(int cost)
        {
            var currentIdx = saveBlock.currentIndex;
            saveBlock.previousDropCosts[saveBlock.currentIndex] = cost;
            int idx = saveBlock.currentIndex;
            idx += 1;
            idx %= DCECore.settings.averagedDrops;
            saveBlock.currentIndex = idx;
            DCECore.modLog.Info?.Write($"Updating drop stat: {currentIdx} to value: {cost}, new Idx: {idx}");
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
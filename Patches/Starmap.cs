using BattleTech;
using DropCostsEnhanced.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using HBS;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(Starmap), "PopulateMap", new Type[] { typeof(SimGameState) })]
    public static class Starmap_PopulateMap_Patch {
        
        public static bool Prepare()
        {
            return DCECore.settings.diffMode != EDifficultyType.NotActive;
        }

        static void Prefix(ref bool __runOriginal, Starmap __instance, SimGameState simGame) {
            
            if (!__runOriginal)
            {
                return;
            }
            var watch = System.Diagnostics.Stopwatch.StartNew();
            if (DCECore.settings.diffMode == EDifficultyType.Company || DCECore.settings.diffMode == EDifficultyType.LegacyCompany || DCECore.settings.diffMode == EDifficultyType.ChooseYourAdventure) {
                foreach (StarSystem system in simGame.StarSystems)
                {
                    system.Def.DefaultDifficulty = 0;
                    system.Def.DifficultyList = new List<int>();
                    system.Def.DifficultyModes = new List<SimGameState.SimGameType>();
                }
            }
            watch.Stop();
            DCECore.modLog.Debug?.Write($"StarMap Update Prefix took: {watch.ElapsedMilliseconds} ms.");
        }

        static void Postfix(Starmap __instance, SimGameState simGame) {
            if (DCECore.settings.diffMode == EDifficultyType.Reputation) {
                foreach (StarSystem system in simGame.StarSystems) {
                    StarSystem capital = simGame.StarSystems.Find(x => x.Name.Equals(DCECore.settings.getCapitalForFaction(system.OwnerValue.Name)));
                    if (capital != null) {
                        StarSystemNode systemByID = __instance.GetSystemByID(system.ID);
                        StarSystemNode systemByID2 = __instance.GetSystemByID(capital.ID);
                        AStar.PathFinder starmapPathfinder = new AStar.PathFinder();
                        starmapPathfinder.InitFindPath(systemByID, systemByID2, 1, 1E-06f, new Action<AStar.AStarResult>(OnPathfindingComplete));
                        while (!starmapPathfinder.IsDone) {
                            starmapPathfinder.Step();
                        }
                    }
                    else {
                        system.Def.DefaultDifficulty = 1;
                    }
                    system.Def.DifficultyList = new List<int>();
                    system.Def.DifficultyModes = new List<SimGameState.SimGameType>();
                }
            }
        }
        private static void OnPathfindingComplete(AStar.AStarResult result) {
            try {
                int baseDifficulty = 5;
                GameInstance game = LazySingletonBehavior<UnityGameInstance>.Instance.Game;
                Dictionary<string, int> allCareerFactionReputations = new Dictionary<string, int>();
                foreach (FactionValue faction in FactionEnumeration.FactionList)
                {
                    if (faction.DoesGainReputation)
                    {
                        allCareerFactionReputations.Add(faction.Name, game.Simulation.GetRawReputation(faction));
                    }
                }
                int count = result.path.Count;
                StarSystemNode starSystemNode = (StarSystemNode)result.path[0];
                int rangeDifficulty = 0;
                int repModifier = 0;
                string repFaction = starSystemNode.System.OwnerValue.Name;
                if (!DCECore.settings.isCapital(starSystemNode.System)) {
                    rangeDifficulty = Mathf.RoundToInt((count - 1));
                } else {
                    repFaction = DCECore.settings.getFactionForCapital(starSystemNode.System.Name);
                }
                if (allCareerFactionReputations.ContainsKey(repFaction)) {
                    int repOfOwner = allCareerFactionReputations[repFaction];
                    repModifier = Mathf.CeilToInt(repOfOwner / 20f);
                }
                int endDifficulty = Mathf.Clamp(baseDifficulty + rangeDifficulty - repModifier, 1, DCECore.settings.maxDifficulty);
                starSystemNode.System.Def.DefaultDifficulty = endDifficulty;
            }catch(Exception e) {
                DCECore.modLog.Error?.Write(e);
            }
        }
    }
}
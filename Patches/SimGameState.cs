using BattleTech;
using BattleTech.UI;
using Harmony;
using DropCostsEnhanced;
using DropCostsEnhanced.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(SimGameState), "ShowDifficultMissionPopup")]
    public static class SimGameState_ShowDifficultMissionPopup {
        
        public static bool Prepare()
        {
            return DCECore.settings.diffMode != EDifficultyType.NotActive;
        }
        static bool Prefix(SimGameState __instance, SimGameInterruptManager ___interruptQueue) {
            try {
                ___interruptQueue.QueuePauseNotification("Difficult Mission", "Careful, Commander. This drop looks like it might require more firepower than that.", __instance.GetCrewPortrait(SimGameCrew.Crew_Darius), string.Empty, new Action(__instance.RoomManager.CmdCenterRoom.lanceConfigBG.LC.ContinueConfirmClicked), "CONFIRM", null, "BACK");
                return false;
            }
            catch (Exception e) {
                DCECore.modLog.Error?.Write(e);
                return true;
            }
        }
    }
    
    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("GlobalDifficulty", MethodType.Getter)]
    public static class SimGameState_GlobalDifficulty_Getter_Patch {
        
        public static bool Prepare()
        {
            return DCECore.settings.diffMode != EDifficultyType.NotActive;
        }
        
        static void Postfix(SimGameState __instance, ref float __result) {
            try {
                if (DCECore.settings.diffMode == EDifficultyType.Company) {
                    if (DateTime.UtcNow.Ticks > DCECore.cacheValidUntil.Ticks)
                    {
                        __result = DCECore.cachedDifficulty;
                    }
                    else
                    {
                        int totalMechWorth = 0;
                        List<MechDef> mechlist = __instance.ActiveMechs.Values.ToList();
                        mechlist = mechlist.OrderByDescending(x => DropCostManager.Instance.CalculateMechCost(x))
                            .ToList();
                        int countedmechs = DCECore.settings.defaultMechsToCount;
                        if (__instance.CompanyStats.ContainsStatistic("BiggerDrops_AdditionalMechSlots") &&
                            countedmechs > 4)
                        {
                            int deploySize =
                                4 + __instance.CompanyStats.GetValue<int>("BiggerDrops_AdditionalMechSlots");
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

                        float difficulty = totalMechWorth / DCECore.settings.valuePerHalfSkull;
                        DCECore.cachedDifficulty = Mathf.Min(DCECore.settings.maxDifficulty, Mathf.Round(difficulty));
                        DCECore.cacheValidUntil = DateTime.UtcNow.AddSeconds(30);
                        __result = DCECore.cachedDifficulty;
                        DCECore.modLog.Info?.Write($"Setting Global difficulty to: {__result}, Counted Mechs: {countedmechs}, worth of counted mechs: {totalMechWorth}");
                        
                    }
                }
                else {
                    __result = 0;
                }
            }
            catch (Exception e) {
                DCECore.modLog.Error?.Write(e);

            }
        }
        
        [HarmonyPatch(typeof(SimGameState), "GetNormalizedDifficulty")]
        public static class SimGameState_GetNormalizedDifficulty_Patch {
            
            public static bool Prepare()
            {
                return DCECore.settings.diffMode != EDifficultyType.NotActive;
            }
            
            static void Postfix(SimGameState __instance, ref int __result) {
                if (DCECore.settings.diffMode == EDifficultyType.Company) {
                    __result = Mathf.RoundToInt(Mathf.Clamp(__instance.GlobalDifficulty, 0, DCECore.settings.maxDifficulty));
                }
            }
        }
    }
}
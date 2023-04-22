using System;
using System.Collections.Generic;
using BattleTech;
using DropCostsEnhanced.Data;
using UnityEngine;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(Contract), "CompleteContract")]
    public static class Contract_CompleteContract_Patch {

        static void Postfix(Contract __instance)
        {
            try
            {
                CombatGameState combat = __instance.BattleTechGame.Combat;
                DCECore.modLog.Info?.Write($"Calculating Drop Cost for {__instance.Name}. Original MoneyResults: {__instance.MoneyResults}");
                List<AbstractActor> actors = combat.AllActors;

                if (DCECore.settings.diffMode != EDifficultyType.NotActive)
                {
                    bool fpAllowed = true;
                    if (DCECore.settings.excludeFlashpointsFromDropAverage &&
                        (__instance.IsFlashpointContract || __instance.IsFlashpointCampaignContract))
                    {
                        fpAllowed = false;
                    }

                    bool excludedId = false;
                    if (__instance.Override != null)
                    {
                        if (DCECore.settings.excludedContractIds.Contains(__instance.Override.ID))
                        {
                            excludedId = true;
                        }
                    }
                    
                    // only include the mission in the average if its not an excluded type, is not an excluded ID and possibily if
                    // not a flashpoint
                    if (!DCECore.settings.excludedContractTypes.Contains(__instance.ContractTypeValue.Name) && fpAllowed && !excludedId)
                    {
                        DifficultyManager.Instance.updateDropAverage(DropCostManager.Instance.RawCost);
                    }
                }


                int TotalCost = 0;
                if (DCECore.settings.enableDropCosts)
                {
                    TotalCost += DropCostManager.Instance.Cost;
                }

                if (DCECore.settings.enableAmmoCosts)
                {
                    TotalCost += AmmoCostManager.Instance.CalculateFinalCosts(actors);
                }
                if (DCECore.settings.enableHeatCosts)
                {
                    TotalCost += HeatCostManager.Instance.CalculateFinalCosts(actors);
                }
                DCECore.modLog.Info?.Write($"Total Drop Cost: {TotalCost}");

                int newResult = Mathf.FloorToInt(__instance.MoneyResults - TotalCost);
                Traverse.Create(__instance).Property("MoneyResults").SetValue(newResult);



            }
            catch (Exception e)
            {
                DCECore.modLog.Error?.Write(e);
            }
        }
    }
}

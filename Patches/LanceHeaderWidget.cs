using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using DropCostsEnhanced;
using UnityEngine;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using TMPro;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(LanceHeaderWidget), "RefreshLanceInfo")]
    public static class LanceHeaderWidget_RefreshLanceInfo {
        
        public static bool Prepare()
        {
            return DCECore.settings.enableDropCosts;
        }
        
        static void Postfix(LanceHeaderWidget __instance, List<MechDef> mechs, LocalizableText ___simLanceTonnageText, LanceConfiguratorPanel ___LC) {
            try {
                if (___LC.IsSimGame) {
                    DropCostManager.Instance.CalculateLanceCost(mechs);

                    // longer strings interfere with messages about incorrect lance configurations
                    ___simLanceTonnageText.SetText($"DROP COST: ¢{DropCostManager.Instance.FormattedCosts}   LANCE WEIGHT: {DropCostManager.Instance.LanceTonnage} TONS");
                }
            }
            catch (Exception e) {
                DCECore.modLog.Error?.Write(e);
            }
        }
    }
}
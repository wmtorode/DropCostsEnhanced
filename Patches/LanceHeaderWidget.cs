using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;
using DropCostsEnhanced;
using UnityEngine;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using DropCostsEnhanced.Data;
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
                    if (DCECore.settings.diffMode != EDifficultyType.NotActive)
                    {
                        SGDifficultyIndicatorWidget lanceRatingWidget = (SGDifficultyIndicatorWidget) AccessTools
                            .Field(typeof(LanceHeaderWidget), "lanceRatingWidget").GetValue(__instance);
                        TextMeshProUGUI label = lanceRatingWidget.transform.parent
                            .GetComponentsInChildren<TextMeshProUGUI>()
                            .FirstOrDefault(t => t.transform.name == "label-lanceRating");
                        label.text = "Lance Rating";
                        int difficulty = DropCostManager.Instance.RawCost / DCECore.settings.valuePerHalfSkull;
                        DCECore.modLog.Debug?.Write($"Calculated Drop Rating: {difficulty}, total value: {DropCostManager.Instance.RawCost}");
                        lanceRatingWidget.SetDifficulty(Mathf.Min(10, difficulty));
                    }
                }
            }
            catch (Exception e) {
                DCECore.modLog.Error?.Write(e);
            }
        }
    }
}
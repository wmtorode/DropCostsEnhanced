using BattleTech;
using BattleTech.UI;
using System;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(AAR_ContractObjectivesWidget), "FillInObjectives")]
    public static class AAR_ContractObjectivesWidget_FillInObjectives {

        static void Postfix(AAR_ContractObjectivesWidget __instance) {
            try {
                if (DCECore.settings.enableDropCosts)
                {
                    __instance.AddObjective(DropCostManager.Instance.GetObjectiveResult());
                }
                if (DCECore.settings.enableAmmoCosts)
                {
                    __instance.AddObjective(AmmoCostManager.Instance.GetObjectiveResult());
                }
                if (DCECore.settings.enableHeatCosts)
                {
                    __instance.AddObjective(HeatCostManager.Instance.GetObjectiveResult());
                }
            }
            catch (Exception e) {
                DCECore.modLog.Error?.Write(e);
            }
        }
    }
}
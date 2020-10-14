using Harmony;
using BattleTech;
using BattleTech.UI;
using BattleTech.Framework;
using System;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(AAR_ContractObjectivesWidget), "FillInObjectives")]
    public static class AAR_ContractObjectivesWidget_FillInObjectives {

        static void Postfix(AAR_ContractObjectivesWidget __instance) {
            try {
                Traverse addObjective = Traverse.Create(__instance).Method("AddObjective", new Type[] {typeof(MissionObjectiveResult)});
                if (DCECore.settings.enableDropCosts)
                {
                    addObjective.GetValue(new object[] {DropCostManager.Instance.GetObjectiveResult()});
                }
                if (DCECore.settings.enableAmmoCosts)
                {
                    addObjective.GetValue(new object[] {AmmoCostManager.Instance.GetObjectiveResult()});
                }
                if (DCECore.settings.enableHeatCosts)
                {
                    addObjective.GetValue(new object[] {HeatCostManager.Instance.GetObjectiveResult()});
                }
            }
            catch (Exception e) {
                DCECore.modLog.Error?.Write(e);
            }
        }
    }
}
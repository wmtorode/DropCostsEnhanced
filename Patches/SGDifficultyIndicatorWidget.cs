using BattleTech;
using BattleTech.UI;
using Harmony;
using DropCostsEnhanced;
using DropCostsEnhanced.Data;
using UnityEngine;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(SGDifficultyIndicatorWidget), "SetDifficulty")]
    public static class SGDifficultyIndicatorWidget_SetDifficulty
    {
        public static bool Prepare()
        {
            return DCECore.settings.diffMode != EDifficultyType.NotActive;
        }

        static void Prefix(SGDifficultyIndicatorWidget __instance, ref int difficulty)
        {
            difficulty = Mathf.Min(10, difficulty);
        }
    }
}
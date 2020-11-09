using BattleTech;
using BattleTech.UI;
using Harmony;
using DropCostsEnhanced;
using UnityEngine;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(SGDifficultyIndicatorWidget), "SetDifficulty")]
    public static class SGDifficultyIndicatorWidget_SetDifficulty
    {

        static void Prefix(SGDifficultyIndicatorWidget __instance, ref int difficulty)
        {
            difficulty = Mathf.Min(10, difficulty);
        }
    }
}
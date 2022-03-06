using BattleTech;
using BattleTech.UI;
using Harmony;
using DropCostsEnhanced;
using DropCostsEnhanced.Data;
using UnityEngine;


namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(SGNavigationScreen), "OnDifficultySelectionChanged")]
    public static class SGNavigationScreen_OnDifficultySelectionChanged
    {
        public static bool Prepare()
        {
            return DCECore.settings.diffMode == EDifficultyType.Company || DCECore.settings.diffMode == EDifficultyType.LegacyCompany || DCECore.settings.diffMode == EDifficultyType.ChooseYourAdventure;
        }

        static void Prefix(SGNavigationScreen __instance, ref int index)
        {
            index = -1;
        }

    }
}
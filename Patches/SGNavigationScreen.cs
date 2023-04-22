using BattleTech.UI;
using DropCostsEnhanced.Data;


namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(SGNavigationScreen), "OnDifficultySelectionChanged")]
    public static class SGNavigationScreen_OnDifficultySelectionChanged
    {
        public static bool Prepare()
        {
            return DCECore.settings.diffMode == EDifficultyType.Company || DCECore.settings.diffMode == EDifficultyType.LegacyCompany || DCECore.settings.diffMode == EDifficultyType.ChooseYourAdventure;
        }

        static void Prefix(ref bool __runOriginal, SGNavigationScreen __instance, ref int index)
        {
            if (!__runOriginal)
            {
                return;
            }
            
            index = -1;
        }

    }
}
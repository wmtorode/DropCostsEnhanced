using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using DropCostsEnhanced;
using DropCostsEnhanced.Data;
using UnityEngine;
using SVGImporter;
using System.Collections.Generic;


namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(SGDifficultyIndicatorWidget), "SetDifficulty")]
    public static class SGDifficultyIndicatorWidget_SetDifficulty
    {
        public static bool Prepare()
        {
            return DCECore.settings.diffMode != EDifficultyType.NotActive;
        }

        static void Prefix(SGDifficultyIndicatorWidget __instance, List<UIColorRefTracker> ___pips, UIColor ___activeColor, ref int difficulty)
        {
            Traverse difficultySetter = Traverse.Create(__instance).Property("Difficulty");
            difficultySetter.SetValue(difficulty);
            __instance.Reset();
            float f = (float) difficulty / 2f;
            int maxIdx = Math.Min(5, Mathf.FloorToInt(f));
            int index;
            for (index = 0; index < Mathf.FloorToInt(f); ++index)
            {
                UIColorRefTracker pip = ___pips[index];
                pip.GetComponent<SVGImage>().fillAmount = 1f;
                pip.SetUIColor(___activeColor);
            }
            if ((double) index >= (double) f)
                return;
            UIColorRefTracker pip1 = ___pips[index];
            SVGImage component = pip1.GetComponent<SVGImage>();
            pip1.SetUIColor(___activeColor);
            component.fillAmount = f - (float) index;
        }
    }
}
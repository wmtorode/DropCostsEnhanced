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

        static bool Prefix(SGDifficultyIndicatorWidget __instance, List<UIColorRefTracker> ___pips, UIColor ___activeColor, ref int difficulty)
        {
            Traverse difficultySetter = Traverse.Create(__instance).Property("Difficulty");
            difficultySetter.SetValue(difficulty);
            __instance.Reset();
            float f =  ((float)(difficulty)%10)/ 2f;
            if (!DCECore.settings.useDiffRungs)
            {
                f =  Mathf.Min((float)difficulty/ 2f, 5f);
            }
            else
            {
                if ((difficulty % 10) == 0 && difficulty != 0)
                {
                    f = 5f;
                }
            }

            int index;
            Color color;
            bool customColor = DCECore.settings.getRungColor(Math.Max((difficulty - 1) / 10, 0), out color);
            for (index = 0; index < Mathf.FloorToInt(f); ++index)
            {
                UIColorRefTracker pip = ___pips[index];
                pip.GetComponent<SVGImage>().fillAmount = 1f;
                
                if (customColor)
                {
                    pip.SetUIColor(UIColor.Custom);
                    pip.OverrideWithColor(color);
                }
                else
                {
                    pip.SetUIColor(___activeColor);
                }
            }
            if ((double) index >= (double) f)
                return false;
            UIColorRefTracker pip1 = ___pips[index];
            SVGImage component = pip1.GetComponent<SVGImage>();
            
            if (customColor)
            {
                pip1.SetUIColor(UIColor.Custom);
                pip1.OverrideWithColor(color);
            }
            else
            {
                pip1.SetUIColor(___activeColor);
            }
            component.fillAmount = f - (float) index;
            return false;
        }
    }
}
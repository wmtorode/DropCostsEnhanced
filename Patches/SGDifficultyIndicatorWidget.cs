using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using DropCostsEnhanced;
using DropCostsEnhanced.Data;
using UnityEngine;
using SVGImporter;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine.UI;


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
            SVGAsset asset;
            SVGAsset assetBacking;
            bool customColor = DCECore.settings.getRungColor(Math.Max((difficulty - 1) / 10, 0), out color);
            bool customAsset = DCECore.settings.getRungIcon(Math.Max((difficulty - 1) / 10, 0), out asset);
            bool customAssetBacking = DCECore.settings.getRungBackingIcon(Math.Max((difficulty - 1) / 10, 0), out assetBacking);
            if (customAsset)
            {
                HorizontalLayoutGroup[] backingComponents = __instance.GetComponentsInChildren<HorizontalLayoutGroup>();
                for (int i = 0; i < backingComponents.Length; i++)
                {
                    if (backingComponents[i].name == "pip-backings")
                    {
                        SVGImage[] backingImages = backingComponents[i].GetComponentsInChildren<SVGImage>();
                        for (int j = 0; j < backingImages.Length; j++)
                        {
                            backingImages[j].GetComponent<SVGImage>().vectorGraphics = assetBacking;
                        }
                    }
                }
            }
            for (index = 0; index < Mathf.FloorToInt(f); ++index)
            {
                UIColorRefTracker pip = ___pips[index];
                pip.GetComponent<SVGImage>().fillAmount = 1f;
                if (customAsset)
                {
                    SVGImage svgImage = pip.GetComponent<SVGImage>();
                    svgImage.vectorGraphics = asset;
                }
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
            if (customAsset)
            {
                SVGImage svgImage = pip1.GetComponent<SVGImage>();
                svgImage.vectorGraphics = asset;
            }
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
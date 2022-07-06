using BattleTech;
using BattleTech.Data;
using UnityEngine;
using Newtonsoft.Json;
using SVGImporter;

namespace DropCostsEnhanced.Data
{
    public class DifficultyWidgetLevel
    {
        public string colour = "#FF00FF";
        public string iconOverride = "";
        public string iconBackingOverride = "";
        public int rung = 1;
        
        private Color refColor;
        private bool cSet = false;

        private SVGAsset iconOverrideAsset;
        private SVGAsset iconBackingOverrideAsset;
        private bool iSet = false;
        private bool bSet = false;
        
        public Color GetColor()
        {
            if (!cSet)
            {
                ColorUtility.TryParseHtmlString(colour, out refColor);
                cSet = true;
            }
            return refColor;
        }

        public SVGAsset GetIconAsset()
        {
            if (!iSet && !string.IsNullOrEmpty(iconOverride))
            {
                DataManager dm = UnityGameInstance.BattleTechGame.DataManager;
                iconOverrideAsset = dm.GetObjectOfType<SVGAsset>(iconOverride, BattleTechResourceType.SVGAsset);
                iSet = true;
            }
            return iconOverrideAsset;
        }

        public SVGAsset GetIconBackingAsset()
        {
            if (!bSet && ! string.IsNullOrEmpty(iconBackingOverride))
            {
                DataManager dm = UnityGameInstance.BattleTechGame.DataManager;
                iconBackingOverrideAsset = dm.GetObjectOfType<SVGAsset>(iconBackingOverride, BattleTechResourceType.SVGAsset);
                bSet = true;
            }
            return iconBackingOverrideAsset;
        }
    }
}
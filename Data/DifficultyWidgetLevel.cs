using UnityEngine;
using Newtonsoft.Json;

namespace DropCostsEnhanced.Data
{
    public class DifficultyWidgetLevel
    {
        public string colour = "#FF00FF";
        public int rung = 1;
        
        private Color refColor;
        private bool cSet = false;
        
        
        public Color GetColor()
        {
            if (!cSet)
            {
                ColorUtility.TryParseHtmlString(colour, out refColor);
                cSet = true;
            }
            return refColor;
        }

    }
}
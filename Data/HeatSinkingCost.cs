using System;
using CustomComponents;

namespace DropCostsEnhanced.Data
{
    [CustomComponent("HeatSinkingCost")]
    public class HeatSinkingCost : SimpleCustomComponent
    {
        public float HeatUpkeepCost = 1.0f;
    }
}
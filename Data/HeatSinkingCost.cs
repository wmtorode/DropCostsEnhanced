using System;
using CustomComponents;

namespace DropCostsEnhanced.Data
{
    [CustomComponent("HeatSinkingCost")]
    public class HeatSinkingCost : SimpleCustomComponent
    {
        public float HeatUpkeepCost = 0.0f;
        public float HeatUpkeepMult = 1.0f;
    }
}
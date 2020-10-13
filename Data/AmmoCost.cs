using System;
using CustomComponents;

namespace DropCostsEnhanced.Data
{
    [CustomComponent("AmmoCost")]
    public class AmmoCost : SimpleCustomComponent
    {
        public float PerUnitCost = 1.0f;
    }
}
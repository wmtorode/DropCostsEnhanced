using System;
using CustomComponents;

namespace DropCostsEnhanced.Data
{
    [CustomComponent("AmmoCost")]
    public partial class AmmoCost : SimpleCustomComponent
    {
        public float PerUnitCost = 1.0f;
        public bool CostPerShot = false;
    }
}
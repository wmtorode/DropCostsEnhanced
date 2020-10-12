
using System;
using CustomComponents;

namespace DropCostsEnhanced.Data
{
    [CustomComponent("DropCostFactor")]
    public class DropCostFactor : SimpleCustomChassis
    {
        public float DropModifier = 1.0f;
    }
}
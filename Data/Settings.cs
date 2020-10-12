using System;
using System.Collections.Generic;

namespace DropCostsEnhanced.Data
{
    public class Settings
    {
        public bool debug = false;
        public bool trace = false;
        public bool enableDropCosts = true;
        public float costFactor = 0.002f;
        public bool useCostByTons = false;
    }
}
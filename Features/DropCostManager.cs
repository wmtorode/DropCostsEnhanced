using BattleTech;
using System.Collections.Generic;

namespace DropCostsEnhanced.Features
{
    public class DropCostManager: BaseCostManager
    {
        private static DropCostManager _instance;
        
        public static DropCostManager Instance
        {
            get
            {
                if (_instance == null) _instance = new DropCostManager();
                return _instance;
            }
        }
        
        public void Initialize()
        {
            Cost = 0;
            uuid = "7facf07a-626d-4a3b-a1ec-b29a35ff1ac0";
            ObjectiveText = "DROP COSTS DEDUCED";
        }

        public int CalculateFinalCosts(List<AbstractActor> actors)
        {
            return Cost;
        }
        
        
    }
}
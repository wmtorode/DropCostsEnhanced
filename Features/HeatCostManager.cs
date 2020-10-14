using BattleTech;
using System.Collections.Generic;
using System;
using CustomComponents;
using UnityEngine;
using DropCostsEnhanced.Data;

namespace DropCostsEnhanced
{
    public class HeatCostManager: BaseCostManager
    {
        private static HeatCostManager _instance;

        public static HeatCostManager Instance
        {
            get
            {
                if (_instance == null) _instance = new HeatCostManager();
                return _instance;
            }
        }

        private float getHeatCost(MechComponent mechComponent)
        {
            if (mechComponent.componentDef.Is<HeatSinkingCost>(out var costFactor))
            {
                return costFactor.HeatUpkeepCost;
            }

            return 0f;

        }

        private float getTotalHeatSunk(AbstractActor actor)
        {
            if (actor.StatCollection.ContainsStatistic(DCECore.settings.heatSunkStat))
            {
                return actor.StatCollection.GetValue<float>(DCECore.settings.heatSunkStat);
            }
            
            DCECore.modLog.Warn?.Write($"Failed to find heat stat for unit {actor.UnitName}");
            return 0f;
        }
        
        
        new public void Initialize()
        {
            Cost = 0;
            uuid = "7facf07a-626d-4a3b-a1ec-b29a35ff1ac2";
            ObjectiveText = "HEATSINK UPKEEP & COOLANT COSTS DEDUCTED";
        }

        new public int CalculateFinalCosts(List<AbstractActor> actors)
        {
            Cost = 0;
            foreach (AbstractActor actor in actors)
            {
                float HeatSunk = 0;
                float HeatCost = 0;
                int actorHeatCost = 0;
                float componentHeatCost = 0f;
                
                if (actor.team != null && actor.team.IsLocalPlayer)
                {
                    List<MechComponent> mechComponents = actor.allComponents;
                    DCECore.modLog.Info?.Write($"Calculating Heat Costs for unit: {actor.UnitName}");

                    foreach (MechComponent component in mechComponents)
                    {
                        componentHeatCost = getHeatCost(component);
                        HeatCost += componentHeatCost;
                        if (componentHeatCost > 0f)
                        {
                            DCECore.modLog.Info?.Write($"Component: {component.UIName} has heat upkeep of {componentHeatCost}");
                        }
                    }

                    HeatSunk = getTotalHeatSunk(actor);
                    actorHeatCost = Mathf.FloorToInt(HeatCost * HeatSunk);
                    Cost += actorHeatCost;
                    DCECore.modLog.Info?.Write($"Unit Sunk {HeatSunk} Heat, at an upkeep value of {HeatCost}, for a total of: {actorHeatCost}");

                }
            }

            return Cost;
        }
        
        
    }
}
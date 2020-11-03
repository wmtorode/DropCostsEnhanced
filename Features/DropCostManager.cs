using BattleTech;
using System.Collections.Generic;
using System;
using CustomComponents;
using UnityEngine;
using DropCostsEnhanced.Data;

namespace DropCostsEnhanced
{
    public class DropCostManager: BaseCostManager
    {
        private static DropCostManager _instance;

        public int LanceTonnage
        {
            get;
            private set;
        }
        
        public static DropCostManager Instance
        {
            get
            {
                if (_instance == null) _instance = new DropCostManager();
                return _instance;
            }
        }

        private float CalculateMechValue(MechDef mech)
        {
            float currentCBillValue = (float)mech.Chassis.Description.Cost;
            float armorValue = 0f;
            armorValue += mech.Head.CurrentArmor;
            armorValue += mech.CenterTorso.CurrentArmor;
            armorValue += mech.CenterTorso.CurrentRearArmor;
            armorValue += mech.LeftTorso.CurrentArmor;
            armorValue += mech.LeftTorso.CurrentRearArmor;
            armorValue += mech.RightTorso.CurrentArmor;
            armorValue += mech.RightTorso.CurrentRearArmor;
            armorValue += mech.LeftArm.CurrentArmor;
            armorValue += mech.RightArm.CurrentArmor;
            armorValue += mech.LeftLeg.CurrentArmor;
            armorValue += mech.RightLeg.CurrentArmor;
            armorValue *= UnityGameInstance.BattleTechGame.MechStatisticsConstants.CBILLS_PER_ARMOR_POINT;
            currentCBillValue += armorValue;
            foreach (MechComponentRef mechComponentRef in mech.Inventory)
            {
                currentCBillValue += (float)mechComponentRef.Def.Description.Cost;
            }
            currentCBillValue = Mathf.Round(currentCBillValue / DCECore.settings.roundToNearist) * DCECore.settings.roundToNearist;
            return currentCBillValue;
        }
        
        new public void Initialize()
        {
            Cost = 0;
            uuid = "7facf07a-626d-4a3b-a1ec-b29a35ff1ac0";
            ObjectiveText = "DROP COSTS DEDUCTED";
        }

        new public int CalculateFinalCosts(List<AbstractActor> actors)
        {
            return Cost;
        }

        public void CalculateLanceCost(List<MechDef> mechDefs)
        {
            Cost = 0;
            LanceTonnage = 0;
            int dropTonnageCost = 0;
            foreach (MechDef mech in mechDefs)
            {
                DCECore.modLog.Info?.Write($"Calculating Drop Costs for unit: {mech.Name}");
                float modifier = 1.0f;
                if (mech.Chassis.Is<DropCostFactor>(out var costFactor))
                {
                    modifier = costFactor.DropModifier;
                }
                DCECore.modLog.Info?.Write($"unit has cost modifier of: {modifier}");
                if (DCECore.settings.useCostByTons)
                {
                    dropTonnageCost = (int) ((mech.Chassis.Tonnage * DCECore.settings.dropCostPerTon) * modifier);
                    DCECore.modLog.Info?.Write($"tonnage cost: {dropTonnageCost}");
                    Cost += dropTonnageCost;
                }

                int valueCost = (int) ((CalculateMechValue(mech) * DCECore.settings.costFactor) * modifier);
                DCECore.modLog.Info?.Write($"value based cost: {valueCost}");
                DCECore.modLog.Info?.Write($"Total Drop cost: {valueCost + dropTonnageCost}");
                Cost += valueCost;

                LanceTonnage += (int) mech.Chassis.Tonnage;
            }
            

        }
        
    }
}
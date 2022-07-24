using BattleTech;
using System.Collections.Generic;
using System;
using CustomComponents;
using UnityEngine;
using DropCostsEnhanced.Data;

namespace DropCostsEnhanced
{
    public class AmmoCostManager: BaseCostManager
    {
        private static AmmoCostManager _instance;

        public static AmmoCostManager Instance
        {
            get
            {
                if (_instance == null) _instance = new AmmoCostManager();
                return _instance;
            }
        }

        private float getInternalMunitionCost(Weapon weapon)
        {
            if (weapon.weaponDef.Is<AmmoCost>(out var costFactor))
            {
                return costFactor.PerUnitCost;
            }

            return (weapon.weaponDef.Description.Cost)/ weapon.weaponDef.StartingAmmoCapacity;
        }
        
        private float getMunitionCost(AmmunitionBox ammunitionBox)
        {
            if (ammunitionBox.ammunitionBoxDef.Is<AmmoCost>(out var costFactor))
            {
                return costFactor.PerUnitCost;
            }

            return (ammunitionBox.ammunitionBoxDef.Description.Cost)/ ammunitionBox.AmmoCapacity;

        }
        
        
        new public void Initialize()
        {
            Cost = 0;
            uuid = "7facf07a-626d-4a3b-a1ec-b29a35ff1ac1";
            ObjectiveText = "AMMO COSTS DEDUCTED";
        }

        new public int CalculateFinalCosts(List<AbstractActor> actors)
        {
            Cost = 0;
            foreach (AbstractActor actor in actors)
            {
                int actorAmmoCost = 0;
                int unitsUsed = 0;
                float costPerUnit = 0;
                int componentCost = 0;
                int currentAmmo = 0;
                
                if (actor.team != null && actor.team.IsLocalPlayer)
                {
                    List<Weapon> weapons = actor.Weapons;
                    List<AmmunitionBox> ammunitionBoxes = actor.ammoBoxes;
                    DCECore.modLog.Info?.Write($"Calculating Ammo Costs for unit: {actor.UnitName}");
                    foreach (Weapon weapon in weapons)
                    {
                        if (weapon.weaponDef.StartingAmmoCapacity > 0)
                        {
                            costPerUnit = getInternalMunitionCost(weapon);
                            currentAmmo = weapon.InternalAmmo;
                            if (currentAmmo < 0)
                            {
                                DCECore.modLog.Warn?.Write($"Negative Ammo Detected!");
                                currentAmmo = 0;
                            }
                            unitsUsed = weapon.weaponDef.StartingAmmoCapacity - currentAmmo;
                            componentCost = Mathf.FloorToInt(unitsUsed * costPerUnit);
                            DCECore.modLog.Info?.Write($"Weapon: {weapon.UIName}, internal ammo: {currentAmmo}/{weapon.weaponDef.StartingAmmoCapacity}, cost: {componentCost}");
                            actorAmmoCost += componentCost;
                                
                        }

                    }

                    foreach (AmmunitionBox ammunitionBox in ammunitionBoxes)
                    {
                        costPerUnit = getMunitionCost(ammunitionBox);
                        currentAmmo = ammunitionBox.CurrentAmmo;
                        if (currentAmmo < 0)
                        {
                            DCECore.modLog.Warn?.Write($"Negative Ammo Detected!");
                            currentAmmo = 0;
                        }
                        unitsUsed = ammunitionBox.AmmoCapacity - currentAmmo;
                        componentCost = Mathf.FloorToInt(unitsUsed * costPerUnit);
                        DCECore.modLog.Info?.Write($"AmmoBox: {ammunitionBox.UIName}, remaining ammo: {currentAmmo}/{ammunitionBox.AmmoCapacity}, cost: {componentCost}");
                        actorAmmoCost += componentCost;
                    }

                    Cost += actorAmmoCost;
                    DCECore.modLog.Info?.Write($"Total Ammo Cost for Unit is: {actorAmmoCost}");

                }
            }

            return Cost;
        }
        
        
    }
}
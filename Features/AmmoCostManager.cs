using BattleTech;
using System.Collections.Generic;
using System;
using System.Linq;
using CustomComponents;
using UnityEngine;
using DropCostsEnhanced.Data;

#if USE_CAC
using CustAmmoCategories;
#endif

namespace DropCostsEnhanced
{
    public class AmmoCostManager: BaseCostManager
    {
        
        private static readonly string InternalAmmoNameCAC = "InternalAmmo_";
        
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

        private bool WeaponHasInternalAmmo(Weapon weapon)
        {
            #if USE_CAC
            return weapon.exDef().InternalAmmo.Count > 0;
            #endif
            return weapon.weaponDef.StartingAmmoCapacity > 0;
        }

        private int GetInternalAmmoStartCapacity(Weapon weapon)
        {
            #if USE_CAC
            if (weapon.exDef().InternalAmmo.Count == 1)
            {
                return weapon.exDef().InternalAmmo.First().Value;
            }

            // in RogueTech some weapons have multiple internal ammo types...because why not
            DCECore.modLog.Info?.Write($"Weapon: {weapon.UIName} has multiple internal ammo types...");
            int startingCapacity = 0;
            foreach (var ammoKVP in weapon.exDef().InternalAmmo)
            {
                startingCapacity += ammoKVP.Value;
                DCECore.modLog.Info?.Write($"Ammo Type: {ammoKVP.Key} has capacity: {ammoKVP.Value}");
            }

            return startingCapacity;
            #endif
            return weapon.weaponDef.StartingAmmoCapacity;
        }

        private int GetRemainingInternalAmmoCount(Weapon weapon)
        {
            #if USE_CAC
            // CAC case for a weapon with 1 internal ammo type is the same as the vanilla case
            if (weapon.exDef().InternalAmmo.Count > 1)
            {
                var currentAmmo = 0;
                // There is probably a better way of doing this, but this way is in theory easy
                // and performance not really an issue given this is calculated only once per mission, during fade out
                // and weapons stat size is not that big to begin with
                foreach (var stat in weapon.statCollection.stats.Values)
                {
                    if (stat.name.StartsWith(InternalAmmoNameCAC))
                    {
                        var statAmmo = stat.Value<int>();
                        DCECore.modLog.Info?.Write($"Ammo Stat: {stat.name} has {statAmmo} remaining shots");
                        currentAmmo += statAmmo;
                    }
                }

                return currentAmmo;
            }
            
            #endif
            return weapon.InternalAmmo;
        }

        private int GetInternalWeaponAmmoCosts(Weapon weapon)
        {
            int componentCost = 0;
            if (WeaponHasInternalAmmo(weapon))
            {
                var costPerUnit = getInternalMunitionCost(weapon);
                var startingCapacity = GetInternalAmmoStartCapacity(weapon);
                var currentAmmo = GetRemainingInternalAmmoCount(weapon);
                if (currentAmmo < 0)
                {
                    DCECore.modLog.Warn?.Write($"Negative Ammo Detected!");
                    currentAmmo = 0;
                }
                var unitsUsed = startingCapacity - currentAmmo;
                componentCost = Mathf.FloorToInt(unitsUsed * costPerUnit);
                DCECore.modLog.Info?.Write($"Weapon: {weapon.UIName}, internal ammo: {currentAmmo}/{startingCapacity}, cost: {componentCost}");

            }

            return componentCost;
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
                        actorAmmoCost += GetInternalWeaponAmmoCosts(weapon);
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
using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using DropCostsEnhanced;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(Contract), "CompleteContract")]
    public static class Contract_CompleteContract_Patch {
        
        static void Postfix(Contract __instance)
        {
            CombatGameState combat = __instance.BattleTechGame.Combat; 
            List<AbstractActor> actors = combat.AllActors;
            foreach (AbstractActor actor in actors)
            {
                if (actor.team != null && actor.team.IsLocalPlayer)
                {
                    List<Weapon> weapons = actor.Weapons;
                    List<AmmunitionBox> ammunitionBoxes = actor.ammoBoxes;
                    DCECore.modLog.Info?.Write($"Calculating Ammo/Heat Sink Maintenance Costs for unit: {actor.UnitName}");
                    foreach (Weapon weapon in weapons)
                    {
                        if (weapon.weaponDef.StartingAmmoCapacity > 0)
                        {
                            DCECore.modLog.Debug?.Write($"Weapon: {weapon.UIName}, internal ammo: {weapon.InternalAmmo}/{weapon.weaponDef.StartingAmmoCapacity}");
                        }
                    }

                    foreach (AmmunitionBox ammunitionBox in ammunitionBoxes)
                    {
                        DCECore.modLog.Debug?.Write($"AmmoBox: {ammunitionBox.UIName}, remaining ammo: {ammunitionBox.CurrentAmmo}/{ammunitionBox.AmmoCapacity}");
                    }
                    
                }
            }
        }
    }
}
using System;
using BattleTech;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(AttackDirector.AttackSequence), "GenerateHitInfo")]
    public static class AttackSequence_GenerateHitInfo
    {
        public static bool Prepare()
        {
            return DCECore.settings.enableAmmoCosts;
        }

        static void Postfix(Weapon weapon, WeaponHitInfo __result)
        {
            try
            {
                AmmoCostManager.RecordShotsFired(weapon, __result.numberOfShots);
            }
            catch (Exception e)
            {
                DCECore.modLog.Error?.Write(e);
            }
        }
    }
}

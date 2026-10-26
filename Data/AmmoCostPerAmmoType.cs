using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;

#if USE_CAC
namespace DropCostsEnhanced.Data
{
    public class AmmoTypeCost
    {
        public string AmmoId;
        public float Cost;
    }

    public partial class AmmoCost : IOnLoaded
    {
        public AmmoTypeCost[] InternalPerAmmoCost;

        public void OnLoaded()
        {
            if (InternalPerAmmoCost == null)
            {
                return;
            }

            string error = GetPerAmmoCostError();
            if (error != null)
            {
                DCECore.modLog.Error?.Write($"AmmoCost of {Def.Description.Id}: {error}, InternalPerAmmoCost is ignored");
                InternalPerAmmoCost = null;
            }
        }

        private string GetPerAmmoCostError()
        {
            if (Def is not WeaponDef)
            {
                return "InternalPerAmmoCost is only for weapons with internal ammo";
            }

            if (InternalPerAmmoCost.Length == 0)
            {
                return "InternalPerAmmoCost is empty";
            }

            HashSet<string> ammoIds = new HashSet<string>();
            foreach (AmmoTypeCost typeCost in InternalPerAmmoCost)
            {
                if (string.IsNullOrEmpty(typeCost.AmmoId))
                {
                    return "InternalPerAmmoCost has an entry without AmmoId";
                }

                if (typeCost.Cost < 0)
                {
                    return $"InternalPerAmmoCost of {typeCost.AmmoId} is negative";
                }

                if (!ammoIds.Add(typeCost.AmmoId))
                {
                    return $"InternalPerAmmoCost has {typeCost.AmmoId} more than once";
                }
            }

            return null;
        }

        public void CheckInternalAmmo(ICollection<string> internalAmmoIds)
        {
            foreach (AmmoTypeCost typeCost in InternalPerAmmoCost)
            {
                if (!internalAmmoIds.Contains(typeCost.AmmoId))
                {
                    DCECore.modLog.Warn?.Write($"AmmoCost of {Def.Description.Id}: InternalPerAmmoCost has {typeCost.AmmoId}, which is not in its internal ammo");
                }
            }

            foreach (string ammoId in internalAmmoIds)
            {
                if (Array.Find(InternalPerAmmoCost, c => c.AmmoId == ammoId) == null)
                {
                    DCECore.modLog.Warn?.Write($"AmmoCost of {Def.Description.Id}: InternalPerAmmoCost has no cost for {ammoId}, PerUnitCost is used");
                }
            }
        }
    }
}
#endif

using BattleTech;
using BattleTech.UI;
using Harmony;
using DropCostsEnhanced;
using System;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(SimGameState), "ShowDifficultMissionPopup")]
    public static class SimGameState_ShowDifficultMissionPopup {

        static bool Prefix(SimGameState __instance, SimGameInterruptManager ___interruptQueue) {
            try {
                ___interruptQueue.QueuePauseNotification("Difficult Mission", "Careful, Commander. This drop looks like it might require more firepower than that.", __instance.GetCrewPortrait(SimGameCrew.Crew_Darius), string.Empty, new Action(__instance.RoomManager.CmdCenterRoom.lanceConfigBG.LC.ContinueConfirmClicked), "CONFIRM", null, "BACK");
                return false;
            }
            catch (Exception e) {
                DCECore.modLog.Error?.Write(e);
                return true;
            }
        }
    }
}
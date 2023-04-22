using BattleTech;
using BattleTech.UI;
using BattleTech.Save;
using DropCostsEnhanced.Data;
using System;
using BattleTech.Data;
using SVGImporter;
using UnityEngine;

namespace DropCostsEnhanced.Patches
{
    
    [HarmonyPatch(typeof(SimGameState), "Rehydrate", typeof(GameInstanceSave))]
    class SimGameState_RehydratePatch
    {
        public static void Postfix(SimGameState __instance, GameInstanceSave gameInstanceSave)
        {
            DifficultyManager.Instance.setCompanyStats(__instance);
            if (DCECore.settings.useDiffRungs)
            {
                DataManager dm = UnityGameInstance.BattleTechGame.DataManager;
                LoadRequest loadRequest = dm.CreateLoadRequest();
                foreach (var rung in DCECore.settings.diffWidgetRungs)
                {
                    if (!string.IsNullOrEmpty(rung.iconOverride)) loadRequest.AddLoadRequest<SVGAsset>(BattleTechResourceType.SVGAsset, rung.iconOverride,
                        null);
                    if (!string.IsNullOrEmpty(rung.iconBackingOverride)) loadRequest.AddLoadRequest<SVGAsset>(BattleTechResourceType.SVGAsset, rung.iconBackingOverride,
                        null);
                }
                loadRequest.ProcessRequests();
            }
        }
    }

    [HarmonyPatch(typeof(SimGameState), "InitCompanyStats")]
    class SimGameState_InitCompanyStatsPatch
    {
        public static void Postfix(SimGameState __instance)
        {
            DifficultyManager.Instance.setCompanyStats(__instance);
            if (DCECore.settings.useDiffRungs)
            {
                DataManager dm = UnityGameInstance.BattleTechGame.DataManager;
                LoadRequest loadRequest = dm.CreateLoadRequest();
                foreach (var rung in DCECore.settings.diffWidgetRungs)
                {
                    if (!string.IsNullOrEmpty(rung.iconOverride)) loadRequest.AddLoadRequest<SVGAsset>(BattleTechResourceType.SVGAsset, rung.iconOverride,
                        null);
                    if (!string.IsNullOrEmpty(rung.iconBackingOverride)) loadRequest.AddLoadRequest<SVGAsset>(BattleTechResourceType.SVGAsset, rung.iconBackingOverride,
                        null);
                }
                loadRequest.ProcessRequests();
            }
        }
    }
    
    [HarmonyPatch(typeof(SimGameState), "ShowDifficultMissionPopup")]
    public static class SimGameState_ShowDifficultMissionPopup {
        
        public static bool Prepare()
        {
            return DCECore.settings.diffMode != EDifficultyType.NotActive;
        }
        static void Prefix(ref bool __runOriginal, SimGameState __instance, SimGameInterruptManager ___interruptQueue) {
            
            if (!__runOriginal)
            {
                return;
            }
            
            try {
                ___interruptQueue.QueuePauseNotification("Difficult Mission", "Careful, Commander. This drop looks like it might require more firepower than that.", __instance.GetCrewPortrait(SimGameCrew.Crew_Darius), string.Empty, new Action(__instance.RoomManager.CmdCenterRoom.lanceConfigBG.LC.ContinueConfirmClicked), "CONFIRM", null, "BACK");
                __runOriginal = false;
            }
            catch (Exception e) {
                DCECore.modLog.Error?.Write(e);
                __runOriginal = true;
            }
        }
    }
    
    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("GlobalDifficulty", MethodType.Getter)]
    public static class SimGameState_GlobalDifficulty_Getter_Patch {
        
        public static bool Prepare()
        {
            return DCECore.settings.diffMode != EDifficultyType.NotActive;
        }
        
        static void Postfix(SimGameState __instance, ref float __result) {
            try {
                if (DCECore.settings.diffMode == EDifficultyType.Company || DCECore.settings.diffMode == EDifficultyType.LegacyCompany) {
                    if (DateTime.UtcNow.Ticks < DCECore.cacheValidUntil.Ticks)
                    {
                        __result = DCECore.cachedDifficulty;
                        DCECore.modLog.Debug?.Write($"using cached diff: {DateTime.UtcNow.Ticks}, {DCECore.cacheValidUntil.Ticks}");
                    }
                    else
                    {
                        if (DCECore.settings.diffMode == EDifficultyType.Company)
                        {
                            DCECore.cachedDifficulty = Mathf.Min(DCECore.settings.maxDifficulty, Mathf.Round(DifficultyManager.Instance.getCompanyDifficulty()));
                            DCECore.modLog.Info?.Write($"Setting Global difficulty to: {DCECore.cachedDifficulty}, Counted Contracts: {DCECore.settings.averagedDrops}, average worth of counted drops: {DifficultyManager.Instance.dropWorth}");
                        }
                        else
                        {
                            DCECore.cachedDifficulty = Mathf.Min(DCECore.settings.maxDifficulty, Mathf.Round(DifficultyManager.Instance.getLegacyCompanyDifficulty(__instance)));
                            DCECore.modLog.Info?.Write($"Setting Global difficulty to: {DCECore.cachedDifficulty}, Counted Mechs: {DifficultyManager.Instance.legacyCountedMechs}, worth of counted mechs: {DifficultyManager.Instance.legacyWorth}");
                        }
                        
                        DCECore.cacheValidUntil = DateTime.UtcNow.AddSeconds(30);
                        __result = DCECore.cachedDifficulty;
                        
                        
                    }
                }
                else {
                    if (DCECore.settings.diffMode == EDifficultyType.ChooseYourAdventure)
                    {
                        __result = DifficultyManager.Instance.getChooseYourOwnAdventureDifficulty();
                        DCECore.modLog.Debug?.Write($"using CYOA diff: {__result}");
                    }
                    else
                    {
                        __result = 0;
                        DCECore.modLog.Debug?.Write($"using 0 diff: {DCECore.settings.diffMode}");
                    }
                }
            }
            catch (Exception e) {
                DCECore.modLog.Error?.Write(e);

            }
        }
        
        [HarmonyPatch(typeof(SimGameState), "GetNormalizedDifficulty")]
        public static class SimGameState_GetNormalizedDifficulty_Patch {
            
            public static bool Prepare()
            {
                return DCECore.settings.diffMode != EDifficultyType.NotActive;
            }
            
            static void Postfix(SimGameState __instance, ref int __result) {
                if (DCECore.settings.diffMode == EDifficultyType.Company || DCECore.settings.diffMode == EDifficultyType.LegacyCompany || DCECore.settings.diffMode == EDifficultyType.ChooseYourAdventure) {
                    __result = Mathf.RoundToInt(Mathf.Clamp(__instance.GlobalDifficulty, 0, DCECore.settings.maxDifficulty));
                }
            }
        }
    }
}
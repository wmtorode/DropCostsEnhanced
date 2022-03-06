using BattleTech;
using BattleTech.UI;
using BattleTech.Save;
using Harmony;
using DropCostsEnhanced;
using DropCostsEnhanced.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HBS;
using UnityEngine.Events;

namespace DropCostsEnhanced.Patches
{
    [HarmonyPatch(typeof(SGRoomController_CptQuarters), "CharacterClickedOn")]
    public class SGRoomController_CptQuarters_CharacterClickedOnPatch
    {
        public static bool Prepare()
        {
            return DCECore.settings.diffMode == EDifficultyType.ChooseYourAdventure;
        }

        public static void onSet(int difficulty)
        {
            DifficultyManager.Instance.setChooseYourOwnAdventureDifficulty(difficulty);
        }
        
        static bool Prefix(SGRoomController_CptQuarters __instance, SimGameState.SimGameCharacterType characterClicked) {
            try
            {
                if (characterClicked == SimGameState.SimGameCharacterType.BREAKDOWN)
                {

                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        LazySingletonBehavior<UIManager>.Instance.GetOrCreatePopupModule<SG_Stores_MultiPurchasePopup>()
                            .SetData(__instance.simState, null, "Difficulty", DCECore.settings.maxDifficulty, 0, new UnityAction<int>(onSet));
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                DCECore.modLog.Error?.Write(e);
            }
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GungeonAPI;
using Dungeonator;
using HarmonyLib;
using SaveAPI;
using MonoMod.Cil;
using System.Reflection;
using Mono.Cecil.Cil;
using Brave.BulletScript;


namespace Planetside.Controllers
{
    public class ResourcefulRatMasteryController : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Starting ResourcefulRatMasteryController setup...");
            try
            {
                ETGMod.Databases.Strings.Core.Set("#BIG_CHEESE_IMPRESSED",
                    "Consider me inpressed! Not good enough to conquer the Big Cheese, however." +
                    "\n\nHave one of my finer samples, as an expression of my inspiration. -R.R");
                DungeonHooks.OnPostDungeonGeneration += () =>
                {
                    Phase_1_NoHit = false;
                    Phase_2_NoHit = false;
                    Punchout_Win = false;
                };
                Debug.Log("Finished ResourcefulRatMasteryController setup without failure!");
            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish ResourcefulRatMasteryController setup!");
                Debug.Log(e);
            }
        }

        [HarmonyPatch(typeof(ResourcefulRatDeathController), nameof(ResourcefulRatDeathController.OnBossDeath))]
        public class Patch_ResourcefulRatDeathController_OnBossDeath
        {
            [HarmonyPrefix]
            private static void Awake(ResourcefulRatDeathController __instance)
            {
                if (__instance.aiActor && __instance.aiActor.parentRoom != null)
                {
                    Phase_1_NoHit = !__instance.aiActor.parentRoom.PlayerHasTakenDamageInThisRoom;
                }
            }
        }
        [HarmonyPatch(typeof(MetalGearRatDeathController), nameof(MetalGearRatDeathController.OnBossDeath))]
        public class Patch_MetalGearRatDeathController_OnBossDeath
        {
            [HarmonyPrefix]
            private static void Awake(MetalGearRatDeathController __instance)
            {
                if (__instance.aiActor && __instance.aiActor.parentRoom != null)
                {
                    Phase_2_NoHit = !__instance.aiActor.parentRoom.PlayerHasTakenDamageInThisRoom;
                }
            }
        }

        [HarmonyPatch(typeof(PunchoutPlayerController), nameof(PunchoutPlayerController.Win))]
        public class Patch_PunchoutPlayerController_Win
        {
            [HarmonyPrefix]
            private static void Awake(MetalGearRatDeathController __instance)
            {
                Punchout_Win = true;
            }
        }

        [HarmonyPatch]
        private static class PunchoutControllerTeardownPunchoutPatch
        {
            [HarmonyPatch(typeof(PunchoutController), nameof(PunchoutController.TeardownPunchout))]
            [HarmonyILManipulator]
            private static void GameUIAmmoControllerUpdateUIGunIL(ILContext il)
            {
                ILCursor cursor = new ILCursor(il);
                if (!cursor.TryGotoNext(MoveType.Before,
                    instr => instr.MatchCallvirt<PunchoutAIActor>("get_NumKeysDropped")))
                    return;

                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Call, typeof(PunchoutControllerTeardownPunchoutPatch).GetMethod(nameof(SpawnPedestal), BindingFlags.Static | BindingFlags.NonPublic));
            }

            private static void SpawnPedestal(PunchoutController punchoutController)
            {
                if (Phase_1_NoHit && Phase_2_NoHit)
                {
                    MetalGearRatRoomController metalGearRatRoomController = UnityEngine.Object.FindObjectOfType<MetalGearRatRoomController>();
                    if (metalGearRatRoomController)
                    {
                        Vector3 position = metalGearRatRoomController.transform.position;
                        if (Punchout_Win)
                        {
                            var item = LootEngine.SpawnItem(PickupObjectDatabase.GetById(ForgottenRoundRat.ForgottenRoundRatID).gameObject, position + new Vector3(22, 8), Vector3.zero, 0, false);
                            item.GetComponent<ForgottenRoundRat>().SetSprite(true);
                        }
                        else
                        {

                            GameObject gameObject = GameManager.Instance.Dungeon.sharedSettingsPrefab.ChestsForBosses.SelectByWeight();
                            Chest chest = gameObject.GetComponent<Chest>();
                            if (chest == null)
                            {
                                DungeonData data = GameManager.Instance.Dungeon.data;
                                RewardPedestal component = gameObject.GetComponent<RewardPedestal>();
                                if (component)
                                {
                                    RewardPedestal rewardPedestal = RewardPedestal.Spawn(component, new IntVector2((int)position.x + 22, (int)position.y + 6));
                                    rewardPedestal.IsBossRewardPedestal = true;
                                    rewardPedestal.contents = PickupObjectDatabase.GetById(ForgottenRoundRat.ForgottenRoundRatID);
                                }
                            }
                        }
                    }
                        
                }
            }
        }
        [HarmonyPatch]
        private static class PunchoutControllerPlaceNotePatch
        {
            [HarmonyPatch(typeof(PunchoutController), nameof(PunchoutController.PlaceNote))]
            [HarmonyILManipulator]
            private static void GameUIAmmoControllerUpdateUIGunIL(ILContext il)
            {

                ILCursor cursor = new ILCursor(il);
                if (!cursor.TryGotoNext(MoveType.After,
                    instr => instr.MatchCall<UnityEngine.Object>("op_Implicit")))
                    return;

                cursor.Emit(OpCodes.Ldloc_3);
                cursor.Emit(OpCodes.Call, typeof(PunchoutControllerPlaceNotePatch).GetMethod(nameof(ModifyNote), BindingFlags.Static | BindingFlags.NonPublic));
            }
            private static void ModifyNote(GameObject note)
            {
                if (Phase_1_NoHit && Phase_2_NoHit)
                {
                    var note_ = note.GetComponent<NoteDoer>();
                    note_.m_selectedDisplayString = StringTableManager.GetLongString("#BIG_CHEESE_IMPRESSED");
                    note_.stringKey = "#BIG_CHEESE_IMPRESSED";
                }
            }
        }

        public static bool Phase_1_NoHit = false;
        public static bool Phase_2_NoHit = false;
        public static bool Punchout_Win = false;
    }
}

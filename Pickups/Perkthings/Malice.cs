using Alexandria.ItemAPI;
using Dungeonator;
using HarmonyLib;
using ItemAPI;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Brave.BulletScript;
using static UmbraController;
using Alexandria.VisualAPI;
using Pathfinding;
using ChallengeAPI;
using Alexandria;

namespace Planetside
{
    class Malice : PerkPickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Malice";
            //string resourcePath = "Planetside/Resources/PerkThings/lazyAllstatsUp.png";
            GameObject gameObject = new GameObject(name);
            Malice item = gameObject.AddComponent<Malice>();

            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemAPI.ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("psogmalice"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Refined Hatred";
            string longDesc = "Challenge the Jammed and its armies and reap the rewards of your effort.\n\nGood luck.";
            item.SetupItem(shortDesc, longDesc, "psog");
            item.encounterTrackable.DoNotificationOnEncounter = false;

            Malice.MaliceID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.black;
            particles.ParticleSystemColor2 = Color.red;
            item.OutlineColor = new Color(0.1f, 0f, 0f);

            var LJ = PrefabDatabase.Instance.SuperReaper.GetComponent<SuperReaperController>();
            LJ.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("bigBullet"));
            LJ.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));

            item.StackPickupNotificationText = "Curse is more fruitful.";
            item.InitialPickupNotificationText = "Curse is more potent and rewarding.";
            CursePot = (((GameObject)BraveResources.Load("Global Prefabs/_ChallengeManager", ".prefab")).GetComponent<ChallengeManager>().FindChallenge<CursePotChallengeModifier>().challenge as CursePotChallengeModifier).CursePot;

            Alexandria.ItemAPI.AlexandriaTags.SetTag(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Advanced_Dragun_Knife_GUID).aiActor, "PSOG:MaliceUmbralBan");
            Alexandria.ItemAPI.AlexandriaTags.SetTag(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Draguns_Knife_GUID).aiActor, "PSOG:MaliceUmbralBan");

            Alexandria.ItemAPI.AlexandriaTags.SetTag(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Bullat_GUID).aiActor, "PSOG:MaliceUmbralBan");
            Alexandria.ItemAPI.AlexandriaTags.SetTag(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Chicken_GUID).aiActor, "PSOG:MaliceUmbralBan");
            Alexandria.ItemAPI.AlexandriaTags.SetTag(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Bombshee_GUID).aiActor, "PSOG:MaliceUmbralBan");
            Alexandria.ItemAPI.AlexandriaTags.SetTag(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Rat_GUID).aiActor, "PSOG:MaliceUmbralBan");
            Alexandria.ItemAPI.AlexandriaTags.SetTag(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Spirat_GUID).aiActor, "PSOG:MaliceUmbralBan");
            Alexandria.ItemAPI.AlexandriaTags.SetTag(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Tombstoner_GUID).aiActor, "PSOG:MaliceUmbralBan");
            GungeonAPI.DungeonHooks.OnPostDungeonGeneration += () =>
            {
                RoomHandler_HandleBossClearReward_Patch.PlusPedestals = 0;
            };

        }
        private static DungeonPlaceable CursePot;
        public override CustomDungeonFlags FlagToSetOnStack => CustomDungeonFlags.MALICE_FLAG_STACK;

        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {

                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 0,
                    LockedString = "\"Curse is more potent and rewarding.\"",
                    UnlockedString = "\"Curse is more potent and rewarding.\"",
                    requiresFlag = false
                },

                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("Potential"),
                    UnlockedString = "Curse becomes more potent, but grants additional opportunities for rewards.",
                    requiresFlag = false,
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("The Cursed Horde"),
                    UnlockedString = "Jammed enemy spawn rates are altered and no longer capped.",
                    requiresFlag = false,
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 2,
                    LockedString = AlphabetController.ConvertString("Reward"),
                    UnlockedString = "Jammed/Umbral enemies can now drop loot. Chance increases with Curse.",
                    requiresFlag = false,
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 2,
                    LockedString = AlphabetController.ConvertString("Calm Shopping"),
                    UnlockedString = "Lord Of The Jammed no longer scares shopkeepers.\nLord Of The Jammed spawns later.",
                    requiresFlag = true,
                    FlagToTrack = CustomDungeonFlags.MALICE_FLAG_SUPER_LJ,
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("Super Jammed"),
                    UnlockedString = "Umbral Enemies can now naturally spawn.",
                    requiresFlag = true,
                    FlagToTrack = CustomDungeonFlags.MALICE_FLAG_NATURAL_UMBRALS,
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 4,
                    LockedString = AlphabetController.ConvertString("Cursed Loot"),
                    UnlockedString = "Chests can now contain additional, but cursed items. Chance increases with Curse.",
                    requiresFlag = true,
                    FlagToTrack = CustomDungeonFlags.MALICE_FLAG_EXTRA_CHEST_LOOT,
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 5,
                    LockedString = AlphabetController.ConvertString("Super Lord"),
                    UnlockedString = "The Lord Of The Jammed can become stronger.",
                    requiresFlag = true,
                    FlagToTrack = CustomDungeonFlags.MALICE_FLAG_SUPER_LJ,
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 6,
                    LockedString = AlphabetController.ConvertString("Boss Loot"),
                    UnlockedString = "Jammed Bosses drop extra rewards.",
                    requiresFlag = true,
                    FlagToTrack = CustomDungeonFlags.MALICE_FLAG_EXTRA_BOSS_LOOT,
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("Stacking Increases Risk"),
                    UnlockedString = "Stacking increases all Malice probabilities, and Bosses can now be Umbral.",
                    requiresFlag = true,
                    FlagToTrack = CustomDungeonFlags.MALICE_FLAG_STACK,
                },
        };


        public static int MaliceID;

        public new bool PrerequisitesMet()
        {
            EncounterTrackable component = base.GetComponent<EncounterTrackable>();
            return component == null || component.PrerequisitesMet();
        }



        public override void OnStack(PlayerController player)
        {

        }
        public override void OnInitialPickup(PlayerController playerController)
        {
            OtherTools.ApplyStat(playerController, PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
            GameManager.Instance.OnNewLevelFullyLoaded += this.PlacePotsIntoRoom;
            PlacePotsIntoRoom();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.Instance.OnNewLevelFullyLoaded -= this.PlacePotsIntoRoom;
        }

        public void PlacePotsIntoRoom()
        {
            var rewards = GameManager.Instance.Dungeon.data.rooms.Where(x => x.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.REWARD);
            if (rewards != null && rewards.Count() > 0)
            {
                foreach (var entry in rewards)
                {
                    RoomHandler currentRoom = entry;
                    int num = currentRoom.CellsWithoutExits.Count / 50;
                    num = Mathf.Min(4, Mathf.Max(1, num));
                    CellValidator cellValidator = delegate (IntVector2 pos)
                    {
                        for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                        {
                            if (Vector2.Distance(GameManager.Instance.AllPlayers[j].CenterPosition, pos.ToCenterVector2()) < 8f)
                            {
                                return false;
                            }
                        }
                        return true;
                    };
                    for (int i = 0; i < num; i++)
                    {
                        IntVector2? randomAvailableCell = currentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One), new CellTypes?(CellTypes.FLOOR), false, cellValidator);
                        if (randomAvailableCell != null)
                        {
                            CellData cellData = GameManager.Instance.Dungeon.data[randomAvailableCell.Value];
                            if (cellData.parentRoom == currentRoom && cellData.type == CellType.FLOOR && !cellData.isOccupied && !cellData.containsTrap && !cellData.isOccludedByTopWall)
                            {
                                cellData.containsTrap = true;
                                CursePot.InstantiateObject(currentRoom, cellData.position - currentRoom.area.basePosition, false, false);
                            }
                        }
                    }
                }
            }


        }



        [HarmonyPatch(typeof(AIActor), nameof(AIActor.CheckForBlackPhantomness))]
        public class Patch_AIActor_CheckForBlackPhantomness
        {
            [HarmonyPrefix]
            private static bool OverrideCanTeleport(AIActor __instance)
            {
                var i = PerkHelper.GetGlobalStacksFromAllPlayers(MaliceID);
                if (i > 0)
                {
                    if (__instance.CompanionOwner != null || !__instance.IsNormalEnemy)
                    {
                        return false;
                    }
                    if (__instance.PreventBlackPhantom)
                    {
                        return false;
                    }

                    float totalCurse = (float)PlayerStats.GetTotalCurse();
                    float iFloat = (float)i;

                    if (__instance.healthHaver.IsBoss)
                    {
                        if (__instance.ForceBlackPhantom || UnityEngine.Random.value < GetChance(totalCurse, iFloat, 0.666f))
                        {
                            __instance.BecomeBlackPhantom();
                            if (i > 1)
                            {
                                if (!__instance.HasTag("PSOG:MaliceUmbralBan") && UnityEngine.Random.value < GetChance(totalCurse, iFloat, 0.25f))
                                {
                                    var a = __instance.GetOrAddComponent<UmbraController>();
                                    SaveAPIManager.SetFlag(CustomDungeonFlags.MALICE_FLAG_NATURAL_UMBRALS, true);
                                    a.SilentSpawn = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (__instance.ForceBlackPhantom || UnityEngine.Random.value < GetChance(totalCurse, iFloat))
                        {
                            //0.025f
                            __instance.BecomeBlackPhantom();
                            if (!__instance.HasTag("PSOG:MaliceUmbralBan") && UnityEngine.Random.value < GetChance(totalCurse, iFloat, 0.025f))
                            {
                                var a = __instance.GetOrAddComponent<UmbraController>();
                                SaveAPIManager.SetFlag(CustomDungeonFlags.MALICE_FLAG_NATURAL_UMBRALS, true);
                                a.SilentSpawn = true;
                            }
                        }
                    }


                    return false;
                }
                return true;
            }
        }

        private static float GetChance(float Curse, float Stacks, float Mult = 1)
        {
            float p1 = 1 + (Stacks * 0.01f);
            float p2 = p1 * (Curse * Curse);
            float p3 = (Curse + 1) + (Curse * (Curse * 0.06f));


            return ((p2 / p3) * 0.1f) * Mult;
        }



        [HarmonyPatch(typeof(Chest), nameof(Chest.Open))]
        public class Patch_Chest_Open
        {
            [HarmonyPrefix]
            private static void ChestOpenForMalice(Chest __instance, PlayerController player)
            {

                var ii = PerkHelper.GetGlobalStacksFromAllPlayers(MaliceID);
                if (ii > 0)
                {
                    float totalCurse = (float)PlayerStats.GetTotalCurse();
                    float iFloat = (float)ii;

                    float M = 1;
                    var item = __instance.lootTable.GetItemsForPlayer(player, 0, null);

                    switch (GameManager.Instance.RewardManager.GetQualityFromChest(__instance))
                    {
                        case ItemQuality.D:
                            M = 1.2f;
                            break;
                        case ItemQuality.C:
                            M = 1.1f;
                            break;
                        case ItemQuality.B:
                            M = 1;
                            break;
                        case ItemQuality.A:
                            M = 0.8f;
                            break;
                        case ItemQuality.S:
                            M = 0.7f;
                            break;
                    }

                    M = 1000;

                    if (UnityEngine.Random.value < GetChestChance(totalCurse, iFloat, M))
                    {
                        __instance.DetermineContents(player);
                        if (__instance.contents == null)
                        {
                            __instance.contents = new List<PickupObject>();
                        }
                        __instance.contents.AddRange(item);
                        __instance.AddComponent<MalicedChest>();
                        SaveAPIManager.SetFlag(CustomDungeonFlags.MALICE_FLAG_EXTRA_CHEST_LOOT, true);
                    }
                }
            }
        }


        private static float GetChestChance(float Curse, float Stacks, float Mult = 1)
        {
            float p1 = Mathf.Sqrt(Curse + (Stacks * 0.3f));
            return (p1 * 0.1f) * Mult;
        }

        private class MalicedChest : MonoBehaviour { }

        [HarmonyPatch]
        private static class Chest_SpewContentsOntoGround_Patch
        {
            [HarmonyPatch(typeof(Chest), nameof(Chest.SpewContentsOntoGround))]
            [HarmonyILManipulator]
            private static void GameUIAmmoControllerUpdateUIGunIL(ILContext il)
            {
                ILCursor cursor = new ILCursor(il);

                if (!cursor.TryGotoNext(MoveType.Before,
                    instr => instr.MatchLdarg(0),
                    instr => instr.MatchLdfld<Chest>("IsRainbowChest"),
                    instr => instr.MatchBrfalse(out _),
                    instr => instr.MatchLdloc(1),
                    instr => instr.MatchBrfalse(out _)))
                    return;

                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldloc, 0);
                cursor.Emit(OpCodes.Call, typeof(Chest_SpewContentsOntoGround_Patch).GetMethod("ModifyPickup", BindingFlags.Static | BindingFlags.NonPublic));
            }

            private static void ModifyPickup(Chest chest, List<DebrisObject> list2)
            {
                if (list2.Count == 0)
                    return;
                var maliceChest = chest.GetComponent<MalicedChest>();
                if (maliceChest != null)
                {
                    Destroy(maliceChest);
                    var l = list2.Shuffle();
                    for (int i = 0; i < l.Count; i++)
                    {
                        if (l[i] == null)
                            continue;
                        var pp = l[i].GetComponent<PickupObject>();
                        var pp1 = l[i].GetComponentInChildren<PickupObject>();

                        if (pp != null)
                        {
                            if (pp is PassiveItem || pp is PlayerItem)
                            {
                                Alexandria.ItemAPI.ItemBuilder.AddPassiveStatModifier(pp, PlayerStats.StatType.Curse, 1);
                                break;
                            }

                        }
                        if (pp1 != null)
                        {
                            if (pp1 is Gun)
                            {
                                Alexandria.ItemAPI.ItemBuilder.AddPassiveStatModifier(pp1, PlayerStats.StatType.Curse, 1);
                                break;
                            }
                        }
                    }
                }
            }
        }


        [HarmonyPatch(typeof(AIActor), nameof(AIActor.HandleLootPinata))]
        public class Patch_AIActor_HandleLootPinata
        {
            [HarmonyPrefix]
            private static void ChestOpenForMalice(AIActor __instance)
            {

                if (!__instance.healthHaver.IsBoss)
                {
                    var ii = PerkHelper.GetGlobalStacksFromAllPlayers(MaliceID);
                    if (ii > 0)
                    {
                        float totalCurse = (float)PlayerStats.GetTotalCurse();
                        float iFloat = (float)ii;


                        float perc = 0.25f;

                        if (__instance.healthHaver)
                        {
                            perc += (__instance.healthHaver.maximumHealth * 0.0025f);
                        }


                        if (__instance.GetComponent<UmbraController>() != null)
                        {
                            if (UnityEngine.Random.value < GetChestChance(totalCurse, iFloat * 2.5f, perc))
                            {
                                if (__instance.AdditionalSafeItemDrops == null)
                                    __instance.AdditionalSafeItemDrops = new List<PickupObject> { };

                                var itemTier = GameManager.Instance.RewardManager.CurrentRewardData.GetRandomTargetQuality(false);
                                var a = UnityEngine.Random.value < 0.33 ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable;

                                __instance.AdditionalSafeItemDrops.Add(LootEngine.GetItemOfTypeAndQuality<PickupObject>(itemTier, a));
                            }
                        }
                        else if (__instance.IsBlackPhantom)
                        {
                            if (UnityEngine.Random.value < GetChestChance(totalCurse, iFloat * 2.5f, perc))
                            {
                                if (__instance.AdditionalSafeItemDrops == null)
                                    __instance.AdditionalSafeItemDrops = new List<PickupObject> { };

                                __instance.AdditionalSafeItemDrops.Add(GameManager.Instance.RewardManager.CurrentRewardData.SingleItemRewardTable.SelectByWeight(false).GetComponent<PickupObject>());
                            }
                        }
                    }
                }
            }
        }


        [HarmonyPatch]
        private static class RoomHandler_HandleBossClearReward_Patch
        {
            [HarmonyPatch(typeof(RoomHandler), nameof(RoomHandler.HandleBossClearReward))]
            [HarmonyILManipulator]
            private static void GameUIAmmoControllerUpdateUIGunIL(ILContext il)
            {
                ILCursor cursor = new ILCursor(il);

                if (!cursor.TryGotoNext(MoveType.Before,
                    instr => instr.MatchLdloc(10),
                    instr => instr.MatchLdloc(2),
                    instr => instr.MatchLdarg(0),
                    instr => instr.MatchCall<RewardPedestal>("Spawn")))
                    return;

                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldloc, 2);
                cursor.Emit(OpCodes.Ldloc, 12);

                cursor.Emit(OpCodes.Call, typeof(RoomHandler_HandleBossClearReward_Patch).GetMethod("ModifyPickup", BindingFlags.Static | BindingFlags.NonPublic));
            }

            private static void ModifyPickup(RoomHandler roomHandler, IntVector2 intVector2, bool IsMastery)
            {
                int offset = PlusPedestals == 2 ? -1 : 0;
                for (int i = 0; i < PlusPedestals; i++)
                {
                    SaveAPIManager.SetFlag(CustomDungeonFlags.MALICE_FLAG_EXTRA_BOSS_LOOT, true);

                    var newPos = intVector2 + new IntVector2((IsMastery ? 1 : 0) + offset, -3) + new IntVector2(PlusPedestals * i, 0);

                    GameObject gameObject = GameManager.Instance.Dungeon.sharedSettingsPrefab.ChestsForBosses.SelectByWeight();
                    RewardPedestal component = gameObject.GetComponent<RewardPedestal>();

                    DungeonData data = GameManager.Instance.Dungeon.data;
                    //IntVector2 centeredVisibleClearSpot = roomHandler.GetCenteredVisibleClearSpot(2, 2);
                    RewardPedestal rewardPedestal2 = RewardPedestal.Spawn(component, newPos, roomHandler);
                    rewardPedestal2.IsBossRewardPedestal = true;
                    rewardPedestal2.lootTable.lootTable = roomHandler.OverrideBossRewardTable;
                    data[newPos].isOccupied = true;
                    data[newPos + IntVector2.Right].isOccupied = true;
                    data[newPos + IntVector2.Up].isOccupied = true;
                    data[newPos + IntVector2.One].isOccupied = true;
                }
                PlusPedestals = 0;
            }


            public static int PlusPedestals = 0;

            [HarmonyPatch(typeof(AIActor), nameof(AIActor.PreDeath))]
            public class Patch_AIActor_PreDeath
            {
                [HarmonyPrefix]
                private static bool ChestOpenForMalice(AIActor __instance)
                {
                    if (__instance.CompanionOwner != null || !__instance.IsNormalEnemy)
                    {
                        return true;
                    }
                    if (__instance.PreventBlackPhantom)
                    {
                        return true;
                    }
                    var ii = PerkHelper.GetGlobalStacksFromAllPlayers(MaliceID);
                    if (ii > 0)
                    {
                        if (__instance.healthHaver.IsBoss)
                        {
                            if (__instance.GetComponent<UmbraController>())
                            {
                                PlusPedestals = 2;
                            }
                            else if (__instance.IsBlackPhantom)
                            {
                                PlusPedestals = 1;
                            }
                        }
                    }

                    return true;
                }
            }
        }



        [HarmonyPatch(typeof(SuperReaperController), nameof(SuperReaperController.Update))]
        public class Patch_SuperReaperController_Update
        {
            [HarmonyPrefix]
            private static bool ChestOpenForMalice(SuperReaperController __instance)
            {
                var ii = PerkHelper.GetGlobalStacksFromAllPlayers(MaliceID);
                if (ii > 0)
                {
                    if (__instance.m_shootTimer == -67)
                    {
                        return false;
                    }
                    if (__instance.aiAnimator.IsPlaying("intro"))
                    {
                        return false;
                    }
                    float totalCurse = (float)PlayerStats.GetTotalCurse();
                    float iFloat = (float)ii;
                    if (totalCurse >= 25)
                    {
                        if (__instance.MaxSpeed != 12)
                        {
                            __instance.MaxSpeed = 12;
                            __instance.MinSpeed = 2.25f;
                            __instance.ShootTimer = 8;

                            GameManager.Instance.StartCoroutine(DoUpgrade(__instance, 2));
                            SaveAPIManager.SetFlag(CustomDungeonFlags.MALICE_FLAG_SUPER_LJ, true);

                        }
                    }
                    else if (totalCurse >= 20)
                    {
                        if (__instance.MaxSpeed != 11)
                        {
                            __instance.MaxSpeed = 11;
                            __instance.MinSpeed = 2.5f;
                            __instance.ShootTimer = 5;
                            GameManager.Instance.StartCoroutine(DoUpgrade(__instance, 1));
                            SaveAPIManager.SetFlag(CustomDungeonFlags.MALICE_FLAG_SUPER_LJ, true);
                        }
                    }
                }
                return true;
            }

            public static IEnumerator DoUpgrade(SuperReaperController controller, int Tier)
            {
                controller.m_shootTimer = -67;

                var v = controller.specRigidbody.Velocity;
                float e = 0;
                while (e < 2.5f)
                {
                    e += BraveTime.DeltaTime;
                    controller.specRigidbody.Velocity = Vector2.Lerp(v, Vector2.zero, e);


                    int num3 = 2;
                    Vector2 vector = controller.sprite.WorldBottomLeft;
                    Vector2 vector2 = controller.sprite.WorldTopRight;

                    GlobalSparksDoer.DoRandomParticleBurst(num3, vector, vector2, MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), e * (3 * Tier)), 0f, 0.5f, 0.3f, 1, Color.black, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);

                    yield return null;
                }

                if (Tier == 1)
                {
                    controller.sprite.usesOverrideMaterial = true;
                    controller.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
                    controller.sprite.renderer.material.SetFloat("_PhantomGradientScale", 10);
                    controller.sprite.renderer.material.SetFloat("_PhantomContrastPower", 20);

                    AkSoundEngine.PostEvent("Play_CHR_shadow_curse_01", controller.gameObject);
                    AkSoundEngine.PostEvent("Play_ENM_bulletking_slam_01", controller.gameObject);

                    UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, controller.sprite.WorldTopCenter + new Vector2(-1.25f, 1), Quaternion.identity);
                    //ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject
                    Exploder.DoDistortionWave(controller.sprite.WorldCenter, 25, 0.333f, 50f, 0.3f);

                    controller.BulletScript = new CustomBulletScriptSelector(typeof(SmallRotations));
                }
                if (Tier == 2)
                {
                    controller.sprite.usesOverrideMaterial = true;
                    controller.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
                    controller.sprite.renderer.material.SetFloat("_PhantomGradientScale", 10);
                    controller.sprite.renderer.material.SetFloat("_PhantomContrastPower", 20);

                    AkSoundEngine.PostEvent("Play_ENM_kali_blast_01", controller.gameObject);
                    Exploder.DoDistortionWave(controller.sprite.WorldCenter, 100, 0.333f, 50f, 0.3f);

                    var improvedAfterImage = controller.gameObject.AddComponent<ImprovedAfterImage>();
                    improvedAfterImage.dashColor = Color.black;
                    improvedAfterImage.spawnShadows = true;
                    improvedAfterImage.shadowTimeDelay = 0.0333f;
                    improvedAfterImage.shadowLifetime = 1f;

                    //controller.transform
                    var p = UnityEngine.Object.Instantiate(UmbralEye, controller.sprite.WorldTopCenter + new Vector2(-1.25f, 1), Quaternion.identity);
                    p.transform.SetParent(controller.sprite.transform, true);

                    controller.BulletScript = new CustomBulletScriptSelector(typeof(DeathApproaches));
                }
                controller.m_shootTimer = controller.ShootTimer;
                yield break;
            }







            public class SmallRotations : Script
            {
                public override IEnumerator Top()
                {
                    base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);

                    float Aim = base.AimDirection;
                    float delta = 15f;

                    delta = 30f;
                    for (int j = 0; j < 12; j++)
                    {
                        base.Fire(new Direction(-(float)j * delta, DirectionType.Absolute), new Speed(8, SpeedType.Absolute), new SmallRotations.OopsANull("sweep", 0.75f));
                    }

                    for (int j = 0; j < 12; j++)
                    {
                        base.Fire(new Direction(-(float)j * delta, DirectionType.Absolute), new Speed(12, SpeedType.Absolute), new SmallRotations.OopsANull("sweep", -0.75f));
                    }
                    yield break;
                }
                public class OopsANull : Bullet
                {
                    public OopsANull(string BulletType, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
                    {
                        this.m_angle = angle;
                        this.SuppressVfx = true;
                    }

                    public override IEnumerator Top()
                    {
                        this.ChangeDirection(new Brave.BulletScript.Direction(180 * m_angle, DirectionType.Relative), 180);
                        yield break;
                    }
                    private float m_angle;
                }

            }


            public class DeathApproaches : Script
            {
                public override IEnumerator Top()
                {
                    base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);

                    float Aim = base.AimDirection;
                    float delta = 36;
                    for (int j = 0; j < 10; j++)
                    {
                        base.Fire(new Direction(-(float)j * delta, DirectionType.Absolute), new Speed(4f, SpeedType.Absolute), new OopsANull("sweep", 0.9f));
                        base.Fire(new Direction(-(float)j * delta, DirectionType.Absolute), new Speed(7, SpeedType.Absolute), new OopsANull("sweep", 0));
                        base.Fire(new Direction(-(float)j * delta, DirectionType.Absolute), new Speed(4f, SpeedType.Absolute), new OopsANull("sweep", -0.9f));
                    }

                    float a = BraveUtility.RandomAngle();
                    var p = this.GetPredictedTargetPositionExact(1, 100);

                    var _1 = MathToolbox.GetUnitOnCircle(120 + a, 5f);
                    var _2 = MathToolbox.GetUnitOnCircle(240 + a, 5f);
                    var _3 = MathToolbox.GetUnitOnCircle(0 + a, 5f);

                    GameManager.Instance.StartCoroutine(DoAttackOfDoom(this.RootTransform.position + new Vector3(-1, 3), p + _1));
                    GameManager.Instance.StartCoroutine(DoAttackOfDoom(this.RootTransform.position + new Vector3(-1, 3), p + _2));
                    GameManager.Instance.StartCoroutine(DoAttackOfDoom(this.RootTransform.position + new Vector3(-1, 3), p + _3));
                    yield break;
                }
                public class OopsANull : Bullet
                {
                    public OopsANull(string BulletType, float angle = 0f, float aradius = 0) : base(BulletType, false, false, true)
                    {
                        this.m_angle = angle;
                        this.SuppressVfx = true;
                    }

                    public override IEnumerator Top()
                    {
                        this.ChangeDirection(new Brave.BulletScript.Direction(180 * m_angle, DirectionType.Relative), 180);
                        yield break;
                    }
                    private float m_angle;
                }

                public IEnumerator DoAttackOfDoom(Vector3 start, Vector3 end)
                {
                    var t = UnityEngine.Object.Instantiate(UmbralLockOnFloor, end, Quaternion.identity).GetComponent<LockOnFloor>();
                    t.SetState(true);
                    var m = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1).normalized * 8;
                    AkSoundEngine.PostEvent("Play_ENM_cannonarmor_charge_01", t.gameObject);


                    yield return new WaitForSeconds(0.5f);

                    AkSoundEngine.PostEvent("Play_ENM_creecher_burst_01", t.gameObject);

                    for (int i = 0; i < 16; i++)
                    {
                        GlobalSparksDoer.DoSingleParticle(start, MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(2.2f, 6.1f)), 0.5f, 0.35f, Color.red * 2, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    }
                    float e = 0;
                    while (e < 1)
                    {
                        e += Time.deltaTime;
                        var newPosition = Vector3.Lerp(start, end, e) + (m.ToVector3ZUp() * MathToolbox.EaseInAndBack(e));
                        GlobalSparksDoer.DoSingleParticle(newPosition, Vector3.zero, (MathToolbox.EaseInAndBack(e) + 0.25f) * 0.5f, 2f, Color.red * 1.25f, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                        yield return null;
                    }


                    GameObject effect = UnityEngine.Object.Instantiate(StaticVFXStorage.DragunBoulderLandVFX, end, Quaternion.identity);
                    Destroy(effect, 2.5f);
                    AkSoundEngine.PostEvent("Play_ENM_bulletking_skull_01", t.gameObject);
                    EnemyToolbox.SpawnBulletScript(null, end, OuroborosController.BulletBankDummy.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(SmallSlam)), "Reflection");


                    t.SetState(false);
                    Destroy(t.gameObject, 0.5f);
                    yield break;
                }

            }

        }




        [HarmonyPatch]
        private static class BaseShopController_Update_Patch
        {
            [HarmonyPatch(typeof(BaseShopController), nameof(BaseShopController.Update))]
            [HarmonyILManipulator]
            private static void DoPreventLJ(ILContext il)
            {
                ILCursor cursor = new ILCursor(il);
                if (!cursor.TryGotoNext(MoveType.Before,
                    instr => instr.MatchLdarg(0),
                    instr => instr.MatchLdcI4(1),
                    instr => instr.MatchStfld<BaseShopController>("PreventTeleportingPlayerAway"),
                    instr => instr.MatchLdarg(0),
                    instr => instr.MatchLdcI4(4)))
                    //instr => instr.MatchCall<BaseShopController.ShopState>("set_State")))
                    return;

                cursor.RemoveRange(6);

                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Call, typeof(BaseShopController_Update_Patch).GetMethod("OverrideLJChecks", BindingFlags.Static | BindingFlags.NonPublic));
            }

            private static void OverrideLJChecks(BaseShopController chest)
            {
                var ii = PerkHelper.GetGlobalStacksFromAllPlayers(MaliceID);
                if (ii == 0)
                {
                    chest.PreventTeleportingPlayerAway = true;
                    chest.State = BaseShopController.ShopState.TeleportAway;
                }
            }
        }

        [HarmonyPatch]
        private static class PlayerStats_RecalculateStatsInternal_Patch
        {
            [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.RecalculateStatsInternal))]
            [HarmonyILManipulator]
            private static void RecalculateStatsInternal(ILContext il)
            {
                ILCursor cursor = new ILCursor(il);

                if (!cursor.TryGotoNext(MoveType.Before,
                    instr => instr.MatchPop(),
                    instr => instr.MatchLdloc(45),
                    instr => instr.MatchLdcI4(10)))
                    return;

                cursor.Index += 2;
                cursor.Remove();
                cursor.EmitDelegate<Func<int>>(() =>
                {
                    var ii = PerkHelper.GetGlobalStacksFromAllPlayers(MaliceID);
                    if (ii > 0)
                    {
                        return 15;
                    }
                    return 10;
                });
            }
        }
    }
}

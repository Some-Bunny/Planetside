﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Dungeonator;
using SaveAPI;
using Gungeon;
using ItemAPI;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Brave.BulletScript;



namespace Planetside
{
    public static class Hooks
    {
        public delegate T9 Func<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);


        public static Mesh GetChunkMeshHook(Func<DeadlyDeadlyGoopManager, Single, Single, Mesh> orig, DeadlyDeadlyGoopManager self, int x, int y)
        {
            GameObject gameObject = new GameObject(string.Format("goop_{0}_chunk_{1}_{2}", self.goopDefinition.name, x, y));
            ETGModConsole.Log("Layer:" + gameObject.layer.ToString());
            ETGModConsole.Log("goopDepth:" + self.goopDepth.ToString());
            return orig(self, x, y);
        }

        public static void Init()
        {
            try
            {
                new Hook(typeof(GameManager).GetMethod("DelayedQuickRestart", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("OnQuickRestart1"));

                //new Hook(typeof(Foyer).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PlanetsideModule).GetMethod("ReloadBreachShrinesPSOG"));

                new Hook(typeof(RoomHandler).GetMethod("HandleRoomClearReward", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("GalaxyChestReward"));

                new Hook(typeof(Chest).GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("GalaxyChestPls"));

                new Hook(typeof(PlayerController).GetMethod("OnDidDamage", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("DamageHook"));

                Hook h = new Hook(
                    typeof(Projectile).GetMethod("OnPreCollision", BindingFlags.NonPublic | BindingFlags.Instance),
                    typeof(Hooks).GetMethod("PreCollisionHook")
                );

                Hook h2 = new Hook(
                    typeof(Projectile).GetMethod("HandleDamage", BindingFlags.NonPublic | BindingFlags.Instance),
                    typeof(UndodgeableProjectile).GetMethod("HandleDamageHook", BindingFlags.Public    | BindingFlags.Static)
                );

                Hook ffasdafsdsf = new Hook(
                 typeof(PlayerController).GetMethod("HandleDodgedBeam", BindingFlags.Public | BindingFlags.Instance),
                 typeof(Hooks).GetMethod("HandleDodgedBeamHook", BindingFlags.Public | BindingFlags.Static)
                );

                new Hook(
                typeof(Projectile).GetMethod("BecomeBlackBullet", BindingFlags.Public | BindingFlags.Instance),
                typeof(Hooks).GetMethod("BecomeBlackBulletHook"));

                new Hook(
                typeof(Projectile).GetMethod("ReturnFromBlackBullet", BindingFlags.Public | BindingFlags.Instance),
                typeof(Hooks).GetMethod("ReturnFromBlackBulletHook"));

                new Hook(typeof(AIActor).GetMethod("TeleportSomewhere", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("TeleportationImmunity"));

                //new Hook(typeof(AkSoundEngine).GetMethods().Single((MethodInfo m) => m.Name == "PostEvent" && m.GetParameters().Length == 2 && m.GetParameters()[0].ParameterType == typeof(string)), typeof(Hooks).GetMethod("PostEventHook", BindingFlags.Static | BindingFlags.Public));
            }
            catch (Exception e)
            {
                ItemAPI.Tools.PrintException(e, "FF0000");
            }
        }

        public static uint PostEventHook(Func<string, GameObject, uint> orig, string name, GameObject obj)
        {
            if (name != null)
            {
                ETGModConsole.Log(name, true);
            }
            return orig(name, obj);
        }


        public static void TeleportationImmunity(Action<AIActor, IntVector2?, bool> orig, AIActor self, IntVector2? overrideClearance = null, bool keepClose = false)
        {
            if (self.GetComponent<TeleportationImmunity>() != null) { return; }
            orig(self, overrideClearance, keepClose);
        }

        public static void BecomeBlackBulletHook(Action<Projectile> orig, Projectile self)
        {
            if (self.GetComponent<MarkForUndodgeAbleBullet>() != null)
            {
                if (!self.IsBlackBullet && self.sprite)
                {
                    self.IsBlackBullet = true;
                    self.sprite.renderer.material.SetFloat("_BlackBullet", -1);
                }
            }
            else
            {
                orig(self);
            }
        }
        public static void ReturnFromBlackBulletHook(Action<Projectile> orig, Projectile self)
        {
            if (self.GetComponent<MarkForUndodgeAbleBullet>() != null)
            {
                if (self.IsBlackBullet && self.sprite)
                {
                    self.IsBlackBullet = false;
                    self.sprite.renderer.material.SetFloat("_BlackBullet", 0);
                }
            }
            else
            {
                orig(self);
            }
        }

        public static void AmmoHook(Action<AmmonomiconDeathPageController> orig, AmmonomiconDeathPageController self)
        {
            orig(self);
        }

        public static void HandleDodgedBeamHook(Action<PlayerController ,BeamController> orig, PlayerController self, BeamController beam)
        {
            orig(self, beam);
            if (beam.projectile.GetComponent<MarkForUndodgeAbleBeam>() != null && !self.IsEthereal)
            {
                HealthHaver healthHaver = self.healthHaver;
                float damage = 0.5f;
                if (beam.projectile.BlackPhantomDamageMultiplier != 1f && beam.projectile.Owner.aiActor && beam.projectile.Owner.aiActor.IsBlackPhantom)
                {
                    damage *= beam.projectile.BlackPhantomDamageMultiplier;
                }
                Vector2 velocity = self.specRigidbody.Velocity;
                string ownerName = beam.projectile.OwnerName;
                CoreDamageTypes coreDamageTypes = beam.projectile.damageTypes;
                DamageCategory damageCategory = (!beam.projectile.IsBlackBullet) ? DamageCategory.Normal : DamageCategory.BlackBullet;
                if (PlanetsideReflectionHelper.ReflectGetField<bool>(typeof(HealthHaver), "vulnerable", self.healthHaver) == true && self.healthHaver.spriteAnimator.QueryInvulnerabilityFrame())
                {

                    AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", self.gameObject);
                    GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                    tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                    component.PlaceAtPositionByAnchor(self.transform.position + new Vector3(0.375f, 0.375f), tk2dBaseSprite.Anchor.MiddleCenter);
                    component.HeightOffGround = 35f;
                    component.UpdateZDepth();
                    tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                    if (component2 != null)
                    {
                        component2.ignoreTimeScale = true;
                        component2.AlwaysIgnoreTimeScale = true;
                        component2.AnimateDuringBossIntros = true;
                        component2.alwaysUpdateOffscreen = true;
                        component2.playAutomatically = true;
                    }
                }
                healthHaver.ApplyDamage(damage, velocity, ownerName, coreDamageTypes, damageCategory, true, null, beam.projectile.ignoreDamageCaps);
            }
        }

        public static IEnumerator FireplaceControllerStart(Func<FireplaceController, IEnumerator> orig, FireplaceController self)
        {
           
            IEnumerator origEnum = orig(self);
            while (origEnum.MoveNext())
            {
                object obj = origEnum.Current;
                yield return obj;
            }
        }
        /*
        public static void StartHookMinorBreakable(Action<MinorBreakable> orig, MinorBreakable self)
        {
            Material mat = self.sprite.renderer.material;
            Shader shader = self.sprite.renderer.material.shader;

            if (mat != null) { ETGModConsole.Log("Material: " + mat.ToString()); }
            if (shader != null) { ETGModConsole.Log("Shader: "+shader.ToString()); }

            orig(self);
        }
        */

        public static void PreCollisionHook(Action<Projectile, SpeculativeRigidbody, PixelCollider, SpeculativeRigidbody, PixelCollider> orig, Projectile self, SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody
            otherRigidbody, PixelCollider otherCollider)
        {
            if (self.GetType() == typeof(Projectile) && (self.Owner == null || !(self.Owner is PlayerController)) && self.gameObject.GetComponent<MarkForUndodgeAbleBullet>() != null && otherRigidbody.aiActor == null)
            {
                if (otherRigidbody == self.Shooter && !self.allowSelfShooting)
                {
                    PhysicsEngine.SkipCollision = true;
                    return;
                }
                if (otherRigidbody.gameActor != null && otherRigidbody.gameActor is PlayerController && (!self.collidesWithPlayer || (otherRigidbody.gameActor as PlayerController).IsGhost || (otherRigidbody.gameActor as PlayerController).IsEthereal))
                {
                    PhysicsEngine.SkipCollision = true;
                    return;
                }
                if (otherRigidbody.aiActor)
                {
                    if (self.Owner is PlayerController && !otherRigidbody.aiActor.IsNormalEnemy)
                    {
                        PhysicsEngine.SkipCollision = true;
                        return;
                    }
                    if (self.Owner is AIActor && !self.collidesWithEnemies && otherRigidbody.aiActor.IsNormalEnemy && !otherRigidbody.aiActor.HitByEnemyBullets)
                    {
                        PhysicsEngine.SkipCollision = true;
                        return;
                    }
                }
                if (!GameManager.PVP_ENABLED && self.Owner is PlayerController && otherRigidbody.GetComponent<PlayerController>() != null && !self.allowSelfShooting)
                {
                    PhysicsEngine.SkipCollision = true;
                    return;
                }
                if (GameManager.Instance.InTutorial)
                {
                    PlayerController component = otherRigidbody.GetComponent<PlayerController>();
                    if (component)
                    {
                        if (component.spriteAnimator.QueryInvulnerabilityFrame())
                        {
                            GameManager.BroadcastRoomTalkDoerFsmEvent("playerDodgedBullet");
                        }
                        else if (component.IsDodgeRolling)
                        {
                            GameManager.BroadcastRoomTalkDoerFsmEvent("playerAlmostDodgedBullet");
                        }
                        else
                        {
                            GameManager.BroadcastRoomTalkDoerFsmEvent("playerDidNotDodgeBullet");
                        }
                    }
                }
                if (self.collidesWithProjectiles && self.collidesOnlyWithPlayerProjectiles && otherRigidbody.projectile && !(otherRigidbody.projectile.Owner is PlayerController))
                {
                    PhysicsEngine.SkipCollision = true;
                    return;
                }
            }
            else
            {
                orig(self, myRigidbody, myCollider, otherRigidbody, otherCollider);
            }
        }

        public static IEnumerator TopHook(Func<BabyDragunBurst1, IEnumerator> orig, BabyDragunBurst1 self)
        {
            //IEnumerator origEnum = orig(self);
            for (int i = 0; i < 200; i++)
            {
                self.Fire(new Direction(UnityEngine.Random.Range(-45, 45), DirectionType.Aim, 0f), new Speed(UnityEngine.Random.Range(4f, 20f), SpeedType.Absolute), null);
            }

            yield return null;
            /*
            while (origEnum.MoveNext())
            {
                ETGModConsole.Log("get blocked");
                object obj = origEnum.Current;
                yield return obj;
            }
            */
        }


        public static IEnumerator HandleExplosionHook(Hooks.Func<Exploder, Vector3, ExplosionData, Vector2, Action, bool, CoreDamageTypes, bool, IEnumerator> orig, Exploder self, Vector3 position, ExplosionData data, Vector2 sourceNormal, Action onExplosionBegin, bool ignoreQueues, CoreDamageTypes damageTypes, bool ignoreDamageCaps)
        {
            IEnumerator origEnum = orig(self, position, data, sourceNormal, onExplosionBegin, ignoreQueues, damageTypes, ignoreDamageCaps);
            while (origEnum.MoveNext())
            {
                object obj = origEnum.Current;
                yield return obj;
            }
        }



        public static Vector2 Hook_PlayerController_HandlePlayerInput(Func<PlayerController, Vector2> orig, PlayerController self)
        {
            Vector2 vec = orig(self);
            return vec;
        }

        public static void endme(Action<ResourcefulRatMazeSystemController> orig, ResourcefulRatMazeSystemController self)
        {
            orig(self);
            Type type = typeof(ResourcefulRatMazeSystemController); FieldInfo _property = type.GetField("m_centralRoomSeries", BindingFlags.NonPublic | BindingFlags.Instance); _property.GetValue(self);
            List<RoomHandler> uses = (List<RoomHandler>)_property.GetValue(self);
            foreach(RoomHandler handler in uses)
            {
                //ETGModConsole.Log(handler.GetRoomName().ToString());
            }
        }

        public static void OuroborousRoomDrop(Action<RoomHandler> orig, RoomHandler self)
        {
            bool LoopOn = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.LOOPING_ON);
            if (LoopOn == false)
            {
                orig(self);
            }
            else
            {
                float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
                if (GameManager.Instance.IsFoyer || GameManager.Instance.InTutorial)
                {
                    return;
                }
                if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST)
                {
                    return;
                }
                Type type = typeof(RoomHandler); FieldInfo _property = type.GetField("m_hasGivenReward", BindingFlags.NonPublic | BindingFlags.Instance); _property.GetValue(self);
                bool Has = (bool)_property.GetValue(self);
                if (Has == true)
                {
                    return;
                }
                else
                {
                    Has = true;
                }
                if (self.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.REWARD)
                {
                    return;
                }
                if (self.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS && self.area.PrototypeRoomBossSubcategory == PrototypeDungeonRoom.RoomBossSubCategory.FLOOR_BOSS)
                {
                    HandleBossClearReward(self);
                }
                if (self.PreventStandardRoomReward)
                {
                    return;
                }
                FloorRewardData currentRewardData = GameManager.Instance.RewardManager.CurrentRewardData;
                LootEngine.AmmoDropType ammoDropType = LootEngine.AmmoDropType.DEFAULT_AMMO;
                bool flag = LootEngine.DoAmmoClipCheck(currentRewardData, out ammoDropType);
                string path = (ammoDropType != LootEngine.AmmoDropType.SPREAD_AMMO) ? "Ammo_Pickup" : "Ammo_Pickup_Spread";
                float value = UnityEngine.Random.value;
                float num = currentRewardData.ChestSystem_ChestChanceLowerBound;
                float num2 = GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.Coolness) / 100f + (Loop)*5;
                float num3 = -(GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.Curse) / 100f + (Loop) * 5);
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                {
                    num2 += GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.Coolness) / 100f - Loop;
                    num3 -= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.Curse) / 100f - Loop;
                }
                if (PassiveItem.IsFlagSetAtAll(typeof(ChamberOfEvilItem)))
                {
                    num3 *= -2f;
                }
                num = Mathf.Clamp(num + GameManager.Instance.PrimaryPlayer.AdditionalChestSpawnChance, currentRewardData.ChestSystem_ChestChanceLowerBound, currentRewardData.ChestSystem_ChestChanceUpperBound) + num2 + num3;
                bool flag2 = currentRewardData.SingleItemRewardTable != null;
                bool flag3 = false;
                float num4 = 0.1f - Loop/1000;
                if (!RoomHandler.HasGivenRoomChestRewardThisRun && MetaInjectionData.ForceEarlyChest)
                {
                    flag3 = true;
                }
                if (flag3)
                {
                    if (!RoomHandler.HasGivenRoomChestRewardThisRun && (GameManager.Instance.CurrentFloor == 1 || GameManager.Instance.CurrentFloor == -1))
                    {
                        flag2 = false;
                        num += num4;
                        if (GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.NumRoomsCleared > 4)
                        {
                            num = 1f;
                        }
                    }
                    if (!RoomHandler.HasGivenRoomChestRewardThisRun && self.distanceFromEntrance < RoomHandler.NumberOfRoomsToPreventChestSpawning)
                    {
                        GameManager.Instance.Dungeon.InformRoomCleared(false, false);
                        return;
                    }
                }
                BraveUtility.Log("Current chest spawn chance: " + num, Color.yellow, BraveUtility.LogVerbosity.IMPORTANT);
                if (value > num)
                {
                    if (flag)
                    {
                        IntVector2 bestRewardLocation = self.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.CameraCenter, true);
                        LootEngine.SpawnItem((GameObject)BraveResources.Load(path, ".prefab"), bestRewardLocation.ToVector3(), Vector2.up, 1f, true, true, false);
                    }
                    GameManager.Instance.Dungeon.InformRoomCleared(false, false);
                    return;
                }
                if (flag2)
                {
                    float num5 = currentRewardData.PercentOfRoomClearRewardsThatAreChests;
                    if (PassiveItem.IsFlagSetAtAll(typeof(AmazingChestAheadItem)))
                    {
                        num5 *= 2f;
                        num5 = Mathf.Max(0.5f, num5);
                    }
                    flag2 = (UnityEngine.Random.value > num5);
                }
                if (flag2)
                {
                    float num6 = (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER) ? GameManager.Instance.RewardManager.SinglePlayerPickupIncrementModifier : GameManager.Instance.RewardManager.CoopPickupIncrementModifier;
                    GameObject gameObject;
                    if (UnityEngine.Random.value < 1f / num6)
                    {
                        gameObject = currentRewardData.SingleItemRewardTable.SelectByWeight(false);
                    }
                    else
                    {
                        gameObject = ((UnityEngine.Random.value >= 0.9f) ? GameManager.Instance.RewardManager.FullHeartPrefab.gameObject : GameManager.Instance.RewardManager.HalfHeartPrefab.gameObject);
                    }
                    //UnityEngine.Debug.Log(gameObject.name + "SPAWNED");
                    DebrisObject debrisObject = LootEngine.SpawnItem(gameObject, self.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.CameraCenter, true).ToVector3() + new Vector3(0.25f, 0f, 0f), Vector2.up, 1f, true, true, false);
                    Exploder.DoRadialPush(debrisObject.sprite.WorldCenter.ToVector3ZUp(debrisObject.sprite.WorldCenter.y), 8f, 3f);
                    AkSoundEngine.PostEvent("Play_OBJ_item_spawn_01", debrisObject.gameObject);
                    GameManager.Instance.Dungeon.InformRoomCleared(true, false);
                }
                else
                {
                    IntVector2 bestRewardLocation = self.GetBestRewardLocation(new IntVector2(2, 1), RoomHandler.RewardLocationStyle.CameraCenter, true);
                    bool isRainbowRun = GameStatsManager.Instance.IsRainbowRun;
                    if (isRainbowRun)
                    {
                        LootEngine.SpawnBowlerNote(GameManager.Instance.RewardManager.BowlerNoteChest, bestRewardLocation.ToCenterVector2(), self, true);
                        RoomHandler.HasGivenRoomChestRewardThisRun = true;
                    }
                    else
                    {
                        Chest exists = self.SpawnRoomRewardChest(null, bestRewardLocation);
                        if (exists)
                        {
                            RoomHandler.HasGivenRoomChestRewardThisRun = true;
                        }
                    }
                    GameManager.Instance.Dungeon.InformRoomCleared(true, true);
                }
                if (flag)
                {
                    IntVector2 bestRewardLocation = self.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.CameraCenter, true);
                    LootEngine.DelayedSpawnItem(1f, (GameObject)BraveResources.Load(path, ".prefab"), bestRewardLocation.ToVector3() + new Vector3(0.25f, 0f, 0f), Vector2.up, 1f, true, true, false);
                }
                
            }
        }
        public static void HandleBossClearReward(RoomHandler room)
        {
            if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.SHORTCUT)
            {
                GameStatsManager.Instance.CurrentResRatShopSeed = UnityEngine.Random.Range(1, 1000000);
            }
            GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.Dungeon.tileIndices.tilesetId;
            if (!room.PlayerHasTakenDamageInThisRoom && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE)
            {
                if (tilesetId != GlobalDungeonData.ValidTilesets.GUNGEON)
                {
                    if (tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON)
                    {
                        if (tilesetId != GlobalDungeonData.ValidTilesets.MINEGEON)
                        {
                            if (tilesetId != GlobalDungeonData.ValidTilesets.CATACOMBGEON)
                            {
                                if (tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
                                {
                                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_NOBOSSDAMAGE_FORGE, true);
                                }
                            }
                            else
                            {
                                GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_NOBOSSDAMAGE_HOLLOW, true);
                            }
                        }
                        else
                        {
                            GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_NOBOSSDAMAGE_MINES, true);
                        }
                    }
                    else
                    {
                        GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_NOBOSSDAMAGE_CASTLE, true);
                    }
                }
                else
                {
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_NOBOSSDAMAGE_GUNGEON, true);
                }
            }
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST)
            {
                return;
            }
            if (tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON)
            {
                return;
            }
            if (tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)
            {
                return;
            }
            for (int i = 0; i < room.connectedRooms.Count; i++)
            {
                if (room.connectedRooms[i].area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.EXIT)
                {
                    room.connectedRooms[i].OnBecameVisible(GameManager.Instance.BestActivePlayer);
                }
            }
            IntVector2 intVector = IntVector2.Zero;
            if (room.OverrideBossPedestalLocation != null)
            {
                intVector = room.OverrideBossPedestalLocation.Value;
            }
            else if (!room.area.IsProceduralRoom && room.area.runtimePrototypeData.rewardChestSpawnPosition != IntVector2.NegOne)
            {
                intVector = room.area.basePosition + room.area.runtimePrototypeData.rewardChestSpawnPosition;
            }
            else
            {
                UnityEngine.Debug.LogWarning("tollred");
                intVector = room.GetCenteredVisibleClearSpot(2, 2);
            }
            GameObject gameObject = GameManager.Instance.Dungeon.sharedSettingsPrefab.ChestsForBosses.SelectByWeight();
            Chest chest = gameObject.GetComponent<Chest>();
            bool isRainbowRun = GameStatsManager.Instance.IsRainbowRun;
            if (isRainbowRun)
            {
                chest = null;
            }
            if (chest != null)
            {
                Chest chest2 = Chest.Spawn(chest, intVector, room, false);
                chest2.RegisterChestOnMinimap(room);
            }
            else
            {
                DungeonData data = GameManager.Instance.Dungeon.data;
                RewardPedestal component = gameObject.GetComponent<RewardPedestal>();
                if (component)
                {
                    bool flag = tilesetId != GlobalDungeonData.ValidTilesets.FORGEGEON;
                    bool flag2 = !room.PlayerHasTakenDamageInThisRoom && GameManager.Instance.Dungeon.BossMasteryTokenItemId >= 0 && !GameManager.Instance.Dungeon.HasGivenMasteryToken;
                    if (flag && flag2)
                    {
                        intVector += IntVector2.Left;
                    }
                    if (flag)
                    {
                        RewardPedestal rewardPedestal = RewardPedestal.Spawn(component, intVector, room);
                        rewardPedestal.IsBossRewardPedestal = true;
                        rewardPedestal.lootTable.lootTable = room.OverrideBossRewardTable;
                        rewardPedestal.RegisterChestOnMinimap(room);
                        data[intVector].isOccupied = true;
                        data[intVector + IntVector2.Right].isOccupied = true;
                        data[intVector + IntVector2.Up].isOccupied = true;
                        data[intVector + IntVector2.One].isOccupied = true;
                        if (flag2)
                        {
                            rewardPedestal.OffsetTertiarySet = true;
                        }
                        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.NumberOfLivingPlayers == 1)
                        {
                            rewardPedestal.ReturnCoopPlayerOnLand = true;
                        }
                        if (room.area.PrototypeRoomName == "DoubleBeholsterRoom01")
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                IntVector2 centeredVisibleClearSpot = room.GetCenteredVisibleClearSpot(2, 2);
                                RewardPedestal rewardPedestal2 = RewardPedestal.Spawn(component, centeredVisibleClearSpot, room);
                                rewardPedestal2.IsBossRewardPedestal = true;
                                rewardPedestal2.lootTable.lootTable = room.OverrideBossRewardTable;
                                data[centeredVisibleClearSpot].isOccupied = true;
                                data[centeredVisibleClearSpot + IntVector2.Right].isOccupied = true;
                                data[centeredVisibleClearSpot + IntVector2.Up].isOccupied = true;
                                data[centeredVisibleClearSpot + IntVector2.One].isOccupied = true;
                            }
                        }
                    }
                    else if (tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.NumberOfLivingPlayers == 1)
                    {
                        PlayerController playerController = (!GameManager.Instance.PrimaryPlayer.healthHaver.IsDead) ? GameManager.Instance.SecondaryPlayer : GameManager.Instance.PrimaryPlayer;
                        playerController.specRigidbody.enabled = true;
                        playerController.gameObject.SetActive(true);
                        playerController.ResurrectFromBossKill();
                    }
                    if (flag2)
                    {
                        GameStatsManager.Instance.RegisterStatChange(TrackedStats.MASTERY_TOKENS_RECEIVED, 1f);
                        GameManager.Instance.PrimaryPlayer.MasteryTokensCollectedThisRun++;
                        if (flag)
                        {
                            intVector += new IntVector2(2, 0);
                        }
                        RewardPedestal rewardPedestal3 = RewardPedestal.Spawn(component, intVector, room);
                        data[intVector].isOccupied = true;
                        data[intVector + IntVector2.Right].isOccupied = true;
                        data[intVector + IntVector2.Up].isOccupied = true;
                        data[intVector + IntVector2.One].isOccupied = true;
                        GameManager.Instance.Dungeon.HasGivenMasteryToken = true;
                        rewardPedestal3.SpawnsTertiarySet = false;
                        rewardPedestal3.contents = PickupObjectDatabase.GetById(GameManager.Instance.Dungeon.BossMasteryTokenItemId);
                        rewardPedestal3.MimicGuid = null;
                    }
                }
                if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATHEDRALGEON && GameManager.Options.CurrentGameLootProfile == GameOptions.GameLootProfile.CURRENT)
                {
                    IntVector2? randomAvailableCell = room.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 4), new CellTypes?(CellTypes.FLOOR), false, null);
                    IntVector2? intVector2 = (randomAvailableCell == null) ? null : new IntVector2?(randomAvailableCell.GetValueOrDefault() + IntVector2.One);
                    if (intVector2 != null)
                    {
                        Chest chest3 = Chest.Spawn(GameManager.Instance.RewardManager.Synergy_Chest, intVector2.Value);
                        if (chest3)
                        {
                            chest3.RegisterChestOnMinimap(room);
                        }
                    }
                }
            }
        }

        public static void TableFlipOuroborous(Action<FlippableCover,PlayerController> orig, FlippableCover self, PlayerController player)
        {
            orig(self, player);
            float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
            bool LoopOn = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.LOOPING_ON);
            if (LoopOn == true)
            {
               
            }
        }

        public static void DamageHook(Action<PlayerController, float, bool, HealthHaver> orig, PlayerController self, float damagedone, bool fatal, HealthHaver target)
        {
            orig(self, damagedone, fatal, target);
            if (target.IsBoss && damagedone >= 500 && fatal == true)
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND, true);
            }

        }

        public static void OnQuickRestart1(Action<GameManager, float, QuickRestartOptions> orig, GameManager self, float duration, QuickRestartOptions options = default(QuickRestartOptions))
        {
            orig(self, duration, options);
            Resault.ResetMaxAmmo();
        }
        public static void HandleCustomEnemyChanges(Action<AIActor> orig, AIActor self)
        {

            orig(self);
            try
            {
                PlayerController player = GameManager.Instance.PrimaryPlayer;

                bool flag = self && self.aiActor && self.aiActor.EnemyGuid != null;
                if (flag)
                {
                    string text;
                    if (self == null)
                    {
                        text = null;
                    }
                    else
                    {
                        AIActor aiActor = self.aiActor;
                        text = ((aiActor != null) ? aiActor.EnemyGuid : null);
                    }
                    string text2 = text;
                    bool flag2 = !string.IsNullOrEmpty(text2);
                    if (flag2)
                    {
                        try
                        {
                            bool flag3 = PlanetBlade.weakEnemies.Contains(text2);
                            if (flag3)
                            {

                                self.aiActor.ApplyEffect(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect, 1f, null);
                                self.aiActor.CanTargetPlayers = true;
                                self.aiActor.CanTargetEnemies = true;
                                self.aiActor.IsHarmlessEnemy = true;
                                self.aiActor.IgnoreForRoomClear = true;
                                CompanionController yup = self.gameObject.AddComponent<CompanionController>();
                                yup.companionID = CompanionController.CompanionIdentifier.NONE;
                                yup.CanCrossPits = true;
                                yup.Initialize(player);
                                Planetside.OtherTools.CompanionisedEnemyBulletModifiers yeehaw = yup.gameObject.AddComponent<Planetside.OtherTools.CompanionisedEnemyBulletModifiers>();
                                yeehaw.jammedDamageMultiplier *= 3;
                                yeehaw.baseBulletDamage = 10f;
                                yeehaw.TintBullets = false;

                                bool flag4 = self.gameObject.GetComponent<SpawnEnemyOnDeath>();
                                if (flag4)
                                {
                                    UnityEngine.Object.Destroy(self.gameObject.GetComponent<SpawnEnemyOnDeath>());
                                }
                                var bs = self.GetComponent<BehaviorSpeculator>();

                                foreach (MovementBehaviorBase att in self.behaviorSpeculator.MovementBehaviors)
                                {
                                    if (att is SeekTargetBehavior)
                                    {
                                        SeekTargetBehavior tagr = att as SeekTargetBehavior;
                                        tagr.ReturnToSpawn = false;
                                        tagr.StopWhenInRange = false;
                                        tagr.CustomRange = 7;
                                        tagr.LineOfSight = true;
                                        tagr.SpawnTetherDistance = 0;
                                        tagr.PathInterval = 0.25f;
                                        tagr.SpecifyRange = false;
                                        tagr.MinActiveRange = 0;
                                        tagr.MaxActiveRange = 0;

                                    }
                                }

                                AIAnimator aiAnimator = self.aiAnimator;

                                CompanionFollowPlayerBehavior comp = new CompanionFollowPlayerBehavior();
                                comp.CanRollOverPits = false;
                                comp.CatchUpOutAnimation = aiAnimator.MoveAnimation.Prefix;
                                comp.DisableInCombat = false;
                                comp.IdleAnimations = aiAnimator.IdleAnimation.AnimNames;
                                comp.PathInterval = 0.25f;
                                comp.IdealRadius = 3;
                                comp.CatchUpRadius = 8;
                                comp.CatchUpAccelTime = 5;
                                comp.CatchUpSpeed = self.aiActor.MovementSpeed *= 1.125f;
                                comp.CatchUpMaxSpeed = self.aiActor.MovementSpeed *= 1.4f;
                                comp.CatchUpAnimation = aiAnimator.MoveAnimation.Prefix;
                                comp.RollAnimation = aiAnimator.MoveAnimation.Prefix;
                                comp.TemporarilyDisabled = true;

                                bs.MovementBehaviors.Add(comp);
                                self.aiActor.CompanionOwner = player;
                            }

                        }
                        catch (Exception ex)
                        {
                            ETGModConsole.Log(ex.Message, false);
                        }
                    }
                }
            }
            catch
            {
                ETGModConsole.Log("epic fail");
            }
        }

		public static void GalaxyChestReward(Action<RoomHandler> orig, RoomHandler self)
		{
			orig(self);
            float rng;
            rng = UnityEngine.Random.Range(0.0000000f, 1.0000000f);
            if (rng <= 0.0000001f)
            {
                //1,000,000

                PlayerController player = GameManager.Instance.PrimaryPlayer;
                Chest rainbow_Chest = GameManager.Instance.RewardManager.Rainbow_Chest;
                Chest chest2 = Chest.Spawn(rainbow_Chest, player.CurrentRoom.GetRandomVisibleClearSpot(1, 1));
                chest2.sprite.usesOverrideMaterial = true;

                var texture = StaticTextures.NebulaTexture;
                chest2.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
                chest2.sprite.renderer.material.SetTexture("_EeveeTex", texture);

                chest2.sprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
                chest2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");

                chest2.lootTable.S_Chance = 0.2f;
                chest2.lootTable.A_Chance = 0.2f;
                chest2.lootTable.B_Chance = 0.22f;
                chest2.lootTable.C_Chance = 0.22f;
                chest2.lootTable.D_Chance = 0.16f;
                chest2.lootTable.Common_Chance = 0f;
                chest2.lootTable.canDropMultipleItems = true;
                chest2.lootTable.multipleItemDropChances = new WeightedIntCollection();
                chest2.lootTable.multipleItemDropChances.elements = new WeightedInt[1];
                chest2.lootTable.overrideItemLootTables = new List<GenericLootTable>();
                chest2.lootTable.lootTable = GameManager.Instance.RewardManager.GunsLootTable;

                for (int i = 0; i < 12; i++)
                {
                    chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
                    chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
                }
                WeightedInt weightedInt = new WeightedInt();
                weightedInt.value = 24;
                weightedInt.weight = 1f;
                weightedInt.additionalPrerequisites = new DungeonPrerequisite[0];
                chest2.lootTable.multipleItemDropChances.elements[0] = weightedInt;
                chest2.lootTable.onlyOneGunCanDrop = false;
                chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
            }
		}

        public static void GalaxyChestPls(Action<Chest> orig, Chest self)
		{
            orig(self);
            float rng;
            rng = UnityEngine.Random.Range(0.0000f, 1.0000f);

            if (!self.IsGlitched && !self.IsMimic && !self.IsRainbowChest && (rng <= 0.0001f))
            {

                
                self.sprite.usesOverrideMaterial = true;
                self.BecomeRainbowChest();


                self.IsLocked= false;
                var texture = StaticTextures.NebulaTexture;

                self.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
                self.sprite.renderer.material.SetTexture("_EeveeTex", texture);

                self.sprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
                self.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");

                self.lootTable.S_Chance = 0.2f;
                self.lootTable.A_Chance = 0.2f;
                self.lootTable.B_Chance = 0.22f;
                self.lootTable.C_Chance = 0.22f;
                self.lootTable.D_Chance = 0.16f;
                self.lootTable.Common_Chance = 0f;
                self.lootTable.canDropMultipleItems = true;
                self.lootTable.multipleItemDropChances = new WeightedIntCollection();
                self.lootTable.multipleItemDropChances.elements = new WeightedInt[1];
                self.lootTable.overrideItemLootTables = new List<GenericLootTable>();
                self.lootTable.lootTable = GameManager.Instance.RewardManager.GunsLootTable;
                for (int i = 0; i < 12; i++)
                {
                    self.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
                    self.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
                }
                WeightedInt weightedInt = new WeightedInt();
                weightedInt.value = 24;
                weightedInt.weight = 1f;
                weightedInt.additionalPrerequisites = new DungeonPrerequisite[0];
                self.lootTable.multipleItemDropChances.elements[0] = weightedInt;
                self.lootTable.onlyOneGunCanDrop = false;                
            }
		}



	}
}


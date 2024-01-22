using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using SaveAPI;

namespace Planetside
{
    class PitLordsPactController : MonoBehaviour
    {
        public PitLordsPactController()
        {
            this.EnemySacrificeDamage = 30;
            this.EnemyAbovePitDamage = 6.66f;
            this.EnemySacrificedBonus = 4;

            this.ItemSacrificablePerFloor = 1;

            this.TablesSarificeBonusMin = 3;
            this.TablesSarificeBonusMax = 7;
            this.TablesSarificeChance = 0.2f;

            this.SelfSacrificeWithPitLordAmuletCap = 3;

            this.TemporaryFlightTime = 1;

            this.hasBeenPickedup = false;
        }
        public void Start() 
        { 
            this.hasBeenPickedup = true;
            GameManager.Instance.OnNewLevelFullyLoaded += this.OnNewFloorLoaded;
            if (player != null)
            {
                OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, 0.90f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                OtherTools.ApplyStat(player, PlayerStats.StatType.DamageToBosses, 0.95f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                OtherTools.ApplyStat(player, PlayerStats.StatType.KnockbackMultiplier, 1.66f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            }
        }
        public void Update()
        {
            if (player != null)
            {          
                if (player.CurrentRoom != null)
                {
                    List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    if (activeEnemies != null && activeEnemies.Count >= 0)
                    {
                        foreach (AIActor ai in activeEnemies)
                        {
                            if (ai.specRigidbody != null && ai != null)
                            {
                                if (EnemyIsOverPit(ai) == true && ai != null && !ai.healthHaver.IsDead && !ai.IsFalling && ai.healthHaver != null && ai.isActiveAndEnabled == true)
                                {
                                    ai.healthHaver.ApplyDamage(EnemyAbovePitDamage * BraveTime.DeltaTime, Vector2.zero, "Pit Lords Wrath", CoreDamageTypes.None, DamageCategory.Environment, false, null, false);
                                }
                            }              
                        }
                    }
                }
                       
            }
        }

        public bool EnemyIsOverPit(AIActor aIActor)
        {
            if (aIActor == null) { return false; }

            Vector2 v = aIActor.specRigidbody == null ? aIActor.sprite.WorldCenter : aIActor.specRigidbody.GroundPixelCollider != null ? aIActor.specRigidbody.GroundPixelCollider.UnitCenter : aIActor.sprite.WorldCenter;
            if (GameManager.Instance.Dungeon == null) { return false; }
            return GameManager.Instance.Dungeon.CellSupportsFalling(v);
        }

        private void OnNewFloorLoaded()
        {
            AmountOfItemsSacrificed = 0;
            AmountSelfSacrificedWithPitLordAmulet = 0;
        }
        public void IncrementStack()
        {
            this.EnemySacrificeDamage += 30;
            this.EnemyAbovePitDamage += 6.66f;
            this.EnemySacrificedBonus += 2;

            this.ItemSacrificablePerFloor++;

            this.TablesSarificeChance += 0.05f;
            this.TablesSarificeBonusMin++;
            this.TablesSarificeBonusMax++;
        }


        public float EnemySacrificedBonus;
        public float EnemySacrificeDamage;

        public float EnemyAbovePitDamage;

        public int ItemSacrificablePerFloor;
        public int AmountOfItemsSacrificed;


        public int TablesSarificeBonusMin;
        public int TablesSarificeBonusMax;
        public float TablesSarificeChance;

        public float SelfSacrificeWithPitLordAmuletCap;
        public float AmountSelfSacrificedWithPitLordAmulet;


        public float TemporaryFlightTime;

        public bool hasBeenPickedup;

        public PlayerController player;
    }

    class PitLordsPact : PerkPickupObject, IPlayerInteractable
    {
        public static void Init()
        {


            string name = "Pit Lords Pact";
            //string resourcePath = "Planetside/Resources/PerkThings/pitLordsPact.png";
            GameObject gameObject = new GameObject(name);
            PitLordsPact item = gameObject.AddComponent<PitLordsPact>();


            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("pitLordsPact"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Literally just an all stats up.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            PitLordsPact.PitLordsPactID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.black;
            particles.ParticleSystemColor2 = new Color(50f, 0f, 0f);
            item.OutlineColor = new Color(0f, 0f, 0f);

            new Hook(typeof(FlippableCover).GetMethod("StartFallAnimation", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PitLordsPact).GetMethod("StartFallAnimationHook"));
            new Hook(typeof(DebrisObject).GetMethod("MaybeRespawnIfImportant", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PitLordsPact).GetMethod("MaybeRespawnIfImportantHook"));
            new Hook(typeof(AIActor).GetMethod("Fall", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PitLordsPact).GetMethod("FallHook"));
            new Hook(typeof(PlayerController).GetMethod("Fall", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PitLordsPact).GetMethod("FallHookPlayer"));


           

            GenericLootTable pitLordsPactPickupTable = LootTableTools.CreateLootTable();
            pitLordsPactPickupTable.AddItemsToPool(new Dictionary<int, float>() { { 70, 1f }, { 73, 0.7f }, { 120, 0.6f }, { 85, 0.6f }, { 565, 0.5f }, { 224, 0.5f }, { 67, 0.33f }, { LeSackPickup.SaccID, 0.02f }, { NullPickupInteractable.NollahID, 0.02f }, });
            PitLordsPactTable = pitLordsPactPickupTable;

            GenericLootTable pitLordsPactPickupTableNoHP = LootTableTools.CreateLootTable();
            pitLordsPactPickupTableNoHP.AddItemsToPool(new Dictionary<int, float>() { { 70, 1f },{ 565, 0.5f }, { 224, 0.5f }, { 67, 0.33f }, { LeSackPickup.SaccID, 0.02f }, { NullPickupInteractable.NollahID, 0.02f }, });
            PitLordsPactTableNoHP = pitLordsPactPickupTableNoHP;
        }

        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("Sacrifice To Pits"),
                    UnlockedString = "Sacrificing various things to pits can grant rewards.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 2,
                    LockedString = AlphabetController.ConvertString("Pit Blast"),
                    UnlockedString = "Live sacrifices do soul explosions.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("Pit Damage"),
                    UnlockedString = "Enemies above pits take damage.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 7,
                    LockedString = AlphabetController.ConvertString("Sacrifice Foes"),
                    UnlockedString = "Sacrificing living things can pay out with loot.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.PITLORDPACT_FLAG_FALLLIVING
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 8,
                    LockedString = AlphabetController.ConvertString("Reroll Items"),
                    UnlockedString = "Sacrificing items can exchange them, up to a certain point.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.PITLORDPACT_FLAG_ITEM
                },

                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 9,
                    LockedString = AlphabetController.ConvertString("Larger Selection"),
                    UnlockedString = "Stacking grants more pit damage, rerolls and sacrifice damage.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.PITLORDPACT_FLAG_STACK

                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 15,
                    LockedString = AlphabetController.ConvertString("No Cheese"),
                    UnlockedString = "Fall Damage Immunity Prevents farming pickups.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 999,
                    LockedString = AlphabetController.ConvertString("I Knew Someone Would Do It"),
                    UnlockedString = "Sacrificing tables pays out with money.",
                    requiresStack = false
                },
        };
        public override CustomTrackedStats StatToIncreaseOnPickup => SaveAPI.CustomTrackedStats.AMOUNT_BOUGHT_PITLORDPACT;

        public static GenericLootTable PitLordsPactTable;
        private static GenericLootTable PitLordsPactTableNoHP;

        public static int PitLordsPactID;
        public new bool PrerequisitesMet()
        {
            EncounterTrackable component = base.GetComponent<EncounterTrackable>();
            return component == null || component.PrerequisitesMet();
        }
        public static void FallHookPlayer(Action<PlayerController> orig, PlayerController self)
        {
            orig(self);
            bool IsFallingIntoElevatorShaft = self.CurrentRoom != null && self.CurrentRoom.RoomFallValidForMaintenance();
            bool IsFallingIntoOtherRoom = (self.CurrentRoom != null && self.CurrentRoom.TargetPitfallRoom != null) || GetCurrentCellPitfallTarget(self) != null;
            bool GunpreventsDamage = self.CurrentGun && self.CurrentGun.gunName == "Mermaid Gun";
            bool IsPitimmune = self.ImmuneToPits.Value;

            PitLordsPactController pact = self.GetComponent<PitLordsPactController>();
            if (pact != null && IsFallingIntoOtherRoom != true && IsFallingIntoElevatorShaft != true && GunpreventsDamage != true && IsPitimmune != true)
            {
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.PITLORDPACT_FLAG_FALLLIVING, true);

                GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, self.transform.position, Quaternion.identity, false);
                AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", self.gameObject);
                if (gameObject && gameObject.GetComponent<tk2dSprite>())
                {
                    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                    component.scale *= 2;
                    component.HeightOffGround = 5f;
                    component.UpdateZDepth();

                }
                ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
                boomboom.damageToPlayer = 0;
                boomboom.damageRadius = 8f;
                boomboom.damage = pact.EnemySacrificeDamage * 3;
                boomboom.preventPlayerForce = true;
                boomboom.ignoreList.Add(self.specRigidbody);
                boomboom.playDefaultSFX = false;
                boomboom.doExplosionRing = false;
                Exploder.Explode(self.sprite.WorldCenter, boomboom, self.transform.PositionVector2());
                if (UnityEngine.Random.value < 1 - (self.healthHaver.GetCurrentHealthPercentage()) || self.ForceZeroHealthState == true)
                {
                    PickupObject pickupObject = LootEngine.GetItemOfTypeAndQuality<PickupObject>(PickupObject.ItemQuality.COMMON, PitLordsPact.PitLordsPactTableNoHP, false);
                    IntVector2 bestRewardLocation = self.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
                    DebrisObject debrisObject = LootEngine.SpawnItem(pickupObject.gameObject, bestRewardLocation.ToVector3(), Vector2.up, 0f, true, false, false);           
                    Vector2 v = (!debrisObject.sprite) ? (debrisObject.transform.position.XY() + new Vector2(0.5f, 0.5f)) : debrisObject.sprite.WorldCenter;
                    GameObject gameObject2 = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, v, Quaternion.identity, false);
                    if (gameObject2 && gameObject2.GetComponent<tk2dSprite>())
                    {
                        tk2dSprite component2 = gameObject2.GetComponent<tk2dSprite>();
                        component2.HeightOffGround = 5f;
                        component2.UpdateZDepth();
                    }
                }
            }
            else if (pact != null && pact.SelfSacrificeWithPitLordAmuletCap >= pact.AmountSelfSacrificedWithPitLordAmulet && IsFallingIntoOtherRoom != true && IsFallingIntoElevatorShaft != true)
            {
                pact.AmountSelfSacrificedWithPitLordAmulet++;
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.PITLORDPACT_FLAG_FALLPLAYER_AMULET, true);
                GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, self.transform.position, Quaternion.identity, false);
                AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", self.gameObject);
                if (gameObject && gameObject.GetComponent<tk2dSprite>())
                {
                    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                    component.scale *= 2;
                    component.HeightOffGround = 5f;
                    component.UpdateZDepth();

                }
                ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
                boomboom.damageToPlayer = 0;
                boomboom.damageRadius = 12f;
                boomboom.damage = pact.EnemySacrificeDamage * 5;
                boomboom.preventPlayerForce = true;
                boomboom.ignoreList.Add(self.specRigidbody);
                boomboom.playDefaultSFX = false;
                boomboom.doExplosionRing = false;
                Exploder.Explode(self.sprite.WorldCenter, boomboom, self.transform.PositionVector2());
                PickupObject pickupObject = LootEngine.GetItemOfTypeAndQuality<PickupObject>(PickupObject.ItemQuality.COMMON, PitLordsPact.PitLordsPactTableNoHP, false);
                IntVector2 bestRewardLocation = self.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
                DebrisObject debrisObject = LootEngine.SpawnItem(pickupObject.gameObject, bestRewardLocation.ToVector3(), Vector2.up, 0f, true, false, false);
                Vector2 v = (!debrisObject.sprite) ? (debrisObject.transform.position.XY() + new Vector2(0.5f, 0.5f)) : debrisObject.sprite.WorldCenter;
                GameObject gameObject2 = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, v, Quaternion.identity, false);
                if (gameObject2 && gameObject2.GetComponent<tk2dSprite>())
                {
                    tk2dSprite component2 = gameObject2.GetComponent<tk2dSprite>();
                    component2.HeightOffGround = 5f;
                    component2.UpdateZDepth();
                }
            }
            else if (pact != null && pact.SelfSacrificeWithPitLordAmuletCap <= pact.AmountSelfSacrificedWithPitLordAmulet && IsFallingIntoOtherRoom != true && IsFallingIntoElevatorShaft != true)
            {
                GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, self.transform.position, Quaternion.identity, false);
                AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", self.gameObject);
                if (gameObject && gameObject.GetComponent<tk2dSprite>())
                {
                    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                    component.scale *= 1.33f;
                    component.HeightOffGround = 5f;
                    component.UpdateZDepth();

                }
                ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
                boomboom.damageToPlayer = 0;
                boomboom.damageRadius = 6f;
                boomboom.damage = pact.EnemySacrificeDamage/4;
                boomboom.preventPlayerForce = true;
                boomboom.ignoreList.Add(self.specRigidbody);
                boomboom.playDefaultSFX = false;
                boomboom.doExplosionRing = false;
                Exploder.Explode(self.sprite.WorldCenter, boomboom, self.transform.PositionVector2());
            }
        }

        public static IEnumerator PitRespawnHook(Func<PlayerController, Vector2, IEnumerator> orig, PlayerController self, Vector2 splashPoint)
        {
            bool IsFallingIntoElevatorShaft = self.CurrentRoom != null && self.CurrentRoom.RoomFallValidForMaintenance();
            bool IsFallingIntoOtherRoom = (self.CurrentRoom != null && self.CurrentRoom.TargetPitfallRoom != null) || GetCurrentCellPitfallTarget(self) != null;
            bool GunpreventsDamage = self.CurrentGun && self.CurrentGun.gunName == "Mermaid Gun";
            bool IsPitimmune = self.ImmuneToPits.Value;

            PitLordsPactController pact = self.GetComponent<PitLordsPactController>();
            if (pact != null && IsFallingIntoOtherRoom != true && IsFallingIntoElevatorShaft != true && GunpreventsDamage != true && IsPitimmune != true)
            {
                GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, self.transform.position, Quaternion.identity, false);
                AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", self.gameObject);
                if (gameObject && gameObject.GetComponent<tk2dSprite>())
                {
                    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                    component.scale *= 2;
                    component.HeightOffGround = 5f;
                    component.UpdateZDepth();

                }
                ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
                boomboom.damageToPlayer = 0;
                boomboom.damageRadius = 8f;
                boomboom.damage = pact.EnemySacrificeDamage * 5;
                boomboom.preventPlayerForce = true;
                boomboom.ignoreList.Add(self.specRigidbody);
                boomboom.playDefaultSFX = false;
                boomboom.doExplosionRing = false;
                Exploder.Explode(self.sprite.WorldCenter, boomboom, self.transform.PositionVector2());

                if (GameManager.Instance.Dungeon.IsEndTimes == false)
                {
                    if (UnityEngine.Random.value < 1 - (self.healthHaver.GetCurrentHealthPercentage()) || self.ForceZeroHealthState == true)
                    {
                        PickupObject pickupObject = LootEngine.GetItemOfTypeAndQuality<PickupObject>(PickupObject.ItemQuality.COMMON, PitLordsPact.PitLordsPactTableNoHP, false);
                        DebrisObject debrisObject = LootEngine.SpawnItem(pickupObject.gameObject, self.transform.position, Vector2.up, 0f, true, false, false);
                        Vector2 v = (!debrisObject.sprite) ? (debrisObject.transform.position.XY() + new Vector2(0.5f, 0.5f)) : debrisObject.sprite.WorldCenter;
                        GameObject gameObject2 = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, v, Quaternion.identity, false);
                        if (gameObject2 && gameObject2.GetComponent<tk2dSprite>())
                        {
                            tk2dSprite component2 = gameObject2.GetComponent<tk2dSprite>();
                            component2.HeightOffGround = 5f;
                            component2.UpdateZDepth();
                        }
                    }
                }

                
            }
            IEnumerator origEnum = orig(self, splashPoint);
            while (origEnum.MoveNext())
            {
                object obj = origEnum.Current;
                yield return obj;
            }
           
        }
        private static RoomHandler GetCurrentCellPitfallTarget(PlayerController player)
        {
            IntVector2 intVector = player.CenterPosition.ToIntVector2(VectorConversions.Floor);
            if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector))
            {
                CellData cellData = GameManager.Instance.Dungeon.data[intVector];
                return cellData.targetPitfallRoom;
            }
            return null;
        }
        public static void FallHook(Action<AIActor> orig, AIActor self)
        {
            orig(self);

            if (!self.IsHarmlessEnemy && self.GetComponent<CompanionController>() == null)
            {
                PlayerController[] players = GameManager.Instance.AllPlayers;
                for (int i = 0; i < players.Length; i++)
                {
                    PlayerController player = players[i];
                    PitLordsPactController pact = player.GetComponent<PitLordsPactController>();
                    if (pact != null)
                    {
                        GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, self.transform.position, Quaternion.identity, false);
                        AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", player.gameObject);
                        if (gameObject && gameObject.GetComponent<tk2dSprite>())
                        {
                            tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                            component.scale *= 2;
                            component.HeightOffGround = 5f;
                            component.UpdateZDepth();

                        }
                        ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
                        boomboom.damageToPlayer = 0;
                        boomboom.damageRadius = 8f;
                        boomboom.damage = pact.EnemySacrificeDamage;
                        boomboom.preventPlayerForce = true;
                        boomboom.ignoreList.Add(player.specRigidbody);
                        boomboom.playDefaultSFX = false;
                        boomboom.doExplosionRing = false;
                        Exploder.Explode(self.sprite.WorldCenter, boomboom, self.transform.PositionVector2());
                        if (UnityEngine.Random.value < self.healthHaver.GetMaxHealth() / 200)
                        {
                            PickupObject pickupObject = LootEngine.GetItemOfTypeAndQuality<PickupObject>(PickupObject.ItemQuality.COMMON, PitLordsPact.PitLordsPactTable, false);
                            DebrisObject debrisObject = LootEngine.SpawnItem(pickupObject.gameObject, player.transform.position, Vector2.up, 0f, true, false, false);
                            Vector2 v = (!debrisObject.sprite) ? (debrisObject.transform.position.XY() + new Vector2(0.5f, 0.5f)) : debrisObject.sprite.WorldCenter;
                            GameObject gameObject2 = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, v, Quaternion.identity, false);
                            if (gameObject2 && gameObject2.GetComponent<tk2dSprite>())
                            {
                                tk2dSprite component2 = gameObject2.GetComponent<tk2dSprite>();
                                component2.HeightOffGround = 5f;
                                component2.UpdateZDepth();
                            }
                        }
                    }
                }
            }          
        }


        public static IEnumerator FallDownCRHook(Func<AIActor, IEnumerator> orig, AIActor self)
        {
            IEnumerator origEnum = orig(self);
            while (origEnum.MoveNext())
            {
                object obj = origEnum.Current;
                yield return obj;
            }
            PlayerController[] players = GameManager.Instance.AllPlayers;
            for (int i = 0; i < players.Length; i++)
            {
                PlayerController player = players[i];
                PitLordsPactController pact = player.GetComponent<PitLordsPactController>();
                if (pact != null)
                {
                    SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.PITLORDPACT_FLAG_FALLLIVING, true);
                    GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, self.transform.position, Quaternion.identity, false);
                    AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", player.gameObject);
                    if (gameObject && gameObject.GetComponent<tk2dSprite>())
                    {
                        tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                        component.scale *= 3;
                        component.HeightOffGround = 5f;
                        component.UpdateZDepth();
                      
                    }
                    ETGModConsole.Log("4");
                    ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericLargeExplosion;
                    boomboom.damageToPlayer = 0;
                    boomboom.damageRadius = 6.5f;
                    boomboom.damage = pact.EnemySacrificeDamage;
                    boomboom.preventPlayerForce = true;
                    boomboom.ignoreList.Add(player.specRigidbody);
                    boomboom.playDefaultSFX = false;
                    ETGModConsole.Log("5");
                    Exploder.Explode(player.sprite.WorldCenter, boomboom, player.transform.PositionVector2());

                    if (UnityEngine.Random.value > self.healthHaver.GetMaxHealth()/100)
                    {
                        PickupObject pickupObject = LootEngine.GetItemOfTypeAndQuality<PickupObject>(PickupObject.ItemQuality.COMMON, PitLordsPact.PitLordsPactTable, false);
                        DebrisObject debrisObject = LootEngine.SpawnItem(pickupObject.gameObject, player.transform.position, Vector2.up, 0f, true, false, false);
                        Vector2 v = (!debrisObject.sprite) ? (debrisObject.transform.position.XY() + new Vector2(0.5f, 0.5f)) : debrisObject.sprite.WorldCenter;
                        GameObject gameObject2 = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, v, Quaternion.identity, false);
                        if (gameObject2 && gameObject2.GetComponent<tk2dSprite>())
                        {
                            tk2dSprite component2 = gameObject2.GetComponent<tk2dSprite>();
                            component2.HeightOffGround = 5f;
                            component2.UpdateZDepth();
                        }
                    }
                   
                    ETGModConsole.Log("6");
                }
            }
          
            ETGModConsole.Log("7");
            yield break;
        }
        public static void MaybeRespawnIfImportantHook(Action<DebrisObject> orig, DebrisObject self)
        {      
            orig(self);
            PickupObject pickupObj = self.GetComponent<PickupObject>();
            if (self.GetComponentInChildren<Gun>())
            {
                Gun droppedGun = self.GetComponentInChildren<Gun>();
                PlayerController[] players = GameManager.Instance.AllPlayers;
                for (int i = 0; i < players.Length; i++)
                {
                    PlayerController player = players[i];
                    PitLordsPactController pact = player.GetComponent<PitLordsPactController>();
                    if (pact != null)
                    {
                        SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.PITLORDPACT_FLAG_ITEM, true);


                        if (pact.AmountOfItemsSacrificed < pact.ItemSacrificablePerFloor)
                        {
                            pact.AmountOfItemsSacrificed++;
                            PickupObject.ItemQuality quality = droppedGun.quality;
                            if (quality != PickupObject.ItemQuality.COMMON && quality != PickupObject.ItemQuality.SPECIAL && quality != PickupObject.ItemQuality.EXCLUDED)
                            {
                                PickupObject pickupObject = LootEngine.GetItemOfTypeAndQuality<Gun>(quality, GameManager.Instance.RewardManager.GunsLootTable, false);
                                if (pickupObject)
                                {

                                    DebrisObject debrisObject = LootEngine.SpawnItem(pickupObject.gameObject, player.transform.position, Vector2.up, 0f, true, false, false);
                                    PickupObject componentInChildren = debrisObject.GetComponentInChildren<PickupObject>();
                                    if (componentInChildren && !componentInChildren.IgnoredByRat)
                                    {
                                        componentInChildren.ClearIgnoredByRatFlagOnPickup = true;
                                        componentInChildren.IgnoredByRat = true;
                                    }
                                    Vector2 v = (!debrisObject.sprite) ? (debrisObject.transform.position.XY() + new Vector2(0.5f, 0.5f)) : debrisObject.sprite.WorldCenter;
                                    GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, v, Quaternion.identity, false);
                                    if (gameObject && gameObject.GetComponent<tk2dSprite>())
                                    {
                                        tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                                        component.HeightOffGround = 5f;
                                        component.UpdateZDepth();
                                    }
                                    AkSoundEngine.PostEvent("Play_PrisonerLaugh", player.gameObject);
                                }
                            }
                        }
                        else
                        {
                            PickupObject.ItemQuality quality = droppedGun.quality;
                            if (quality != PickupObject.ItemQuality.COMMON && quality != PickupObject.ItemQuality.SPECIAL && quality != PickupObject.ItemQuality.EXCLUDED)
                            {
                                int moneyTogive = 24;
                                switch (quality)
                                {
                                    case PickupObject.ItemQuality.D:
                                        moneyTogive = 10;
                                        break;
                                    case PickupObject.ItemQuality.C:
                                        moneyTogive = 16;
                                        break;
                                    case PickupObject.ItemQuality.B:
                                        moneyTogive = 24;
                                        break;
                                    case PickupObject.ItemQuality.A:
                                        moneyTogive = 32;
                                        break;
                                    case PickupObject.ItemQuality.S:
                                        moneyTogive = 40;
                                        break;
                                }
                                AkSoundEngine.PostEvent("Play_OBJ_coin_medium_01", self.gameObject);
                                LootEngine.SpawnCurrency((!player.sprite) ? player.specRigidbody.UnitCenter : player.sprite.WorldCenter, moneyTogive + UnityEngine.Random.Range(-8, 8), false);
                            }
                        }
                    }
                }
            }



            if (self && self.IsPickupObject && pickupObj != null)
            {
                PlayerController[] players = GameManager.Instance.AllPlayers;
                for (int i = 0; i < players.Length; i++)
                {
                    PlayerController player = players[i];
                    PitLordsPactController pact = player.GetComponent<PitLordsPactController>();
                    if (pact != null)
                    {
                        if (pact.AmountOfItemsSacrificed <= pact.ItemSacrificablePerFloor)
                        {
                            pact.AmountOfItemsSacrificed++;
                            PickupObject.ItemQuality quality = pickupObj.quality;
                            if (quality != PickupObject.ItemQuality.COMMON && quality != PickupObject.ItemQuality.SPECIAL && quality != PickupObject.ItemQuality.EXCLUDED)
                            {
                                PickupObject pickupObject = null;
                                if (pickupObj.GetComponent<Gun>() != null)
                                {
                                    pickupObject = LootEngine.GetItemOfTypeAndQuality<Gun>(quality, GameManager.Instance.RewardManager.GunsLootTable, false);
                                }
                                else if (pickupObj is PassiveItem)
                                {
                                    pickupObject = LootEngine.GetItemOfTypeAndQuality<PassiveItem>(quality, GameManager.Instance.RewardManager.ItemsLootTable, false);
                                }
                                else if (pickupObj is PlayerItem)
                                {
                                    pickupObject = LootEngine.GetItemOfTypeAndQuality<PlayerItem>(quality, GameManager.Instance.RewardManager.ItemsLootTable, false);
                                }
                                if (pickupObject)
                                {

                                    DebrisObject debrisObject = LootEngine.SpawnItem(pickupObject.gameObject, player.transform.position, Vector2.up, 0f, true, false, false);
                                    PickupObject componentInChildren = debrisObject.GetComponentInChildren<PickupObject>();
                                    if (componentInChildren && !componentInChildren.IgnoredByRat)
                                    {
                                        componentInChildren.ClearIgnoredByRatFlagOnPickup = true;
                                        componentInChildren.IgnoredByRat = true;
                                    }
                                    Vector2 v = (!debrisObject.sprite) ? (debrisObject.transform.position.XY() + new Vector2(0.5f, 0.5f)) : debrisObject.sprite.WorldCenter;
                                    GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, v, Quaternion.identity, false);
                                    if (gameObject && gameObject.GetComponent<tk2dSprite>())
                                    {
                                        tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                                        component.HeightOffGround = 5f;
                                        component.UpdateZDepth();
                                    }
                                    AkSoundEngine.PostEvent("Play_PrisonerLaugh", player.gameObject);
                                }
                            }
                        }
                        else
                        {
                            PickupObject.ItemQuality quality = pickupObj.quality;
                            if (quality != PickupObject.ItemQuality.COMMON && quality != PickupObject.ItemQuality.SPECIAL && quality != PickupObject.ItemQuality.EXCLUDED)
                            {
                                int moneyTogive = 24;
                                switch (quality)
                                {
                                    case PickupObject.ItemQuality.D:
                                        moneyTogive = 10;
                                        break;
                                    case PickupObject.ItemQuality.C:
                                        moneyTogive = 16;
                                        break;
                                    case PickupObject.ItemQuality.B:
                                        moneyTogive = 24;
                                        break;
                                    case PickupObject.ItemQuality.A:
                                        moneyTogive = 32;
                                        break;
                                    case PickupObject.ItemQuality.S:
                                        moneyTogive = 40;
                                        break;
                                }
                                AkSoundEngine.PostEvent("Play_OBJ_coin_medium_01", self.gameObject);
                                LootEngine.SpawnCurrency((!player.sprite) ? player.specRigidbody.UnitCenter : player.sprite.WorldCenter, moneyTogive + UnityEngine.Random.Range(-8, 8), false);
                            }
                        }
                    }
                }         
            }
        }
        public static IEnumerator StartFallAnimationHook(Func<FlippableCover, IntVector2, IEnumerator> orig, FlippableCover self, IntVector2 dir)
        {
            IEnumerator origEnum = orig(self, dir);
            while (origEnum.MoveNext())
            {
                object obj = origEnum.Current;
                yield return obj;
            }
            PlayerController[] players = GameManager.Instance.AllPlayers;
            for (int i = 0; i < players.Length; i++)
            {
                PlayerController player = players[i];
                PitLordsPactController pact = player.GetComponent<PitLordsPactController>();
                if (pact != null)
                {
                    SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.PITLORDPACT_FLAG_TABLE, true);
                    AkSoundEngine.PostEvent("Play_OBJ_coin_medium_01", self.gameObject);
                    LootEngine.SpawnCurrency((!player.sprite) ? player.specRigidbody.UnitCenter : player.sprite.WorldCenter, UnityEngine.Random.Range(pact.TablesSarificeBonusMax, pact.TablesSarificeBonusMax), false);
                }
            }
            yield break;
        }


        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            SaveAPI.AdvancedGameStatsManager.Instance.RegisterStatChange(StatToIncreaseOnPickup, 1);
            m_hasBeenPickedUp = true;
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null && player != null) { cont.DoBigBurst(player); }
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);

            PitLordsPactController pact = player.gameObject.GetOrAddComponent<PitLordsPactController>();
            pact.player = player;
            if (pact.hasBeenPickedup == true)
            { pact.IncrementStack(); SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.PITLORDPACT_FLAG_STACK, true); }

            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = pact.hasBeenPickedup == true ? "Offerings Yield More." : "The Abyss demands all offerings.";
            //OtherTools.Notify("Pit Lords Pact.", BlurbText, "Planetside/Resources/PerkThings/pitLordsPact", UINotificationController.NotificationColor.GOLD);
            OtherTools.NotifyCustom("Pit Lords Pact", BlurbText, "pitLordsPact", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);

            UnityEngine.Object.Destroy(base.gameObject);
        }

        public void Start()
        {
            try
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(this);
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            }
            catch (Exception er)
            {
                ETGModConsole.Log(er.Message, false);
            }
        }


        private void Update()
        {
            if (!this.m_hasBeenPickedUp && !this.m_isBeingEyedByRat && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
            {
                GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
            }
        }
        private bool m_hasBeenPickedUp;

        public PitLordsPact()
        {
        }
    }
}

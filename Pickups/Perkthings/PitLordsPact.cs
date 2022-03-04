using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;

namespace Planetside
{
    class PitLordsPactController : MonoBehaviour
    {
        public PitLordsPactController()
        {
            this.EnemySacrificeDamage = 40;
            this.EnemyAbovePitDamage = 10;
            this.EnemySacrificedBonus = 5;

            this.ItemSacrificablePerFloor = 1;

            this.TablesSarificeBonusMin = 2;
            this.TablesSarificeBonusMax = 6;
            this.TablesSarificeChance = 0.20f;

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
                OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                OtherTools.ApplyStat(player, PlayerStats.StatType.DamageToBosses, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                OtherTools.ApplyStat(player, PlayerStats.StatType.KnockbackMultiplier, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
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
                            if (ai.specRigidbody != null)
                            {
                                bool isOver = ai.IsOverPit;

                                if (isOver && ai != null && !ai.healthHaver.IsDead && !ai.IsFalling && ai.healthHaver != null)
                                {
                                    ai.healthHaver.ApplyDamage(EnemyAbovePitDamage * BraveTime.DeltaTime, Vector2.zero, "Pit Lords Wrath", CoreDamageTypes.None, DamageCategory.Environment, false, null, false);
                                }
                            }              
                        }
                    }
                }
                       
            }
        }
        private void OnNewFloorLoaded()
        {
            AmountOfItemsSacrificed = 0;
            AmountSelfSacrificedWithPitLordAmulet = 0;
        }
        public void IncrementStack()
        {
            this.EnemySacrificeDamage += 40;
            this.EnemyAbovePitDamage += 10;
            this.EnemySacrificedBonus += 3;

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

    class PitLordsPact : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {


            string name = "Pit Lords Pact Perk";
            string resourcePath = "Planetside/Resources/PerkThings/pitLordsPact.png";
            GameObject gameObject = new GameObject(name);
            PitLordsPact item = gameObject.AddComponent<PitLordsPact>();
          
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Literally just an all stats up.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            PitLordsPact.PitLordsPactID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.black;
            particles.ParticleSystemColor2 = new Color(50f, 0f, 0f);
            OutlineColor = new Color(0f, 0f, 0f);

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
        public static GenericLootTable PitLordsPactTable;
        private static GenericLootTable PitLordsPactTableNoHP;

        public static int PitLordsPactID;
        private static Color OutlineColor;

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
                GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, self.transform.position, Quaternion.identity, false);
                AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", self.gameObject);
                if (gameObject && gameObject.GetComponent<tk2dSprite>())
                {
                    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                    component.scale *= 2;
                    component.HeightOffGround = 5f;
                    component.UpdateZDepth();

                }
                ExplosionData boomboom = StaticExplosionDatas.genericSmallExplosion;
                boomboom.damageToPlayer = 0;
                boomboom.damageRadius = 12f;
                boomboom.damage = pact.EnemySacrificeDamage * 5;
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
                GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, self.transform.position, Quaternion.identity, false);
                AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", self.gameObject);
                if (gameObject && gameObject.GetComponent<tk2dSprite>())
                {
                    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                    component.scale *= 2;
                    component.HeightOffGround = 5f;
                    component.UpdateZDepth();

                }
                ExplosionData boomboom = StaticExplosionDatas.genericSmallExplosion;
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
                ExplosionData boomboom = StaticExplosionDatas.genericSmallExplosion;
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
            //ETGModConsole.Log("1");
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
                ExplosionData boomboom = StaticExplosionDatas.genericSmallExplosion;
                boomboom.damageToPlayer = 0;
                boomboom.damageRadius = 8f;
                boomboom.damage = pact.EnemySacrificeDamage * 5;
                boomboom.preventPlayerForce = true;
                boomboom.ignoreList.Add(self.specRigidbody);
                boomboom.playDefaultSFX = false;
                boomboom.doExplosionRing = false;
                Exploder.Explode(self.sprite.WorldCenter, boomboom, self.transform.PositionVector2());
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
                        ExplosionData boomboom = StaticExplosionDatas.genericSmallExplosion;
                        boomboom.damageToPlayer = 0;
                        boomboom.damageRadius = 10f;
                        boomboom.damage = pact.EnemySacrificeDamage;
                        boomboom.preventPlayerForce = true;
                        boomboom.ignoreList.Add(player.specRigidbody);
                        boomboom.playDefaultSFX = false;
                        boomboom.doExplosionRing = false;
                        Exploder.Explode(self.sprite.WorldCenter, boomboom, self.transform.PositionVector2());
                        if (UnityEngine.Random.value < self.healthHaver.GetMaxHealth() / 150)
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
                    ExplosionData boomboom = StaticExplosionDatas.genericLargeExplosion;
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
                        if (pact.AmountOfItemsSacrificed <= pact.ItemSacrificablePerFloor)
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
                                        moneyTogive = 12;
                                        break;
                                    case PickupObject.ItemQuality.C:
                                        moneyTogive = 24;
                                        break;
                                    case PickupObject.ItemQuality.B:
                                        moneyTogive = 36;
                                        break;
                                    case PickupObject.ItemQuality.A:
                                        moneyTogive = 48;
                                        break;
                                    case PickupObject.ItemQuality.S:
                                        moneyTogive = 60;
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
                                        moneyTogive = 12;
                                        break;
                                    case PickupObject.ItemQuality.C:
                                        moneyTogive = 24;
                                        break;
                                    case PickupObject.ItemQuality.B:
                                        moneyTogive = 36;
                                        break;
                                    case PickupObject.ItemQuality.A:
                                        moneyTogive = 48;
                                        break;
                                    case PickupObject.ItemQuality.S:
                                        moneyTogive = 60;
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
                    if(UnityEngine.Random.value > pact.TablesSarificeChance)
                    {
                        AkSoundEngine.PostEvent("Play_OBJ_coin_medium_01", self.gameObject);
                        LootEngine.SpawnCurrency((!player.sprite) ? player.specRigidbody.UnitCenter : player.sprite.WorldCenter, UnityEngine.Random.Range(pact.TablesSarificeBonusMax, pact.TablesSarificeBonusMax), false);
                    }
                }
            }
            yield break;
        }


        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            m_hasBeenPickedUp = true;
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null && player != null) { cont.DoBigBurst(player); }
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);

            PitLordsPactController pact = player.gameObject.GetOrAddComponent<PitLordsPactController>();
            pact.player = player;
            if (pact.hasBeenPickedup == true)
            { pact.IncrementStack(); }

            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = pact.hasBeenPickedup == true ? "Offerings Yield More." : "The Abyss demands all offerings.";
            OtherTools.Notify("Pit Lords Pact.", BlurbText, "Planetside/Resources/PerkThings/pitLordsPact", UINotificationController.NotificationColor.GOLD);

            UnityEngine.Object.Destroy(base.gameObject);
        }

        public float distortionMaxRadius = 30f;
        public float distortionDuration = 2f;
        public float distortionIntensity = 0.7f;
        public float distortionThickness = 0.1f;
        protected void Start()
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

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite)
            {
                return 1000f;
            }
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return 1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        private void Update()
        {
            if (!this.m_hasBeenPickedUp && !this.m_isBeingEyedByRat && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
            {
                GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
            }
        }

        public void Interact(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                RoomHandler.unassignedInteractableObjects.Remove(this);
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            this.Pickup(interactor);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private bool m_hasBeenPickedUp;

        public PitLordsPact()
        {
        }
    }
}

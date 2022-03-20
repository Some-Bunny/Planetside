using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

using UnityEngine.Serialization;
using MonoMod.Utils;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;

using System.IO;
using Planetside;
using FullInspector.Internal;
using UnityEngine.SceneManagement;


namespace Planetside
{
    public class HeresyHammer : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Heresy Breaker";
            string resourceName = "Planetside/Resources/heresyhammer.png";
            GameObject obj = new GameObject(itemName);
            HeresyHammer activeitem = obj.AddComponent<HeresyHammer>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "BLASPHEMER";
            string longDesc = "A large hammer, originally used by protesters off-world to destroy monuments of the corrupt in power. It found new life in here after many of said protestors were shipped to the Gungeon.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 1f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.B;
            activeitem.SetupUnlockOnCustomFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK, true);
            ItemBuilder.AddPassiveStatModifier(activeitem, PlayerStats.StatType.Curse, 0.5f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(activeitem, PlayerStats.StatType.AdditionalItemCapacity, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(activeitem, PlayerStats.StatType.RateOfFire, 1.20f, StatModifier.ModifyMethod.MULTIPLICATIVE);


            SpriteBuilder.AddSpriteToCollection(ChallnegeIcon, SpriteBuilder.ammonomiconCollection);
            SpriteBuilder.AddSpriteToCollection(CleanseIcon, SpriteBuilder.ammonomiconCollection);
            SpriteBuilder.AddSpriteToCollection(ShelltanIcon, SpriteBuilder.ammonomiconCollection);

            activeitem.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);


            HeresyHammer.HeresyBreakerID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

        }
        public static int HeresyBreakerID;

        public static string ChallnegeIcon = "Planetside/Resources/ShrineIcons/HeresyIcons/shrineChallengeIcon";
        public static string CleanseIcon = "Planetside/Resources/ShrineIcons/HeresyIcons/cleanseshrineanger";
        public static string ShelltanIcon = "Planetside/Resources/ShrineIcons/HeresyIcons/ShelltanIcon";

        public override void Pickup(PlayerController player)
        {
            if (base.m_pickedUpThisRun != true)
            {
                    bool Retrash = ETGMod.Databases.Items["Glass Smelter"] != null;
                if (Retrash)
                {
                    GlassItemsID.Add(ETGMod.Databases.Items["Glass Smelter"].PickupObjectId);
                }
                bool Nevernamed = ETGMod.Databases.Items["Glass Ammolet"] != null && ETGMod.Databases.Items["Glass Chamber"] != null && ETGMod.Databases.Items["Glass Rounds"] != null && ETGMod.Databases.Items["Glass God"] != null;
                if (Nevernamed)
                {
                    GlassItemsID.Add(ETGMod.Databases.Items["Glass Ammolet"].PickupObjectId);
                    GlassItemsID.Add(ETGMod.Databases.Items["Glass Chamber"].PickupObjectId);
                    GlassItemsID.Add(ETGMod.Databases.Items["Glass Rounds"].PickupObjectId);
                    GlassItemsID.Add(ETGMod.Databases.Items["Glass God"].PickupObjectId);
                }

            }
            base.Pickup(player);
        }

            
        List<string> Names = new List<string>();
        public override bool CanBeUsed(PlayerController user)
        {
            try
            {
                IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.sprite.WorldCenter, 2.4f, user);
                bool flag2 = nearestInteractable != null && nearestInteractable is SimpleShrine;
                if (flag2)
                {
                    SimpleShrine simp = nearestInteractable as SimpleShrine;
                    Func<PlayerController, GameObject, bool> aaa = simp.CanUse;
                    if (aaa.Invoke(user, simp.gameObject) == true)
                    {
                        return true;
                    }

                }
                else if (nearestInteractable != null && nearestInteractable is BeholsterShrineController)
                {
                    BeholsterShrineController hjolster = nearestInteractable as BeholsterShrineController;
                    Type type = typeof(BeholsterShrineController); FieldInfo _property = type.GetField("m_useCount", BindingFlags.NonPublic | BindingFlags.Instance); _property.GetValue(hjolster);
                    int uses = (int)_property.GetValue(hjolster);
                    if (uses < 1)
                    {
                        return true;
                    }
                }
                else if (nearestInteractable != null && nearestInteractable is ChallengeShrineController)
                {
                    ChallengeShrineController challenge = nearestInteractable as ChallengeShrineController;
                    Type type = typeof(ChallengeShrineController); FieldInfo _property = type.GetField("m_useCount", BindingFlags.NonPublic | BindingFlags.Instance); _property.GetValue(challenge);
                    int uses = (int)_property.GetValue(challenge);
                    if (uses < 1)
                    {
                        return true;
                    }
                }
                else
                {
                    List<AdvancedShrineController> allDebris = StaticReferenceManager.AllAdvancedShrineControllers;
                    bool flag3 = allDebris != null;
                    if (flag3)
                    {
                        for (int i = 0; i < allDebris.Count; i++)
                        {
                            AdvancedShrineController debrisObject = allDebris[i];
                            bool flag4 = debrisObject != null && debrisObject.isActiveAndEnabled;
                            if (flag4)
                            {
                                AdvancedShrineController DN = debrisObject as AdvancedShrineController;
                                Type type = typeof(AdvancedShrineController); FieldInfo _property = type.GetField("m_useCount", BindingFlags.NonPublic | BindingFlags.Instance); _property.GetValue(DN);
                                int uses = (int)_property.GetValue(DN);
                                if (uses < 1 && !debrisObject.CanBeReused)
                                {
                                    float sqrMagnitude = (user.CenterPosition - debrisObject.transform.position.XY()).sqrMagnitude;
                                    bool flag5 = sqrMagnitude <= 25f;
                                    if (flag5)
                                    {
                                        float num = Mathf.Sqrt(sqrMagnitude);
                                        bool flag7 = num < 2.4f;
                                        if (flag7)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else if (debrisObject.CanBeReused)
                                {
                                    float sqrMagnitude = (user.CenterPosition - debrisObject.transform.position.XY()).sqrMagnitude;
                                    bool flag5 = sqrMagnitude <= 25f;
                                    if (flag5)
                                    {
                                        float num = Mathf.Sqrt(sqrMagnitude);
                                        bool flag7 = num < 2.4f;
                                        if (flag7)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                //ETGModConsole.Log("fuck.");
                return false;
            }
        }

        protected override void DoEffect(PlayerController user)
        {
            bool PurpleParticles = false;
            AkSoundEngine.PostEvent("Play_RockBreaking", base.gameObject);            
            IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.sprite.WorldCenter, 2.5f, user);
            bool flag2 = nearestInteractable != null && nearestInteractable is SimpleShrine;
            if (flag2)
            {
                SimpleShrine death = nearestInteractable as SimpleShrine;
                if (death.name == "psog:nullshrine(Clone)")
                {
                    OtherTools.ApplyStat(user, PlayerStats.StatType.Curse, 0.5f, StatModifier.ModifyMethod.ADDITIVE);
                    if (UnityEngine.Random.value <= 0.4f)
                    {
                        LootEngine.SpawnCurrency(death.sprite.WorldCenter, 1);
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(NullPickupInteractable.NollahID).gameObject, death.sprite.WorldCenter, new Vector2(2f, 0f), 2.2f, false, true, false);
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(NullPickupInteractable.NollahID).gameObject, death.sprite.WorldCenter, new Vector2(-2f, 0f), 2.2f, false, true, false);
                    }
                    else
                    {
                        base.StartCoroutine(DoNormalBoringAssReward(death.specRigidbody.UnitCenter));
                    }
                    base.StartCoroutine(ShrineParticlesOnDestory(death.sprite.WorldBottomLeft, death.sprite.WorldTopRight, PurpleParticles));
                }
                if (death.name == "psog:shrineofdarkness(Clone)" | death.name == "psog:shrineofcurses(Clone)" | death.name == "psog:shrineofpetrification(Clone)" | death.name == "psog:shrineofsomething(Clone)")
                {
                    PurpleParticles = true;
                    if (UnityEngine.Random.value <= 0.08f)
                    {
                        int id = BraveUtility.RandomElement<int>(this.CursedID);
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, death.sprite.WorldCenter, new Vector2(0f, 0f), 0f, false, true, false);
                    }
                    else
                    {
                        base.StartCoroutine(DoNormalBoringAssReward(death.specRigidbody.UnitCenter));
                    }
                    base.StartCoroutine(ShrineParticlesOnDestory(death.sprite.WorldBottomLeft, death.sprite.WorldTopRight, PurpleParticles));
                }
                if (death.name == "psog:shrineofpurity(Clone)")
                {
                    if (UnityEngine.Random.value <= 0.125f)
                    {
                        OtherTools.ApplyStat(user, PlayerStats.StatType.DamageToBosses, 0.85f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                        OtherTools.ApplyStat(user, PlayerStats.StatType.RateOfFire, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                        for (int i = 0; i < 4; i++)
                        {
                            SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(538) as SilverBulletsPassiveItem).SynergyPowerVFX, death.sprite.WorldCenter.ToVector3ZisY(0f) + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 100), Quaternion.identity).GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.sprite.WorldCenter.ToVector3ZisY(0f), tk2dBaseSprite.Anchor.MiddleCenter);
                        }
                    }
                    else
                    {
                        base.StartCoroutine(DoNormalBoringAssReward(death.specRigidbody.UnitCenter));
                    }
                    base.StartCoroutine(ShrineParticlesOnDestoryBlue(death.sprite.WorldBottomLeft, death.sprite.WorldTopRight));
                }
                if (death.name == "psog:brokenchambershrine(Clone)")
                {
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(BrokenChamber.BrokenChamberID).gameObject, death.sprite.WorldCenter, new Vector2(0f, 0f), 2.2f, false, true, false);
                    base.StartCoroutine(ShrineParticlesOnDestory(death.sprite.WorldBottomLeft, death.sprite.WorldTopRight, PurpleParticles));
                }
                if (death.name == "psog:endofeverythingshrine(Clone)")
                {
                    base.StartCoroutine(ShrineParticlesOnDestory(death.sprite.WorldBottomLeft, death.sprite.WorldTopRight, PurpleParticles));
                    Gun gun = PickupObjectDatabase.GetByEncounterName(InitialiseGTEE.GunIDForEOE) as Gun;
                    int StoredGunID = gun.PickupObjectId;
                    int Item1ID = Game.Items[InitialiseGTEE.HOneToFireIt].PickupObjectId;
                    int Item2ID = Game.Items[InitialiseGTEE.HOneToPrimeIt].PickupObjectId;
                    int Item3ID = Game.Items[InitialiseGTEE.HOneToHoldIt].PickupObjectId;
                    List<int> ID = new List<int>
                    {
                        StoredGunID,
                        Item1ID,
                        Item2ID,
                        Item3ID
                    };
                    int Ident = BraveUtility.RandomElement<int>(ID);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(Ident).gameObject, death.sprite.WorldCenter, new Vector2(2f, 0f), 2.2f, false, true, false);
                }
                if (death.name == "psog:gunorbitingshrine(Clone)")
                {
                    if (UnityEngine.Random.value <= 0.4f)
                    {
                        OtherTools.ApplyStat(user, PlayerStats.StatType.AmmoCapacityMultiplier, 1.3f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                        OtherTools.ApplyStat(user, PlayerStats.StatType.AdditionalClipCapacityMultiplier, 2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(78).gameObject, death.sprite.WorldCenter, new Vector2(2f, 0f), 2.2f, false, true, false);
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(78).gameObject, death.sprite.WorldCenter, new Vector2(-2f, 0f), 2.2f, false, true, false);
                    }
                    else
                    {
                        base.StartCoroutine(DoNormalBoringAssReward(death.specRigidbody.UnitCenter));
                    }
                    base.StartCoroutine(ShrineParticlesOnDestory(death.sprite.WorldBottomLeft, death.sprite.WorldTopRight, PurpleParticles));
                }
                if (death.name == "psog:holychambershrine(Clone)")
                {
                    AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HAS_COMMITED_HERESY, true);
                    OtherTools.Notify("HERETIC!", "How Dare You Do This!", "Planetside/Resources/ShrineIcons/HeresyIcons/ShelltanIcon");

                    OtherTools.ApplyStat(user, PlayerStats.StatType.Damage, 2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    OtherTools.ApplyStat(user, PlayerStats.StatType.Health,2f, StatModifier.ModifyMethod.ADDITIVE);
                    AkSoundEngine.PostEvent("Play_BOSS_lichB_intro_01", base.gameObject);
                    GameObject gameObject = new GameObject();
                    gameObject.transform.position = death.transform.position;
                    BulletScriptSource source = gameObject.GetOrAddComponent<BulletScriptSource>();
                    gameObject.AddComponent<BulletSourceKiller>();
                    var bulletScriptSelected = new CustomBulletScriptSelector(typeof(UltraAngryGodsScript));
                    AIActor aIActor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
                    AIBulletBank bulletBank = aIActor.GetComponent<AIBulletBank>();
                    bulletBank.CollidesWithEnemies = false;
                    source.BulletManager = bulletBank;
                    source.BulletScript = bulletScriptSelected;
                    source.Initialize();//to fire the script once
                    user.gameObject.AddComponent<HERETIC>();
                    user.gameObject.AddComponent<BrokenChamberComponent>();
                    base.StartCoroutine(ShrineParticlesOnDestoryBlue(death.sprite.WorldBottomLeft, death.sprite.WorldTopRight));
                }

                Destroy(death.gameObject);
            }
            else if (nearestInteractable != null && nearestInteractable is BeholsterShrineController)
            {
                BeholsterShrineController death = nearestInteractable as BeholsterShrineController;
                if (UnityEngine.Random.value <= 0.3f && ((GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON || GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON || GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON) && GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.NUMBER_ATTEMPTS) > 10f && GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_BEHOLSTER)))
                {
                    GameManager.Instance.InjectedFlowPath = "Core Game Flows/Secret_DoubleBeholster_Flow";
                    Pixelator.Instance.FadeToBlack(0.5f, false, 0f);
                    GameManager.Instance.DelayedLoadNextLevel(0.5f);

                }
                else
                {
                    base.StartCoroutine(DoNormalBoringAssReward(death.specRigidbody.UnitCenter));
                }
                Destroy(death.gameObject);
                base.StartCoroutine(ShrineParticlesOnDestory(death.sprite.WorldBottomLeft, death.sprite.WorldTopRight, PurpleParticles));
            }
            else if (nearestInteractable != null && nearestInteractable is ChallengeShrineController)
            {
                ChallengeShrineController death = nearestInteractable as ChallengeShrineController;
                if (UnityEngine.Random.value <= 0.5f)
                {
                    OtherTools.Notify("The Spirits", "Hold Back", "Planetside/Resources/ShrineIcons/HeresyIcons/shrineChallengeIcon");
                    OtherTools.ApplyStat(user, PlayerStats.StatType.Damage, 1.4f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    OtherTools.ApplyStat(user, PlayerStats.StatType.Coolness, 3f, StatModifier.ModifyMethod.ADDITIVE);
                    user.stats.AddFloorMagnificence(1);
                }
                else
                {
                    base.StartCoroutine(DoNormalBoringAssReward(death.specRigidbody.UnitCenter));
                }
                Destroy(death.gameObject);
                death.GetRidOfMinimapIcon();

                base.StartCoroutine(ShrineParticlesOnDestory(death.sprite.WorldBottomLeft, death.sprite.WorldTopRight, PurpleParticles));
            }
            else
            {
                List<AdvancedShrineController> allDebris = StaticReferenceManager.AllAdvancedShrineControllers;
                bool flag3 = allDebris != null;
                if (flag3)
                {
                    for (int i = 0; i < allDebris.Count; i++)
                    {
                        AdvancedShrineController debrisObject = allDebris[i];
                        bool flag4 = debrisObject && debrisObject.isActiveAndEnabled;
                        if (flag4)
                        {
                            float sqrMagnitude = (user.CenterPosition - debrisObject.transform.position.XY()).sqrMagnitude;
                            bool flag5 = sqrMagnitude <= 25f;
                            if (flag5)
                            {
                                float num = Mathf.Sqrt(sqrMagnitude);
                                bool flag7 = num < 2.5f;
                                if (flag7)
                                {
                                    Vector2 offset = new Vector2(0, 0);
                                    if (debrisObject.IsLegendaryHeroShrine)
                                    {
                                        OtherTools.ApplyStat(user, PlayerStats.StatType.Curse, 4f, StatModifier.ModifyMethod.ADDITIVE);
                                        PurpleParticles = true;
                                        GameObject superReaper = PrefabDatabase.Instance.SuperReaper;
                                        SpeculativeRigidbody component = superReaper.GetComponent<SpeculativeRigidbody>();
                                        if (component)
                                        {
            
                                            PixelCollider primaryPixelCollider = component.PrimaryPixelCollider;
                                            Vector2 a = PhysicsEngine.PixelToUnit(new IntVector2(primaryPixelCollider.ManualOffsetX, primaryPixelCollider.ManualOffsetY));
                                            Vector2 vector2 = PhysicsEngine.PixelToUnit(new IntVector2(primaryPixelCollider.ManualWidth, primaryPixelCollider.ManualHeight));
                                            Vector2 vector3 = new Vector2((float)Mathf.CeilToInt(vector2.x), (float)Mathf.CeilToInt(vector2.y));
                                            Vector2 b = new Vector2((vector3.x - vector2.x) / 0f, 0f).Quantize(0.0625f);

                                        }
                                        
                                        GameObject reaper =UnityEngine.Object.Instantiate<GameObject>(superReaper, debrisObject.gameObject.transform.position - new Vector3(3.25f, 0), Quaternion.identity);
                                        SuperReaperController scythe = reaper.GetComponent<SuperReaperController>();
                                        scythe.ShootTimer *= 0.75f;
                                        DebrisObject item = GameManager.Instance.RewardManager.SpawnTotallyRandomItem(debrisObject.gameObject.transform.position + new Vector3(1.75f, 1), ItemQuality.B, ItemQuality.S);
                                        bool flag = item;
                                        PickupObject result;
                                        if (flag)
                                        {
                                            Vector2 v = (!item.sprite) ? (item.transform.position.XY()) : item.sprite.WorldCenter;
                                            GameObject gameObject = SpawnManager.SpawnVFX((GameObject)BraveResources.Load("Global VFX/VFX_BlackPhantomDeath", ".prefab"), v, Quaternion.identity, false);
                                            bool eee = gameObject && gameObject.GetComponent<tk2dSprite>();
                                            if (eee)
                                            {
                                                tk2dSprite aa = gameObject.GetComponent<tk2dSprite>();
                                                aa.HeightOffGround = 5f;
                                                aa.UpdateZDepth();
                                            }
                                            result = debrisObject.GetComponentInChildren<PickupObject>();
                                        }
                                        base.StartCoroutine(ShrineParticlesOnDestory(debrisObject.sprite.WorldBottomLeft, debrisObject.sprite.WorldTopRight, PurpleParticles));
                                    }
                                    //==============================================================================================================================================================================================
                                    if (debrisObject.IsBlankShrine | debrisObject.IsBloodShrine | debrisObject.IsGlassShrine | debrisObject.IsJunkShrine | debrisObject.IsRNGShrine | debrisObject.IsHealthArmorSwapShrine)
                                    {
                                        //==============================================================================================================================================================================================
                                        if (debrisObject.IsBlankShrine && UnityEngine.Random.value <= 0.4f)
                                        {
                                            GameObject gameObjectBlankVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX");
                                            GameObject gameObjectBlank = new GameObject("silencer");
                                            SilencerInstance silencerInstance = gameObjectBlank.AddComponent<SilencerInstance>();
                                            silencerInstance.TriggerSilencer(debrisObject.specRigidbody.UnitCenter, 50f, 25f, gameObjectBlankVFX, 0.25f, 0.2f, 50f, 10f, 140f, 15f, 0.5f, user, true, false);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(224).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0.5f, 0.5f), 2.2f, false, true, false);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(224).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0.5f, -0.5f), 2.2f, false, true, false);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(224).gameObject, debrisObject.sprite.WorldCenter, new Vector2(-0.5f, 0.5f), 2.2f, false, true, false);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(224).gameObject, debrisObject.sprite.WorldCenter, new Vector2(-0.5f, -0.5f), 2.2f, false, true, false);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(224).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0f, 0.5f), 2.2f, false, true, false);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(224).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0f, -0.5f), 2.2f, false, true, false);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(224).gameObject, debrisObject.sprite.WorldCenter, new Vector2(-0.5f, 0f), 2.2f, false, true, false);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(224).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0.5f, 0f), 2.2f, false, true, false);
                                        }
                                        else if(debrisObject.IsBlankShrine)
                                        {
                                            base.StartCoroutine(DoNormalBoringAssReward(debrisObject.specRigidbody.UnitCenter));

                                        }
                                        //==============================================================================================================================================================================================
                                        if (debrisObject.IsBloodShrine && UnityEngine.Random.value <= 0.4f)
                                        {
                                            offset = new Vector2(-2, -2);
                                            int id = BraveUtility.RandomElement<int>(this.BloddItemIDS);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, debrisObject.sprite.WorldCenter - new Vector2(2f, 2f), new Vector2(0f, 0f), 0f, false, true, false);
                                            this.teleporter = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>();
                                            UnityEngine.Object.Instantiate<GameObject>(this.teleporter.TelefragVFXPrefab, debrisObject.sprite.WorldCenter - new Vector2(2f, 2f), Quaternion.identity);
                                        }
                                        else if (debrisObject.IsBloodShrine)
                                        {
                                            offset = new Vector2(-2, -2);
                                            base.StartCoroutine(DoNormalBoringAssReward(debrisObject.specRigidbody.UnitCenter));
                                        }
                                        //==============================================================================================================================================================================================
                                        if (debrisObject.IsGlassShrine)
                                        {
                                            AkSoundEngine.PostEvent("Play_OBJ_crystal_shatter_01", base.gameObject);
                                            if (UnityEngine.Random.value <= 0.40)
                                            {
                                                int id = BraveUtility.RandomElement<int>(this.GlassItemsID);

                                                LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0f, 0f), 2.2f, false, true, false);
                                            }
                                            else
                                            {
                                                base.StartCoroutine(DoNormalBoringAssReward(debrisObject.specRigidbody.UnitCenter));
                                            }
                                        }
                                        //==============================================================================================================================================================================================
                                        if (debrisObject.IsJunkShrine && UnityEngine.Random.value <= 0.4f && debrisObject.name == "Shrine_Junk")
                                        {
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(127).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0.5f, 0.5f), 2.2f, false, true, false);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(127).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0.5f, -0.5f), 2.2f, false, true, false);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(127).gameObject, debrisObject.sprite.WorldCenter, new Vector2(-0.5f, 0.5f), 2.2f, false, true, false);
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(127).gameObject, debrisObject.sprite.WorldCenter, new Vector2(-0.5f, -0.5f), 2.2f, false, true, false);
                                        }
                                        else if (debrisObject.IsJunkShrine && debrisObject.name == "Shrine_Junk")
                                        {
                                            base.StartCoroutine(DoNormalBoringAssReward(debrisObject.specRigidbody.UnitCenter));
                                        }
                                        //==============================================================================================================================================================================================
                                        if (debrisObject.IsRNGShrine && UnityEngine.Random.value <= 0.4f && debrisObject.name == "Shrine_Dice")
                                        {
                                            for (int e = 0; e < 6; e++)
                                            {
                                                base.StartCoroutine(DoNormalBoringAssReward(debrisObject.specRigidbody.UnitCenter));
                                            }
                                        }
                                        else if (debrisObject.IsRNGShrine && debrisObject.name == "Shrine_Dice")
                                        {
                                            LootEngine.SpawnCurrency(debrisObject.sprite.WorldCenter, UnityEngine.Random.Range(1, 70));
                                        }
                                        //==============================================================================================================================================================================================
                                        if (debrisObject.IsHealthArmorSwapShrine)// && UnityEngine.Random.value <= 0.99f)
                                        {
                                            base.StartCoroutine(DoNormalBoringAssReward(debrisObject.specRigidbody.UnitCenter));
                                            ETGModConsole.Log("idk what shrine this is");
                                        }
                                        //==============================================================================================================================================================================================
                                        base.StartCoroutine(ShrineParticlesOnDestory(debrisObject.sprite.WorldBottomLeft + offset, debrisObject.sprite.WorldTopRight+ offset, PurpleParticles));

                                    }
                                    //==============================================================================================================================================================================================
                                    if (debrisObject.IsCleanseShrine)
                                    {
                                        OtherTools.Notify("The cleansing waters", "Spill On Tainted Ground.", "Planetside/Resources/ShrineIcons/HeresyIcons/cleanseshrineanger");
                                        user.stats.AddFloorMagnificence(-3);
                                        if (user.stats.GetBaseStatValue(PlayerStats.StatType.Curse) == 0)
                                        {
                                            OtherTools.ApplyStat(user, PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
                                        }
                                        else
                                        {
                                            OtherTools.ApplyStat(user, PlayerStats.StatType.Curse, 2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                                        }
                                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.WaterGoop).TimedAddGoopCircle(debrisObject.sprite.WorldCenter, 4, 1.2f, false);
                                        base.StartCoroutine(ShrineParticlesOnDestoryBlue(debrisObject.sprite.WorldBottomLeft, debrisObject.sprite.WorldTopRight));
                                    }
                                    //==============================================================================================================================================================================================
                                    if (debrisObject.name == "Shrine_YV")
                                    {
                                        if (UnityEngine.Random.value <= 0.4f)
                                        {
                                            OtherTools.ApplyStat(user, PlayerStats.StatType.MoneyMultiplierFromEnemies, 1.2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                                            user.gameObject.AddComponent<GoldPlatedGun>();
                                        }
                                        else
                                        {
                                            base.StartCoroutine(DoNormalBoringAssReward(debrisObject.specRigidbody.UnitCenter));
                                        }
                                        base.StartCoroutine(ShrineParticlesOnDestory(debrisObject.sprite.WorldBottomLeft, debrisObject.sprite.WorldTopRight, PurpleParticles));

                                    }
                                    //==============================================================================================================================================================================================
                                    if (debrisObject.name == "Shrine_FallenAngel")
                                    {
                                        if (UnityEngine.Random.value <= 0.4f)
                                        {
                                            AkSoundEngine.PostEvent("Play_BOSS_lichB_grab_01", base.gameObject);
                                            GameObject hand = UnityEngine.Object.Instantiate<GameObject>(PlanetsideModule.hellDrag.HellDragVFX);
                                            tk2dBaseSprite component1 = hand.GetComponent<tk2dBaseSprite>();
                                            component1.usesOverrideMaterial = true;
                                            component1.PlaceAtLocalPositionByAnchor(debrisObject.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.LowerCenter);
                                            component1.renderer.material.shader = ShaderCache.Acquire("Brave/Effects/StencilMasked");
                                            OtherTools.ApplyStat(user, PlayerStats.StatType.AmmoCapacityMultiplier, 1.25f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                                            OtherTools.ApplyStat(user, PlayerStats.StatType.GlobalPriceMultiplier, 0.90f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                                            user.ReceivesTouchDamage = false;
                                        }
                                        else
                                        {
                                            base.StartCoroutine(DoNormalBoringAssReward(debrisObject.specRigidbody.UnitCenter));
                                        }
                                        base.StartCoroutine(ShrineParticlesOnDestory(debrisObject.sprite.WorldBottomLeft, debrisObject.sprite.WorldTopRight, PurpleParticles));
                                    }
                                    //==============================================================================================================================================================================================
                                    if (debrisObject.name == "Shrine_Ammo")
                                    {
                                        offset = new Vector2(1, 1);
                                        if (SaveAPIManager.GetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED) == true)
                                        {
                                            OtherTools.ApplyStat(user, PlayerStats.StatType.Accuracy, 1.2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                                            OtherTools.ApplyStat(user, PlayerStats.StatType.Damage, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                                            OtherTools.ApplyStat(user, PlayerStats.StatType.AmmoCapacityMultiplier, 0.7f, StatModifier.ModifyMethod.MULTIPLICATIVE);

                                            OtherTools.Notify("Shelltan Forgives You.", "His Favor Was Earned.", "Planetside/Resources/ShrineIcons/HeresyIcons/ShelltanIcon");
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(ShelltansBlessing.ShelltainsBlessingID).gameObject, debrisObject.sprite.WorldCenter - new Vector2(0.125f, 1), new Vector2(0f, 0f), 2.2f, false, true, false);
                                        }
                                        else
                                        {
                                            OtherTools.Notify("Shelltan Waits", "In Dismay.", "Planetside/Resources/ShrineIcons/HeresyIcons/ShelltanIcon");
                                            base.StartCoroutine(ShellSucc(debrisObject.transform.position));
                                        }
                                        base.StartCoroutine(ShrineParticlesOnDestory(debrisObject.sprite.WorldBottomLeft-offset, debrisObject.sprite.WorldTopRight- offset, PurpleParticles));

                                    }
                                    //==============================================================================================================================================================================================
                                    if (debrisObject.name == "Shrine_Health")
                                    {
                                        if (UnityEngine.Random.value <= 0.4f)
                                        {
                                            if (UnityEngine.Random.value <= 0.99f)
                                            {
                                                PlayableCharacters characterIdentity = user.characterIdentity;
                                                bool flag = characterIdentity != PlayableCharacters.Robot;
                                                if (flag)
                                                {
                                                    if (UnityEngine.Random.value <= 0.30f)
                                                    {
                                                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(164).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0f, 0f), 2.2f, false, true, false);
                                                    }
                                                    else
                                                    {
                                                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(85).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0.5f, 0.5f), 2.2f, false, true, false);
                                                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(85).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0.5f, -0.5f), 2.2f, false, true, false);
                                                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(85).gameObject, debrisObject.sprite.WorldCenter, new Vector2(-0.5f, 0.5f), 2.2f, false, true, false);
                                                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(85).gameObject, debrisObject.sprite.WorldCenter, new Vector2(-0.5f, -0.5f), 2.2f, false, true, false);
                                                    }
                                                }
                                                else
                                                {
                                                    bool HHH = characterIdentity == PlayableCharacters.Robot;
                                                    if (HHH)
                                                    {
                                                        if (UnityEngine.Random.value <= 0.30f)
                                                        {
                                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(450).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0f, 0f), 2.2f, false, true, false);
                                                        }
                                                        else
                                                        {
                                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(120).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0.5f, 0.5f), 2.2f, false, true, false);
                                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(120).gameObject, debrisObject.sprite.WorldCenter, new Vector2(0.5f, -0.5f), 2.2f, false, true, false);
                                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(120).gameObject, debrisObject.sprite.WorldCenter, new Vector2(-0.5f, 0.5f), 2.2f, false, true, false);
                                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(120).gameObject, debrisObject.sprite.WorldCenter, new Vector2(-0.5f, -0.5f), 2.2f, false, true, false);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            base.StartCoroutine(DoNormalBoringAssReward(debrisObject.specRigidbody.UnitCenter));
                                        }
                                        base.StartCoroutine(ShrineParticlesOnDestory(debrisObject.sprite.WorldBottomLeft, debrisObject.sprite.WorldTopRight, PurpleParticles));
                                       
                                    }
                                    //==============================================================================================================================================================================================
                                    if (debrisObject.name == "Shrine_Companion")
                                    {
                                        if (UnityEngine.Random.value <= 0.4f)
                                        {
                                            OtherTools.ApplyStat(user, PlayerStats.StatType.MovementSpeed, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                                            PlayerOrbitalItem.CreateOrbital(user, RandomPiecesOfStuffToInitialise.SoulSynergyGuon, false);
                                            PlayerOrbitalItem.CreateOrbital(user, RandomPiecesOfStuffToInitialise.SoulSynergyGuon, false);
                                            PlayerOrbitalItem.CreateOrbital(user, RandomPiecesOfStuffToInitialise.SoulSynergyGuon, false);
                                            ImprovedAfterImage yes = user.gameObject.AddComponent<ImprovedAfterImage>();
                                            yes.spawnShadows = true;
                                            yes.shadowLifetime = 0.5f;
                                            yes.shadowTimeDelay = 0.001f;
                                            yes.dashColor = Color.clear;
                                        }
                                        else
                                        {
                                            base.StartCoroutine(DoNormalBoringAssReward(debrisObject.specRigidbody.UnitCenter));
                                        }
                                        base.StartCoroutine(ShrineParticlesOnDestory(debrisObject.sprite.WorldBottomLeft, debrisObject.sprite.WorldTopRight, PurpleParticles));
                                    }
                                    //==============================================================================================================================================================================================
                                    //ETGModConsole.Log(debrisObject.name.ToString());

                                    debrisObject.GetRidOfMinimapIcon();
                                    Destroy(debrisObject.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }

        List<int> BloddItemIDS = new List<int>
        {
            167,
            259,
            285,
            313,
            436,
            524,
            595,
            333,
            TableTechDevour.TableTechDevourID,
            BloodIdol.BloodIdolID
        };
        List<int> GlassItemsID = new List<int>
        {
            540,
            290
        };

        List<int> CursedID = new List<int>
        {
            WitherLance.WitherLanceID,
            BloodIdol.BloodIdolID,
            403,
            571,
            213,
            158,
            346,
            464,
            33,
            347
        };


        private TeleporterPrototypeItem teleporter;
        private IEnumerator ShrineParticlesOnDestoryBlue(Vector2 BottmLeft, Vector2 TopRight)
        {
            Vector2 lol = TopRight.RoundToInt();

            float yes = lol.x.RoundToNearest(1);
            for (int j = 0; j < (300 - yes) / 8; j++)
            {
                Vector2 pos = BraveUtility.RandomVector2(BottmLeft, TopRight, new Vector2(0.025f, 0.025f));
                LootEngine.DoDefaultSynergyPoof(pos, false);
                yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.01f));
            }
            yield break;
        }
        private IEnumerator ShrineParticlesOnDestory(Vector2 BottmLeft,Vector2 TopRight, bool PurpleRain)
        {
            Vector2 lol = TopRight.RoundToInt();
            float yes = lol.x.RoundToNearest(1);
            for (int j = 0; j < (300-yes)/8; j++)
            {
                Vector2 pos = BraveUtility.RandomVector2(BottmLeft, TopRight, new Vector2(0.025f, 0.025f));
                if (PurpleRain == true) {LootEngine.DoDefaultPurplePoof(pos, false);}
                else {LootEngine.DoDefaultItemPoof(pos, false, true);}
                yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.01f));
            }
            yield break;
        }
        private IEnumerator DoNormalBoringAssReward(Vector2 Center)
        {
            this.random = UnityEngine.Random.Range(0.0f, 1.0f);
            if (random < 0.45 && random > 0)
            {
                LootEngine.SpawnCurrency(Center, UnityEngine.Random.Range(32, 56));
            }
            else if (random > 0.45 && random < 0.75)
            {
                float itemsToSpawn = UnityEngine.Random.Range(6, 8);
                float X = 3 / itemsToSpawn;
                for (int i = 0; i < itemsToSpawn; i++)
                {
                    int id = BraveUtility.RandomElement<int>(LeSackPickup.HPPool);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject,Center, new Vector2(3-(X*i) ,0), 1.2f, false, true, false);
                }
            }
            if (random > 0.75 && random < 1)
            {
                GameManager.Instance.RewardManager.SpawnTotallyRandomItem(Center, ItemQuality.B, ItemQuality.A);
            }
            yield break;
        }
        private float random;

        private IEnumerator ShellSucc(Vector2 Center)
        {
            AkSoundEngine.PostEvent("Play_ENM_shells_gather_01", base.gameObject);
            float ela = 0f;
            float dura = 1f;
            while (ela < dura)
            {
                ela += BraveTime.DeltaTime;
                
                for (int i = 0; i < StaticReferenceManager.AllDebris.Count; i++)
                {
                    this.AdjustDebrisVelocity(StaticReferenceManager.AllDebris[i], Center);
                }
                
                yield return null;
            }
            
            LootEngine.SpawnItem(PickupObjectDatabase.GetById(78).gameObject, Center + new Vector2(1,1), new Vector2(0f, 0f), 2.2f, false, true, false);
            LootEngine.DoDefaultItemPoof(Center, false, false);

            yield break;
        }
        private bool AdjustDebrisVelocity(DebrisObject debris, Vector2 SuccSpace)
        {
            if (debris.IsPickupObject)
            {
                return false;
            }
            if (debris.GetComponent<BlackHoleDoer>() != null)
            {
                return false;
            }
            Vector2 a = debris.sprite.WorldCenter - SuccSpace;
            float num = Vector2.SqrMagnitude(a);
            if (num > this.m_radiusSquared)
            {
                return false;
            }
            float num2 = Mathf.Sqrt(num);
            if (num2 < this.destroyRadius)
            {
                UnityEngine.Object.Destroy(debris.gameObject);
                return true;
            }
            Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(debris.sprite.WorldCenter, num2, this.gravityForce, SuccSpace);
            float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
            if (debris.HasBeenTriggered)
            {
                debris.ApplyVelocity(frameAccelerationForRigidbody * d);
            }
            else if (num2 < this.radius / 2f)
            {
                debris.Trigger(frameAccelerationForRigidbody * d, 0.5f, 1f);
            }
            return true;
        }
        private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, float currentDistance, float g, Vector2 ctr)
        {
            float num = Mathf.Clamp01(1f - currentDistance / this.radius);
            float d = g * num * num;
            return (ctr - unitCenter).normalized * d;
        }
        public float radius = 100;

        public float gravityForce = 50;
        private float m_radiusSquared = 5 * 5;
        public float destroyRadius = 0.2f;
    }
    public class GoldPlatedGun : BraveBehaviour
    {
        public void Start()
        {
            this.Microwave = base.GetComponent<RoomHandler>();
            this.playeroue = base.GetComponent<PlayerController>();
            {
                PlayerController player = GameManager.Instance.PrimaryPlayer;
                player.GunChanged += this.HandleGunChanged;

            }


            Material shade = new Material(ShaderCache.Acquire("Brave/ItemSpecific/LootGlintAdditivePass"));
            shade.SetColor("_OverrideColor", new Color32(235, 208, 103, 255));

            this.m_glintShader = shade.shader;
            if (playeroue.CurrentGun)
            {
                Gun guon = playeroue.CurrentGun as Gun;
                Material material = UnityEngine.Object.Instantiate<Material>(guon.renderer.material);
                material.DisableKeyword("TINTING_OFF");
                material.EnableKeyword("TINTING_ON");
                material.SetColor("_OverrideColor", new Color(1f, 0.77f, 0f));
                material.DisableKeyword("EMISSIVE_OFF");
                material.EnableKeyword("EMISSIVE_ON");
                material.SetFloat("_EmissivePower", 1.75f);
                material.SetFloat("_EmissiveColorPower", 1f);
                guon.renderer.material = material;
                this.ProcessGunShader(playeroue.CurrentGun);
            }
        }
        private void HandleGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            Material material = UnityEngine.Object.Instantiate<Material>(newGun.renderer.material);
            material.DisableKeyword("TINTING_OFF");
            material.EnableKeyword("TINTING_ON");
            material.SetColor("_OverrideColor", new Color(1f, 0.77f, 0f));
            material.DisableKeyword("EMISSIVE_OFF");
            material.EnableKeyword("EMISSIVE_ON");
            material.SetFloat("_EmissivePower", 1.75f);
            material.SetFloat("_EmissiveColorPower", 1f);
            newGun.renderer.material = material;
            this.RemoveGunShader(oldGun);
            this.ProcessGunShader(newGun);
        }
        private void RemoveGunShader(Gun g)
        {
            if (!g)
            {
                return;
            }
            MeshRenderer component = g.GetComponent<MeshRenderer>();
            if (!component)
            {
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            List<Material> list = new List<Material>();
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader != this.m_glintShader)
                {
                    list.Add(sharedMaterials[i]);
                }
            }
            component.sharedMaterials = list.ToArray();
        }
        private void ProcessGunShader(Gun g)
        {
            MeshRenderer component = g.GetComponent<MeshRenderer>();
            if (!component)
            {
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == this.m_glintShader)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(this.m_glintShader);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;
        }
        public void Update()
        {
            Dungeon dung = GameManager.Instance.Dungeon;
            if (this.playeroue != null)// && PastNames.Contains(SceneManager.GetActiveScene().name)) //SceneManager.GetActiveScene().name.Contains())// && validTilesets == GlobalDungeonData.ValidTilesets.A)
            {
                if (dung.LevelOverrideType == GameManager.LevelOverrideState.CHARACTER_PAST)
                {
                    if (playeroue.GetComponent<GoldPlatedGun>() != null)
                    {
                        GoldPlatedGun cham = playeroue.GetComponent<GoldPlatedGun>();
                        Destroy(cham);
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            this.RemoveGunShader(playeroue.CurrentGun);
            playeroue.GunChanged -= this.HandleGunChanged;
        }
        private RoomHandler Microwave;
        private PlayerController playeroue;
        private Shader m_glintShader;
    }
    /*
    public class ChallengeUP : BraveBehaviour
    {
        public void Start()
        {
            this.Microwave = base.GetComponent<RoomHandler>();
            this.playeroue = base.GetComponent<PlayerController>();
            {
                PlayerController player = GameManager.Instance.PrimaryPlayer;

            }

        }
        
        public void Update()
        {

        }

        protected override void OnDestroy()
        {

        }
        private RoomHandler Microwave;
        private PlayerController playeroue;
    }
    */
    public class HERETIC : BraveBehaviour
    {
        public void Start()
        {
            CanHeresy = false;
            this.Microwave = base.GetComponent<RoomHandler>();
            this.playeroue = base.GetComponent<PlayerController>();
            {
                PlayerController player = GameManager.Instance.PrimaryPlayer;
                GameManager.Instance.OnNewLevelFullyLoaded += this.OnNewFloor;
            }

        }

        private void OnNewFloor()
        {
            CanHeresy = true;
        }
        public void Update()
        {
            Dungeon dung = GameManager.Instance.Dungeon;
            if (this.playeroue != null)// && PastNames.Contains(SceneManager.GetActiveScene().name)) //SceneManager.GetActiveScene().name.Contains())// && validTilesets == GlobalDungeonData.ValidTilesets.A)
            {
                if (dung.LevelOverrideType == GameManager.LevelOverrideState.CHARACTER_PAST)
                {
                    if (playeroue.GetComponent<HERETIC>() != null)
                    {
                        HERETIC cham = playeroue.GetComponent<HERETIC>();
                        Destroy(cham);
                    }
                }
            }
            if (CanHeresy == true)
            {
                this.elapsed += BraveTime.DeltaTime;
                bool flag3 = this.elapsed > 30;
                if (flag3)
                {
                    if (this.playeroue != null)
                    {
                        AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("jammed_guardian");
                        IntVector2 bestRewardLocation = GameManager.Instance.BestActivePlayer.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
                        AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, bestRewardLocation, GameManager.Instance.BestActivePlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Spawn, true);
                        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
                        aiactor.gameObject.AddComponent<KillOnRoomClear>();
                        aiactor.IgnoreForRoomClear = true;
                        aiactor.HandleReinforcementFallIntoRoom(0f);
                    }
                    this.elapsed = 0f;
                    this.CanHeresy = false;
                }
            }
        }

        protected override void OnDestroy()
        {

        }
        public bool CanHeresy;
        public float TimeBetweenRockFalls;
        private float elapsed;
        private RoomHandler Microwave;
        private PlayerController playeroue;
    }
}







public class UltraAngryGodsScript : Script
{
    protected override IEnumerator Top()
    {
        PlayerController player = (GameManager.Instance.PrimaryPlayer);
        RoomHandler currentRoom = player.CurrentRoom;
        AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_002");
        this.Mines_Cave_In = assetBundle.LoadAsset<GameObject>("Mines_Cave_In");
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Mines_Cave_In, player.sprite.WorldCenter, Quaternion.identity);
        HangingObjectController RockSlideController = gameObject.GetComponent<HangingObjectController>();
        RockSlideController.triggerObjectPrefab = null;
        GameObject[] additionalDestroyObjects = new GameObject[]
        {
                RockSlideController.additionalDestroyObjects[1]
        };
        RockSlideController.additionalDestroyObjects = additionalDestroyObjects;
        UnityEngine.Object.Destroy(gameObject.transform.Find("Sign").gameObject);
        RockSlideController.ConfigureOnPlacement(currentRoom);
        yield return Wait(60);
        IntVector2? vector = player.CurrentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 2), CellTypes.FLOOR | CellTypes.PIT, false, null);
        Vector2 vector2 = player.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2));
        base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("4d164ba3f62648809a4a82c90fc22cae").bulletBank.GetBullet("big_one"));
        base.Fire(Offset.OverridePosition(vector2 + new Vector2(8f, 30f)), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new UltraAngryGodsScript.BigBullet());
        base.Fire(Offset.OverridePosition(vector2 + new Vector2(-8f, 30f)), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new UltraAngryGodsScript.BigBullet());
        base.Fire(Offset.OverridePosition(vector2 + new Vector2(0f, 38f)), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new UltraAngryGodsScript.BigBullet());
        base.Fire(Offset.OverridePosition(vector2 + new Vector2(0f, 22f)), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new UltraAngryGodsScript.BigBullet());


        yield break;
    }
    private GameObject Mines_Cave_In;
    private class BigBullet : Bullet
    {
        public BigBullet() : base("big_one", false, false, false)
        {
        }

        public override void Initialize()
        {
            this.Projectile.spriteAnimator.StopAndResetFrameToDefault();
            base.Initialize();
        }

        protected override IEnumerator Top()
        {
            base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
            this.Projectile.specRigidbody.CollideWithTileMap = false;
            this.Projectile.specRigidbody.CollideWithOthers = false;
            yield return base.Wait(60);
            base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);
            this.Speed = 0f;
            this.Projectile.spriteAnimator.Play();
            base.Vanish(true);
            yield break;
        }

        public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
        {
            if (!preventSpawningProjectiles)
            {
                base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                var list = new List<string>
                {
                "jammed_guardian"
                };
                string guid = BraveUtility.RandomElement<string>(list);
                var Enemy = EnemyDatabase.GetOrLoadByGuid(guid);

                AIActor.Spawn(Enemy.aiActor, this.Projectile.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);
                if ((GameManager.Instance.PrimaryPlayer.HasPickupID(DiamondChamber.DiamondChamberID)))
                {
                    Enemy.aiActor.IsHarmlessEnemy = true;
                    Enemy.aiActor.CanTargetPlayers = false;
                    Enemy.aiActor.CanTargetEnemies = true;
                }
                float num = base.RandomAngle();
                float Amount = 12;
                float Angle = 360 / Amount;
                for (int i = 0; i < Amount; i++)
                {
                    base.Fire(new Direction(num + Angle * (float)i + 10, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new BurstBullet());
                }
                base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);
                return;
            }
        }
        public class BurstBullet : Bullet
        {
            public BurstBullet() : base("reversible", false, false, false)
            {
            }
            protected override IEnumerator Top()
            {
                base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
                yield return base.Wait(60);
                base.Vanish(false);
                yield break;
            }
        }

    }
}

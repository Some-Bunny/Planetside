    using System;
using System.Collections;
using ItemAPI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using SaveAPI;

namespace Planetside
{
    public class ShelltansBlessing : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Shelltans Blessing";
            //string resourceName = "Planetside/Resources/shelltansblessing.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ShelltansBlessing>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("shelltansblessing"), data, obj);
            item.sprite.SortingOrder = 3;

            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Defiled";
            string longDesc = "A piece of a long-defiled Shelltan shrine." +
                "\n\nAlthough its power is weak, the power originally found within still lingers, waiting...";
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 0.8f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RateOfFire, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);


            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.C;
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:shelltans_blessing",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "ammo_synthesizer",
                "zombie_bullets",
                "bloody_9mm",
                "holey_grail",
                "bullet_idol",
                "sixth_chamber",
                "yellow_chamber"
            };
            CustomSynergies.Add("Invigorated", mandatoryConsoleIDs, optionalConsoleIDs, true);
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED, true);
            item.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);
            ShelltansBlessing.ShelltainsBlessingID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);


            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("ShelltansBlessing", debuffCollection.GetSpriteIdByName("blessing_001"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.VFX_Animation_Data;
            animator.DefaultClipId = animator.GetClipIdByName("shelltan_blessing");
            animator.playAutomatically = true;
            ShelltansBlessingVFX = BrokenArmorVFXObject;


            new Hook(typeof(AdvancedShrineController).GetMethod("DoShrineEffect", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ShelltansBlessing).GetMethod("DoShrineEffectHook"));
            GameManager.Instance.RainbowRunForceExcludedIDs.Add(item.PickupObjectId);

        }

        public static void DoShrineEffectHook(Action<AdvancedShrineController, PlayerController>orig, AdvancedShrineController self, PlayerController player)
        {
            if (self.name == "Shrine_Ammo" && player.HasPassiveItem(ShelltainsBlessingID))
            {
                self.GetRidOfMinimapIcon();
                AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", self.gameObject);
                OtherTools.Notify("Shelltan senses your faith.", "His Power Is Gained!", "Planetside/Resources/ShrineIcons/HeresyIcons/ShelltanIcon");
                OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, 0.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                OtherTools.ApplyStat(player, PlayerStats.StatType.DamageToBosses, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                OtherTools.ApplyStat(player, PlayerStats.StatType.RateOfFire, 1.75f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                OtherTools.ApplyStat(player, PlayerStats.StatType.AmmoCapacityMultiplier, 2.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                for (int i = 0; i < player.inventory.AllGuns.Count; i++)
                {
                    player.inventory.AllGuns[i].GainAmmo(Mathf.FloorToInt((float)player.inventory.AllGuns[i].AdjustedMaxAmmo));
                }
            }
            else
            {
                orig(self, player);
            }
        }
        private static GameObject ShelltansBlessingVFX;

        public static int ShelltainsBlessingID;
        private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemy)
        {
            PlayerController player = base.Owner;
            if (base.Owner != null)
            {
                if (fatal)
                {
                    if (!enemy.IsBoss)
                    {
                        //float anotherGoddamnCluculation = player.CurrentGun.InfiniteAmmo == false ? m : 0;
                        if (player.CurrentGun.InfiniteAmmo == false)
                        {
                            float m = (1 - Mathf.Sqrt((float)player.CurrentGun.AdjustedMaxAmmo / 1000f) * 0.45f);

                            if (UnityEngine.Random.value <= m)
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", player.gameObject);
                               
                                
                                GameObject lightning = player.PlayEffectOnActor(ShelltansBlessingVFX, new Vector3(0f, 1.25f, 0f));
                                lightning.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("shelltan_blessing");
                                
                                
                                if (player.PlayerHasActiveSynergy("Invigorated"))
                                {
                                    for (int i = 0; i < player.inventory.AllGuns.Count; i++)
                                    {
                                        if (player.inventory.AllGuns[i] && player.CurrentGun != player.inventory.AllGuns[i])
                                        {
                                            player.inventory.AllGuns[i].GainAmmo(Mathf.CeilToInt((float)player.inventory.AllGuns[i].AdjustedMaxAmmo * 0.02f));
                                        }
                                    }
                                }
                                else
                                {
                                    player.inventory.CurrentGun.GainAmmo(Mathf.CeilToInt((float)player.inventory.CurrentGun.AdjustedMaxAmmo * 0.04f));
                                }
                                player.CurrentGun.ForceImmediateReload(false);
                            }
                        }
                    }
                    else
                    {
                        if (player.CurrentGun.InfiniteAmmo == false)
                        {
                            AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", player.gameObject);
                            
                            GameObject lightning = player.PlayEffectOnActor(ShelltansBlessingVFX, new Vector3(0f, 1.25f, 0f));
                            lightning.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("shelltan_blessing");

                            if (player.PlayerHasActiveSynergy("Invigorated"))
                            {
                                for (int i = 0; i < player.inventory.AllGuns.Count; i++)
                                {
                                    if (player.inventory.AllGuns[i] && player.CurrentGun != player.inventory.AllGuns[i])
                                    {
                                        player.inventory.AllGuns[i].GainAmmo(Mathf.CeilToInt((float)player.inventory.AllGuns[i].AdjustedMaxAmmo * 0.2f));
                                    }
                                }
                            }
                            else
                            {
                                player.inventory.CurrentGun.GainAmmo(Mathf.CeilToInt((float)player.inventory.CurrentGun.AdjustedMaxAmmo * 0.1f));
                            }
                            player.CurrentGun.ForceImmediateReload(false);
                        }
                    }

                    
                    
                }
            }          
		}

		public override DebrisObject Drop(PlayerController player)
		{
            player.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Remove(player.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            DebrisObject result = base.Drop(player);
			return result;
		}
        public override void Pickup(PlayerController player)
		{
            player.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Combine(player.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            base.Pickup(player);
		}
		public override void OnDestroy()
		{
            if (base.Owner != null)
            {
                base.Owner.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Remove(base.Owner.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            }
            base.OnDestroy();
		}
	}
}
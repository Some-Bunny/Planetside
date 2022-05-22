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
            string resourceName = "Planetside/Resources/shelltansblessing.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ShelltansBlessing>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Defiled";
            string longDesc = "A piece of a long-defiled Shelltan shrine." +
                "\n\nAlthough its power is weak, the power originally found within still lingers, waiting...";
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
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

            GameObject blessingObj = ItemBuilder.AddSpriteToObject("shelltansBlessingVFX", "Planetside/Resources/VFX/ShelltansBlessing/blessing_001", null);
            FakePrefab.MarkAsFakePrefab(blessingObj);
            UnityEngine.Object.DontDestroyOnLoad(blessingObj);
            tk2dSpriteAnimator animator = blessingObj.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = blessingObj.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(blessingObj, ("ShelltansBlessing_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 15 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i < 10; i++)
            {
                tk2dSpriteCollectionData collection = DeathMarkcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/ShelltansBlessing/blessing_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = false;

            ShelltansBlessingVFX = blessingObj;
            new Hook(typeof(AdvancedShrineController).GetMethod("DoShrineEffect", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ShelltansBlessing).GetMethod("DoShrineEffectHook"));

        }

        public static void DoShrineEffectHook(Action<AdvancedShrineController, PlayerController>orig, AdvancedShrineController self, PlayerController player)
        {
            if (self.name == "Shrine_Ammo" && player.HasPassiveItem(ShelltainsBlessingID))
            {
                self.GetRidOfMinimapIcon();
                AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", self.gameObject);
                OtherTools.Notify("Shelltan senses your faith.", "His Power Is Gained.", "Planetside/Resources/ShrineIcons/HeresyIcons/ShelltanIcon");
                OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                OtherTools.ApplyStat(player, PlayerStats.StatType.AmmoCapacityMultiplier, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
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
                    float random = UnityEngine.Random.Range(0.00f, 1.00f);
                    float anotherGoddamnCluculation = player.CurrentGun.InfiniteAmmo == false ? player.CurrentGun.AdjustedMaxAmmo / 1000 : 0;
                    if (player.CurrentGun.InfiniteAmmo == false)
                    {
                        float Value = 0.05f + anotherGoddamnCluculation;
                        if (Value >= 1) { Value = 1; }
                        if (random <= Value)
                        {
                            AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", player.gameObject);
                            GameObject lightning = player.PlayEffectOnActor(ShelltansBlessingVFX, new Vector3(0f, -1f, 0f));
                            lightning.transform.position.WithZ(transform.position.z + 99999);
                            lightning.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(player.CenterPosition + new Vector2(0, 1.25f), tk2dBaseSprite.Anchor.MiddleCenter);
                            player.sprite.AttachRenderer(lightning.GetComponent<tk2dBaseSprite>());
                            lightning.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
                            if (player.PlayerHasActiveSynergy("Invigorated"))
                            {
                                for (int i = 0; i < player.inventory.AllGuns.Count; i++)
                                {
                                    if (player.inventory.AllGuns[i] && player.CurrentGun != player.inventory.AllGuns[i])
                                    {
                                        player.inventory.AllGuns[i].GainAmmo(Mathf.FloorToInt((float)player.inventory.AllGuns[i].AdjustedMaxAmmo * 0.025f));
                                    }
                                }
                            }
                            else
                            {
                                player.inventory.CurrentGun.GainAmmo(Mathf.FloorToInt((float)player.inventory.CurrentGun.AdjustedMaxAmmo * 0.05f));
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
		protected override void OnDestroy()
		{
            if (base.Owner != null)
            {
                base.Owner.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Remove(base.Owner.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            }
            base.OnDestroy();
		}
	}
}
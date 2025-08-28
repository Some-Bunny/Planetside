﻿using System;
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
using Brave.BulletScript;
using HarmonyLib;

namespace Planetside
{
	public class GunslingersRing : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Gunslingers Ring";
			//string resourceName = "Planetside/Resources/gunslingersring.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<GunslingersRing>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("gunslingersring"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Left Hand Man";
			string longDesc = "Grants powers tuned to your currently held gun.\n\nAn old ring worn by a gunslinger of similar skill to the Lich.\n\nThe gem on the ring shifts colors when the bearer wields different weapons.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.A;
			GunslingersRing.GunslingersRingID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);
		}
		public static int GunslingersRingID;

        [HarmonyPatch(typeof(GameUIRoot), nameof(GameUIRoot.UpdateGunDataInternal))]
        public class NopeOutVisibility
        {
            public const string ERROR = "[color #ff0000][ERROR]: WEAPON JAM[/color]";
            static void Postfix(GameUIRoot __instance, PlayerController targetPlayer, GunInventory inventory, int inventoryShift, GameUIAmmoController targetAmmoController, int labelTarget)
            {
                bool HasActiveRing = (PassiveItem.ActiveFlagItems[targetPlayer].ContainsKey(typeof(GunslingersRing)));
                if (HasActiveRing == false || targetPlayer.CurrentGun == null) { return; }

				string currentHex = ColorUtility.ToHtmlStringRGBA(gunClassRGB[targetPlayer.CurrentGun.gunClass]);
				string Text = gunClassDecsiptions[targetPlayer.CurrentGun.gunClass];
                __instance.m_gunNameVisibilityTimers[labelTarget] -= __instance.m_deltaTime;
                if (__instance.m_gunNameVisibilityTimers[labelTarget] > 1f)
                {
                    __instance.gunNameLabels[labelTarget].processMarkup = true;
                    __instance.gunNameLabels[labelTarget].IsVisible = true;
                    __instance.gunNameLabels[labelTarget].Opacity = 1f;
                    __instance.gunNameLabels[labelTarget].Text = $"[color #{currentHex}]+{Text}+[/color]\n" + __instance.gunNameLabels[labelTarget].Text;

                }
                else if (__instance.m_gunNameVisibilityTimers[labelTarget] > 0f)
                {
                    __instance.gunNameLabels[labelTarget].processMarkup = true;
                    __instance.gunNameLabels[labelTarget].IsVisible = true;
                    __instance.gunNameLabels[labelTarget].Opacity = __instance.m_gunNameVisibilityTimers[labelTarget];
                    __instance.gunNameLabels[labelTarget].Text = $"[color #{currentHex}]+{Text}+[/color]\n" + $"{__instance.gunNameLabels[labelTarget].Text}";

                }
                else
                {
                    __instance.gunNameLabels[labelTarget].processMarkup = true;
                    __instance.gunNameLabels[labelTarget].IsVisible = true;
                    __instance.gunNameLabels[labelTarget].Opacity = 1;
                    __instance.gunNameLabels[labelTarget].Text = $"[color #{currentHex}]+{Text}+";
                }
            }
        }



        public override void Update()
        {
			if (base.Owner != null)
            {
				if (base.Owner.CurrentRoom != null)
				{
                    List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    Vector2 centerPosition = base.Owner.CenterPosition;
                    if (activeEnemies != null && activeEnemies.Count > 0)
                    {
                        foreach (AIActor aiactor in activeEnemies)
                        {
                            if (gunClassCharm.ContainsKey(base.Owner.gameActor.CurrentGun.gunClass))
                            {
                                if (aiactor != null && Vector2.Distance(aiactor.CenterPosition, centerPosition) < 4)
                                {
                                    aiactor.ApplyEffect(DebuffStatics.charmingRoundsEffect);
                                }
                            }
                        }
                    }
                }
			}
		}

		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
        {
			if (sourceProjectile != null && sourceProjectile.PossibleSourceGun != null)
            {
				if (gunClassDebuffs.ContainsKey(sourceProjectile.PossibleSourceGun.gunClass) || gunClassCharm.ContainsKey(sourceProjectile.PossibleSourceGun.gunClass))
                {
					AoEDamageComponent Values = sourceProjectile.gameObject.AddComponent<AoEDamageComponent>();
					Values.DamageperDamageEvent = 1f;
					Values.Radius = 2.5f;
					Values.TimeBetweenDamageEvents = 0.25f;
					Values.DealsDamage = false;
					Values.AreaIncreasesWithProjectileSizeStat = true;
					Values.DamageValuesAlsoScalesWithDamageStat = false;

                    if (sourceProjectile.PossibleSourceGun.gunClass == GunClass.POISON)
                    {
                        Values.debuffs.Add(DebuffStatics.irradiatedLeadEffect, 0.333f);
                    }
                    if (sourceProjectile.PossibleSourceGun.gunClass == GunClass.FIRE)
                    {
                        Values.debuffs.Add(DebuffStatics.hotLeadEffect, 0.333f);
                    }
                    if (sourceProjectile.PossibleSourceGun.gunClass == GunClass.CHARM)
                    {
                        Values.debuffs.Add(DebuffStatics.charmingRoundsEffect, 0.333f);
                    }
                    if (sourceProjectile.PossibleSourceGun.gunClass == GunClass.ICE)
                    {
                        Values.debuffs.Add(DebuffStatics.frostBulletsEffect, 1);
                    }
                }
            }
        }


		private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
		{
			StatModifier statModifier = new StatModifier
			{
				amount = amount,
				statToBoost = statType,
				modifyType = method
			};
			bool flag = this.passiveStatModifiers == null;
			if (flag)
			{
				this.passiveStatModifiers = new StatModifier[]
				{
					statModifier
				};
			}
			else
			{
				this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[]
				{
					statModifier
				}).ToArray<StatModifier>();
			}
		}
		private void RemoveStat(PlayerStats.StatType statType)
		{
			List<StatModifier> list = new List<StatModifier>();
			for (int i = 0; i < this.passiveStatModifiers.Length; i++)
			{
				bool flag = this.passiveStatModifiers[i].statToBoost != statType;
				if (flag)
				{
					list.Add(this.passiveStatModifiers[i]);
				}
			}
			this.passiveStatModifiers = list.ToArray();
		}
		public override DebrisObject Drop(PlayerController player)
		{
			PassiveItem.DecrementFlag(player, typeof(GunslingersRing));
			player.GunChanged -= this.OnGunChanged;
			player.PostProcessProjectile -= this.PostProcessProjectile;
			this.RemoveStat(LastStoredStat);
			if (LastStoredImmunity != null)
			{
				base.Owner.healthHaver.damageTypeModifiers.Remove(LastStoredImmunity);
			}
			DebrisObject result = base.Drop(player);
			return result;
		}
		public override void Pickup(PlayerController player)
		{
            PassiveItem.IncrementFlag(player, typeof(GunslingersRing));
            player.PostProcessProjectile += this.PostProcessProjectile;
			player.GunChanged += this.OnGunChanged;
			base.Pickup(player);
		}
		public override void OnDestroy()
		{
			if (base.Owner != null)
            {
                PassiveItem.DecrementFlag(base.Owner, typeof(GunslingersRing));
                base.Owner.GunChanged -= this.OnGunChanged;
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				this.RemoveStat(LastStoredStat);
				if (LastStoredImmunity != null)
				{
					base.Owner.healthHaver.damageTypeModifiers.Remove(LastStoredImmunity);
				}
				base.OnDestroy();
			}
		}
		private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
		{
			if (newGun != null && base.Owner != null)
			{
				if (LastStoredImmunity != null)
				{
					base.Owner.healthHaver.damageTypeModifiers.Remove(LastStoredImmunity);
				}
				this.RemoveStat(LastStoredStat);
				Dictionary<PlayerStats.StatType, float> stats = new Dictionary<PlayerStats.StatType, float>();
				gunClassStats.TryGetValue(newGun.gunClass, out stats);
				if (gunClassStats.ContainsKey(newGun.gunClass))
				{
					foreach (var Entries in stats)
					{
						this.RemoveStat(Entries.Key);
						this.AddStat(Entries.Key, Entries.Value, StatModifier.ModifyMethod.MULTIPLICATIVE);
						LastStoredStat = Entries.Key;
					}
				}
				base.Owner.stats.RecalculateStats(base.Owner, true, false);


				Color color = new Color();
				gunClassRGB.TryGetValue(base.Owner.gameActor.CurrentGun.gunClass, out color);
				if (color != null)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), base.Owner.primaryHand.transform.position != null ? base.Owner.primaryHand.transform.position : base.Owner.transform.position, Quaternion.identity);
					tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
					component.PlaceAtPositionByAnchor(base.Owner.primaryHand.transform.position != null ? base.Owner.primaryHand.transform.position : base.Owner.transform.position, tk2dBaseSprite.Anchor.MiddleCenter);
					component.HeightOffGround = 35f;
					component.UpdateZDepth();
					tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
					if (component2 != null)
					{
						component.scale *= 0.4f;
						component2.ignoreTimeScale = true;
						component2.AlwaysIgnoreTimeScale = true;
						component2.AnimateDuringBossIntros = true;
						component2.alwaysUpdateOffscreen = true;
						component2.playAutomatically = true;
						component2.sprite.usesOverrideMaterial = true;
						component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
						component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 1);
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.1f);
						component2.sprite.renderer.material.SetColor("_OverrideColor", color);
						component2.sprite.renderer.material.SetColor("_EmissiveColor", color);
					}
					Destroy(gameObject, 1.5f);
				}
				if (gunClassDebuffs.ContainsKey(newGun.gunClass))
                {
					CoreDamageTypes damageType = new CoreDamageTypes();
					gunClassDebuffs.TryGetValue(newGun.gunClass, out damageType);
					LastStoredImmunity = new DamageTypeModifier();
					LastStoredImmunity.damageMultiplier = 0f;
					LastStoredImmunity.damageType = damageType;
					base.Owner.healthHaver.damageTypeModifiers.Add(LastStoredImmunity);
				}
				HelmetItem blastProt = new HelmetItem();
				bool Contains = PassiveItem.ActiveFlagItems[base.Owner].ContainsKey(blastProt.GetType());
				if (gunClassExplo.ContainsKey(newGun.gunClass) && !Contains)
				{
					PassiveItem.ActiveFlagItems[base.Owner].Add(blastProt.GetType(), 1);
				}
				else if (!base.Owner.HasPickupID(312) && gunClassExplo.ContainsKey(newGun.gunClass))
                {
					PassiveItem.ActiveFlagItems[base.Owner].Remove(blastProt.GetType());
				}
			}
		}

		private static Dictionary<GunClass, Dictionary<PlayerStats.StatType, float>> gunClassStats = new Dictionary<GunClass, Dictionary<PlayerStats.StatType, float>>()
		{
			{GunClass.BEAM, new Dictionary<PlayerStats.StatType, float>{{ PlayerStats.StatType.PlayerBulletScale, 2f}}},
			{GunClass.CHARGE, new Dictionary<PlayerStats.StatType, float>{{ PlayerStats.StatType.ChargeAmountMultiplier, 1.5f}}},			
			{GunClass.RIFLE, new Dictionary<PlayerStats.StatType, float>{{ PlayerStats.StatType.AdditionalClipCapacityMultiplier, 1.5f}}},
			{GunClass.FULLAUTO, new Dictionary<PlayerStats.StatType, float>{{ PlayerStats.StatType.RateOfFire, 1.2f}}},
			{GunClass.NONE, new Dictionary<PlayerStats.StatType, float>{{ PlayerStats.StatType.Damage, 1.2f}}},
			{GunClass.PISTOL, new Dictionary<PlayerStats.StatType, float>{{ PlayerStats.StatType.ReloadSpeed, 0.66f}}},
			{GunClass.SHITTY, new Dictionary<PlayerStats.StatType, float>{{ PlayerStats.StatType.MoneyMultiplierFromEnemies, 1.15f}}},
			{GunClass.SHOTGUN, new Dictionary<PlayerStats.StatType, float>{{ PlayerStats.StatType.Accuracy, 0.66f}}},
			{GunClass.SILLY, new Dictionary<PlayerStats.StatType, float>{{ PlayerStats.StatType.AdditionalShotBounces, 3f}}},
            {GunClass.ICE, new Dictionary<PlayerStats.StatType, float>{{ PlayerStats.StatType.Coolness, 2f}}},
        };

		private static Dictionary<GunClass, Color> gunClassRGB = new Dictionary<GunClass, Color>()
		{
			{GunClass.BEAM, Color.blue},//DONE
			{GunClass.CHARM, new Color(1, 0.2f, 1f)},//DONE
			{GunClass.CHARGE, Color.yellow},	//DONE
			{GunClass.EXPLOSIVE, new Color(1, 0.2f, 0f)},
			{GunClass.RIFLE, Color.gray},//DONE
			{GunClass.FIRE, Color.red},//DONE
			{GunClass.FULLAUTO, new Color(0.9f, 0.8f, 0.6f)},//DONE
			{GunClass.ICE, Color.cyan },
			{GunClass.NONE, Color.white},//DONE
			{GunClass.PISTOL, Color.black},//DONE
			{GunClass.POISON, Color.green},//DONE
			{GunClass.SHITTY, new Color(0.6f,0.3f, 0.2f)},//DONE
			{GunClass.SHOTGUN, new Color(0.3f, 0.4f, 0.1f)},//DONE
			{GunClass.SILLY, new Color(0.6f, 0.02f, 0.6f)},//DONE
        };
        private static Dictionary<GunClass, CoreDamageTypes> gunClassDebuffs = new Dictionary<GunClass, CoreDamageTypes>()
        {
			{GunClass.FIRE, CoreDamageTypes.Fire},
			{GunClass.POISON, CoreDamageTypes.Poison },
		};
        private static Dictionary<GunClass, float> gunClassExplo = new Dictionary<GunClass, float>()
		{
			{GunClass.EXPLOSIVE, 0.1f},
		};
        private static Dictionary<GunClass, float> gunClassCharm = new Dictionary<GunClass, float>()
		{
			{GunClass.CHARM, 2.5f},
		};

        private static Dictionary<GunClass, string> gunClassDecsiptions = new Dictionary<GunClass, string>()
        {
            {GunClass.BEAM, "SIZE"},
			{GunClass.CHARM, "LOVE"},
			{GunClass.CHARGE, "CHARGE"},	
			{GunClass.EXPLOSIVE, "BLAST RESIST"},
            {GunClass.RIFLE, "CLIP"},
			{GunClass.FIRE, "FIRE"},
			{GunClass.FULLAUTO, "FIRERATE"},
			{GunClass.ICE, "FREEZE" },
            {GunClass.NONE, "DAMAGE"},
			{GunClass.PISTOL, "RELOAD"},
			{GunClass.POISON,"POISON"},
			{GunClass.SHITTY, "MONEY"},
			{GunClass.SHOTGUN, "ACCURACY"},
			{GunClass.SILLY, "BOUNCE"},
        };

        private PlayerStats.StatType LastStoredStat;
		private DamageTypeModifier LastStoredImmunity;
	}
}
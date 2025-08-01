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
using HarmonyLib;
using Alexandria.Assetbundle;
using System.ComponentModel;
using static ETGMod;

namespace Planetside
{
    public class MopEffectContainer
    {
        public Color projectileColor;
        public Type component;
        public List<GameActorEffect> debuffs;
        public string Key;
        public List<string> goopNames;
		public float DamageMultiplier = 1;
    }

    public class Mop : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Mop", "mopgun");
			Game.Items.Rename("outdated_gun_mods:mop", "psog:mop");
			gun.gameObject.AddComponent<Mop>();
			gun.SetShortDescription("Honest Work");
			gun.SetLongDescription("A mop thats been left inside of a chest. Surely the goops found around the Gungeon could be mopped up to some benefit?");
            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "mopgun_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.idleAnimation = "mop_idle";
            gun.shootAnimation = "mop_fire";
            gun.reloadAnimation = "mop_reload";

            //gun.SetupSprite(null, "mopgun_idle_001", 11);
            //GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
            //GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 10);
            //GunExt.SetAnimationFPS(gun, gun.idleAnimation, 2);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(404) as Gun).gunSwitchGroup;

			for (int i = 0; i < 3; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(404) as Gun, true, false);
			}

			EnemyToolbox.AddSoundsToAnimationFrame(gun.spriteAnimator, gun.shootAnimation, new Dictionary<int, string>() { {0, "Play_ENM_wizardred_swing_01" } });

			gun.Volley.projectiles[0].ammoCost = 1;
			gun.Volley.projectiles[0].shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.Volley.projectiles[0].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.Volley.projectiles[0].cooldownTime = 0.5f;
			gun.Volley.projectiles[0].angleVariance = 10f;
			gun.Volley.projectiles[0].numberOfShotsInClip = -1;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[0].projectiles[0]);
			projectile.gameObject.SetActive(false);
			gun.Volley.projectiles[0].projectiles[0] = projectile;
			projectile.baseData.damage = 8.5f;
			projectile.AdditionalScaleMultiplier *= 1.5f;
			projectile.baseData.speed *= 1.25f;
			projectile.gameObject.AddComponent<MopProjectile>();
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			bool flag = gun.Volley.projectiles[0] != gun.DefaultModule;
			if (flag)
			{
				gun.Volley.projectiles[0].ammoCost = 0;
			}


			projectile.transform.parent = gun.barrelOffset;

			gun.Volley.projectiles[1].ammoCost = 1;
			gun.Volley.projectiles[1].shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.Volley.projectiles[1].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.Volley.projectiles[1].cooldownTime = 0.5f;
			gun.Volley.projectiles[1].angleVariance = 9f;
			gun.Volley.projectiles[1].numberOfShotsInClip = -1;
			gun.gunClass = GunClass.SILLY;

			Projectile projectile1 = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[1].projectiles[0]);
			projectile1.gameObject.SetActive(false);
			gun.Volley.projectiles[1].projectiles[0] = projectile1;
			projectile1.gameObject.AddComponent<MopProjectile>();

			projectile1.baseData.damage = 4f;
			FakePrefab.MarkAsFakePrefab(projectile1.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile1);
			bool aa = gun.Volley.projectiles[1] != gun.DefaultModule;
			if (aa)
			{
				gun.Volley.projectiles[1].ammoCost = 0;
			}
			projectile1.transform.parent = gun.barrelOffset;

			gun.Volley.projectiles[2].ammoCost = 1;
			gun.Volley.projectiles[2].shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.Volley.projectiles[2].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.Volley.projectiles[2].cooldownTime = 0.5f;
			gun.Volley.projectiles[2].angleVariance = 10f;
			gun.Volley.projectiles[2].numberOfShotsInClip = -1;

			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[2].projectiles[0]);
			projectile1.gameObject.SetActive(false);
			gun.Volley.projectiles[2].projectiles[0] = projectile1;

			projectile2.baseData.damage = 4.5f;


			projectile2.gameObject.AddComponent<MopProjectile>();


			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);
			bool ee = gun.Volley.projectiles[2] != gun.DefaultModule;
			if (ee)
			{
				gun.Volley.projectiles[2].ammoCost = 0;
			}
			projectile1.transform.parent = gun.barrelOffset;


			gun.barrelOffset.transform.localPosition = new Vector3(0.5f, 0.5f, 0f);
			gun.reloadTime = 2.5f;
			gun.SetBaseMaxAmmo(25);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(417) as Gun).muzzleFlashEffects;
			gun.CanReloadNoMatterAmmo = true;


			gun.quality = PickupObject.ItemQuality.D;
			gun.encounterTrackable.EncounterGuid = "mop";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			Mop.MopID = gun.PickupObjectId;

			gun.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);
            Alexandria.ItemAPI.ItemBuilder.AddToGunslingKingTable(gun, 1);


            ItemIDs.AddToList(gun.PickupObjectId);

            tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName(gun.shootAnimation);
            float[] offsetsX = new float[] { 1.25f, -0.25f, -0.1875f, -0.125f, -0.0625f, -0.0625f, };
            float[] offsetsY = new float[] { 0.5f, -0.625f, -0.5f, -0.375f, -0.25f, -0.125f };
            for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < fireClip.frames.Length; i++)
            {
                int id = fireClip.frames[i].spriteId;
				SpriteOffseter.MakeOffset(StaticSpriteDefinitions.Gun_Sheet_Data, fireClip.frames[i].spriteCollection.spriteDefinitions[id], 
					new Vector3(offsetsX[i], offsetsY[i]), new string[4] {"PrimaryHand", "SecondaryHand", "Clip", "Casing"});
            }



			tk2dSpriteAnimationClip fireClip2 = gun.sprite.spriteAnimator.GetClipByName(gun.reloadAnimation);
			float[] offsetsX2 = new float[] { 0.25f , 0.375f, 0.5625f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.625f, 0.375f, 0.25f};
			float[] offsetsY2 = new float[] { 0f , -0.125f, -0.25f, -0.375f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.4375f, -0.375f, -0.25f, -0.125f, 0};
			for (int i = 0; i < offsetsX2.Length && i < offsetsY2.Length && i < fireClip2.frames.Length; i++)
			{
				int id = fireClip2.frames[i].spriteId;
                SpriteOffseter.MakeOffset(StaticSpriteDefinitions.Gun_Sheet_Data, fireClip2.frames[i].spriteCollection.spriteDefinitions[id],
				new Vector3(offsetsX2[i], offsetsY2[i]), new string[4] { "PrimaryHand", "SecondaryHand", "Clip", "Casing" });
            }
			


        }


        public static int MopID;
		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
			gun.PreventNormalFireAudio = true;
		}
		private bool HasReloaded;

		public override void Update()
		{
			base.Update();
			if (gun.CurrentOwner)
			{

				if (!gun.PreventNormalFireAudio)
				{
					this.gun.PreventNormalFireAudio = true;
				}
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
			}

		}
		public override void OnPickup(PlayerController player)
		{		
			base.OnPickup(player);
		}

		public override void OnPostDrop(PlayerController player)
		{
			base.OnPostDrop(player);
		}

		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{	
			if (gun.IsReloading && this.HasReloaded)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
			GoopDefinition currentGoop = player.CurrentGoop;
			if (currentGoop != null && currentGoop.name != null)
			{
                gun.GainAmmo(Mathf.Max(0, gun.ClipCapacity - gun.ClipShotsRemaining));

				bool check = true;
                string Name = currentGoop.name;
				foreach (var container in containers)
				{
					if (container.goopNames.Contains(Name.ToLower()))
					{
						currentEffectContainer = container;
						check = false;
                    }
				}
				if (check == true)
				{
                    Debug.Log("Unrecognized Goop, attempting to breakdown");

                    List<GameActorEffect> gameActorEffects = new List<GameActorEffect>();
                    if (currentGoop.CharmModifierEffect != null) { gameActorEffects.Add(currentGoop.CharmModifierEffect); }
                    if (currentGoop.fireEffect != null) { gameActorEffects.Add(currentGoop.fireEffect); }
                    if (currentGoop.HealthModifierEffect != null) { gameActorEffects.Add(currentGoop.HealthModifierEffect); }
                    if (currentGoop.CheeseModifierEffect != null) { gameActorEffects.Add(currentGoop.CheeseModifierEffect); }
                    if (currentGoop.SpeedModifierEffect != null) { gameActorEffects.Add(currentGoop.SpeedModifierEffect); }

					MopEffectContainer c = new MopEffectContainer()
					{
						component = null,
						DamageMultiplier = 1,
						goopNames = new List<string>()
						{
							currentGoop.name.ToLower()
						},
						debuffs = gameActorEffects,
						Key = currentGoop.name + "_Key",
						projectileColor = currentGoop.baseColor32
					};

					containers.Add(c);
					currentEffectContainer = c; 
                }
			}
			else
            {
                bool check_ = true;

                if (player.CurrentRoom != null)
				{
					var cell = player.CurrentRoom.GetNearestCellToPosition(player.transform.position);
                    if (cell != null)
					{
                        if (cell.cellVisualData.floorType == CellVisualData.CellFloorType.Water) 
						{
                            gun.GainAmmo(Mathf.Max(0, gun.ClipCapacity - gun.ClipShotsRemaining));
                            currentEffectContainer = containers.Where(self => self.Key == "water").FirstOrDefault();
                            check_ = true;
                        }
						if (cell.doesDamage && (cell.damageDefinition.damageToPlayersPerTick > 0f ))
						{
                            if (cell.damageDefinition.isPoison || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON)
                            {
								check_ = true;
                                gun.GainAmmo(Mathf.Max(0, gun.ClipCapacity - gun.ClipShotsRemaining));
                                currentEffectContainer = containers.Where(self => self.Key == "poison").FirstOrDefault();
                            }
                            else if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
                            {
                                check_ = true;
                                gun.GainAmmo(Mathf.Max(0, gun.ClipCapacity - gun.ClipShotsRemaining));
                                currentEffectContainer = containers.Where(self => self.Key == "fire").FirstOrDefault();
                            }
						}
                    }

                    //player.transform.position 
                }
				if (check_ == false)
				{
                    currentEffectContainer = null;
                }
            }
			DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(player.CenterPosition, 2f);
		}



		public MopEffectContainer currentEffectContainer;


		public static List<MopEffectContainer> containers = new List<MopEffectContainer>()
		{
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){ DebuffStatics.hotLeadEffect },
				Key = "fire",
				projectileColor = new Color32(255, 102, 0, 255),
				goopNames = new List<string>()
				{
					EasyGoopDefinitions.FireDef2.name.ToLower(),
					EasyGoopDefinitions.FireDef.name.ToLower(),
					"helicopternapalmgoop",
					"napalm goop",
					"napalmgoopshortlife",
					"bulletkingwinegoop",
					"devilgoop",
					"flamelinegoop",
					"demonwallgoop"
				}
			},
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){ DebuffStatics.greenFireEffect },
				Key = "hellfire",
				projectileColor = new Color32(211, 229, 73, 255),
				goopNames = new List<string>()
				{
					"greennapalmgoopthatworks"
				}
			},
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){  DebuffLibrary.MopBlobEffect  },
				Key = "blob",
				projectileColor = new Color32(213, 77, 77, 255),
				goopNames = new List<string>()
				{
					EasyGoopDefinitions.BlobulonGoopDef.name.ToLower(),
					"blobulordgoop"
				}
			},
			new MopEffectContainer()
			{
				component = new ApplyStep2().GetType(),
				debuffs = new List<GameActorEffect>(){ },
				Key = "oil",
				projectileColor = new Color32(10, 6, 18, 255),
				goopNames = new List<string>()
				{
					EasyGoopDefinitions.OilDef.name.ToLower()
				}
			},
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){ DebuffStatics.cheeseeffect },
				Key = "cheese",
				projectileColor = new Color32(255, 102, 0, 255),
				goopNames = new List<string>()
				{
					EasyGoopDefinitions.CheeseDef.name.ToLower()
				}
			},
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){ DebuffStatics.charmingRoundsEffect },
				Key = "charm",
				projectileColor = new Color32(252, 72, 241, 255),
				goopNames = new List<string>()
				{
					EasyGoopDefinitions.CharmGoopDef.name.ToLower()
				}
			},
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){ },
				Key = "water",
				projectileColor = new Color(0,0,0,0),
				goopNames = new List<string>()
				{
					EasyGoopDefinitions.WaterGoop.name.ToLower(),
					"mimicspitgoop"
				},
				DamageMultiplier = 1.2f
			},
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){ },
				Key = "water",
				projectileColor = new Color(0,0,0,0),
				goopNames = new List<string>()
				{
					EasyGoopDefinitions.WaterGoop.name.ToLower(),
					"mimicspitgoop"
				},
				DamageMultiplier = 1.2f
			},
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){ DebuffStatics.irradiatedLeadEffect },
				Key = "poison",
				projectileColor = new Color32(145, 227, 120, 255),
				goopNames = new List<string>()
				{
					EasyGoopDefinitions.PoisonDef.name.ToLower(),
					"resourcefulratpoisongoop",
					"meduzipoisongoop"
				},
			},
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){ DebuffLibrary.MopWebEffect },
				Key = "web",
				projectileColor = new Color32(184, 181, 147, 255),
				goopNames = new List<string>()
				{
					EasyGoopDefinitions.WebGoop.name.ToLower()
				},
			},
			new MopEffectContainer()
			{
				component = new ApplyEnrage().GetType(),
				debuffs = new List<GameActorEffect>(){ },
				Key = "blood",
				projectileColor = new Color32(136, 8, 8, 255),
				goopNames = new List<string>()
				{
					"permanentbloodgoop",
					"bloodgoop",
					"bloodbulongoop"
				}
			},
			new MopEffectContainer()
			{
				component = new ApplyFear().GetType(),
				debuffs = new List<GameActorEffect>(){ },
				Key = "poop",
				projectileColor = new Color32(123, 92, 0, 255),
				goopNames = new List<string>()
				{
					"poopulongoop"
				}
			},
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){ DebuffLibrary.Possessed },
				Key = "possessed",
				projectileColor =  new Color32(255, 188, 76, 255),
				goopNames = new List<string>()
				{
					DebuffLibrary.PossesedPuddle.name.ToLower()
				}
			},
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){ DebuffLibrary.Frailty },
				Key = "frailty",
				projectileColor =  new Color32(136, 25, 149, 255),
				goopNames = new List<string>()
				{
					DebuffLibrary.FrailPuddle.name.ToLower()
				}
			},
			new MopEffectContainer()
			{
				component = null,
				debuffs = new List<GameActorEffect>(){ DebuffLibrary.Corrosion },
				Key = "tarnish",
				projectileColor = new Color32(157, 147, 0, 255),
				goopNames = new List<string>()
				{
					DebuffLibrary.TarnishedGoop.name.ToLower()
				}
			},
		};
	}
}
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
using BreakAbleAPI;

using UnityEngine.Serialization;
using Alexandria.Assetbundle;

namespace Planetside
{
	public class Polarity : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Polarity", "polarity");
			Game.Items.Rename("outdated_gun_mods:polarity", "psog:polarity");
			gun.gameObject.AddComponent<Polarity>();
			GunExt.SetShortDescription(gun, "Climate Contrast");
			GunExt.SetLongDescription(gun, "A weapon forged by two wanderers from polar-opposite climates.\n\nThrough smart refridgeration technology, the cool and hot parts are prevented from damaging each other.");

			GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "polarity_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.idleAnimation = "polarity_idle";
            gun.shootAnimation = "polarity_fire";
            gun.reloadAnimation = "polarity_reload";

            //GunExt.SetupSprite(gun, null, "polarity_idle_001", 8);
            //GunExt.SetAnimationFPS(gun, gun.shootAnimation, 160);
            //GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 10);
            //GunExt.SetAnimationFPS(gun, gun.idleAnimation, 4);


            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(223) as Gun, true, false);
			gun.SetBaseMaxAmmo(450);
			
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_Yari";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

			EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> { { 0, "Play_BOSS_tank_grenade_01" }, { 6, "Play_OBJ_lock_unlock_01" }, { 13, "Play_OBJ_lock_unlock_01" } });

			Gun gun2 = PickupObjectDatabase.GetById(402) as Gun;
			Gun gun3 = PickupObjectDatabase.GetById(336) as Gun;
			gun.DefaultModule.customAmmoType = gun2.DefaultModule.customAmmoType;
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(16) as Gun).gunSwitchGroup;



            Projectile replacementProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(336) as Gun).DefaultModule.projectiles[0]);
            FakePrefab.MarkAsFakePrefab(replacementProjectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(replacementProjectile); 
			replacementProjectile.baseData.damage = 8;
            gun.DefaultModule.usesOptionalFinalProjectile = true;
			PolarityProjectile pol1 = replacementProjectile.gameObject.AddComponent<PolarityProjectile>();
			pol1.IsUp = true;


			gun.DefaultModule.numberOfFinalProjectiles = 15;
			gun.DefaultModule.finalProjectile = replacementProjectile;
			gun.DefaultModule.finalCustomAmmoType = gun3.DefaultModule.customAmmoType;
			gun.DefaultModule.finalAmmoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.reloadTime = 1.6f;
			gun.DefaultModule.cooldownTime = 0.05f;
			gun.DefaultModule.numberOfShotsInClip = 30;
			gun.DefaultModule.angleVariance = 4f;
			gun.barrelOffset.transform.localPosition += new Vector3(2.25f, 0.0625f, 0f);
			gun.quality = PickupObject.ItemQuality.S;
			gun.encounterTrackable.EncounterGuid = "opposites attract";
			gun.gunClass = GunClass.RIFLE;
			gun.CanBeDropped = true;
			Gun gun4 = PickupObjectDatabase.GetById(387) as Gun;
			gun.muzzleFlashEffects = gun4.muzzleFlashEffects;
			Gun gun5 = PickupObjectDatabase.GetById(384) as Gun;
			gun.finalMuzzleFlashEffects = gun5.muzzleFlashEffects;
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]); projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.transform.parent = gun.barrelOffset;
			projectile.AdditionalScaleMultiplier *= 1.33f;
			projectile.baseData.damage = 8f;
			PolarityProjectile aaaaaaa = projectile.gameObject.AddComponent<PolarityProjectile>();
			aaaaaaa.IsUp = false;
			Polarity.PolarityID = gun.PickupObjectId;

			List<string> mandatoryConsoleIDs1 = new List<string>
			{
				"psog:polarity"
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"hot_lead",
				"copper_ammolet",
				"napalm_strike",
				"ring_of_fire_resistance",
				"flame_hand",
				"phoenix",
				"pitchfork",
				"frost_bullets",
				"frost_ammolet",
				"heart_of_ice",
				"ice_cube",
				"ice_bomb",
				"glacier"
			};
			CustomSynergies.Add("Refridgeration", mandatoryConsoleIDs1, optionalConsoleIDs, false);
			ItemIDs.AddToList(gun.PickupObjectId);

			string[] clipPaths = new string[]
			{
				"Planetside/Resources/GunClips/Polarity/polBlueClip.png",
				"Planetside/Resources/GunClips/Polarity/polRedClip.png",
				"Planetside/Resources/GunClips/Polarity/polSynClip.png",
			};
			DebrisObject ClipBlue = BreakableAPIToolbox.GenerateDebrisObject(clipPaths[0], true, 3f, 3, 150, 60, null, 4f, null, null, 1, false);
			DebrisObject ClipRed = BreakableAPIToolbox.GenerateDebrisObject(clipPaths[1], true, 2f, 4, 150, 60, null, 6f, null, null, 1, false);
			
			DebrisObject ClipBlue1 = BreakableAPIToolbox.GenerateDebrisObject(clipPaths[0], true, 3f, 3, 120, 60, null, 2f, null, null, 0, false);
			DebrisObject ClipRed1 = BreakableAPIToolbox.GenerateDebrisObject(clipPaths[1], true, 2f, 4, 120, 60, null, 3f, null, null, 1, false);
			
			DebrisObject ClipBlue2 = BreakableAPIToolbox.GenerateDebrisObject(clipPaths[0], true, 3f, 3, 200, 100, null, 3f, null, null, 1, false);
			DebrisObject ClipRed2 = BreakableAPIToolbox.GenerateDebrisObject(clipPaths[1], true, 2f, 4, 200, 100, null, 4f, null, null, 0, false);
			
			ShardCluster ClipsBlueCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { ClipBlue, ClipBlue1, ClipBlue2 }, 0.5f, 2f, 1, 1, 1f);
			ShardCluster ClipsRedCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { ClipRed, ClipRed1, ClipRed2 }, 0.6f, 1.5f, 1, 1, 1f);

			DebrisObject ClipSyn1 = BreakableAPIToolbox.GenerateDebrisObject(clipPaths[2], true, 3f, 3, 90, 50, null, 4f, null, null, 1, false);
			DebrisObject ClipSyn2 = BreakableAPIToolbox.GenerateDebrisObject(clipPaths[2], true, 2f, 4, 180, 90, null, 6f, null, null, 1, false);
			ShardCluster ClipsSynergyCluster1 = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { ClipSyn1, ClipSyn2 }, 0.45f, 1.4f, 1, 1, 1f);
			ShardCluster ClipsSynergyCluster2 = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { ClipSyn1, ClipSyn2 }, 0.6f, 1.7f, 1, 1, 1f);


			ClipCluster = new ShardCluster[] { ClipsBlueCluster, ClipsRedCluster };
			ClipClusterSynergy = new ShardCluster[] { ClipsSynergyCluster1, ClipsSynergyCluster2 };

		}
		public static int PolarityID;

		private static ShardCluster[] ClipCluster;
		private static ShardCluster[] ClipClusterSynergy;


		private bool HasReloaded;

	
		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
				if (GameManager.Options.DebrisQuantity == GameOptions.GenericHighMedLowOption.VERY_LOW)
				{
					return;
				}
				if (gun.sprite == null || gun.transform == null)
				{
					Debug.LogError("shit");
					return;
				}

				ShardCluster[] clusterToUse = player.PlayerHasActiveSynergy("Refridgeration") == true ? ClipClusterSynergy : ClipCluster;

				Vector3 position = player.primaryHand.attachPoint.PositionVector2() + MathToolbox.GetUnitOnCircle(gun.CurrentAngle + 225, 1.25f);
				GameObject PoofVFX = GameManager.Instance.RewardManager.D_Chest.VFX_PreSpawn;
				PoofVFX.SetActive(true);
				GameObject poofObj = UnityEngine.Object.Instantiate<GameObject>(PoofVFX, position, Quaternion.identity);
				poofObj.GetComponent<tk2dSpriteAnimator>().transform.localRotation = gun.transform.rotation;

				if (clusterToUse != null && clusterToUse.Length > 0)
				{
					int num = UnityEngine.Random.Range(0, 10);
					for (int i = 0; i < clusterToUse.Length; i++)
					{
						ShardCluster shardCluster = clusterToUse[i];
						int num2 = UnityEngine.Random.Range(shardCluster.minFromCluster, shardCluster.maxFromCluster + 1);
						int num3 = UnityEngine.Random.Range(0, shardCluster.clusterObjects.Length);
						for (int j = 0; j < num2; j++)
						{
							float lowDiscrepancyRandom = BraveMathCollege.GetLowDiscrepancyRandom(num);
							num++;
							float z = Mathf.Lerp(180, -180, lowDiscrepancyRandom);
							Vector3 vector = Quaternion.Euler(0f, 0f, z) * (gun.transform.PositionVector2().normalized * UnityEngine.Random.Range(-45, 45)).ToVector3ZUp(2);
							int num4 = (num3 + j) % shardCluster.clusterObjects.Length;
							GameObject gameObject = SpawnManager.SpawnDebris(shardCluster.clusterObjects[num4].gameObject, position, Quaternion.identity);
							tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
							if (gun.sprite.attachParent != null && component != null)
							{
								component.attachParent = gun.sprite.attachParent;
								component.HeightOffGround = gun.sprite.HeightOffGround;
							}
							DebrisObject component2 = gameObject.GetComponent<DebrisObject>();
							vector = Vector3.Scale(vector, shardCluster.forceAxialMultiplier) * shardCluster.forceMultiplier;
							component2.Trigger(vector, 1, shardCluster.rotationMultiplier);
						}
					}
				}
			}
			this.HalfOFClip(player);
		}
		public class EyeProjUp : MonoBehaviour
		{
		}

		public class EyeProjDown : MonoBehaviour
		{
		}
		public override void Update()
		{
			if (gun.CurrentOwner)
			{
				PlayerController player = gun.CurrentOwner as PlayerController;
				this.HalfOFClip(player);
				this.gun.PreventNormalFireAudio = true;
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
			}
		}
		public override void OnInitializedWithOwner(GameActor actor)
		{
			base.OnInitializedWithOwner(actor);
			PlayerController player = actor as PlayerController;
			this.HalfOFClip(player);
		}
		public void HalfOFClip(PlayerController player)
		{
			float clip = (player.stats.GetStatValue(PlayerStats.StatType.AdditionalClipCapacityMultiplier));
			int num = (int)(30 * clip);
			int num2 = (int)(num / 2); ;
			this.gun.DefaultModule.numberOfFinalProjectiles = num2;
		}
	}
}

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
using Brave.BulletScript;

namespace Planetside
{
	public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	public class VengefulShell : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Vengeful Shell";
			string resourceName = "Planetside/Resources/vengefulshell.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<VengefulShell>();
			ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
			string shortDesc = "I'll See You Again";
			string longDesc = "Bullets are birthed twice.\n\nA singular casing thats seeking vengeance against the bullet that abandoned it long ago, leaving it alone in this world.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");

			Gun gun4 = PickupObjectDatabase.GetById(43) as Gun;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun4.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			projectile.baseData.damage = 13f;
			projectile.baseData.speed *= 1f;
			projectile.AdditionalScaleMultiplier = 1f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.baseData.range = 30f;
			projectile.SetProjectileSpriteRight("vengbullet", 8, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 8, 5);

			OtherTools.EasyTrailComponent trail = projectile.gameObject.AddComponent<OtherTools.EasyTrailComponent>();

			trail.TrailPos = projectile.transform.position;
			trail.StartWidth = 0.125f;
			trail.EndWidth = 0;
			trail.LifeTime = 0.2f;
			trail.BaseColor = new Color(1f, 0f, 0f, 0.6f);
			trail.StartColor = new Color(1f, 1f, 0f, 0.6f);
			trail.EndColor = new Color(0.1f, 0f, 0f, 0f);


			VengefulProjectile = projectile;
			item.quality = PickupObject.ItemQuality.EXCLUDED;
			VengefulShell.VengefulShellID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);
			
			/*
			var HookToWriteLogToTxtFile2 = new Hook(typeof(Bullet).GetMethods().Single(
				  m =>
					  m.Name == "Fire" &&
					  m.GetParameters().Length == 4 &&
					  m.GetParameters()[0].ParameterType == typeof(Offset)),
				  typeof(VengefulShell).GetMethod("FireVengefulBullet", BindingFlags.Static | BindingFlags.Public));
			*/
		}


		private static int StoredAmmo;

		protected override void Update()
        {
			if (base.Owner && base.Owner.IsInCombat && Candofuckening == true)
            {
				ETGModConsole.Log(StoredAmmo.ToString());
				PlayerController player = base.Owner;
				if (StoredAmmo >= 10 && StoredAmmo >=0)
				{
					
					int BurstSize = 10;
					int Backwards = (BurstSize/2)-BurstSize;
					AkSoundEngine.PostEvent("Play_ENM_bulletking_wine_01", player.gameObject);
					for (int i = Backwards; i < (BurstSize/2) + 1; i++)
					{
						float finaldir = ProjSpawnHelper.GetAccuracyAngled(player.CurrentGun.CurrentAngle, 0, player);
						GameObject prefab = VengefulShell.VengefulProjectile.gameObject;
						GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(prefab, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (6f * i) + finaldir), true);
						Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
						if (component != null)
						{
							component.baseData.speed = 15;
							component.Owner = player;
							component.Shooter = player.specRigidbody;
							SpriteOutlineManager.AddOutlineToSprite(component.sprite, Color.black);
						}
					}
					GameManager.Instance.StartCoroutine(this.Cooldown());
					StoredAmmo -= 10;
				}
				else if (StoredAmmo >= 0)
				{

					int BurstSize = StoredAmmo;
					int Backwards = BurstSize - BurstSize * 2;
					AkSoundEngine.PostEvent("Play_ENM_bulletking_wine_01", player.gameObject);
					for (int i = Backwards; i < BurstSize + 1; i++)
					{
						float finaldir = ProjSpawnHelper.GetAccuracyAngled(player.CurrentGun.CurrentAngle, 0, player);
						GameObject prefab = VengefulShell.VengefulProjectile.gameObject;
						GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(prefab, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (6f * i) + finaldir), true);
						Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
						if (component != null)
						{
							component.baseData.speed = 15;
							component.Owner = player;
							component.Shooter = player.specRigidbody;
							SpriteOutlineManager.AddOutlineToSprite(component.sprite, Color.black);
						}
					}
					GameManager.Instance.StartCoroutine(this.Cooldown());
					StoredAmmo -= StoredAmmo;
				}
			}
        }

		private bool Candofuckening = true;
		private IEnumerator Cooldown()
        {
			Candofuckening = false;
			yield return new WaitForSeconds(1);
			Candofuckening = true;
			yield break;
        }


		public static void FireVengefulBullet(Action<Bullet, Offset ,Direction, Speed, Bullet> orig, Bullet self, Offset offset = null, Direction direction = null, Speed speed = null, Bullet bullet = null)
        {
			orig(self, offset, direction, speed, bullet);
			for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
			{
				PlayerController player = GameManager.Instance.AllPlayers[i];
				if (player != null && player.HasPickupID(VengefulShell.VengefulShellID))
				{
				   StoredAmmo += 1;
				}
				/*
				bool Boss = self.BulletBank.aiActor.healthHaver.IsBoss || self.BulletBank.aiActor.healthHaver.IsSubboss;
				if (Boss && UnityEngine.Random.value >= 0.10f)
				{
					PlayerController player = GameManager.Instance.AllPlayers[i];
					if (player != null && player.HasPickupID(VengefulShell.VengefulShellID))
					{
						float finaldir = ProjSpawnHelper.GetAccuracyAngled(player.CurrentGun.CurrentAngle, 6, player);
						GameObject prefab = VengefulShell.VengefulProjectile.gameObject;
						GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(prefab, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, direction != null ? direction.direction + finaldir : finaldir), true);
						Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
						if (component != null)
						{
							component.baseData.speed = 11;
							component.Owner = player;
							component.Shooter = player.specRigidbody;
							SpriteOutlineManager.AddOutlineToSprite(component.sprite, Color.black);
						}
					}
				}
				else
                {
					PlayerController player = GameManager.Instance.AllPlayers[i];
					if (player != null && player.HasPickupID(VengefulShell.VengefulShellID))
					{
						float finaldir = ProjSpawnHelper.GetAccuracyAngled(player.CurrentGun.CurrentAngle, 6, player);
						GameObject prefab = VengefulShell.VengefulProjectile.gameObject;
						GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(prefab, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, direction != null ? direction.direction + finaldir : finaldir), true);
						Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
						if (component != null)
						{
							component.baseData.speed = 11;
							component.Owner = player;
							component.Shooter = player.specRigidbody;
							SpriteOutlineManager.AddOutlineToSprite(component.sprite, Color.black);
						}
					}
				}
				*/
			}
		}
		public static Projectile VengefulProjectile;
		public static int VengefulShellID;

		public static void AddProjectile(Projectile p)
		{
			if (p.Owner is AIActor)
            {
				for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
				{
					PlayerController player = GameManager.Instance.AllPlayers[i];
					if (player != null && player.HasPickupID(VengefulShell.VengefulShellID))
					{
						StoredAmmo += 1;
						/*
						float finaldir = ProjSpawnHelper.GetAccuracyAngled(player.CurrentGun.CurrentAngle, 6, player);
						//AkSoundEngine.PostEvent("Play_ENM_bulletking_wine_01", player.gameObject);
						GameObject prefab = VengefulShell.VengefulProjectile.gameObject;
						GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(prefab, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, finaldir), true);
						Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
						if (component != null)
						{
							component.baseData.speed = 11;
							component.Owner = player;
							component.Shooter = player.specRigidbody;
							SpriteOutlineManager.AddOutlineToSprite(component.sprite, Color.black);
						}
						*/
					}
				}
			}
		}


		public override DebrisObject Drop(PlayerController player)
		{
			StaticReferenceManager.ProjectileAdded += AddProjectile;
			DebrisObject result = base.Drop(player);
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			StaticReferenceManager.ProjectileAdded += AddProjectile;
			base.Pickup(player);
		}

		protected override void OnDestroy()
		{
			StaticReferenceManager.ProjectileAdded += AddProjectile;
			if (base.Owner != null)
            {
				base.OnDestroy();
			}
		}
	}
}
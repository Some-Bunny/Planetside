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



namespace Planetside
{
	public class SporeBulletsComponent : MonoBehaviour
	{
		public void Start()
		{
			currentObject = this.GetComponent<Projectile>();
			if (currentObject)
			{
				currentObject.OnHitEnemy += HandleHit;
			}
		}
		private void HandleHit(Projectile projectile, SpeculativeRigidbody otherBody, bool fatal)
		{
			if (otherBody.aiActor != null && !otherBody.healthHaver.IsDead && otherBody.aiActor.behaviorSpeculator && !otherBody.aiActor.IsHarmlessEnemy)
			{
				if (base.GetComponent<PierceProjModifier>() != null)
				{
					if (base.GetComponent<PierceProjModifier>().penetration == 0)
					{ TransformToSticky(projectile, otherBody); }
				}
				else
				{ TransformToSticky(projectile, otherBody); }
			}
		}

		private void TransformToSticky(Projectile projectile, SpeculativeRigidbody otherBody)
		{
			projectile.DestroyMode = Projectile.ProjectileDestroyMode.DestroyComponent;
			projectile.sprite.spriteAnimator.Play("enlarge");
			objectToLookOutFor = projectile.gameObject;
			objectToLookOutFor.transform.parent = otherBody.transform;
			parent = otherBody.aiActor;
			player = projectile.Owner as PlayerController;
			GameManager.Instance.StartCoroutine(this.EnlargeTumors());
		}
		private IEnumerator EnlargeTumors()
		{
			if (objectToLookOutFor == null) { yield break; }
			Vector3 currentscale = objectToLookOutFor.transform.localScale;
			
			float elapsed = 0f;
			float duration = 2f;
			AkSoundEngine.PostEvent("Play_ENM_blobulord_charge_01", objectToLookOutFor);
			while (elapsed < duration)
			{
				if (objectToLookOutFor.gameObject == null) { break; }
				elapsed += BraveTime.DeltaTime;
				float t = elapsed / duration;
				float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
				objectToLookOutFor.transform.localScale = Vector3.Lerp(currentscale, currentscale * 2f, throne1);
				yield return null;
			}


			ExplosionData data = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
            data.effect = ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001") as GameObject;
			for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
			{
				PlayerController playerController = GameManager.Instance.AllPlayers[i];
				bool flag2 = playerController && playerController.specRigidbody;
				if (flag2)
				{
					data.ignoreList.Add(playerController.specRigidbody);
				}
			}
			data.damage = 9f * (player != null ? player.stats.GetStatValue(PlayerStats.StatType.Damage) : 1);
			data.damageRadius = 2.25f;
			data.doScreenShake = false;
			data.playDefaultSFX = false;
			data.force = 1;
			data.doDestroyProjectiles = false;
			if (objectToLookOutFor != null)
			{
				Exploder.Explode(objectToLookOutFor.transform.position, data, objectToLookOutFor.transform.position);
				AkSoundEngine.PostEvent("Play_ENM_mushroom_cloud_01", objectToLookOutFor);
				for (int i = 0; i < UnityEngine.Random.Range(2, 4); i++)
				{
					GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(SporeBullets.sporeProjectile.gameObject, objectToLookOutFor.transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-180, 180)), true);
					Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
					if (component != null)
					{
						component.Owner = player;
						component.Shooter = player.specRigidbody;
						component.specRigidbody.RegisterTemporaryCollisionException(parent.specRigidbody, 0.5f);
					}
				}
				Destroy(objectToLookOutFor);
			}
			yield break;
		}
		public PlayerController player;
		public Projectile currentObject;
		public GameObject objectToLookOutFor;
		public Material materialToCopy;
		public tk2dSprite objectSprite;
		public AIActor parent;
		public Gun ToCheckReloadFor;
	}
	public class SporeBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Spore Shot";
            //string resourceName = "Planetside/Resources/sporebullets.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SporeBullets>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("sporebullets"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Mushroom!";
            string longDesc = "Chance to fire out infective spores." +
                "\n\nThese rounds are covered in an infectious fungus, how did it even get to that point?\n\nHow does a fungus get to the point of spreading onto lead???";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");


			Gun gun4 = PickupObjectDatabase.GetById(43) as Gun;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun4.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			projectile.baseData.damage = 2f;
			projectile.baseData.speed = 1;
			projectile.AdditionalScaleMultiplier = 1f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.baseData.range = 1000f;


			BounceProjModifier bouncy = projectile.gameObject.AddComponent<BounceProjModifier>();
			bouncy.numberOfBounces = 1;
			bouncy.bouncesTrackEnemies = true;
			bouncy.bounceTrackRadius = 5;

			projectile.AnimateProjectile(new List<string> {
				"sporebullet_001",
				"sporebullet_002",
				"sporebullet_003",
				"sporebullet_004",
			}, 20, true, new List<IntVector2> {
				new IntVector2(6, 6),
				new IntVector2(6, 6),
				new IntVector2(6, 6),
				new IntVector2(6, 6),
			}, AnimateBullet.ConstructListOfSameValues(false, 8), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 8), AnimateBullet.ConstructListOfSameValues(true, 8), AnimateBullet.ConstructListOfSameValues(false, 8), AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 8));
			projectile.AnimateProjectile(new List<string> {
				"sporebulle_enlarget_001",
				"sporebulle_enlarget_002",
				"sporebulle_enlarget_003",
				"sporebulle_enlarget_004",
			}, 2, true, new List<IntVector2> {
				new IntVector2(6, 6),
				new IntVector2(6, 6),
				new IntVector2(6, 6),
				new IntVector2(6, 6),
			}, AnimateBullet.ConstructListOfSameValues(false, 8), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 8), AnimateBullet.ConstructListOfSameValues(true, 8), AnimateBullet.ConstructListOfSameValues(false, 8), AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 8), "enlarge");
			projectile.sprite.spriteAnimator.DefaultClipId = projectile.sprite.spriteAnimator.Library.GetClipIdByName("idle");
			HomingModifier HomingMod = projectile.gameObject.GetOrAddComponent<HomingModifier>();
			HomingMod.AngularVelocity = 60;
			HomingMod.HomingRadius = 6;
			projectile.gameObject.AddComponent<SporeBulletsComponent>();
			projectile.baseData.UsesCustomAccelerationCurve = true;
			projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0, 14, 3, 5);
			sporeProjectile = projectile;

			item.quality = PickupObject.ItemQuality.C;
		}

		public static Projectile sporeProjectile;

		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
				float procChance = 0.22f;
				procChance *= effectChanceScalar;
				SporeBulletsComponent cont = sourceProjectile.gameObject.GetComponent<SporeBulletsComponent>();
				if (UnityEngine.Random.value <= procChance && cont == null)
                {
					GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(sporeProjectile.gameObject, sourceProjectile.transform.position, Quaternion.Euler(0f, 0f, base.Owner.FacingDirection + UnityEngine.Random.Range(-10, 10)), true);
					Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
					if (component != null)
					{
						component.Owner = base.Owner;
						component.Shooter = base.Owner.specRigidbody;
					}
				}
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
			}
		}


		private void PostProcessBeam(BeamController obj)
		{

			try
			{
				float procChance = 0.22f;
				if (UnityEngine.Random.value <= procChance)
				{
					GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(sporeProjectile.gameObject, base.Owner.transform.position, Quaternion.Euler(0f, 0f, base.Owner.FacingDirection + UnityEngine.Random.Range(-10, 10)), true);
					Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
					if (component != null)
					{
						component.Owner = base.Owner;
						component.Shooter = base.Owner.specRigidbody;
					}
				}
				ETGModConsole.Log(obj.projectile.sprite.renderer.material.shader.ToString());

			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}

		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeam -= this.PostProcessBeam;

			return result;
		}
		protected override void OnDestroy()
		{
			if (Owner != null)
			{
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				base.Owner.PostProcessBeam += this.PostProcessBeam;
			}
			base.OnDestroy();
		}
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeam += this.PostProcessBeam;

		}

	}
}



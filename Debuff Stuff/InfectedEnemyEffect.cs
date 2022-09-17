using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;

using Brave.BulletScript;
using GungeonAPI;

namespace Planetside
{

	public class InfectedDeath : BraveBehaviour
    {
		public void Start()
        {
			base.aiActor.healthHaver.OnPreDeath += PreDeath;
		}
		private void PreDeath(Vector2 obj)
		{
			if (base.aiActor != null)
			{
				if (base.aiActor.bulletBank == null)
				{
					return;
				}
				else
				{
					if (base.aiActor.bulletBank.Bullets == null) 
					{ 
						base.aiActor.bulletBank.Bullets = new List<AIBulletBank.Entry>(); 
					}

					base.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
					base.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
				}
				AIBulletBank b = null;
				b = base.aiActor.GetComponent<AIBulletBank>() ?? base.aiActor.transform.Find("tempObjInfection").GetOrAddComponent<AIBulletBank>();
				if (b == null)
				{
					var bullet = base.aiActor.AddComponent<AIBulletBank>();

                    bullet.Bullets = new List<AIBulletBank.Entry>();


                    //bulletBank.FixedPlayerRigidbody = null;
                    //bulletBank.ActorName = "Toddy";
                    //bulletBank.transforms = new List<Transform>() { base.aiActor.transform };

                    b = bullet;
                    bullet.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
                    bullet.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);

                    /*
					GameObject ob = new GameObject();


                    AIBulletBank bulletBank = new AIBulletBank();
                    bulletBank.Bullets = new List<AIBulletBank.Entry>();


                    bulletBank.FixedPlayerRigidbody = null;
                    bulletBank.ActorName = "Toddy";
                    bulletBank.transforms = new List<Transform>() { ob.transform };
                    ob.AddComponent(bulletBank);


                    bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
                    bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);

                    b = bulletBank;

                    Destroy(ob, 3);
					*/
                }


                SpawnManager.SpawnBulletScript(base.aiActor, base.aiActor.sprite.WorldCenter, b, new CustomBulletScriptSelector(typeof(Splat)), StringTableManager.GetEnemiesString("#TRAP", -1));

				AkSoundEngine.PostEvent("Play_ENM_blobulord_bubble_01", base.aiActor.gameObject);
				GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.BlueSynergyPoofVFX, false);

				gameObject.transform.position = base.aiActor.sprite.WorldCenter;
				gameObject.transform.localScale = Vector3.one * 2;
				UnityEngine.Object.Destroy(gameObject, 2);
			}
		}

		

		public class Splat : Script
		{
			public static float ReturnBulletAmount(AIActor ai)
			{
				float HP = ai != null ? ai.healthHaver.GetMaxHealth() : 36;
				float Bullets = 1;
				for (int i = 0; i < HP; i++)
				{
					if (i % 12 == 0)
					{ Bullets++; }
				}
				return (int)Bullets;
			}
		

			public int AmountOFBullets;

			protected override IEnumerator Top()
			{
				for (int e = 0; e < (int)ReturnBulletAmount(base.BulletBank.aiActor); e++)
				{
					this.Fire(new Direction(UnityEngine.Random.Range(-180, 180), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(1f, 4), SpeedType.Absolute), new Spore());
				}
				yield break;
			}

			public class Spore : Bullet
			{
				public Spore() : base(UnityEngine.Random.value > 0.33f ? StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name : StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name, false, false, false) { }
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), UnityEngine.Random.Range(60, 150));
					yield return this.Wait(450);
					base.Vanish(false);
					yield break;
				}
			}
		}

	}

	public class InfectedEnemyEffect : GameActorFreezeEffect {

		public Color TintColorInfection= new Color(0.05f, 0.3f, 0.9f, 0.7f);

		public static void Init()
        {
			GeneratedInfectionCrystals = new List<GameObject>();
			GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob1", "Planetside/Resources/VFX/Infection/blob_1.png"));
			GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob2", "Planetside/Resources/VFX/Infection/blob_2.png"));
			GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob3", "Planetside/Resources/VFX/Infection/blob_3.png"));
			GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob4", "Planetside/Resources/VFX/Infection/blob_4.png"));
			GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob5", "Planetside/Resources/VFX/Infection/blob_5.png"));
			GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob6", "Planetside/Resources/VFX/Infection/blob_6.png"));
			GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob7", "Planetside/Resources/VFX/Infection/blob_7.png"));

		}
		public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
        {
			foreach (Tuple<GameObject, float> tuple in effectData.vfxObjects)
			{
				if (tuple.First != null)
				{
					tuple.First.GetComponent<tk2dSprite>().renderer.enabled = EnemyIsVisible(actor.aiActor);
				}
			}
			if (actor != null)
            {
				Elasped += BraveTime.DeltaTime;
				if (Elasped > 1)
                {
					Elasped = 0;
					if (EnemyIsVisible(actor.aiActor) == true)
                    {
						if (actor.GetComponent<AIBulletBank>() == null && actor.transform.Find("tempObjInfection").GetComponent<AIBulletBank>() == null) { return; }
                            SpawnManager.SpawnBulletScript(actor, actor.sprite.WorldCenter, actor.GetComponent<AIBulletBank>() ?? actor.transform.Find("tempObjInfection").GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(SplatWeak)), StringTableManager.GetEnemiesString("#TRAP", -1));
					}
				}
            }
		}

		public float Elasped;


        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
			if (actor.aiActor != null) 
			{
				if (this.FreezeCrystals.Count > 0)
				{
					if (effectData.vfxObjects == null)
					{
						effectData.vfxObjects = new List<Tuple<GameObject, float>>();
					}
					int num = this.crystalNum;
					if (effectData.actor && effectData.actor.specRigidbody && effectData.actor.specRigidbody.HitboxPixelCollider != null)
					{
						float num2 = effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.x * effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.y;
						num = Mathf.Max(this.crystalNum, (int)((float)this.crystalNum * (0.5f + num2 / 4f)));
					}
					for (int i = 0; i < num; i++)
					{
						GameObject prefab = BraveUtility.RandomElement<GameObject>(this.FreezeCrystals);
						Vector2 vector = actor.specRigidbody.HitboxPixelCollider.UnitCenter;
						Vector2 vector2 = BraveUtility.RandomVector2(-this.crystalVariation, this.crystalVariation);
						vector += vector2;
						float num3 = BraveMathCollege.QuantizeFloat(vector2.ToAngle(), 360f / (float)this.crystalRot);
						Quaternion rotation = Quaternion.Euler(0f, 0f, num3);
						GameObject gameObject = SpawnManager.SpawnVFX(prefab, vector, rotation, true);
						gameObject.transform.parent = actor.transform;
						tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
						if (component)
						{
							actor.sprite.AttachRenderer(component);
							component.HeightOffGround = 0.1f;
						}
						if (effectData.actor && effectData.actor.specRigidbody && effectData.actor.specRigidbody.HitboxPixelCollider != null)
						{
							Vector2 unitCenter = effectData.actor.specRigidbody.HitboxPixelCollider.UnitCenter;
							float num4 = (float)i * (360f / (float)num);
							Vector2 normalized = BraveMathCollege.DegreesToVector(num4, 1f).normalized;
							normalized.x *= effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.x / 2f;
							normalized.y *= effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.y / 2f;
							float magnitude = normalized.magnitude;
							Vector2 vector3 = unitCenter + normalized;
							vector3 += (unitCenter - vector3).normalized * (magnitude * UnityEngine.Random.Range(0.15f, 0.85f));
							gameObject.transform.position = vector3.ToVector3ZUp(0f);
							gameObject.transform.rotation = Quaternion.Euler(0f, 0f, num4);
						}
						effectData.vfxObjects.Add(Tuple.Create<GameObject, float>(gameObject, num3));
					}					
				}

				


				actor.RegisterOverrideColor(TintColorInfection, "Infection");
				actor.gameObject.AddComponent<InfectedDeath>();

				actorToTrack = actor.aiActor;

				if (actorToTrack.bulletBank == null)
				{
					AIBulletBank bulletBank = new AIBulletBank();
					bulletBank.Bullets = new List<AIBulletBank.Entry>();

                    bulletBank.FixedPlayerRigidbody = actorToTrack.specRigidbody;

                    bulletBank.ActorName = actorToTrack.name ?? "Toddy";

                    bulletBank.transforms = new List<Transform>() { actor.transform };
					//bulletBank.gameActor = actorToTrack;

                    actorToTrack.gameObject.AddComponent(bulletBank);

                    bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
                    bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);

                }
                else
                {

                    if (actorToTrack.bulletBank.Bullets == null) { actorToTrack.bulletBank.Bullets = new List<AIBulletBank.Entry>(); }

                    actorToTrack.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
					actorToTrack.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);

                }

                if (UnityEngine.Random.value > actor.aiActor.healthHaver.GetMaxHealth())
                {
					SpawnEnemyOnDeath spawnEnemy = actor.aiActor.gameObject.AddComponent<SpawnEnemyOnDeath>();
					spawnEnemy.deathType = OnDeathBehavior.DeathType.PreDeath;
					spawnEnemy.DoNormalReinforcement = false;
					spawnEnemy.spawnsCanDropLoot = false;
					spawnEnemy.spawnAnim = "idle";
					spawnEnemy.spawnRadius = 0.5f;
					spawnEnemy.minSpawnCount = 1;
					spawnEnemy.maxSpawnCount = 2;
					spawnEnemy.enemySelection = SpawnEnemyOnDeath.EnemySelection.Random;
					spawnEnemy.enemyGuidsToSpawn = new string[] { "unwilling", "unwilling", "unwilling" };
					spawnEnemy.spawnPosition = SpawnEnemyOnDeath.SpawnPosition.InsideRadius;
				}
			}
			base.OnEffectApplied(actor, effectData, partialAmount);

		}

		public bool EnemyIsVisible(AIActor enemyToCheck)
        {
			if (enemyToCheck == null) { return false; }
			if (enemyToCheck.sprite.renderer.enabled == false) { return false; }
			if (enemyToCheck.IsGone == true) { return false; }
			if (enemyToCheck.State == AIActor.ActorState.Awakening) { return false; }
			return true;
        }


		public AIActor actorToTrack;

     

		public class SplatWeak : Script
        {
			protected override IEnumerator Top()
			{
				for (int e = 0; e < 1; e++)
				{
					this.Fire(new Direction(UnityEngine.Random.Range(-180, 180), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(1f, 4), SpeedType.Absolute), new Spore());
				}
				yield break;
			}

			public class Spore : Bullet
			{
				public Spore() : base(UnityEngine.Random.value > 0.33f ? StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name : StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name, false, false, false) { }
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), UnityEngine.Random.Range(30, 120));
					yield return this.Wait(450);
					base.Vanish(false);
					yield break;
				}
			}
		}

		




		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            //base.OnEffectRemoved(actor, effectData);
        }
		public override bool IsFinished(GameActor actor, RuntimeGameActorEffectData effectData, float elapsedTime)
		{
			return false;
		}



		private static GameObject GenerateInfectionCrystalFromPath(string name, string spritePath)
        {
			GameObject vfxObj = ItemBuilder.AddSpriteToObject(name, spritePath, null);
			FakePrefab.MarkAsFakePrefab(vfxObj);
			UnityEngine.Object.DontDestroyOnLoad(vfxObj);
			tk2dSprite sprite = vfxObj.GetComponent<tk2dSprite>();
			sprite.usesOverrideMaterial = true;
			
			Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.mainTexture = sprite.renderer.material.mainTexture;
			mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
			mat.SetFloat("_EmissiveColorPower", 2f);
			mat.SetFloat("_EmissivePower", 35);
			sprite.renderer.material = mat;
			return vfxObj;
		}

		public static List<GameObject> GeneratedInfectionCrystals;
	}
}

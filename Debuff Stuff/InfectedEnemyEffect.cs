﻿using System;
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
using Alexandria;

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
				if (!InfectedEnemyEffect.InfectedEnemyBurstBlacklist.Contains(base.aiActor.EnemyGuid))
				{

                    SpawnManager.SpawnBulletScript(base.aiActor, base.aiActor.sprite.WorldCenter, ContainmentBreachController.InfectionBulletBank, new CustomBulletScriptSelector(typeof(Splat)), "Liminal Infection");

                    AkSoundEngine.PostEvent("Play_ENM_blobulord_bubble_01", base.aiActor.gameObject);
                    GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.BlueSynergyPoofVFX, false);

                    gameObject.transform.position = base.aiActor.sprite.WorldCenter;
                    gameObject.transform.localScale = Vector3.one * 2;
                    UnityEngine.Object.Destroy(gameObject, 2);
                }
			}
		}

		

		public class Splat : Script
		{
			public static float ReturnBulletAmount(AIActor ai)
			{
				float HP = ai != null ? ai.healthHaver.GetMaxHealth() : 60;
				float Bullets = 4;
				for (int i = 0; i < HP; i++)
				{
					if (i % 30 == 0)
					{ Bullets++; }
				}
				return (int)Bullets;
			}
		

			public int AmountOFBullets;

			public override IEnumerator Top()
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
				public override IEnumerator Top()
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

		public static List<GameObject> BuildVFX()
		{
            GeneratedInfectionCrystals = new List<GameObject>();
            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob1", "blob_1"));
            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob2", "blob_2"));
            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob3", "blob_3"));
            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob4", "blob_4"));
            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob5", "blob_5"));
            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob6", "blob_6"));
            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob7", "blob_7"));
            return GeneratedInfectionCrystals;
        }



        private static GameObject GenerateInfectionCrystalFromPath(string name, string fileName)
        {
            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle(name, debuffCollection.GetSpriteIdByName(fileName), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
			var sprite = BrokenArmorVFXObject.GetComponent<tk2dBaseSprite>();
            sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 2f);
            mat.SetFloat("_EmissivePower", 35);
            sprite.renderer.material = mat;
            return BrokenArmorVFXObject;
        }

        public static List<GameObject> GeneratedInfectionCrystals;


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
				if (Elasped > Duration)
                {
					if (actor.aiActor == null) { return; }
                    if (!InfectedEnemyEffect.InfectedEnemyParticleBlacklist.Contains(actor.aiActor.EnemyGuid))
					{
                        Elasped = 0;
                        if (EnemyIsVisible(actor.aiActor) == true)
                        {
                            if (actor.GetComponent<AIBulletBank>() == null && actor.transform?.Find("tempObjInfection")?.GetComponent<AIBulletBank>() == null) { return; }
                            SpawnManager.SpawnBulletScript(actor, actor.sprite.WorldCenter, ContainmentBreachController.InfectionBulletBank, new CustomBulletScriptSelector(typeof(SplatWeak)), "Liminal Infection");
                        }
                    }
                        
				}
            }
		}

		public float Elasped;
        public float Duration = 1;


        public static List<string> InfectedEnemyBurstBlacklist => new List<string>()
        {
            EnemyGuidDatabase.Entries["blobulon"],
            EnemyGuidDatabase.Entries["blobuloid"],
            EnemyGuidDatabase.Entries["poisbulon"],
            EnemyGuidDatabase.Entries["poisbuloid"],
            EnemyGuidDatabase.Entries["mountain_cube"],
            EnemyGuidDatabase.Entries["lead_cube"],
            EnemyGuidDatabase.Entries["flesh_cube"],
            EnemyGuidDatabase.Entries["spent"],
            EnemyGuidDatabase.Entries["bullat"],
            EnemyGuidDatabase.Entries["shotgat"],
            EnemyGuidDatabase.Entries["grenat"],
            EnemyGuidDatabase.Entries["spirat"],
            EnemyGuidDatabase.Entries["wall_mimic"],
            EnemyGuidDatabase.Entries["bullet_shark"],
            EnemyGuidDatabase.Entries["tarnisher"],
            EnemyGuidDatabase.Entries["grip_master"],
            EnemyGuidDatabase.Entries["great_bullet_shark"],
            EnemyGuidDatabase.Entries["mine_flayers_bell"],
            EnemyGuidDatabase.Entries["candle_guy"],
            EnemyGuidDatabase.Entries["fusebot"],
            EnemyGuidDatabase.Entries["mouser"],
            EnemyGuidDatabase.Entries["poopulons_corn"],
            EnemyGuidDatabase.Entries["chicken"],
            EnemyGuidDatabase.Entries["snake"],
            EnemyGuidDatabase.Entries["tiny_blobulord"],
            EnemyGuidDatabase.Entries["rat"],
            EnemyGuidDatabase.Entries["rat_candle"],

			EnemyGUIDs.Blobulin_GUID,
            EnemyGUIDs.Poisbulin_GUID,
        };

        public static List<string> InfectedEnemyParticleBlacklist => new List<string>()
        {
            EnemyGuidDatabase.Entries["blobulin"],
            EnemyGuidDatabase.Entries["poisbulin"],
            EnemyGuidDatabase.Entries["mountain_cube"],
            EnemyGuidDatabase.Entries["lead_cube"],
            EnemyGuidDatabase.Entries["flesh_cube"],
            EnemyGuidDatabase.Entries["misfire_beast"],
            EnemyGuidDatabase.Entries["killithid"],
            EnemyGuidDatabase.Entries["leadbulon"],
            EnemyGuidDatabase.Entries["wall_mimic"],
            EnemyGuidDatabase.Entries["gun_cultist"],
            EnemyGuidDatabase.Entries["rubber_kin"],
            EnemyGuidDatabase.Entries["pot_fairy"],
            EnemyGuidDatabase.Entries["musketball"],
            EnemyGuidDatabase.Entries["tarnisher"],
            EnemyGuidDatabase.Entries["grip_master"],
            EnemyGuidDatabase.Entries["beadie"],
            EnemyGuidDatabase.Entries["mine_flayers_claymore"],
            EnemyGuidDatabase.Entries["mine_flayers_bell"],
            EnemyGuidDatabase.Entries["summoned_treadnaughts_bullet_kin"],
            EnemyGuidDatabase.Entries["candle_guy"],
            EnemyGuidDatabase.Entries["fusebot"],
            EnemyGuidDatabase.Entries["mouser"],
            EnemyGuidDatabase.Entries["poopulons_corn"],
            EnemyGuidDatabase.Entries["chicken"],
            EnemyGuidDatabase.Entries["snake"],
            EnemyGuidDatabase.Entries["tiny_blobulord"],
            EnemyGuidDatabase.Entries["rat_candle"],
            EnemyGuidDatabase.Entries["rat"],

        };


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

                if (actor.bulletBank == null)
				{
                    AIBulletBank bulletBank = actor.gameObject.AddComponent<AIBulletBank>();
					bulletBank.Bullets = new List<AIBulletBank.Entry>();

                    //bulletBank.FixedPlayerRigidbody = actor.specRigidbody;

                    bulletBank.ActorName = actor.name ?? "Toddy";

                    bulletBank.transforms = new List<Transform>() { bulletBank.transform };

                    bulletBank.gameActor = actor;


                    bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
                    bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);

                }
                else
                {

                    if (actor.bulletBank.Bullets == null)
                    {
                        actor.bulletBank.Bullets = new List<AIBulletBank.Entry>(); }

                    actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
                    actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);

                }
				if (actor.aiActor.healthHaver.GetMaxHealth() > 20)
				{
                    if (UnityEngine.Random.value < (actor.aiActor.healthHaver.GetMaxHealth() / 300))
                    {
                        SpawnEnemyOnDeath spawnEnemy = actor.aiActor.gameObject.AddComponent<SpawnEnemyOnDeath>();
                        spawnEnemy.deathType = OnDeathBehavior.DeathType.PreDeath;
                        spawnEnemy.DoNormalReinforcement = false;
                        spawnEnemy.spawnsCanDropLoot = false;
                        spawnEnemy.spawnAnim = "idle";
                        spawnEnemy.spawnRadius = 0.35f;
                        spawnEnemy.minSpawnCount = 1;
                        spawnEnemy.maxSpawnCount = 2;
                        spawnEnemy.enemySelection = SpawnEnemyOnDeath.EnemySelection.Random;
                        spawnEnemy.enemyGuidsToSpawn = new string[] { "unwilling", "unwilling", "unwilling" };
                        spawnEnemy.spawnPosition = SpawnEnemyOnDeath.SpawnPosition.InsideRadius;
                    }
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
			public override IEnumerator Top()
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
				public override IEnumerator Top()
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

	}
}

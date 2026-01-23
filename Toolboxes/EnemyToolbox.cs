using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gungeon;
using ItemAPI;
using UnityEngine;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using Brave.BulletScript;

namespace Planetside
{

	
	internal static class EnemyToolbox
    {

        public static void SpawnBulletScript(GameActor owner, Vector2 pos, AIBulletBank sourceBulletBank, BulletScriptSelector bulletScript, string ownerName, SpeculativeRigidbody sourceRigidbody = null, Vector2? direction = null, bool collidesWithEnemies = false, Action<Bullet, Projectile> OnBulletCreated = null)
        {
            GameObject gameObject = new GameObject("Temp BulletScript Spawner");
            gameObject.transform.position = pos;

            AIBulletBank aibulletBank = gameObject.AddComponent<AIBulletBank>();
            aibulletBank.Bullets = new List<AIBulletBank.Entry>();

            for (int i = 0; i < sourceBulletBank.Bullets.Count; i++)
            {
                aibulletBank.Bullets.Add(new AIBulletBank.Entry(sourceBulletBank.Bullets[i]));
            }

            aibulletBank.useDefaultBulletIfMissing = sourceBulletBank.useDefaultBulletIfMissing;
            aibulletBank.transforms = new List<Transform>(sourceBulletBank.transforms);
            aibulletBank.PlayVfx = false;
            aibulletBank.PlayAudio = false;
            aibulletBank.CollidesWithEnemies = collidesWithEnemies;
            aibulletBank.gameActor = owner;

            if (owner is AIActor)
            {
                aibulletBank.aiActor = (owner as AIActor);
            }

            aibulletBank.ActorName = ownerName;
            if (OnBulletCreated != null)
            {
                aibulletBank.OnBulletSpawned += OnBulletCreated;
            }

            aibulletBank.SpecificRigidbodyException = sourceRigidbody;
            if (direction != null)
            {
                aibulletBank.FixedPlayerPosition = new Vector2?(pos + direction.Value.normalized * 5f);
            }

            BulletScriptSource bulletScriptSource = gameObject.AddComponent<BulletScriptSource>();
            bulletScriptSource.BulletManager = aibulletBank;
            bulletScriptSource.BulletScript = bulletScript;
            bulletScriptSource.Initialize();
            BulletSourceKiller bulletSourceKiller = gameObject.AddComponent<BulletSourceKiller>();
            bulletSourceKiller.BraveSource = bulletScriptSource;
        }

        public static void AddOffset(this tk2dSpriteDefinition tk2DSprite, Vector3 Offset)
        {
            tk2DSprite.position0 += Offset;
            tk2DSprite.position1 += Offset;
            tk2DSprite.position2 += Offset;
            tk2DSprite.position3 += Offset;
        }
        public static AIActor GetNearestEnemy(this RoomHandler room, Vector2 position, out float nearestDistance, List<AIActor> excludedActors, bool includeBosses = true, bool excludeDying = false)
        {
            AIActor aiactor = null;
            nearestDistance = float.MaxValue;
            List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            bool flag = activeEnemies == null;
            AIActor result;
            if (flag)
            {
                result = null;
            }
            else
            {
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    if (includeBosses || !activeEnemies[i].healthHaver.IsBoss)
                    {
                        if (!excludeDying || !activeEnemies[i].healthHaver.IsDead)
                        {
                            float num = Vector2.Distance(position, activeEnemies[i].CenterPosition);
                            bool flag4 = num < nearestDistance && !excludedActors.Contains(activeEnemies[i]);
                            if (flag4)
                            {
                                nearestDistance = num;
                                aiactor = activeEnemies[i];
                            }
                        }
                    }
                }
                result = aiactor;
            }
            return result;
        }


        public static void QuickAssetBundleSpriteSetup(AIActor actorObject, tk2dSpriteCollectionData bundleData, Material mat, bool changesSortingLayer = true)
		{
            Texture texture = mat.GetTexture("_MainTex");
            texture.filterMode = FilterMode.Point;
            
            mat.SetTexture("_MainTex", texture);
            bundleData.material = mat;
            bundleData.materials = new Material[]
            {
                mat,
            };
            bundleData.materialInsts = new Material[]
            {
                mat,
            };
            foreach (var c in bundleData.spriteDefinitions)
            {
                c.material = bundleData.materials[0];
                c.materialInst = bundleData.materials[0];
                c.materialId = 0;
            }
			if (changesSortingLayer == true && actorObject != null)
			{
                actorObject.gameObject.layer = 28;
                actorObject.sprite.SortingOrder = -2;
            }
        }


		public static void ApplyGlitter(this AIActor target)
		{
			int count = target.healthHaver.bodySprites.Count;
			List<tk2dBaseSprite> bodySprites = target.healthHaver.bodySprites;
			for (int i = 0; i < count; i++)
			{
				bodySprites[i].usesOverrideMaterial = true;
				MeshRenderer component = target.healthHaver.bodySprites[i].GetComponent<MeshRenderer>();
				Material[] sharedMaterials = component.sharedMaterials;
				Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
				Material material = UnityEngine.Object.Instantiate<Material>(target.renderer.material);
				material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
				sharedMaterials[sharedMaterials.Length - 1] = material;
				component.sharedMaterials = sharedMaterials;
				sharedMaterials[sharedMaterials.Length - 1].shader = ShaderCache.Acquire("Brave/Internal/GlitterPassAdditive");
			}
		}
		public static AIBeamShooter2 AddAIBeamShooter(AIActor enemy, Transform transform, string name, Projectile beamProjectile, ProjectileModule beamModule = null ,float angle = 0)
        {
			AIBeamShooter2 bholsterbeam1 = enemy.gameObject.AddComponent<AIBeamShooter2>();
            bholsterbeam1.beamTransform = transform;
            bholsterbeam1.beamModule = beamModule;
            bholsterbeam1.beamProjectile = beamProjectile;
            bholsterbeam1.firingEllipseCenter = transform.position;
            bholsterbeam1.name = name;
            bholsterbeam1.northAngleTolerance = angle;
            return bholsterbeam1;
        }

		public static AIBeamShooter AddAIBeamShooter1(AIActor enemy, Transform transform, string name, Projectile beamProjectile, ProjectileModule beamModule = null, float angle = 0)
		{
			AIBeamShooter bholsterbeam1 = enemy.gameObject.AddComponent<AIBeamShooter>();
			bholsterbeam1.beamTransform = transform;
			bholsterbeam1.beamModule = beamModule;
			bholsterbeam1.beamProjectile = beamProjectile.projectile;
			bholsterbeam1.firingEllipseCenter = transform.position;
			bholsterbeam1.name = name;
			bholsterbeam1.northAngleTolerance = angle;
			return bholsterbeam1;
		}

		public static DirectionalAnimation AddNewDirectionAnimation(AIAnimator animator, string Prefix, string[] animationNames, DirectionalAnimation.FlipType[] flipType, DirectionalAnimation.DirectionType directionType = DirectionalAnimation.DirectionType.Single)
        {
			DirectionalAnimation newDirectionalAnimation = new DirectionalAnimation
			{
				Type = directionType,
				Prefix = Prefix,
				AnimNames = animationNames,
				Flipped = flipType
			};
			AIAnimator.NamedDirectionalAnimation greg = new AIAnimator.NamedDirectionalAnimation
			{
				name = Prefix,
				anim = newDirectionalAnimation
			};

			if (animator.OtherAnimations == null){
				animator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>{greg};}
            animator.OtherAnimations.Add(greg); 
			return newDirectionalAnimation;
        }

        public static DirectionalAnimation AddNewDirectionAnimation(AIAnimator animator, string Prefix, string NamedDirectionPrefix, string[] animationNames, DirectionalAnimation.FlipType[] flipType, DirectionalAnimation.DirectionType directionType = DirectionalAnimation.DirectionType.Single)
        {
            DirectionalAnimation newDirectionalAnimation = new DirectionalAnimation
            {
                Type = directionType,
                Prefix = Prefix,
                AnimNames = animationNames,
                Flipped = flipType
            };
            AIAnimator.NamedDirectionalAnimation greg = new AIAnimator.NamedDirectionalAnimation
            {
                name = NamedDirectionPrefix,
                anim = newDirectionalAnimation
            };

            if (animator.OtherAnimations == null)
            {
                animator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation> { greg };
            }
            animator.OtherAnimations.Add(greg);
            return newDirectionalAnimation;
        }


        public static void AddEventTriggersToAnimation(tk2dSpriteAnimator animator, string animationName, Dictionary<int, string> frameAndEventName)
        {
			foreach (var value in frameAndEventName)
            {
				var clip = animator.GetClipByName(animationName);
				clip.frames[value.Key].eventInfo = value.Value;
				clip.frames[value.Key].triggerEvent = true;
			}
		}

		public static void AddOffsetToFrames(tk2dSpriteAnimator animator, string animationName, Dictionary<int, Vector3> Offset, List<int> idListfallback = null)
		{
            tk2dSpriteAnimationClip awakenClip = animator.GetClipByName(animationName);
			List<int> idsModified = idListfallback ?? new List<int>();
            foreach (var value in Offset)
			{
				int i = value.Key;
                var Value = value.Value;
                var s = awakenClip.frames[i];
                if (s != null)
                {
                    int id = s.spriteId;
					if (!idsModified.Contains(id))
					{
						idsModified.Add(id);
                        awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position0 += Value;
                        awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position1 += Value;
                        awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position2 += Value;
                        awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position3 += Value;
                    }
                }
            }
        }

		public static void AddSoundsToAnimationFrame(tk2dSpriteAnimator animator, string animationName, Dictionary<int, string> frameAndSoundName)//int frame, string soundName)
		{
			foreach (var value in frameAndSoundName)
			{
				animator.GetClipByName(animationName).frames[value.Key].eventAudio = value.Value;
				animator.GetClipByName(animationName).frames[value.Key].triggerEvent = true;
			}
		}
        public static void AddSoundsToAnimationFrame(tk2dSpriteAnimation animator, string animationName, Dictionary<int, string> frameAndSoundName)//int frame, string soundName)
        {
            foreach (var value in frameAndSoundName)
            {
                animator.GetClipByName(animationName).frames[value.Key].eventAudio = value.Value;
                animator.GetClipByName(animationName).frames[value.Key].triggerEvent = true;
            }
        }
        public static void AddEventTriggersToAnimation(tk2dSpriteAnimation animator, string animationName, Dictionary<int, string> frameAndEventName)
        {
            foreach (var value in frameAndEventName)
            {
                var clip = animator.GetClipByName(animationName);
                clip.frames[value.Key].eventInfo = value.Value;
                clip.frames[value.Key].triggerEvent = true;
            }
        }


        public static void MarkAnimationAsSpawn(tk2dSpriteAnimator animator, string animationName)//int frame, string soundName)
        {
            foreach (var value in animator.GetClipByName(animationName).frames)
            {
				value.finishedSpawning = false;
			}
        }

        public static void AddInvulnverabilityFramesToAnimation(tk2dSpriteAnimator animator, string animationName, Dictionary<int, bool> frameAndBool)//int frame, string soundName)
		{
			foreach (var value in frameAndBool)
			{
				animator.GetClipByName(animationName).frames[value.Key].invulnerableFrame = value.Value;
				animator.GetClipByName(animationName).frames[value.Key].triggerEvent = true;
			}
		}

		public static void DestroyUnnecessaryHandObjects(Transform transform)
        {
			foreach (Transform obj in transform.transform)
			{
				if (obj.name == "BulletSkeletonHand(Clone)")
                {
					UnityEngine.Object.Destroy(obj.gameObject);
                }
			}
		}

		public static void AddShadowToAIActor(AIActor actor,GameObject shadowObject ,Vector2 attachpoint, string name = "shadowPosition")
        {
			actor.HasShadow = true;
			actor.ShadowPrefab = shadowObject;
			GameObject shadowPoint = new GameObject(name);
			shadowPoint.transform.parent = actor.gameObject.transform;
			shadowPoint.transform.position = attachpoint;
			actor.ShadowParent = shadowPoint.transform;
		}
	

		public static GameObject GenerateShootPoint(GameObject attacher, Vector2 attachpoint, string name = "shootPoint")
        {
			GameObject shootpoint = new GameObject(name);
			shootpoint.transform.SetParent(attacher.transform);
			shootpoint.transform.position = attachpoint;
			return attacher.transform.Find(name).gameObject;
		}

		public static AIActor CreateNewBulletBankerEnemy(string guid, string DisplayName,int sizeX, int sizeY,List<int> IdleFrameKeys, List<int> DeathFrameKeys, List<int> AttackFrameKeys,Script bulletScript = null, float MovementSpeed = 2.5f, float HP = 14, float IdleFPS = 5f,float MovementFPS = 10f, float DeathFPS = 8f, float attackFPS = 6f)
        {

			var collectionData = StaticSpriteDefinitions.Modder_Bullet_Sheet_Data;
			if (collectionData == null)
			{
				ETGModConsole.Log("Modder_Bullet_Sheet_Data is NULL");
				return null;
			}

            GameObject prefab = EnemyBuilder.BuildPrefabBundle(guid, guid, collectionData, IdleFrameKeys[0], new IntVector2(0, 0), new IntVector2(8, 9), false, false);
			StaticInformation.ModderBulletGUIDs.Add(guid);
			var companion = prefab.AddComponent<BulletEnemyBehavior>();
			companion.aiActor.knockbackDoer.weight = 800;
			companion.aiActor.MovementSpeed = MovementSpeed;
			companion.aiActor.healthHaver.PreventAllDamage = false;
			companion.aiActor.CollisionDamage = 1f;
			companion.aiActor.HasShadow = true;
			companion.aiActor.IgnoreForRoomClear = true;
			companion.aiActor.aiAnimator.HitReactChance = 0f;
			companion.aiActor.specRigidbody.CollideWithOthers = true;
			companion.aiActor.specRigidbody.CollideWithTileMap = true;
			companion.aiActor.PreventFallingInPitsEver = true;
			companion.aiActor.healthHaver.ForceSetCurrentHealth(HP);
			companion.aiActor.CollisionKnockbackStrength = 0f;
			companion.aiActor.procedurallyOutlined = false;
			companion.aiActor.CanTargetPlayers = true;
			companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
			companion.aiActor.healthHaver.SetHealthMaximum(HP, null, false);

            companion.aiActor.PreventFallingInPitsEver = true;
            companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;
            companion.aiActor.FallingProhibited = true;
            companion.aiActor.SetIsFlying(true, "I can fly", true, true);

			companion.aiActor.healthHaver.damageTypeModifiers = new List<DamageTypeModifier>();
            DamageTypeModifier fireRes = new DamageTypeModifier();
            fireRes.damageMultiplier = 0f;
            fireRes.damageType = CoreDamageTypes.Fire;
            companion.aiActor.healthHaver.damageTypeModifiers.Add(fireRes);

            var nur = companion.aiActor;
            nur.EffectResistances = new ActorEffectResistance[]
            {
                    new ActorEffectResistance()
                    {
                        resistAmount = 1,
                        resistType = EffectResistanceType.Fire
                    },
            };


            companion.aiActor.specRigidbody.PixelColliders.Clear();
			companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
			{
				ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
				CollisionLayer = CollisionLayer.EnemyCollider,
				IsTrigger = false,
				BagleUseFirstFrameOnly = false,
				SpecifyBagelFrame = string.Empty,
				BagelColliderNumber = 0,
				ManualOffsetX = 0,
				ManualOffsetY = 0,
				ManualWidth = sizeX,
				ManualHeight = sizeY,
				ManualDiameter = 0,
				ManualLeftX = 0,
				ManualLeftY = 0,
				ManualRightX = 0,
				ManualRightY = 0
			});
			companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
			{

				ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
				CollisionLayer = CollisionLayer.EnemyHitBox,
				IsTrigger = false,
				BagleUseFirstFrameOnly = false,
				SpecifyBagelFrame = string.Empty,
				BagelColliderNumber = 0,
				ManualOffsetX = 0,
				ManualOffsetY = 0,
				ManualWidth = sizeX,
				ManualHeight = sizeY,
				ManualDiameter = 0,
				ManualLeftX = 0,
				ManualLeftY = 0,
				ManualRightX = 0,
				ManualRightY = 0,



			});
			companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
			companion.aiActor.PreventBlackPhantom = false;
			AIAnimator aiAnimator = companion.aiAnimator;


			aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "die",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{

						   "die_left",
						   "die_right"

							}

						}
					}
				};
			aiAnimator.IdleAnimation = new DirectionalAnimation
			{
				Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
				Flipped = new DirectionalAnimation.FlipType[2],
				AnimNames = new string[]
				{
						"idle_left",
						"idle_right"
				}
			};
			aiAnimator.MoveAnimation = new DirectionalAnimation
			{
				Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
				Flipped = new DirectionalAnimation.FlipType[2],
				AnimNames = new string[]
				{
						"run_left",
						"run_right"
				}
			};



			EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "attack", new string[] { "attack_right", "attack_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);



			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, IdleFrameKeys, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = IdleFPS;
			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, IdleFrameKeys, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = IdleFPS;
			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, IdleFrameKeys, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = MovementFPS;
			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, IdleFrameKeys, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = MovementFPS;
			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, DeathFrameKeys, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = DeathFPS;
			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, DeathFrameKeys, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = DeathFPS;
			if (AttackFrameKeys != null)
            {
				SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, AttackFrameKeys, "attack_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = attackFPS;
				SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, AttackFrameKeys, "attack_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = attackFPS;
			}
			else
            {
				SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, IdleFrameKeys, "attack_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = attackFPS;
				SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, IdleFrameKeys, "attack_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = attackFPS;
			}

			EnemyToolbox.AddEventTriggersToAnimation(companion.GetComponent<tk2dSpriteAnimator>(), "attack_left", new Dictionary<int, string> { { 0, "tellCharge" } });
			EnemyToolbox.AddEventTriggersToAnimation(companion.GetComponent<tk2dSpriteAnimator>(), "attack_right", new Dictionary<int, string> { { 0, "tellCharge" } });

			var bs = prefab.GetComponent<BehaviorSpeculator>();
			prefab.GetComponent<ObjectVisibilityManager>();
			BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
			bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
			bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;


			GameObject shootpoint = new GameObject("baseShootpoint");
			shootpoint.transform.parent = companion.transform;
			shootpoint.transform.position = companion.sprite.WorldCenter;
			GameObject m_CachedGunAttachPoint = companion.transform.Find("baseShootpoint").gameObject;
			bs.TargetBehaviors = new List<TargetBehaviorBase>
			{
				new TargetPlayerBehavior
				{
					Radius = 35f,
					LineOfSight = true,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.25f,
				}
			};

			if (bulletScript != null)
            {
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(bulletScript.GetType()),
					LeadAmount = 0f,
					AttackCooldown = 5f,
					InitialCooldown = 1f,
					RequiresLineOfSight = true,
					StopDuring = ShootBehavior.StopType.Attack,
					Uninterruptible = true,
					ChargeAnimation = "attack"
				}
			};
			}
			bs.MovementBehaviors = new List<MovementBehaviorBase>
			{
				new SeekTargetBehavior
				{
					StopWhenInRange = true,
					CustomRange = 7f,
					LineOfSight = false,
					ReturnToSpawn = false,
					SpawnTetherDistance = 0f,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = 0f,
					MaxActiveRange = 0f
				}
			};
			bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
			bs.TickInterval = behaviorSpeculator.TickInterval;
			bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
			bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
			bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
			bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
			bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
			Game.Enemies.Add("psog:"+guid, companion.aiActor);


            bs.aiActor.sprite.usesOverrideMaterial = true;
            bs.aiActor.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            bs.aiActor.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            bs.aiActor.sprite.renderer.material.SetFloat("_EmissivePower", 25);
            bs.aiActor.sprite.renderer.material.SetFloat("_EmissiveColorPower", 5f);

            PlanetsideModule.Strings.Enemies.Set("#DisplayName"+ DisplayName, DisplayName);
			companion.aiActor.OverrideDisplayName = "#DisplayName" + DisplayName;
			companion.aiActor.ActorName = "#DisplayName" + DisplayName;
			companion.aiActor.name = "#DisplayName" + DisplayName;
			return companion.aiActor;
		}

		private class BulletEnemyBehavior : BraveBehaviour
		{

			private void Update()
			{
			}


			
			private void Start()
			{
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));			
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
				base.aiActor.healthHaver.OnPreDeath += (obj) =>{AkSoundEngine.PostEvent("Play_ENM_highpriest_blast_01", base.aiActor.gameObject);};
			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
            {
				if (clip.GetFrame(frameIdx).eventInfo.Contains("tellCharge"))
                {

					GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.MachoBraceDustupVFX, false);
					gameObject.transform.position = base.aiActor.transform.position - new Vector3(1.25f, 1.25f);
					tk2dSpriteAnimator component2 = gameObject.GetComponent<tk2dSpriteAnimator>();
					if (component2 != null)
					{
						component2.ignoreTimeScale = true;
						component2.AlwaysIgnoreTimeScale = true;
						component2.AnimateDuringBossIntros = true;
						component2.alwaysUpdateOffscreen = true;
						component2.playAutomatically = true;
					}
				}
			}
		}
	}
}

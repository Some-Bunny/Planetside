using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using Pathfinding;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Reflection;
using UnityEngine.UI;

namespace Planetside
{
	public class Revenant_Enemy : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "PSOG_Revenant";
		public static void Init()
		{
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {


                prefab = EnemyBuilder.BuildPrefabBundle("Revenant Enemy", guid, StaticSpriteDefinitions.EnemySpecific_Sheet_Data, 0, new IntVector2(0, 0), new IntVector2(0, 0), false);
                var companion = prefab.AddComponent<RevenantHide>();
                var animator = companion.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
                animator.library = StaticSpriteDefinitions.EnemySpecific_Animation_Data;

                companion.aiActor.knockbackDoer.weight = 15;
                companion.aiActor.MovementSpeed = 0f;
                companion.aiActor.healthHaver.PreventAllDamage = false;
                companion.aiActor.CollisionDamage = 0f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = true;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(125f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = false;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.healthHaver.AllDamageMultiplier = 0;
                companion.aiActor.CanDropCurrency = false;
                companion.aiActor.healthHaver.SuppressDeathSounds = true;
                companion.gameObject.GetOrAddComponent<TeleportationImmunity>();

                companion.aiActor.healthHaver.SetHealthMaximum(125f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();
                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 8,
                    ManualOffsetY = 1,
                    ManualWidth = 17,
                    ManualHeight = 50,
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
                    ManualOffsetX = 8,
                    ManualOffsetY = 1,
                    ManualWidth = 17,
                    ManualHeight = 50,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0,



                });
                companion.aiActor.PreventBlackPhantom = true;
                AIAnimator aiAnimator = companion.aiAnimator;
                aiAnimator.IdleAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
                    Flipped = new DirectionalAnimation.FlipType[2],
                    AnimNames = new string[]
                    {
                        "revenant_idle",
                        "revenant_idle"
                    }
                };






                var bs = prefab.GetComponent<BehaviorSpeculator>();
                var OvM = prefab.GetComponent<ObjectVisibilityManager>();

                BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
                bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
                bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
                var shootpoint = new GameObject("fuck");
                shootpoint.transform.parent = companion.transform;
                shootpoint.transform.position = companion.sprite.WorldCenter;
                GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;
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



                bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
                bs.TickInterval = behaviorSpeculator.TickInterval;
                bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
                bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
                bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
                bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
                bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
                Game.Enemies.Add("psog:revenant", companion.aiActor);


                Material matShader = new Material(Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit"));
                matShader.mainTexture = companion.sprite.renderer.material.mainTexture;
                matShader.SetFloat("_Fade", 0.2f);
                companion.sprite.renderer.material = matShader;

            }
        }







		public class RevenantHide : BraveBehaviour
		{
			private bool isFading = false;
			public void Update()
			{
				if (this.aiActor.parentRoom != null && this.aiActor.parentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear).Count() > 1 && isFading == false)
				{
					isFading = true;
                    this.StartCoroutine(DoDisappear());
                }
			}
			private IEnumerator DoDisappear()
			{
				float e = 0;
				while (e < 1)
				{
					e += Time.deltaTime * 1.5f;
					this.aiActor.sprite.renderer.material.SetFloat("_Fade", Mathf.Lerp(0.2f, 0, e));
					yield return null;
				}
				this.aiActor.EraseFromExistence();
				yield break;
			}
		}
	}
}






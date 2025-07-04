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
	public class TarnishEffect : GameActorHealthEffect
	{
		public static GameObject TarnishVFXObject;
		public Color TintColorFrailty = new Color(0.8f, 0.7f, 0f, 0.7f);
		public Color ClearUp = new Color(0f, 0f, 0f, 0f);


        public static GameObject BuildVFX()
        {
            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Broken Armor", debuffCollection.GetSpriteIdByName("TarnishEffect_idle_001"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            var clip = SpriteBuilder.AddAnimation(animator, debuffCollection, new List<int>()
            {
                debuffCollection.GetSpriteIdByName("TarnishEffect_start_001"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_start_002"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_start_003"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_start_003"),

                debuffCollection.GetSpriteIdByName("TarnishEffect_start_004"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_start_004"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_start_004"),

                debuffCollection.GetSpriteIdByName("TarnishEffect_start_005"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_start_005"),

                debuffCollection.GetSpriteIdByName("TarnishEffect_start_006"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_start_006"),

                debuffCollection.GetSpriteIdByName("TarnishEffect_start_007"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_start_008"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_start_009"),

                debuffCollection.GetSpriteIdByName("TarnishEffect_idle_001"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_idle_002"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_idle_003"),
                debuffCollection.GetSpriteIdByName("TarnishEffect_idle_004"),

            }, "start", tk2dSpriteAnimationClip.WrapMode.LoopSection, 9);

            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            clip.loopStart = 14;
            TarnishVFXObject = BrokenArmorVFXObject;
            return TarnishVFXObject;
        }

		public override void ApplyTint(GameActor actor)
        {
			actor.RegisterOverrideColor(TintColorFrailty, "Tarnish");
			for (int k = 0; k < 1; k++)
            {
				if (actor && actor.aiActor)
                {
					actor.specRigidbody.OnPreRigidbodyCollision += HandlePreCollision;
					actor.aiActor.MovementSpeed *= 0.7f;
					enemyCurrentlyTarnished = actor.aiActor;
					actor.healthHaver.OnPreDeath += OnPreDeath;
				}
			}
		}

		private AIActor enemyCurrentlyTarnished;
        private void OnPreDeath(Vector2 obj)
        {
			if (enemyCurrentlyTarnished != null)
            {
				DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(DebuffLibrary.TarnishedGoop).TimedAddGoopCircle(enemyCurrentlyTarnished.sprite.WorldCenter, 2.3f, 0.5f, false);
			}
		}

        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			bool flag = otherRigidbody && otherRigidbody.projectile;
			if (flag)
			{
				bool flag2 = otherRigidbody.projectile.Owner is PlayerController;
				if (flag2)
				{
					otherRigidbody.projectile.baseData.damage *= 1.15f;
					PierceProjModifier pierce = otherRigidbody.projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
					pierce.penetration++;
					MaintainDamageOnPierce maintain = otherRigidbody.projectile.gameObject.GetComponent<MaintainDamageOnPierce>();
					if (maintain == null) { MaintainDamageOnPierce mhm = otherRigidbody.projectile.gameObject.AddComponent<MaintainDamageOnPierce>();
						mhm.AmountOfPiercesBeforeFalloff = 1;
					}
					else
                    {
						maintain.AmountOfPiercesBeforeFalloff++;
					}
					float procChance = 0.2f;
					if (UnityEngine.Random.value <= procChance)
					{
						otherRigidbody.projectile.AdjustPlayerProjectileTint(new Color(0.6f, 0.7f, 0f), 2, 0f);
						otherRigidbody.projectile.AppliesPoison = true;
						otherRigidbody.projectile.PoisonApplyChance = 1f;
						otherRigidbody.projectile.healthEffect = DebuffLibrary.Corrosion;
					}
				}
			}
		}

		public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			if (this.AffectsEnemies && actor is AIActor && actor.healthHaver.IsBoss)
			{

				float DivideVal = actor.aiActor.healthHaver.IsBoss == true | actor.aiActor.healthHaver.IsSubboss == true ? 250 : 25;  
				float damageToDeal = 3 + actor.aiActor.healthHaver.GetMaxHealth() / DivideVal;
				actor.healthHaver.ApplyDamage(damageToDeal * BraveTime.DeltaTime  , Vector2.zero, "dissolve", CoreDamageTypes.Void, DamageCategory.Normal, false, null, false);
			}
		}



		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			if (actor && actor.aiActor)
            {
				actor.specRigidbody.OnPreRigidbodyCollision -= HandlePreCollision;
				actor.aiActor.MovementSpeed = actor.aiActor.BaseMovementSpeed;

				actor.DeregisterOverrideColor("Tarnish");
				base.OnEffectRemoved(actor, effectData);
				actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
			}
		
		}
	}
}



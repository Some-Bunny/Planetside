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
	public class ExecuteDebuff : GameActorHealthEffect
	{
		public static GameObject LockInVFXPrefab;
		public Color TintColorPosessed = new Color(0f, 1f, 0.1f, 0.5f);
		public Color ClearUp = new Color(0f, 0f, 0f, 0f);

		public static void Init()
        {
            var blessingObj = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/LockedIn/lockedin", null, false);
            FakePrefab.MarkAsFakePrefab(blessingObj);
            UnityEngine.Object.DontDestroyOnLoad(blessingObj);
            tk2dSpriteAnimator animator = blessingObj.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = blessingObj.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(blessingObj, ("Tarnish_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i < 9; i++)
            {
                tk2dSpriteCollectionData collection = DeathMarkcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/LockedIn/lockedin{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter);
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("idle");
            animator.playAutomatically = true;

            animator.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = animator.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(208, 255, 223, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 100);
            animator.sprite.renderer.material = mat;

            LockInVFXPrefab = blessingObj;

            data = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericLargeExplosion);
            data.breakSecretWalls = false;
            data.comprehensiveDelay = 0.15f;
            data.damage = 10;
            data.damageRadius = 4.33f;
            data.damageToPlayer = 0;
            data.debrisForce = 25;
            data.doDamage = true;
            data.pushRadius = 1;
            data.doDestroyProjectiles = false;
            data.doExplosionRing = false;
            data.doForce = true;
            data.doScreenShake = false;
            data.doStickyFriction = false;
            data.preventPlayerForce = true;
            data.force = 25;
            
            data.effect = (PickupObjectDatabase.GetById(328) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX;
        }

        private static ExplosionData data;

		public override void ApplyTint(GameActor actor)
		{
            var explodeOnHit = actor.gameObject.GetOrAddComponent<ExplodeOnHit>();
            explodeOnHit.actor = actor;
            actor.RegisterOverrideColor(TintColorPosessed, "Execute");
		
		}


        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			if (actor.gameObject.GetComponent<ExplodeOnHit>() == null)
            {
                effectData.elapsed = 10000;
            }
        }
		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			base.OnEffectRemoved(actor, effectData);
            if (actor.gameObject.GetComponent<ExplodeOnHit>() != null)
            {
                UnityEngine.Object.Destroy(actor.gameObject.GetComponent<ExplodeOnHit>());
            }
            actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}


        public class ExplodeOnHit : MonoBehaviour
        {
            public void Start()
            {
                if (actor != null)
                {
                    actor.healthHaver.OnDamaged += HealthHaver_OnDamaged;
                }
            }

            public void OnDestroy()
            {
                if (actor != null)
                {
                    actor.healthHaver.OnDamaged -= HealthHaver_OnDamaged;
                }
            }
           
            private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
            {
                actor.healthHaver.OnDamaged -= HealthHaver_OnDamaged;
                AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", actor.gameObject);

                ExplosionData dater = StaticExplosionDatas.CopyFields(ExecuteDebuff.data);
                dater.ignoreList = new List<SpeculativeRigidbody>()
                {
                    GameManager.Instance.PrimaryPlayer.specRigidbody
                };
                if (GameManager.Instance.SecondaryPlayer) { dater.ignoreList.Add(GameManager.Instance.SecondaryPlayer.specRigidbody); }

                dater.damage = Mathf.Min(100, Mathf.Max(2f, maxValue * 0.3f));
                dater.useDefaultExplosion = false;
                dater.playDefaultSFX = false;
                
                Exploder.Explode(actor.sprite.WorldCenter, dater, actor.transform.PositionVector2());
                Destroy(this);
            }

            public GameActor actor;
        }
	}
}

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


        public static GameObject BuildVFX()
        {
            /*
            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Execute", debuffCollection.GetSpriteIdByName("lockedin1"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);


            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            var clip = SpriteBuilder.AddAnimation(animator, debuffCollection, new List<int>()
            {
                debuffCollection.GetSpriteIdByName("lockedin_start_1"),
                debuffCollection.GetSpriteIdByName("lockedin_start_2"),
                debuffCollection.GetSpriteIdByName("lockedin_start_3"),
                debuffCollection.GetSpriteIdByName("lockedin_start_4"),

                debuffCollection.GetSpriteIdByName("lockedin1"),
                debuffCollection.GetSpriteIdByName("lockedin2"),
                debuffCollection.GetSpriteIdByName("lockedin3"),
                debuffCollection.GetSpriteIdByName("lockedin4"),
                debuffCollection.GetSpriteIdByName("lockedin5"),
                debuffCollection.GetSpriteIdByName("lockedin6"),
                debuffCollection.GetSpriteIdByName("lockedin7"),
                debuffCollection.GetSpriteIdByName("lockedin8"),
            }, "start", tk2dSpriteAnimationClip.WrapMode.LoopSection, 9);

            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            */

            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Execute VFX", debuffCollection.GetSpriteIdByName("lockedin1"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            var sprite = BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.VFX_Animation_Data;
            animator.DefaultClipId = animator.GetClipIdByName("execute_effect");
            animator.playAutomatically = true;
            sprite.usesOverrideMaterial = true;


            animator.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = animator.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 2f);
            mat.SetFloat("_EmissivePower", 20);
            animator.sprite.renderer.material = mat;

            LockInVFXPrefab = BrokenArmorVFXObject;
            return LockInVFXPrefab;
        }

		public static void Init()
        {


            data = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericLargeExplosion);
            data.breakSecretWalls = false;
            data.comprehensiveDelay = 0.15f;
            data.damage = 10;
            data.damageRadius = 4.5f;
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

                dater.damage = Mathf.Min(100, Mathf.Max(5f, maxValue * 0.5f));
                dater.useDefaultExplosion = false;
                dater.playDefaultSFX = false;
                
                Exploder.Explode(actor.sprite.WorldCenter, dater, actor.transform.PositionVector2());
                Destroy(this);
            }

            public GameActor actor;
        }
	}
}

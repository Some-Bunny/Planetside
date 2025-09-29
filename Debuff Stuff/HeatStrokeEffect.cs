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
	public class HeatStrokeEffect : GameActorHealthEffect
	{
		public static GameObject heatstrokeVFXObject;
		public float Time;


        public static GameObject BuildVFX()
        {
            /*
            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Heat Stroke", debuffCollection.GetSpriteIdByName("heatstrokeicon7"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            tk2dBaseSprite vfxSprite = BrokenArmorVFXObject.GetComponent<tk2dBaseSprite>();
            vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);

            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            var clip = SpriteBuilder.AddAnimation(animator, debuffCollection, new List<int>()
            {
                debuffCollection.GetSpriteIdByName("heatstrokeicon1"),
                debuffCollection.GetSpriteIdByName("heatstrokeicon1"),
                debuffCollection.GetSpriteIdByName("heatstrokeicon2"),
                debuffCollection.GetSpriteIdByName("heatstrokeicon3"),
                debuffCollection.GetSpriteIdByName("heatstrokeicon4"),
                debuffCollection.GetSpriteIdByName("heatstrokeicon5"),
                debuffCollection.GetSpriteIdByName("heatstrokeicon6"),
                debuffCollection.GetSpriteIdByName("heatstrokeicon7"),
            }, "start", tk2dSpriteAnimationClip.WrapMode.LoopSection, 5);

            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            clip.loopStart = 6;
            */

            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Heat Stroke", debuffCollection.GetSpriteIdByName("heatstrokeicon1"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            var sprite = BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.VFX_Animation_Data;
            animator.DefaultClipId = animator.GetClipIdByName("heatstroke_effect");
            animator.playAutomatically = true;
            sprite.usesOverrideMaterial = true;

            heatstrokeVFXObject = BrokenArmorVFXObject;
            return heatstrokeVFXObject;
        }

        public float customDuration = 1;
        private float HeatDuration = 0;

        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            HeatDuration = 0.1f;
            customDuration = 1;
            actor.specRigidbody.OnPreMovement += ModifyVelocity;
        }
        private float Power;
        public void ModifyVelocity(SpeculativeRigidbody myRigidbody)
        {

            myRigidbody.Velocity *= Mathf.Max(0.01f, (1 - Power));
        }


        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
            base.EffectTick(actor, effectData);
            if (customDuration > 0)
            {
                customDuration -= BraveTime.DeltaTime;
                HeatDuration += BraveTime.DeltaTime;
                if (actor.aiActor && !actor.aiActor.healthHaver.IsBoss)
                {
                    actor.behaviorSpeculator.CooldownScale += BraveTime.DeltaTime * 0.05f;
                }
            }
            else
            {
                HeatDuration -= BraveTime.DeltaTime * 4;
                if(actor.aiActor && !actor.aiActor.healthHaver.IsBoss)
                {
                    actor.behaviorSpeculator.CooldownScale -= BraveTime.DeltaTime * 0.2f;
                }
            }
            //Debug.Log($"{HeatDuration} | {customDuration}");

            if (HeatDuration <= 0)
            {
                //Debug.Log("DIE");
                actor.RemoveEffect(this);
                this.OnEffectRemoved(actor, effectData);
                return;
            }
            Power = HeatDuration * 0.001f;
            if (HeatDuration >= 7.5f && UnityEngine.Random.value < Power)
            {
                actor.behaviorSpeculator.Stun(0.1f, true);
            }
            if (HeatDuration >= 10f && UnityEngine.Random.value < Power)
            {
                actor.ApplyEffect(DebuffStatics.hotLeadEffect, 1f, null);
            }
            if (HeatDuration > 2.5f)
            {
                actor.healthHaver.ApplyDamage((HeatDuration - 2.5f) * BraveTime.DeltaTime, Vector2.zero, "THE SUN", CoreDamageTypes.Fire, DamageCategory.Normal, false, null, false);
            }
        }
        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = actor.sprite.WorldTopCenter,
                startSize = 5,
                rotation = 0,
                startLifetime = 0.3f,
                startColor = new Color(1, 0.6f, 0f, 0.2f)
            });
            actor.specRigidbody.OnPreMovement -= ModifyVelocity;
            base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}

        public override void OnDarkSoulsAccumulate(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1, Projectile sourceProjectile = null)
        {
            for (int i = 0; i < actor.m_activeEffects.Count; i++)
            {
                if (actor.m_activeEffects[i].effectIdentifier == effectIdentifier && actor.m_activeEffects[i] is HeatStrokeEffect heatStroke)
                {
                    heatStroke.customDuration += 0.35f;
                }
            }
            base.OnDarkSoulsAccumulate(actor, effectData, partialAmount, sourceProjectile);
        }


    }
}

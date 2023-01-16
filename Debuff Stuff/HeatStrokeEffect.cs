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
            var debuffCollection = StaticSpriteDefinitions.Debuff_Sheet_Data;
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
            heatstrokeVFXObject = BrokenArmorVFXObject;
            return heatstrokeVFXObject;
        }

       

        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
			//base.OnEffectRemoved(actor, effectData);
        }

		public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
	
		}
		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			if (this.AffectsEnemies && actor is AIActor)
            {
				BulletStatusEffectItem Firecomponent = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>();
				GameActorFireEffect gameActorFire = Firecomponent.FireModifierEffect;
				actor.ApplyEffect(gameActorFire, 1f, null);
                actor.healthHaver.ApplyDamage(2, Vector2.zero, "THE SUN", CoreDamageTypes.Fire, DamageCategory.Normal, false, null, false);
                actor.behaviorSpeculator.Stun(0.4f, true);
            }
            base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}
	}
}

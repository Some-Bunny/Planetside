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

		public static void Init()
        {
			heatstrokeVFXObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/heatstrokeicon", new GameObject("heatstrokeIcon"));
			heatstrokeVFXObject.SetActive(false);
			tk2dBaseSprite vfxSprite = heatstrokeVFXObject.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(heatstrokeVFXObject);
			UnityEngine.Object.DontDestroyOnLoad(heatstrokeVFXObject);
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

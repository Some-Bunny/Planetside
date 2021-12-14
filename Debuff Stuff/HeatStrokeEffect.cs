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
		public static string vfxNameheatstroke = "heatStrokeVFX";
		public static GameObject heatstrokeVFXObject;

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
			var hand = actor.transform.Find("heatStrokeVFX");
            if (hand == null)
			{
				GameObject original;
				original = HeatStrokeEffect.heatstrokeVFXObject;
				tk2dSprite component = GameObject.Instantiate(original, actor.specRigidbody.UnitTopCenter, Quaternion.identity, actor.transform).GetComponent<tk2dSprite>();
				component.transform.position.WithZ(component.transform.position.z + 99999);
				component.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(actor.CenterPosition, tk2dBaseSprite.Anchor.MiddleCenter);
				actor.sprite.AttachRenderer(component.GetComponent<tk2dBaseSprite>());
				component.name = HeatStrokeEffect.vfxNameheatstroke;
				component.PlaceAtPositionByAnchor(actor.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
				component.scale = Vector3.one;
			}
			else
            {
				base.OnEffectRemoved(actor, effectData);
			}
		}

		public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			if (this.AffectsEnemies && actor is AIActor)
			{
				float num = 1f;
				GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
				if (lastLoadedLevelDefinition != null)
				{
					num = lastLoadedLevelDefinition.enemyHealthMultiplier;
				}
				actor.healthHaver.ApplyDamage((4f*num) * BraveTime.DeltaTime, Vector2.zero, "THE SUN", CoreDamageTypes.Fire, DamageCategory.Normal, false, null, false);

			}
		}
		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			if (this.AffectsEnemies && actor is AIActor)
            {
				BulletStatusEffectItem Firecomponent = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>();
				GameActorFireEffect gameActorFire = Firecomponent.FireModifierEffect;
				actor.ApplyEffect(gameActorFire, 10f, null);
				actor.behaviorSpeculator.CooldownScale *= 1.2f;
				if (actor is AIActor && !actor.healthHaver.IsBoss)
				{
					actor.behaviorSpeculator.Stun(5f, true);
				}
			}
			//var hand = actor.transform.Find("heatStrokeVFX").gameObject;
			//UnityEngine.Object.Destroy(hand);
			base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}
	}
}

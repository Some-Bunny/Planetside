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
	public class FrailtyHealthEffect : GameActorHealthEffect
	{
		public static string vfxNamefrailty = "FrailtyVFX";
		public static GameObject frailtyVFXObject;
		public Color TintColorFrailty = new Color(2f, 0f, 2f, 0.75f);
		public Color ClearUp = new Color(0f, 0f, 0f, 0f);
		public static void Init()
        {
			frailtyVFXObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/frailtyeffecticon", new GameObject("FrailtyIcon"));
			frailtyVFXObject.SetActive(false);
			tk2dBaseSprite vfxSprite = frailtyVFXObject.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(frailtyVFXObject);
			UnityEngine.Object.DontDestroyOnLoad(frailtyVFXObject);
		}


		public override void ApplyTint(GameActor actor)
        {
			actor.RegisterOverrideColor(TintColorFrailty, vfxNamefrailty);
			for (int k = 0; k < 1; k++)
            {
				GameObject original;
				original = FrailtyHealthEffect.frailtyVFXObject;
				tk2dSprite component = GameObject.Instantiate(original, actor.specRigidbody.UnitTopCenter, Quaternion.identity, actor.transform).GetComponent<tk2dSprite>();
				component.transform.position.WithZ(component.transform.position.z + 99999);
				component.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(actor.CenterPosition, tk2dBaseSprite.Anchor.MiddleCenter);
				actor.sprite.AttachRenderer(component.GetComponent<tk2dBaseSprite>());
				component.name = FrailtyHealthEffect.vfxNamefrailty;
				component.PlaceAtPositionByAnchor(actor.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
				component.scale = Vector3.one;
			}
		}
        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			
			if (this.AffectsEnemies && actor is AIActor && !actor.healthHaver.IsBoss)
			{
				actor.healthHaver.SetHealthMaximum(actor.healthHaver.GetMaxHealth() * 1 - (0.80f*BraveTime.DeltaTime));
			}
			if (this.AffectsEnemies && actor is AIActor && actor.healthHaver.IsBoss)
			{
				actor.healthHaver.ApplyDamage(18f * BraveTime.DeltaTime, Vector2.zero, "oooo spoooooky", CoreDamageTypes.Void, DamageCategory.Normal, false, null, false);
			}
		}

		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			var hand = actor.transform.Find("FrailtyVFX").gameObject;
			UnityEngine.Object.Destroy(hand);

			actor.DeregisterOverrideColor(vfxNamefrailty);			
			base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}
	}
}



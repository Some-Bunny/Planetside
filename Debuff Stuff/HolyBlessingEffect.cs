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
	public class HolyBlessingEffect : GameActorHealthEffect
	{
		public static string vfxNameHoly = "HolyVFX";
		public static GameObject HolyVFXObject;
		public Color TintColorHoly = new Color(10f, 10f, 10f, 0.75f);
		public Color ClearUp = new Color(0f, 0f, 0f, 0f);
		public static void Init()
        {
			HolyVFXObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/holyvfx", new GameObject("HolyIcon"));
			HolyVFXObject.SetActive(false);
			tk2dBaseSprite vfxSprite = HolyVFXObject.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(HolyVFXObject);
			UnityEngine.Object.DontDestroyOnLoad(HolyVFXObject);
		}


		public override void ApplyTint(GameActor actor)
        {
			actor.RegisterOverrideColor(TintColorHoly, vfxNameHoly);
			for (int k = 0; k < 1; k++)
            {
				GameObject original;
				original = HolyBlessingEffect.HolyVFXObject;
				tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>(original, actor.transform).GetComponent<tk2dSprite>();
				component.name = HolyBlessingEffect.vfxNameHoly;
				component.PlaceAtPositionByAnchor(actor.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
				component.scale = Vector3.one;
			}
		}
        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			if (this.AffectsEnemies && actor is AIActor && actor.aiActor.IsBlackPhantom)
			{
				actor.healthHaver.ApplyDamage(25 * BraveTime.DeltaTime, Vector2.zero, "Lol!", CoreDamageTypes.Void, DamageCategory.Normal, false, null, false);
			}
		}

		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			var hand = actor.transform.Find("HolyVFX").gameObject;
			UnityEngine.Object.Destroy(hand);

			actor.DeregisterOverrideColor(vfxNameHoly);			
			base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}
	}
}



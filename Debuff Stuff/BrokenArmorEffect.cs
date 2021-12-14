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
	public class BrokenArmorEffect : GameActorHealthEffect
	{
		public static string VFXNameBrokenAArmor = "brokenArmorVFX";
		public static GameObject BrokenArmorVFXObject;

		public static void Init()
        {
			BrokenArmorVFXObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
			BrokenArmorVFXObject.SetActive(false);
			tk2dBaseSprite vfxSprite = BrokenArmorVFXObject.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
			UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
		}

		public override void ApplyTint(GameActor actor)
        {
			for (int k = 0; k < 1; k++)
            {
				GameObject original;
				original = BrokenArmorEffect.BrokenArmorVFXObject;
				tk2dSprite component = GameObject.Instantiate(original, actor.specRigidbody.UnitTopCenter, Quaternion.identity, actor.transform).GetComponent<tk2dSprite>();
				component.transform.position.WithZ(component.transform.position.z + 99999);
				component.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(actor.CenterPosition, tk2dBaseSprite.Anchor.MiddleCenter);
				actor.sprite.AttachRenderer(component.GetComponent<tk2dBaseSprite>());
				component.name = BrokenArmorEffect.VFXNameBrokenAArmor;

				component.PlaceAtPositionByAnchor(actor.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
				component.scale = Vector3.Lerp(component.scale, Vector3.one, 0.25f);
			}
		}
		public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1f)
		{
			base.OnEffectApplied(actor, effectData, partialAmount);
			actor.healthHaver.AllDamageMultiplier += 0.35f;
		}

		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			var hand = actor.transform.Find("brokenArmorVFX").gameObject;
			UnityEngine.Object.Destroy(hand);
			actor.healthHaver.AllDamageMultiplier -= 0.35f;

			base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}
	}
}



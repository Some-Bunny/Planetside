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
	// Token: 0x02000047 RID: 71
	public class PossessedEffect : GameActorHealthEffect
	{
		public static string vfxNameposessed = "PosessedVFX";
		public static GameObject posessedVFXObject;
		public Color TintColorPosessed = new Color(3f, 2f, 0f, 0.75f);
		public Color ClearUp = new Color(0f, 0f, 0f, 0f);

		public static void Init()
        {
			posessedVFXObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/possesedeffecticon", new GameObject("PosessedIcon"));
			posessedVFXObject.SetActive(false);
			tk2dBaseSprite vfxSprite = posessedVFXObject.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(posessedVFXObject);
			UnityEngine.Object.DontDestroyOnLoad(posessedVFXObject);
		}

		public override void ApplyTint(GameActor actor)
		{
			actor.RegisterOverrideColor(TintColorPosessed, vfxNameposessed);
			for (int k = 0; k < 1; k++)
			{
				GameObject original;
				original = PossessedEffect.posessedVFXObject;
				tk2dSprite component = GameObject.Instantiate(original, actor.specRigidbody.UnitTopCenter, Quaternion.identity, actor.transform).GetComponent<tk2dSprite>();
				component.transform.position.WithZ(component.transform.position.z + 99999);
				component.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(actor.CenterPosition, tk2dBaseSprite.Anchor.MiddleCenter);
				actor.sprite.AttachRenderer(component.GetComponent<tk2dBaseSprite>());
				component.name = PossessedEffect.vfxNameposessed;
				component.PlaceAtPositionByAnchor(actor.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
				component.scale = Vector3.one;
			}
		}
		public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			Vector2 centerPosition = actor.sprite.WorldCenter;
			if (activeEnemies != null)
			{
				foreach (AIActor aiactor in activeEnemies)
				{
					bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 5 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null;
					if (ae)
					{
						aiactor.healthHaver.ApplyDamage(3.75f* BraveTime.DeltaTime, Vector2.zero, "oooo spoooooky", CoreDamageTypes.Void, DamageCategory.Normal, false, null, false);
					}
				}
			}
		}
		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			var hand = actor.transform.Find("PosessedVFX").gameObject;
			UnityEngine.Object.Destroy(hand);
			base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}
	}
}

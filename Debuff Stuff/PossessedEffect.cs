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
	public class PossessedEffect : GameActorHealthEffect
	{
		public static string vfxNameposessed = "PosessedVFX";
		public static GameObject posessedVFXObject;
		public Color TintColorPosessed = new Color(3f, 2f, 0f, 0.75f);
		public Color ClearUp = new Color(0f, 0f, 0f, 0f);


        public static GameObject BuildVFX()
        {
            var debuffCollection = StaticSpriteDefinitions.Debuff_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Possessed", debuffCollection.GetSpriteIdByName("possesedeffecticon"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            posessedVFXObject = BrokenArmorVFXObject;
            return posessedVFXObject;
        }

    
		public override void ApplyTint(GameActor actor)
		{
			actor.RegisterOverrideColor(TintColorPosessed, vfxNameposessed);
		
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
						aiactor.healthHaver.ApplyDamage(4.5f* BraveTime.DeltaTime, Vector2.zero, "oooo spoooooky", CoreDamageTypes.Void, DamageCategory.Normal, false, null, false);
					}
				}
			}
		}
		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}
	}
}

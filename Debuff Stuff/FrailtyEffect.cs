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

        public static GameObject BuildVFX()
        {
            var debuffCollection = StaticSpriteDefinitions.Debuff_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Frailty", debuffCollection.GetSpriteIdByName("frailtyeffecticon6"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);

            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            var clip = SpriteBuilder.AddAnimation(animator, debuffCollection, new List<int>()
            {
                debuffCollection.GetSpriteIdByName("frailtyeffecticon1"),
                debuffCollection.GetSpriteIdByName("frailtyeffecticon2"),
                debuffCollection.GetSpriteIdByName("frailtyeffecticon3"),
                debuffCollection.GetSpriteIdByName("frailtyeffecticon4"),
                debuffCollection.GetSpriteIdByName("frailtyeffecticon5"),
                debuffCollection.GetSpriteIdByName("frailtyeffecticon6"),

            }, "start", tk2dSpriteAnimationClip.WrapMode.Once, 7);

            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            frailtyVFXObject = BrokenArmorVFXObject;
            return frailtyVFXObject;
        }


		public override void ApplyTint(GameActor actor)
        {
			actor.RegisterOverrideColor(TintColorFrailty, vfxNamefrailty);

		}
        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{			
			if (this.AffectsEnemies && actor is AIActor && !actor.healthHaver.IsBoss && actor.healthHaver.minimumHealth > actor.healthHaver.GetMaxHealth() * 1 - (0.80f * BraveTime.DeltaTime))
			{
				actor.healthHaver.SetHealthMaximum(actor.healthHaver.GetMaxHealth() * 1 - (0.80f*BraveTime.DeltaTime));			
			}
			if (this.AffectsEnemies && actor is AIActor && actor.healthHaver.IsBoss)
			{
				actor.healthHaver.ApplyDamage(32f * BraveTime.DeltaTime, Vector2.zero, "oooo spoooooky", CoreDamageTypes.Void, DamageCategory.Normal, false, null, false);
			}
		}
		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			actor.DeregisterOverrideColor(vfxNamefrailty);			
			base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}
	}
}



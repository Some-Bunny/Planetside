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
		
        public static GameObject BrokenArmorVFX;

        public static GameObject BuildVFX()
        {
            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Broken Armor", debuffCollection.GetSpriteIdByName("brokenarmor1"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            var clip = SpriteBuilder.AddAnimation(animator, debuffCollection, new List<int>()
            {
                debuffCollection.GetSpriteIdByName("brokenarmor_start_1"),
                debuffCollection.GetSpriteIdByName("brokenarmor_start_2"),
                debuffCollection.GetSpriteIdByName("brokenarmor_start_2"),
                debuffCollection.GetSpriteIdByName("brokenarmor_start_3"),
                debuffCollection.GetSpriteIdByName("brokenarmor_start_3"),
                debuffCollection.GetSpriteIdByName("brokenarmor1"),
                debuffCollection.GetSpriteIdByName("brokenarmor1"),
                debuffCollection.GetSpriteIdByName("brokenarmor1"),
                debuffCollection.GetSpriteIdByName("brokenarmor1"),
                debuffCollection.GetSpriteIdByName("brokenarmor2"),
                debuffCollection.GetSpriteIdByName("brokenarmor2"),
                debuffCollection.GetSpriteIdByName("brokenarmor3"),
                debuffCollection.GetSpriteIdByName("brokenarmor4"),
                debuffCollection.GetSpriteIdByName("brokenarmor5"),
                debuffCollection.GetSpriteIdByName("brokenarmor6"),
            }, "start", tk2dSpriteAnimationClip.WrapMode.Once, 10);

            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            clip.loopStart = 11;
            BrokenArmorVFX = BrokenArmorVFXObject;
            return BrokenArmorVFX;
        }


        /*
		            var debuffCollection = StaticSpriteDefinitions.Debuff_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Broken Armor", debuffCollection.GetSpriteIdByName("brokenarmor1"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            tk2dBaseSprite vfxSprite = BrokenArmorVFXObject.GetComponent<tk2dBaseSprite>();
            vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);

            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
			var clip = SpriteBuilder.AddAnimation(animator, debuffCollection, new List<int>()
			{
                debuffCollection.GetSpriteIdByName("brokenarmor1"),
                debuffCollection.GetSpriteIdByName("brokenarmor1"),
                debuffCollection.GetSpriteIdByName("brokenarmor1"),
                debuffCollection.GetSpriteIdByName("brokenarmor2"),
                debuffCollection.GetSpriteIdByName("brokenarmor2"),
                debuffCollection.GetSpriteIdByName("brokenarmor3"),
                debuffCollection.GetSpriteIdByName("brokenarmor4"),
                debuffCollection.GetSpriteIdByName("brokenarmor5"),
                debuffCollection.GetSpriteIdByName("brokenarmor5"),
                debuffCollection.GetSpriteIdByName("brokenarmor6"),
            }, "start", tk2dSpriteAnimationClip.WrapMode.LoopSection);

            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
			clip.loopStart = 8;
            BrokenArmorVFX = BrokenArmorVFXObject;
		*/
        public override void ApplyTint(GameActor actor)
        {
			
		}
		public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1f)
		{
			base.OnEffectApplied(actor, effectData, partialAmount);
			actor.healthHaver.AllDamageMultiplier += 0.3f;
            actor.healthHaver.ModifyDamage += ModifyDamage;

        }

        public void ModifyDamage(HealthHaver healthHaver, HealthHaver.ModifyDamageEventArgs modifyDamageEventArgs)
        {
            if (modifyDamageEventArgs.InitialDamage > 1)
            {
                modifyDamageEventArgs.ModifiedDamage = modifyDamageEventArgs.InitialDamage + 1;
            }
        }

		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			actor.healthHaver.AllDamageMultiplier -= 0.3f;
            actor.healthHaver.ModifyDamage -= ModifyDamage;

            base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}
	}
}



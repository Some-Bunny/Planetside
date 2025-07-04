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
	public class BrainHostDummyBuff : GameActorSpeedEffect {
		public static GameObject BrainHostVFX;



        public static GameObject BuildVFX()
        {
            GameObject vfxObj = new GameObject("Brain Melt");
            tk2dSpriteAnimator animator = vfxObj.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = vfxObj.AddComponent<tk2dSpriteAnimation>();

            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;

            ItemBuilder.AddSpriteToObjectAssetbundle("Broken Armor", debuffCollection.GetSpriteIdByName("brainnerphehehoo1"), debuffCollection, vfxObj);
            vfxObj.GetOrAddComponent<tk2dBaseSprite>();

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i < 7; i++)
            {
                int frameSpriteId = debuffCollection.GetSpriteIdByName($"brainnerphehehoo{i}");//SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/BrainHost/brainnerphehehoo{i}", collection);
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = debuffCollection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("idle");
            animator.playAutomatically = true;
            animator.sprite.usesOverrideMaterial = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = animator.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 2f);
            mat.SetFloat("_EmissivePower", 35);
            animator.sprite.renderer.material = mat;

            FakePrefab.MarkAsFakePrefab(vfxObj);
            UnityEngine.Object.DontDestroyOnLoad(vfxObj);

            BrainHostVFX = vfxObj;
            return BrainHostVFX;
        }


        /*
		 * GameObject vfxObj = new GameObject("Brain Melt"); 
			tk2dSpriteAnimator animator = vfxObj.GetOrAddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimation animation = vfxObj.AddComponent<tk2dSpriteAnimation>();

			var debuffCollection = StaticSpriteDefinitions.Debuff_Sheet_Data;

            ItemBuilder.AddSpriteToObjectAssetbundle("Broken Armor", debuffCollection.GetSpriteIdByName("brainnerphehehoo1"), debuffCollection, vfxObj);
			vfxObj.GetOrAddComponent<tk2dBaseSprite>();

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
			List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

			for (int i = 1; i < 8; i++)
			{
				int frameSpriteId = debuffCollection.GetSpriteIdByName($"brainnerphehehoo{i}");//SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/BrainHost/brainnerphehehoo{i}", collection);
                tk2dSpriteDefinition frameDef = debuffCollection.spriteDefinitions[frameSpriteId];
				frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
				frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = debuffCollection });
			}
			idleClip.frames = frames.ToArray();
			idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animator.Library = animation;
			animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
			animator.DefaultClipId = animator.GetClipIdByName("idle");
			animator.playAutomatically = true;
			animator.sprite.usesOverrideMaterial = true;

			Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.mainTexture = animator.sprite.renderer.material.mainTexture;
			mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
			mat.SetFloat("_EmissiveColorPower", 2f);
			mat.SetFloat("_EmissivePower", 35);
			animator.sprite.renderer.material = mat;

			FakePrefab.MarkAsFakePrefab(vfxObj);
			UnityEngine.Object.DontDestroyOnLoad(vfxObj);

			BrainHostVFX = vfxObj;
		*/
    }
}

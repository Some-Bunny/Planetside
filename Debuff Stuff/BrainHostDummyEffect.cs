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
		public static void Init()
        {
			GameObject vfxObj = ItemBuilder.AddSpriteToObject("TarnishVFX", "Planetside/Resources/VFX/BrainHost/brainnerphehehoo1", null);
			FakePrefab.MarkAsFakePrefab(vfxObj);
			UnityEngine.Object.DontDestroyOnLoad(vfxObj);
			tk2dSpriteAnimator animator = vfxObj.GetOrAddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimation animation = vfxObj.AddComponent<tk2dSpriteAnimation>();

			tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(vfxObj, ("BrainHostVFX"));

			tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
			List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

			for (int i = 1; i < 7; i++)
			{
				tk2dSpriteCollectionData collection = DeathMarkcollection;
				int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/BrainHost/brainnerphehehoo{i}", collection);
				tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
				frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter);
				frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
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

			BrainHostVFX = vfxObj;
		}
	}

}

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;
using ItemAPI;
using NpcApi;
using System.Collections;

namespace Planetside
{

    internal class TitleDioramaHooks
    {
        public static GameObject SpecialDioramaIcon;
        public static GameObject ExtantIcon;

        public static void Init()
        {
            GameObject blessingObj = ItemBuilder.AddSpriteToObject("TitleDioramaFX", "Planetside/Resources/VFX/PortalTitleScreen/oerb1", null);
            FakePrefab.MarkAsFakePrefab(blessingObj);
            UnityEngine.Object.DontDestroyOnLoad(blessingObj);
            blessingObj.transform.position = blessingObj.transform.position.WithZ(1);


            
            tk2dSpriteAnimator animator = blessingObj.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = blessingObj.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(blessingObj, ("Tarnish_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i < 15; i++)
            {
                tk2dSpriteCollectionData collection = DeathMarkcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/PortalTitleScreen/oerb{i}", collection);
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
            SpecialDioramaIcon = blessingObj;

            /*
            var partObj = PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal");
            GameObject portal = FakePrefab.Clone(partObj);
            MeshRenderer rend = portal.GetComponent<MeshRenderer>();
            rend.allowOcclusionWhenDynamic = true;
            portal.name = "Icon";
            portal.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            SpecialDioramaIcon = portal;
            */
            new Hook(
                     typeof(TitleDioramaController).GetMethod("Core", BindingFlags.NonPublic | BindingFlags.Instance),
                     typeof(TitleDioramaHooks).GetMethod("TopHook", BindingFlags.Public | BindingFlags.Static));
            new Hook(
                     typeof(TitleDioramaController).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance),
                     typeof(TitleDioramaHooks).GetMethod("UpdateHook", BindingFlags.Public | BindingFlags.Static));

        }

        public static IEnumerator TopHook(Func<TitleDioramaController, bool, IEnumerator> orig, TitleDioramaController self, bool IsfOYER  = true)
        {
            ExtantIcon = UnityEngine.Object.Instantiate(SpecialDioramaIcon, self.transform.position + new Vector3(1f, 1f, 3f), Quaternion.identity, self.transform);
            if (ExtantIcon) { 
                ExtantIcon.transform.localScale *= 1;
                ExtantIcon.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

            }
            IEnumerator origEnum = orig(self, IsfOYER);
            while (origEnum.MoveNext())
            {
                object obj = origEnum.Current;
                yield return obj;
            }
        }

        public static void UpdateHook(Action<TitleDioramaController> orig, TitleDioramaController self)
        {
            orig(self);
            if (ExtantIcon != null && self.LichArmAnimator != null)
            {
                ExtantIcon.transform.position = self.LichArmAnimator.transform.position + MathToolbox.GetUnitOnCircle(17, 39).ToVector3ZUp().WithZ(3);
            }
        }
    }
}

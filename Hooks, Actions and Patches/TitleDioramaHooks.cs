using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;
using ItemAPI;
using NpcApi;
using System.Collections;
using Planetside.Controllers;

namespace Planetside
{

    internal class TitleDioramaHooks
    {
        public static GameObject SpecialDioramaIcon;
        public static GameObject ExtantIcon;
        public static GameObject Logo;
        public static GameObject ExtantLogo;

        public static void Init()
        {

            var Collection = StaticSpriteDefinitions.Oddments_Sheet_Data;
            var logoObj = ItemBuilder.AddSpriteToObjectAssetbundle("Big Ass Logo", Collection.GetSpriteIdByName(FoolMode.isFoolish ? "logofunmode" : "logo"), Collection);
            FakePrefab.MarkAsFakePrefab(logoObj);
            UnityEngine.Object.DontDestroyOnLoad(logoObj);
            logoObj.transform.position = logoObj.transform.position.WithZ(1);
            logoObj.GetComponent<tk2dBaseSprite>().usesOverrideMaterial = true;
            logoObj.GetComponent<tk2dBaseSprite>().renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
            logoObj.GetComponent<tk2dBaseSprite>().renderer.material.SetFloat("_Fade", 0);
            Logo = logoObj;

            GameObject blessingObj = ItemBuilder.AddSpriteToObject("TitleDioramaFX", "Planetside/Resources/VFX/PortalTitleScreen/oerb1", null);
            FakePrefab.MarkAsFakePrefab(blessingObj);
            UnityEngine.Object.DontDestroyOnLoad(blessingObj);
            blessingObj.transform.position = blessingObj.transform.position.WithZ(1);


            
            tk2dSpriteAnimator animator = blessingObj.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = blessingObj.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(blessingObj, ("TitleDioramaFX_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 9 };
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

            //new Hook(
            //         typeof(TitleDioramaController).GetMethod("Core", BindingFlags.NonPublic | BindingFlags.Instance),
            //         typeof(TitleDioramaHooks).GetMethod("CoreHook", BindingFlags.Public | BindingFlags.Static));
            new Hook(
                     typeof(TitleDioramaController).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance),
                     typeof(TitleDioramaHooks).GetMethod("UpdateHook", BindingFlags.Public | BindingFlags.Static));
        }




        public static IEnumerator CoreHook(Func<TitleDioramaController, bool, IEnumerator> orig, TitleDioramaController self, bool IsfOYER  = true)
        {
            //if (PlanetsideQOL.QOLConfig.TileScreenModifications == true)
            {
                var thing = UnityEngine.Object.FindObjectOfType<MainMenuFoyerController>();
                if (thing != null)
                {
                    var referencedController = thing.TitleCard;
                    if (referencedController != null && referencedController.enabled == true)
                    {
                        if (ExtantIcon == null)
                        {
                            ExtantIcon = UnityEngine.Object.Instantiate(SpecialDioramaIcon, self.transform.position, Quaternion.identity, self.transform);
                            if (ExtantIcon)
                            {
                                ExtantIcon.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                            }
                        }

                        if (ExtantLogo == null)
                        {
                            ExtantLogo = UnityEngine.Object.Instantiate(Logo, self.transform.position, Quaternion.identity, self.transform);
                            if (ExtantLogo)
                            {
                                ExtantLogo.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                            }
                        }
                    }
                }          
            }
         
            IEnumerator origEnum = orig(self, IsfOYER);
            while (origEnum.MoveNext())
            {
                object obj = origEnum.Current;
                yield return obj;
            }
        }


        public static MainMenuFoyerController mainMenuFoyerController;
        public static MainMenuFoyerController storedMainMenuFoyerController;

        public static void UpdateHook(Action<TitleDioramaController> orig, TitleDioramaController self)
        {
            orig(self);

            if (mainMenuFoyerController == null)
            {
                var ff = UnityEngine.Object.FindObjectOfType<MainMenuFoyerController>();
                mainMenuFoyerController = ff;
                storedMainMenuFoyerController = ff;
            }
            //ETGModConsole.Log(1);



            //var thing = UnityEngine.Object.FindObjectOfType<MainMenuFoyerController>();
            {

                if (mainMenuFoyerController != null)
                {
                    //ETGModConsole.Log(3);

                    if (storedMainMenuFoyerController != mainMenuFoyerController)
                    {
                        var ff = UnityEngine.Object.FindObjectOfType<MainMenuFoyerController>();
                        mainMenuFoyerController = ff;
                        storedMainMenuFoyerController = ff;
                    }
                    //ETGModConsole.Log(4);

                    var referencedController = mainMenuFoyerController.TitleCard;
                  //  ETGModConsole.Log(5);

                    if (referencedController != null && referencedController.enabled == true)
                    {
                       // ETGModConsole.Log(6);

                        if (ExtantIcon != null && self != null)
                        {
                            //ETGModConsole.Log(7);

                            ExtantIcon.transform.position = self.transform.position.WithZ(0) + new Vector3(15f, 8.5f).WithZ(10);
                            //ETGModConsole.Log(8);


                        }
                        if (ExtantLogo != null && self != null)
                        {
                            //ETGModConsole.Log(9);

                            ExtantLogo.transform.position = self.transform.position.WithZ(0) + new Vector3(-6.9375f, -6f).WithZ(10);
                            ExtantLogo.GetComponent<tk2dBaseSprite>().renderer.material.SetFloat("_Fade", referencedController != null ? referencedController.Opacity : 0);
                            //ETGModConsole.Log(10);

                        }
                        if (ExtantIcon == null)
                        {
                           // ETGModConsole.Log(11);

                            ExtantIcon = UnityEngine.Object.Instantiate(SpecialDioramaIcon, self.transform.position, Quaternion.identity, self.transform);
                            ExtantIcon.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                            //ETGModConsole.Log(12);

                        }

                        if (ExtantLogo == null)
                        {
                            ExtantLogo = UnityEngine.Object.Instantiate(Logo, self.transform.position, Quaternion.identity, self.transform);
                            ExtantLogo.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                        }
                    }
                }
            }





            //With(22) = Foreground
            //With(40) = Clouds
            //With(19) = Lich Arm
            //With(31) = beacons
            //With(10.5f) = Eyeholes

        }
    }
}

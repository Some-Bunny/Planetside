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
using HarmonyLib;

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
            var spr = logoObj.GetComponent<tk2dBaseSprite>();
            spr.usesOverrideMaterial = true;
            spr.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
            spr.renderer.material.SetFloat("_Fade", 0);
            Logo = logoObj;

            GameObject blessingObj = ItemBuilder.AddSpriteToObjectAssetbundle("Forgotten Portal", Collection.GetSpriteIdByName("oerb1"), Collection);
            FakePrefab.MarkAsFakePrefab(blessingObj);
            UnityEngine.Object.DontDestroyOnLoad(blessingObj);
            blessingObj.transform.position = blessingObj.transform.position.WithZ(1);   
            tk2dSpriteAnimator animator = blessingObj.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.Library = StaticSpriteDefinitions.VFX_Animation_Data;
            animator.DefaultClipId = animator.GetClipIdByName("OrbIdle");
            animator.playAutomatically = true;
            SpecialDioramaIcon = blessingObj;

        }



       


        [HarmonyPatch(typeof(MainMenuFoyerController), nameof(MainMenuFoyerController.Update))]
        public class Patch_MainMenuFoyerController_Update
        {
            [HarmonyPostfix]
            private static void PlaceTitleScreenStuff(MainMenuFoyerController __instance)
            {
                if (__instance.m_tdc != null)
                {
                    var referencedController = __instance.TitleCard;
                    //  ETGModConsole.Log(5);

                    if (referencedController != null && referencedController.enabled == true)
                    {
                        // ETGModConsole.Log(6);

                        if (ExtantIcon != null)
                        {
                            //ETGModConsole.Log(7);

                            ExtantIcon.transform.position = __instance.m_tdc.transform.position.WithZ(0) + new Vector3(15f, 8.5f).WithZ(10);
                            //ETGModConsole.Log(8);


                        }
                        if (ExtantLogo != null)
                        {
                            //ETGModConsole.Log(9);

                            ExtantLogo.transform.position = __instance.m_tdc.transform.position.WithZ(0) + new Vector3(-6.9375f, -6f).WithZ(10);
                            ExtantLogo.GetComponent<tk2dBaseSprite>().renderer.material.SetFloat("_Fade", referencedController != null ? referencedController.Opacity : 0);
                            //ETGModConsole.Log(10);

                        }
                        if (ExtantIcon == null)
                        {
                            // ETGModConsole.Log(11);

                            ExtantIcon = UnityEngine.Object.Instantiate(SpecialDioramaIcon, __instance.m_tdc.transform.position, Quaternion.identity, __instance.m_tdc.transform);
                            ExtantIcon.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                            //ETGModConsole.Log(12);

                        }

                        if (ExtantLogo == null)
                        {
                            ExtantLogo = UnityEngine.Object.Instantiate(Logo, __instance.m_tdc.transform.position, Quaternion.identity, __instance.m_tdc.transform);
                            ExtantLogo.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                        }
                    }
                }
            }
        }


        //public static MainMenuFoyerController mainMenuFoyerController;
        //public static MainMenuFoyerController storedMainMenuFoyerController;

        public static void UpdateHook(Action<TitleDioramaController> orig, TitleDioramaController self)
        {
            /*
            orig(self);

            if (mainMenuFoyerController == null)
            {
                var ff = UnityEngine.Object.FindObjectOfType<MainMenuFoyerController>();
                mainMenuFoyerController = ff;
                storedMainMenuFoyerController = ff;
            }



            //var thing = UnityEngine.Object.FindObjectOfType<MainMenuFoyerController>();
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

                
            }

            */



            //With(22) = Foreground
            //With(40) = Clouds
            //With(19) = Lich Arm
            //With(31) = beacons
            //With(10.5f) = Eyeholes

        }
    }
}

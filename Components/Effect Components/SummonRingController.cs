using Alexandria.cAPI;
using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Planetside.Wailer;

namespace Planetside.Components.Effect_Components
{
    public class SummonRingController : MonoBehaviour
    {
        private static GameObject SummonRingPrefab;
        private static GameObject LetterObject;

        public tk2dSprite RingOuter;
        public tk2dSprite RingInner;



        public static void InitSummoningRing()
        {

            SummonRingPrefab = PrefabBuilder.BuildObject("SummonRing");
            var Ring = SummonRingPrefab.gameObject.AddComponent<tk2dSprite>();
            Ring.SetSprite(StaticSpriteDefinitions.SpecialVFX_Sheet_Data, StaticSpriteDefinitions.SpecialVFX_Sheet_Data.GetSpriteIdByName("DemonRing"));
            Ring.gameObject.layer = LayerMask.NameToLayer("Unoccluded");
            var cont = SummonRingPrefab.AddComponent<SummonRingController>();
            cont.RingOuter = Ring;


            Ring.usesOverrideMaterial = true;
            Material mat = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
            mat.mainTexture = Ring.renderer.material.mainTexture;
            mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            mat.SetFloat("_EmissivePower", 40);
            mat.SetFloat("_EmissiveColorPower", 20);
            Ring.renderer.material = mat;


            var miniRing = PrefabBuilder.BuildObject("MiniRing");
            Ring = miniRing.gameObject.AddComponent<tk2dSprite>();
            Ring.SetSprite(StaticSpriteDefinitions.SpecialVFX_Sheet_Data, StaticSpriteDefinitions.SpecialVFX_Sheet_Data.GetSpriteIdByName("DemonRing"));
            Ring.gameObject.layer = LayerMask.NameToLayer("Unoccluded");
            Ring.transform.SetParent(SummonRingPrefab.transform, false);
            Ring.transform.localScale *= 0.3f;
            cont.RingInner = Ring;

            Ring.usesOverrideMaterial = true;
            mat = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
            mat.mainTexture = Ring.renderer.material.mainTexture;
            mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            mat.SetFloat("_EmissivePower", 40);
            mat.SetFloat("_EmissiveColorPower", 20);
            Ring.renderer.material = mat;



            LetterObject = PrefabBuilder.BuildObject("LetterObject");
            var letterSprite = LetterObject.gameObject.AddComponent<tk2dSprite>();
            letterSprite.SetSprite(StaticSpriteDefinitions.SpecialVFX_Sheet_Data, 0);
            letterSprite.gameObject.layer = LayerMask.NameToLayer("Unoccluded");

            letterSprite.usesOverrideMaterial = true;
            mat = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
            mat.mainTexture = letterSprite.renderer.material.mainTexture;
            mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            mat.SetFloat("_EmissivePower", 40);
            mat.SetFloat("_EmissiveColorPower", 20);
            letterSprite.renderer.material = mat;





        }
        public GameObject FollowObject = null;

        public static SummonRingController CreateSummoningRing(string Text, Vector3 position, float ImmediateScale = 1, bool isRed = false, float ScaleMultiplier = 1, bool VisibleRings = true, float AfterImageTimeMultiplier = 1)
        {
            Text = Text.ToLower();
            var c = UnityEngine.Object.Instantiate(SummonRingPrefab, position, Quaternion.identity).GetComponent<SummonRingController>();
            if (isRed)
            {
                c.RingInner.SetSprite(StaticSpriteDefinitions.SpecialVFX_Sheet_Data.GetSpriteIdByName("redDemonRing"));
                c.RingOuter.SetSprite(StaticSpriteDefinitions.SpecialVFX_Sheet_Data.GetSpriteIdByName("redDemonRing"));
            }

            if (VisibleRings == false)
            {
                c.RingOuter.renderer.enabled = false;
                c.RingInner.renderer.enabled = false;
            }


            //c.RingInner.GetComponent<ImprovedAfterImage>().shadowLifetime *= AfterImageTimeMultiplier;
            //c.RingOuter.GetComponent<ImprovedAfterImage>().shadowLifetime *= AfterImageTimeMultiplier;

            int length = Text.Length;
            float split = 360 / length;
           // float Scale = Mathf.Min(16 / length, 1);
            for (int i = 0; i < length; i++)
            {
                var item = Text[i];
                var sprite = UnityEngine.Object.Instantiate(LetterObject, c.transform).GetComponent<tk2dSprite>();
                var _ = LazySwitch(item);
                if (_ != null)
                {
                    if (isRed)
                    {
                        _ = "red" + _;
                        //sprite.GetComponent<ImprovedAfterImage>().dashColor = new Color(0.25f, 0, 0, 1);
                    }
                    sprite.SetSprite(StaticSpriteDefinitions.SpecialVFX_Sheet_Data.GetSpriteIdByName(_));
                }
                sprite.transform.localPosition = MathToolbox.GetUnitOnCircle(split * i, 2.625f);
                sprite.transform.localScale = Vector3.one * ScaleMultiplier;
                //sprite.GetComponent<ImprovedAfterImage>().shadowLifetime *= AfterImageTimeMultiplier;
                sprite.renderer.enabled = true;

                c.ExtantLetters.Add(sprite);
            }
            c.ForceSetScale(0);
            c.SetScale(ImmediateScale);
            return c;
        }

        public void ForceSetScale(float Scale)
        {
            GetScale = Scale;
            currentScale = Scale;
            this.transform.localScale = Vector3.one * currentScale;
        }

        public void SetScale(float Scale)
        {
            GetScale = Scale;
        }
        private float GetScale;
        private float currentScale = 0;
        public void Update()
        {
            if (FollowObject)
            {
                this.transform.position = FollowObject.transform.position;
            }
            if (currentScale != GetScale)
            {
                currentScale = Mathf.MoveTowards(currentScale, GetScale, UpdateSpeed * Time.deltaTime);
                this.transform.localScale = Vector3.one * currentScale;
            }
            if (WillDestroy == true && currentScale == 0)
            {
                Destroy(this.gameObject);
            }
            this.transform.Rotate(0, 0, SpinSpeed * UpdateSpeed * Time.deltaTime);
            foreach (var entry in ExtantLetters)
            {
                entry.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        bool WillDestroy = false;
        public void SetToDestroy()
        {
            WillDestroy = true;
            GetScale = 0;
        }



        public float UpdateSpeed = 1;
        public float SpinSpeed = 120;
        public List<tk2dSprite> ExtantLetters;
    
    
        public static string LazySwitch(char c) //This code sucks
        {
            switch (c)
            {
                case 'a':
                    return "letter_01";
                case 'b':
                    return "letter_02";
                case 'c':
                    return "letter_03";
                case 'd':
                    return "letter_04";
                case 'e':
                    return "letter_05";
                case 'f':
                    return "letter_06";
                case 'g':
                    return "letter_07";
                case 'h':
                    return "letter_08";
                case 'i':
                    return "letter_09";
                case 'j':
                    return "letter_10";
                case 'k':
                    return "letter_11";
                case 'l':
                    return "letter_12";
                case 'm':
                    return "letter_13";
                case 'n':
                    return "letter_14";
                case 'o':
                    return "letter_15";
                case 'p':
                    return "letter_16";
                case 'q':
                    return "letter_17";
                case 'r':
                    return "letter_18";
                case 's':
                    return "letter_19";
                case 't':
                    return "letter_20";
                case 'u':
                    return "letter_21";
                case 'v':
                    return "letter_22";
                case 'w':
                    return "letter_23";
                case 'x':
                    return "letter_24";
                case 'y':
                    return "letter_25";
                case 'z':
                    return "letter_26";
                default:
                    return null;
            }
        }
    }
}

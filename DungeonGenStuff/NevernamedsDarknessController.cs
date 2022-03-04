using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using UnityEngine;

namespace Planetside
{
    public class NevernamedsDarknessHandler : MonoBehaviour
    {
        public NevernamedsDarknessHandler()
        {
        }
        public static OverridableBool shouldBeDark = new OverridableBool(false);
        public static OverridableBool shouldBeLightOverride = new OverridableBool(false);
        private void Start()
        {
            GameObject ChallengeManagerReference = LoadHelper.LoadAssetFromAnywhere<GameObject>("_ChallengeManager");
            DarknessEffectShader = (ChallengeManagerReference.GetComponent<ChallengeManager>().PossibleChallenges[5].challenge as DarknessChallengeModifier).DarknessEffectShader;
        }
        private bool ReturnShouldBeDark()
        {
            if (shouldBeDark.Value && !shouldBeLightOverride.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void Update()
        {
            if (GameManager.Instance.PrimaryPlayer == null && Pixelator.Instance != null)
            {
                Pixelator.Instance.AdditionalCoreStackRenderPass = null;
                isDark = false;
            }
            if (isDark && GameManager.Instance.PrimaryPlayer != null && Pixelator.Instance != null)
            {
                if (Pixelator.Instance.AdditionalCoreStackRenderPass == null && Pixelator.Instance != null)
                {
                    m_material = new Material(DarknessEffectShader);
                    Pixelator.Instance.AdditionalCoreStackRenderPass = m_material;
                }
                if (m_material != null)
                {
                    float num = GameManager.Instance.PrimaryPlayer.FacingDirection;
                    if (num > 270f)
                    {
                        num -= 360f;
                    }
                    if (num < -270f)
                    {
                        num += 360f;
                    }
                    m_material.SetFloat("_ConeAngle", FlashlightAngle);
                    Vector4 centerPointInScreenUV = GetCenterPointInScreenUV(GameManager.Instance.PrimaryPlayer.CenterPosition);
                    centerPointInScreenUV.z = num;
                    Vector4 vector = centerPointInScreenUV;
                    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                    {
                        num = GameManager.Instance.SecondaryPlayer.FacingDirection;
                        if (num > 270f)
                        {
                            num -= 360f;
                        }
                        if (num < -270f)
                        {
                            num += 360f;
                        }
                        vector = GetCenterPointInScreenUV(GameManager.Instance.SecondaryPlayer.CenterPosition);
                        vector.z = num;
                    }
                    m_material.SetVector("_Player1ScreenPosition", centerPointInScreenUV);
                    m_material.SetVector("_Player2ScreenPosition", vector);
                }
            }
        }
        private static Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
        {
            Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
            return new Vector4(vector.x, vector.y, 0f, 0f);
        }
        public static void EnableDarkness(float ConeAngle = 90, float Time = 1)
        {
            if (isDark) return;
            if (Pixelator.Instance)
            {
                GameManager.Instance.Dungeon.StartCoroutine(EnableDarknessCoroutine(ConeAngle, Time));
            }
            isDark = true;
        }
        public static void DisableDarkness(float Time = 1)
        {
            if (!isDark) return;
            if (Pixelator.Instance)
            {
                GameManager.Instance.Dungeon.StartCoroutine(DisableDarknessCoroutine(Time));
            }
        }
        public static bool IsItDark(){return isDark;}
        private static IEnumerator EnableDarknessCoroutine(float ConeAngle = 90, float ConeTime = 0.75f)
        {
            m_material = new Material(DarknessEffectShader);
            Pixelator.Instance.AdditionalCoreStackRenderPass = m_material;
            float elapsed = 0f;
            float duration = ConeTime;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                FlashlightAngle = Mathf.Lerp(360, ConeAngle, t);

                yield return null;
            }
            yield break;
        }


        private static IEnumerator DisableDarknessCoroutine(float ConeTime = 0.75f)
        {
            float elapsed = 0f;
            float duration = ConeTime;
            float storedAngle = FlashlightAngle;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                FlashlightAngle = Mathf.Lerp(360, storedAngle, duration - t);

                yield return null;
            }
            if (Pixelator.Instance)
            {
                Pixelator.Instance.AdditionalCoreStackRenderPass = null;
            }
            isDark = false;
            yield break;
        }



        public static bool isDark = false;
        public static Shader DarknessEffectShader;
        public static float FlashlightAngle;
        private static Material m_material;
    }
}

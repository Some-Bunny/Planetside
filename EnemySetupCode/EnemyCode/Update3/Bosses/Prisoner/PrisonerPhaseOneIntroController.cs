using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

using Dungeonator;

namespace Planetside
{
	// Token: 0x02000FBB RID: 4027
	[RequireComponent(typeof(GenericIntroDoer))]
	public class PrisonerPhaseOneIntroController : SpecificIntroDoer
	{
		public void Start()
		{
            if (base.aiActor) { Prisoner = base.aiActor; base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;}
        }
        public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
        {
            base.PlayerWalkedIn(player, animators);
            //SpawnChains();		
        }
        public void SpawnChains()
        {
            if (Prisoner != null)
            {
                CellArea area = Prisoner.ParentRoom.area;
                if (area != null)
                {
                    Vector2 roomCentreLeft = area.UnitCenter - (new Vector2(area.UnitLeft, 0));
                    Vector2 roomCentreRight = area.UnitCenter + (new Vector2(area.UnitLeft, 0));
                    for (int i = -1; i < 2; i++)
                    {
                        GameObject chainOnj = GameObject.Instantiate(PrisonerPhaseOne.PrisonerChainVFX, roomCentreLeft, Quaternion.identity);
                        Vector2 handPosLeft = Prisoner.transform.PositionVector2();
                        if (Prisoner.transform.Find("LeftHandChainPoint").gameObject != null)
                        {
                            handPosLeft = Prisoner.transform.Find("LeftHandChainPoint").transform.PositionVector2();
                        }
                        chainOnj.name = "handChainLeft" + i.ToString();
                        activeChainsandPos.Add(chainOnj, new Dictionary<Vector2, Vector2>() { { roomCentreLeft + new Vector2(0, 45f*i), handPosLeft } });
                    }
                    for (int i = -1; i < 2; i++)
                    {
                        GameObject chainOnj = GameObject.Instantiate(PrisonerPhaseOne.PrisonerChainVFX, roomCentreRight, Quaternion.identity);
                        Vector2 handPosRight = Prisoner.transform.PositionVector2();
                        if (Prisoner.transform.Find("RightHandChainPoint").gameObject != null)
                        {
                            handPosRight = Prisoner.transform.Find("RightHandChainPoint").transform.PositionVector2();
                        }
                        chainOnj.name = "handChainRight" + i.ToString();
                        activeChainsandPos.Add(chainOnj, new Dictionary<Vector2, Vector2>() { { roomCentreRight + new Vector2(0, 45f * i), handPosRight } });
                    }
                }
            }
        }

        private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
        {
            if (clip.GetFrame(frameIdx).eventInfo.Contains("HandBreakFree"))
            {
                GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
                objanimator.ignoreTimeScale = true;
                objanimator.AlwaysIgnoreTimeScale = true;
                objanimator.AnimateDuringBossIntros = true;
                objanimator.alwaysUpdateOffscreen = true;
                objanimator.playAutomatically = true;

                ParticleSystem objparticles = silencerVFX.GetComponentInChildren<ParticleSystem>();
                var main = objparticles.main;
                main.useUnscaledTime = true;

                Vector3 pos = clip.GetFrame(frameIdx).eventInfo.Contains("Left") ? base.aiActor.transform.Find("LeftHandChainPoint").position : base.aiActor.transform.Find("RightHandChainPoint").position;
                GameObject.Instantiate(silencerVFX.gameObject, pos, Quaternion.identity);
                GameObject breakVFX = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(178) as Gun).GetComponent<FireOnReloadSynergyProcessor>().DirectedBurstSettings.ProjectileInterface.SpecifiedProjectile.hitEffects.tileMapHorizontal.effects[0].effects[0].effect, clip.GetFrame(frameIdx).eventInfo.Contains("Left") ? base.aiActor.transform.Find("LeftHandChainPoint").position : base.aiActor.transform.Find("RightHandChainPoint").position, Quaternion.identity);
                tk2dBaseSprite component = breakVFX.GetComponent<tk2dBaseSprite>();
                component.PlaceAtPositionByAnchor(pos, tk2dBaseSprite.Anchor.MiddleCenter);
                component.HeightOffGround = 35f;
                component.UpdateZDepth();
                tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                if (component2 != null)
                {
                    component2.ignoreTimeScale = true;
                    component2.AlwaysIgnoreTimeScale = true;
                    component2.AnimateDuringBossIntros = true;
                    component2.alwaysUpdateOffscreen = true;
                    component2.playAutomatically = true;
                    component.scale *= 2.2f;
                }
                foreach (var chain in activeChainsandPos)
                {
                    if (chain.Key != null && chain.Key.gameObject.name.Contains(clip.GetFrame(frameIdx).eventInfo.Contains("Left")?"Left":"Right"))
                    {
                        GameManager.Instance.StartCoroutine(this.DoChainBreakAway(chain.Key.gameObject.GetComponent<tk2dTiledSprite>(), UnityEngine.Random.Range(0.875f, 2f), UnityEngine.Random.Range(0.1f, 0.3f)));
                    }
                }
            }
            if (clip.GetFrame(frameIdx).eventInfo.Contains("LeftHandCastMagic"))
            {
                GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                component.PlaceAtPositionByAnchor(base.aiActor.transform.Find("LeftHandChainPoint").position + new Vector3(-0.3125f, 1.1875f), tk2dBaseSprite.Anchor.MiddleCenter);
                component.HeightOffGround = 35f;
                component.UpdateZDepth();
                tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                if (component2 != null)
                {
                    component2.ignoreTimeScale = true;
                    component2.AlwaysIgnoreTimeScale = true;
                    component2.AnimateDuringBossIntros = true;
                    component2.alwaysUpdateOffscreen = true;
                    component2.playAutomatically = true;
                }
            }
            if (clip.GetFrame(frameIdx).eventInfo.Contains("PrisonerLaugh"))
            {
                GameManager.Instance.StartCoroutine(DoDistortionWaveTimescaleImmune(Prisoner.sprite.WorldCenter, 1.5f, 0.14f, 12, 0.3f));
                for (int i = 0; i < 2; i++)
                {
                    Vector3 pos = base.aiActor.transform.Find("LeftHandChainPoint").position + new Vector3(-0.3125f, 1.3125f);
                    if (i == 1) { pos = base.aiActor.transform.Find("RightHandChainPoint").position + new Vector3(0.3125f, 1.3125f); }
                    GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                    tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                    component.PlaceAtPositionByAnchor(pos, tk2dBaseSprite.Anchor.MiddleCenter);
                    component.HeightOffGround = 35f;
                    component.UpdateZDepth();
                    tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                    if (component2 != null)
                    {
                        component2.ignoreTimeScale = true;
                        component2.AlwaysIgnoreTimeScale = true;
                        component2.AnimateDuringBossIntros = true;
                        component2.alwaysUpdateOffscreen = true;
                        component2.playAutomatically = true;
                    }
                }
            }
        }

        private IEnumerator DoDistortionWaveTimescaleImmune(Vector2 center, float distortionIntensity, float distortionRadius, float maxRadius, float duration)
        {
            Material distMaterial = new Material(ShaderCache.Acquire("Brave/Internal/DistortionWave"));
            Vector4 distortionSettings = this.GetCenterPointInScreenUV(center, distortionIntensity, distortionRadius);
            distMaterial.SetVector("_WaveCenter", distortionSettings);
            Pixelator.Instance.RegisterAdditionalRenderPass(distMaterial);
            float time = 20* duration;
            for (int i = 0; i < time; i++)
            {
                if (BraveUtility.isLoadingLevel && GameManager.Instance.IsLoadingLevel)
                {
                    break;
                }
                float t = i / time;
                t = BraveMathCollege.LinearToSmoothStepInterpolate(0f, 1f, t);
                distortionSettings = this.GetCenterPointInScreenUV(center, distortionIntensity, distortionRadius);
                distortionSettings.w = Mathf.Lerp(distortionSettings.w, 0f, t);
                distMaterial.SetVector("_WaveCenter", distortionSettings);
                float currentRadius = Mathf.Lerp(0f, maxRadius, t);
                distMaterial.SetFloat("_DistortProgress", currentRadius / maxRadius * (maxRadius / 33.75f));
                yield return null;
            }
            Pixelator.Instance.DeregisterAdditionalRenderPass(distMaterial);
            UnityEngine.Object.Destroy(distMaterial);
            yield break;
        }
        private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint, float dIntensity, float dRadius)
        {
            Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
            return new Vector4(vector.x, vector.y, dRadius, dIntensity);
        }


        private IEnumerator DoChainBreakAway(tk2dTiledSprite tiledSprite, float pullBackTime, float Delay)
        {
            float ChainSFXRNG = UnityEngine.Random.Range(5, 25);
            int RepeatSound = 0;
            float elaWait = 0f;
            float duraWait = Delay;
            while (elaWait < duraWait)
            {
                elaWait += 0.0166f;
                yield return null;
            }
            if (tiledSprite != null)
            {
                Dictionary<Vector2, Vector2> positions = new Dictionary<Vector2, Vector2>();
                activeChainsandPos.TryGetValue(tiledSprite.gameObject, out positions);
                Vector2 unitCenter = positions.First().Key;
                Vector2 unitCenter2 = positions.First().Value;
                int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(unitCenter, unitCenter2)), 1);
                for (int i = 0; i < num2; i++)
                {
                    float t = ((float)i / (float)num2)/pullBackTime;
                    Vector2 vector3 = Vector2.Lerp(unitCenter, unitCenter2, 1 - t);
                    Vector2 vector = vector3 - unitCenter;
                    float num = BraveMathCollege.Atan2Degrees(vector.normalized);
                    int num22 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
                    tiledSprite.dimensions = new Vector2((float)num22, 4);
                    tiledSprite.transform.rotation = Quaternion.Euler(0f, 0f, num);
                    tiledSprite.UpdateZDepth();
                    tiledSprite.anchor = tk2dAnimatedSprite.Anchor.MiddleLeft;
                    tiledSprite.HeightOffGround = base.gameObject.GetComponent<tk2dBaseSprite>().HeightOffGround - 10;
                    tiledSprite.renderer.enabled = true;
                    GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"));
                    tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
                    component.PlaceAtPositionByAnchor(vector3.ToVector3ZUp(0f), tk2dBaseSprite.Anchor.MiddleCenter);
                    component.HeightOffGround = 35f;
                    component.UpdateZDepth();
                    tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                    if (component2 != null)
                    {
                        component.scale *= 0.66f;
                        component2.ignoreTimeScale = true;
                        component2.AlwaysIgnoreTimeScale = true;
                        component2.AnimateDuringBossIntros = true;
                        component2.alwaysUpdateOffscreen = true;
                        component2.playAutomatically = true;
                    }
                    if (i % ChainSFXRNG == 0 && RepeatSound <= 1 )
                    {
                        RepeatSound++; 
                        AkSoundEngine.PostEvent("Play_ChainBreak_01", gameObject);
                    }
                    yield return null;
                }
         
                Destroy(tiledSprite.gameObject);
            }
            yield break;
        }
        public void Update()
        {           
            foreach (var chain in activeChainsandPos)
            {
                if (chain.Key != null)
                {
                    if (chain.Key.gameObject.GetComponent<tk2dTiledSprite>() != null)
                    {
                        UpdateLink(chain.Key.gameObject.GetComponent<tk2dTiledSprite>());
                    }
                }
            }
        }

        private void UpdateLink(tk2dTiledSprite m_extantLink)
        {
            if (Prisoner == null)
            {
                if (m_extantLink != null)
                {
                    Destroy(m_extantLink.gameObject);
                }
            }
            else if (m_extantLink != null)
            {
                Dictionary<Vector2, Vector2> positions = new Dictionary<Vector2, Vector2>();
                activeChainsandPos.TryGetValue(m_extantLink.gameObject, out positions);
                Vector2 unitCenter = positions.First().Key;
                Vector2 unitCenter2 = positions.First().Value;
                m_extantLink.transform.position = unitCenter;
                Vector2 vector = unitCenter2 - unitCenter;
                float num = BraveMathCollege.Atan2Degrees(vector.normalized);
                if (vector != null)
                {
                    int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
                    m_extantLink.dimensions = new Vector2((float)num2, 4);
                }
                m_extantLink.transform.rotation = Quaternion.Euler(0f, 0f, num);
                m_extantLink.UpdateZDepth();
                m_extantLink.anchor = tk2dAnimatedSprite.Anchor.MiddleLeft;
                m_extantLink.HeightOffGround = base.gameObject.GetComponent<tk2dBaseSprite>().HeightOffGround - 10;
                m_extantLink.renderer.enabled = true;
            }
        }
        public override void EndIntro()
        {
            base.EndIntro();
            if (Prisoner != null)
            {
                Prisoner.aiAnimator.PlayUntilCancelled("idle", false, null, -1f, false);
                Prisoner.behaviorSpeculator.enabled = true;
                PrisonerPhaseOne.PrisonerController controller = Prisoner.GetComponent<PrisonerPhaseOne.PrisonerController>();
                controller.CurrentSubPhase = PrisonerPhaseOne.PrisonerController.SubPhases.PHASE_1;
            }
            foreach (var chain in activeChainsandPos)
            {
                if (chain.Key != null)
                {
                    Destroy(chain.Key.gameObject);
                }
            }
            activeChainsandPos.Clear();
        }

        public Dictionary<GameObject, Dictionary<Vector2, Vector2>> activeChainsandPos = new Dictionary<GameObject, Dictionary<Vector2, Vector2>>();
        public AIActor Prisoner;
    }

}




using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using Gungeon;
using ItemAPI;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;

namespace Planetside
{


    [RequireComponent(typeof(GenericIntroDoer))]
    public class NemesisIntroController : SpecificIntroDoer
    {

        public bool m_finished;
        public AIActor m_AIActor;


      
        public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
        {
            this.aiShooter.ToggleGunAndHandRenderers(false, "GuardIsSpawning");
            this.aiActor.renderer.enabled = true;
            this.aiActor.sprite.Awake();
        }

        public void Start()
        {
            if (base.aiActor) 
            { 
                
                Nemesis = base.aiActor; 
                base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
            }
        }
        private IEnumerator PortalDoer(MeshRenderer portal, bool DestroyWhenDone = true)
        {
            float elapsed = 0f;
            while (elapsed < 4)
            {
                elapsed += 0.0167f;
                float t = elapsed / 4;
                if (portal.gameObject == null) { yield break; }
                float throne1 = Mathf.Sin(t * (Mathf.PI));
                portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0, 0.31f, throne1));
                portal.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(12, 2, throne1));
                yield return null;
            }

            if (DestroyWhenDone == true)
            {
                Destroy(portal.gameObject);
            }
            yield break;
        }

        private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
        {
            if (clip.GetFrame(frameIdx).eventInfo.Contains("v_s"))
            {
                this.aiActor.HasBeenEngaged = true;
                {
                    GameObject portalObj = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, this.aiActor.sprite.WorldBottomCenter, Quaternion.Euler(0f, 0f, 0f));
                    portalObj.layer = this.aiActor.gameObject.layer + (int)GameManager.Instance.MainCameraController.CurrentZOffset;
                    portalObj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
                    MeshRenderer mesh = portalObj.GetComponent<MeshRenderer>();
                    mesh.material.SetTexture("_PortalTex", StaticTextures.NebulaTexture);
                    GameManager.Instance.StartCoroutine(PortalDoer(mesh, true));
                }
            }

            if (clip.GetFrame(frameIdx).eventInfo.Contains("EquipSelf"))
            {
                if (Nemesis.aiShooter)
                {
                    Nemesis.aiShooter.ToggleGunAndHandRenderers(true, "GuardIsSpawning");
                }
                AkSoundEngine.PostEvent("Play_OBJ_weapon_pickup_01", base.aiActor.gameObject);
                GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.BlueSynergyPoofVFX, false);
                tk2dSpriteAnimator component2 = gameObject.GetComponent<tk2dSpriteAnimator>();
                if (component2 != null)
                {
                    component2.ignoreTimeScale = true;
                    component2.AlwaysIgnoreTimeScale = true;
                    component2.AnimateDuringBossIntros = true;
                    component2.alwaysUpdateOffscreen = true;
                    component2.playAutomatically = true;
                }
                gameObject.transform.position = base.aiActor.transform.Find("GunAttachPoint").gameObject.transform.position;
                gameObject.transform.localScale *= 1.5f;
                Destroy(gameObject, 2);

                GameObject gameObjectTwo = SpawnManager.SpawnVFX(StaticVFXStorage.MachoBraceDustupVFX, false);
                gameObjectTwo.transform.position = base.aiActor.transform.position - new Vector3(1.25f, 1.25f);
                tk2dSpriteAnimator componenta = gameObjectTwo.GetComponent<tk2dSpriteAnimator>();
                if (componenta != null)
                {
                    componenta.ignoreTimeScale = true;
                    componenta.AlwaysIgnoreTimeScale = true;
                    componenta.AnimateDuringBossIntros = true;
                    componenta.alwaysUpdateOffscreen = true;
                    componenta.playAutomatically = true;
                }
                Destroy(gameObjectTwo, 2);
            }
        }

        
        

        public void Update()
        {
          
        }



        public override void EndIntro()
        {
            this.aiActor.GetComponent<NemesisController>().DoEngagePostProcess();
            if (Nemesis.aiShooter)
            {
                Nemesis.aiShooter.ToggleGunAndHandRenderers(true, "GuardIsSpawning");
            }
            AkSoundEngine.PostEvent("Play_OBJ_weapon_pickup_01", base.aiActor.gameObject);
            GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.BlueSynergyPoofVFX, false);
            gameObject.transform.position = base.aiActor.transform.Find("GunAttachPoint").gameObject.transform.position;
            gameObject.transform.localScale *= 1.5f;
            Destroy(gameObject, 2);

            GameObject gameObjectTwo = SpawnManager.SpawnVFX(StaticVFXStorage.MachoBraceDustupVFX, false);
            gameObjectTwo.transform.position = base.aiActor.transform.position - new Vector3(1.25f, 1.25f);
            Destroy(gameObjectTwo, 2);
            int playerMask = CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.PlayerHitBox);

            this.aiActor.behaviorSpeculator.enabled = true;
            this.aiActor.specRigidbody.enabled = true;

            

            this.aiActor.healthHaver.PreventAllDamage = false;
            this.aiActor.specRigidbody.RemoveCollisionLayerIgnoreOverride(playerMask);
            this.aiActor.HasBeenEngaged = true;
            this.aiActor.State = AIActor.ActorState.Normal;

        }

        public AIActor Nemesis;
        /*
        private IEnumerator WaitForSecondsInvariant(float time)
        {
            for (float elapsed = 0f; elapsed < time; elapsed += GameManager.INVARIANT_DELTA_TIME) { yield return null; }
            yield break;
        }
		*/
    }
}

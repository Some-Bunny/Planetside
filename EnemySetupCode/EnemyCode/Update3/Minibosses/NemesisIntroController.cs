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
            
        }

        public void Start()
        {
            if (base.aiActor) 
            { 
                
                Nemesis = base.aiActor; 
                base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
            }
        }

        private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
        {
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
                    component2.ignoreTimeScale = true;
                    component2.AlwaysIgnoreTimeScale = true;
                    component2.AnimateDuringBossIntros = true;
                    component2.alwaysUpdateOffscreen = true;
                    component2.playAutomatically = true;
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

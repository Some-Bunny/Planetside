using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{
    public class OppressorController : BraveBehaviour
    {
        public void Start()
        {
            base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
            for (int i = 0; i < base.aiActor.gameObject.transform.childCount; i++)
            {
                var tr = base.aiActor.gameObject.transform.GetChild(i);
                if (tr.gameObject != null && tr.gameObject.GetComponent<AdvancedBodyPartController>() != null)
                {
                    var mhm = tr.gameObject.GetComponent<AdvancedBodyPartController>();
                    if (mhm != null)
                    {
                        mhm.Render = false;
                        if (mhm.Name.Contains("Left"))
                        {
                            leftArm = mhm;
                        }
                        else
                        {
                            rightArm = mhm;
                        }
                        mhm.OnBodyPartPreDeath += (obj1, obj2, obj3) =>
                        {
                            base.StartCoroutine(DestroyArm(mhm.Name.Contains("Left")));
                        };                       
                        tr.gameObject.SetActive(true);
                    }
                }
            }
        }
        private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
        {//Surprise
            if (clip.GetFrame(frameIdx).eventInfo.Contains("show_hands"))
            {
                if (leftArm) { leftArm.Render = true; leftArm.gameObject.transform.localScale = Vector3.one; leftArm.healthHaver.PreventAllDamage = false; }
                if (rightArm) { rightArm.Render = true; rightArm.gameObject.transform.localScale = Vector3.one; leftArm.healthHaver.PreventAllDamage = false; }
            }
            if (clip.GetFrame(frameIdx).eventInfo.Contains("hide_hands"))
            {
                if (leftArm) { leftArm.Render = false; leftArm.healthHaver.PreventAllDamage = true; }
                if (rightArm) { rightArm.Render = false; rightArm.healthHaver.PreventAllDamage = true; }
            }
        }

        private IEnumerator DestroyArm(bool IsLeft)
        {
            
            if (IsLeft == true) {leftArm = null; }
            else{rightArm = null;}

            if (leftArm == null && rightArm == null)
            {
                AkSoundEngine.PostEvent("Play_Oppresser_BigHurt", base.gameObject);
                Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, 5f, 0.1f, 15, 0.5f);

                AttackBehaviorGroup group = base.behaviorSpeculator.AttackBehaviors[0] as AttackBehaviorGroup;
                List<AttackBehaviorGroup.AttackGroupItem> l1 = group.AttackBehaviors.FindAll((AttackBehaviorGroup.AttackGroupItem a) => a.NickName.Contains("Heart"));
                for (int i = 0; i < l1.Count; i++)
                {
                    l1[i].Probability = 1f;
                }

                List<AttackBehaviorGroup.AttackGroupItem> l =  group.AttackBehaviors.FindAll((AttackBehaviorGroup.AttackGroupItem a) => a.NickName.Contains("Def"));
                for (int i = 0; i < l.Count; i++)
                {
                    l[i].Probability = 0f;
                }
            }

            AkSoundEngine.PostEvent("Play_VO_jammer_death_02", base.gameObject);
            base.behaviorSpeculator.InterruptAndDisable();
            base.aiAnimator.PlayUntilFinished("pain", true, null, 1, false);
            yield return new WaitForSeconds(0.5f);
            base.behaviorSpeculator.enabled = true;
            base.behaviorSpeculator.CooldownScale *= 0.9f;
            yield break;
        }


        private void Update()
        {
           
        }
        public bool EnemyIsVisible(AIActor enemyToCheck)
        {
            if (enemyToCheck == null) { return false; }
            if (enemyToCheck.sprite.renderer.enabled == false) { return false; }
            if (enemyToCheck.IsGone == true) { return false; }
            if (enemyToCheck.State == AIActor.ActorState.Awakening) { return false; }
            return true;
        }

        public override void OnDestroy()
        {
            if (base.healthHaver)
            {
                base.healthHaver.OnPreDeath -= this.OnPreDeath;
            }
            base.OnDestroy();
        }
        private void OnPreDeath(Vector2 obj)
        {
            base.StartCoroutine(this.DIE());
        }


        private IEnumerator DIE()
        {
           
            yield break;
        }
        public AdvancedBodyPartController leftArm;
        public AdvancedBodyPartController rightArm;
    }
}

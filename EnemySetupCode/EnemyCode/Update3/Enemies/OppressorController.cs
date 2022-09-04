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
            for (int i = 0; i < base.aiActor.gameObject.transform.childCount; i++)
            {
                var tr = base.aiActor.gameObject.transform.GetChild(i);
                if (tr.gameObject != null && tr.gameObject.GetComponent<AdvancedBodyPartController>() != null)
                {
                    var mhm = tr.gameObject.GetComponent<AdvancedBodyPartController>();
                    if (mhm != null)
                    {
                        //this is jank, but whatever
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

        public AdvancedBodyPartController leftArm;
        public AdvancedBodyPartController rightArm;



        private IEnumerator DestroyArm(bool IsLeft)
        {
            AkSoundEngine.PostEvent("Play_CHR_shadow_curse_01", base.gameObject);
            base.behaviorSpeculator.InterruptAndDisable();
            base.aiAnimator.PlayUntilFinished("pain", true, null, 1, false);
            yield return new WaitForSeconds(base.aiAnimator.CurrentClipLength);
            base.behaviorSpeculator.enabled = true;
            yield break;
        }


        private void Update()
        {
            if (!base.aiActor.HasBeenEngaged)
            {

            }
        }
        
      
        protected override void OnDestroy()
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

    }

}

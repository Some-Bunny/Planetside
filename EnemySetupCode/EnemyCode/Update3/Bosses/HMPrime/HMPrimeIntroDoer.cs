using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside
{
	public class HMPrimeIntroController : SpecificIntroDoer
    {
        public override void EndIntro()
        {
            if (base.aiActor.aiAnimator != null)
            {
                
                base.aiActor.aiAnimator.IdleAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.FourWay,
                    Prefix = "active",
                    AnimNames = new string[]
                    {
                        "active_top_right",
                        "active_bottom_right",
                        "active_bottom_left",
                        "active_top_left",
                    },
                    Flipped = new DirectionalAnimation.FlipType[4]
                };

                base.aiActor.aiAnimator.OverrideIdleAnimation = "active";
                //base.aiActor.aiAnimator.OverrideMoveAnimation = "active";
                if (base.aiAnimator.ChildAnimator != null)
                {
                    base.aiAnimator.ChildAnimator.OverrideIdleAnimation = "active";
                    //base.aiAnimator.ChildAnimator.OverrideMoveAnimation = "active"; ;
                    PlanetsideReflectionHelper.InvokeMethod(typeof(AIAnimator), "UpdateCurrentBaseAnimation", base.aiAnimator.ChildAnimator, new object[] { });

                }
                PlanetsideReflectionHelper.InvokeMethod(typeof(AIAnimator), "UpdateCurrentBaseAnimation", base.aiAnimator, new object[] { });
                base.aiAnimator.EndAnimation();

            }
            base.EndIntro();
          
        }
    }
}

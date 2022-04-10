using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Dungeonator;

namespace Planetside
{
	public class HMPrimeIntroController : SpecificIntroDoer
    {
        private void ProcessAttackGroup(AttackBehaviorGroup attackGroup)
        {
            for (int i = 0; i < attackGroup.AttackBehaviors.Count; i++)
            {
                AttackBehaviorGroup.AttackGroupItem attackGroupItem = attackGroup.AttackBehaviors[i];
                if (attackGroup != null && attackGroupItem.NickName == "BigLaser")
                {
                    attackGroupItem.Probability = 2f;
                }
            }
        }
        public override void EndIntro()
        {
            if (base.aiActor.aiAnimator != null)
            {
                GameUIBossHealthController gameUIBossHealthController = GameUIRoot.Instance.bossController;
                gameUIBossHealthController.RegisterBossHealthHaver(base.healthHaver, StringTableManager.GetEnemiesString(base.healthHaver.encounterTrackable.journalData.PrimaryDisplayName, -1));

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
                if (base.aiAnimator.ChildAnimator != null)
                {
                    base.aiAnimator.ChildAnimator.OverrideIdleAnimation = "active";
                    PlanetsideReflectionHelper.InvokeMethod(typeof(AIAnimator), "UpdateCurrentBaseAnimation", base.aiAnimator.ChildAnimator, new object[] { });

                }
                PlanetsideReflectionHelper.InvokeMethod(typeof(AIAnimator), "UpdateCurrentBaseAnimation", base.aiAnimator, new object[] { });
                base.aiAnimator.EndAnimation();

            }
            AkSoundEngine.PostEvent("Play_HornofWar", base.aiActor.gameObject);


            RobotShopkeeperEngageDoer engager = base.aiActor.GetComponent<RobotShopkeeperEngageDoer>();
            if (engager.AmountOfPurchases > 3)
            {
                base.aiActor.MovementSpeed *= 1.2f;
                base.aiActor.healthHaver.AllDamageMultiplier *= 0.85f;
                for (int j = 0; j < base.aiActor.behaviorSpeculator.AttackBehaviors.Count; j++)
                {
                    if (base.behaviorSpeculator.AttackBehaviors[j] is AttackBehaviorGroup && base.behaviorSpeculator.AttackBehaviors[j] != null)
                    {
                        this.ProcessAttackGroup(base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup);
                    }
                }
                List<AIAnimator.NamedDirectionalAnimation> anims = base.aiActor.aiAnimator.OtherAnimations;
                foreach (AIAnimator.NamedDirectionalAnimation anim in anims)
                {
                    if (anim.name == "death")
                    {
                        anim.name = "LOL";
                    }
                    if (anim.name == "death_proper")
                    {
                        anim.name = "death";
                    }
                }
            }

            base.EndIntro();
          
        }
    }
}

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
        public bool isActuallyEnded;
        public void Start()
        {
            isActuallyEnded = false;
        }

        private void ProcessAttackGroup(AttackBehaviorGroup attackGroup)
        {
            for (int i = 0; i < attackGroup.AttackBehaviors.Count; i++)
            {
                AttackBehaviorGroup.AttackGroupItem attackGroupItem = attackGroup.AttackBehaviors[i];
                if (attackGroup != null && attackGroupItem.NickName == "BigLaser")
                {
                    attackGroupItem.Probability = 4f;
                }
            }
        }
        public override void EndIntro()
        {
            isActuallyEnded = true;

            base.aiActor.healthHaver.flashesOnDamage = true;
            base.aiActor.healthHaver.IsVulnerable = true;

            GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
            if (lastLoadedLevelDefinition != null)
            {
                base.aiActor.healthHaver.m_damageCap = lastLoadedLevelDefinition.damageCap;
                float num = 1f;
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                {
                    num = (GameManager.Instance.COOP_ENEMY_HEALTH_MULTIPLIER + 2f) / 2f;
                }
                base.aiActor.healthHaver.m_bossDpsCap = lastLoadedLevelDefinition.bossDpsCap * num;
            }

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
            if (base.aiActor.EffectResistances == null)
            {
                base.aiActor.EffectResistances = new ActorEffectResistance[0];
            }
            List<ActorEffectResistance> l = base.aiActor.EffectResistances.ToList();

            switch (engager.AmountOfPurchases)
            {
                case 1:
                    return;
                case 2:

                    base.aiActor.SetResistance(EffectResistanceType.Freeze, 1);
                    //l.Add(new ActorEffectResistance() { resistAmount = 1000, resistType = EffectResistanceType.Freeze });
                    //base.aiActor.EffectResistances = l.ToArray();
                    return;

                case 3:
                    base.aiActor.SetResistance(EffectResistanceType.Freeze, 1);
                    base.aiActor.SetResistance(EffectResistanceType.Poison, 1);

                    //l.Add(new ActorEffectResistance() { resistAmount = 1000, resistType = EffectResistanceType.Freeze });
                    //l.Add(new ActorEffectResistance() { resistAmount = 1000, resistType = EffectResistanceType.Poison });
                    //base.aiActor.EffectResistances = l.ToArray();

                    return;
                case 4:
                    base.aiActor.SetResistance(EffectResistanceType.Freeze, 1);
                    base.aiActor.SetResistance(EffectResistanceType.Poison, 1);
                    base.aiActor.SetResistance(EffectResistanceType.Fire, 1);

                    //l.Add(new ActorEffectResistance() { resistAmount = 1000, resistType = EffectResistanceType.Fire });
                    //l.Add(new ActorEffectResistance() { resistAmount = 1000, resistType = EffectResistanceType.Poison });
                    //l.Add(new ActorEffectResistance() { resistAmount = 1000, resistType = EffectResistanceType.Freeze });
                    //base.aiActor.EffectResistances = l.ToArray();
                    return;
            }
        }
    }
}

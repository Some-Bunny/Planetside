using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using Alexandria;
using static Planetside.NeighborinoBullet;

namespace Planetside
{
	public class LordRiceBullet : AIActor
	{
		public static void Init()
		{
			var enemy = EnemyToolbox.CreateNewBulletBankerEnemy("lord_rice_bullet", "Lord Rice", 21, 20, 
				new List<int> { 268, 267, 265, 261 }, 
				new List<int> { 264, 266, 262, 263 }, null, new SkellScript(), 2.5f);
			enemy.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Lore_Gunjurer_GUID).bulletBank.GetBullet("mage"));
            enemy.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Lore_Gunjurer_GUID).bulletBank.GetBullet("mage_fireball"));

        }
        public class SkellScript : Script 
		{
			public override IEnumerator Top() 
			{
                base.EndOnBlank = true;
                yield return base.Wait(15);
                HealthHaver healthHaver = base.BulletBank.healthHaver;
                float startingHealth = healthHaver.GetCurrentHealth();
                AkSoundEngine.PostEvent("Play_ENM_wizard_summon_01", this.BulletBank.gameObject);
                for (int i = 0; i < 3; i++)
                {
                    Bullet newBullet = new TheWizard();
                    base.Fire(new Direction(120f * i, DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), newBullet);
                }
                for (int j = 0; j < 90; j++)
                {
                    if (!healthHaver || healthHaver.IsDead || healthHaver.GetCurrentHealth() < startingHealth || HasLostTarget(this))
                    {
                        base.ForceEnd();
                        yield break;
                    }
                    yield return base.Wait(1);
                }
            }
            protected static bool HasLostTarget(Bullet bullet)
            {
                AIActor aiActor = bullet.BulletBank.aiActor;
                return aiActor && !aiActor.TargetRigidbody && aiActor.CanTargetEnemies && !aiActor.CanTargetPlayers;
            }
        }

        public class TheWizard : WizardPurpleHomingShots1.StoryBullet
        {
            public TheWizard() : base("mage", 0.5f)
            {

            }

            public override IEnumerator Top()
            {
                this.ChangeSpeed(new Brave.BulletScript.Speed(0), 90);
                tk2dSpriteAnimator spriteAnimator = this.Projectile.spriteAnimator;
                yield return base.Wait(90);
                this.Speed = 2.5f;
                this.Direction = base.OffsetAimDirection;
                int shotsFired = 0;
                int cooldown = UnityEngine.Random.Range(75, 105);
                for (int i = 0; i < 1200; i++)
                {
                    if (base.HasLostTarget())
                    {
                        base.Vanish(false);
                        yield break;
                    }
                    if (!spriteAnimator.Playing)
                    {
                        spriteAnimator.Play(spriteAnimator.DefaultClip);
                    }
                    if (shotsFired < 6)
                    {
                        cooldown--;
                        if (cooldown == 12)
                        {
                            spriteAnimator.Play("enemy_projectile_mage_fire");
                        }
                        if (cooldown <= 0)
                        {
                            float num = BraveMathCollege.ClampAngle360(this.Direction);
                            int num2 = (num <= 90f || num >= 270f) ? 1 : -1;
                            base.Fire(new Offset(PhysicsEngine.PixelToUnit(new IntVector2(num2 * 5, 12)), 0f, string.Empty, DirectionType.Absolute), new Direction(base.GetAimDirection(1f, 10f), DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new Bullet("mage_fireball", false, false, false));
                            shotsFired++;
                            cooldown = 60;
                        }
                    }
                    else if (shotsFired == 6 && cooldown > 0)
                    {
                        cooldown--;
                        base.Vanish(false);
                    }
                    base.ChangeDirection(new Direction(base.OffsetAimDirection, DirectionType.Absolute, 3f), 1);
                    if (this.Projectile)
                    {
                        this.Projectile.Direction = BraveMathCollege.DegreesToVector(this.Direction, 1f);
                    }
                    yield return base.Wait(1);
                }
                base.Vanish(false);
                yield break;
            }
            private const int ShotCooldown = 60;
            private const int NumBullets = 3;
        }

    }
}









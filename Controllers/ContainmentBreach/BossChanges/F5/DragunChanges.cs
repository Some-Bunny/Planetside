using System;
using System.Collections;
using System.Collections.Generic;
using Brave.BulletScript;
using Dungeonator;
using FullInspector;
using UnityEngine;
using ItemAPI;
using EnemyBulletBuilder;

namespace Planetside
{


	public class DragunChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "465da2bb086a4a88a803f79fe3a27677"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 2; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableFrogger);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSkullAudio);


			DraGunNearDeathBehavior DraGunNearDeathBehavior = behaviorSpec.AttackBehaviors[0] as DraGunNearDeathBehavior;
			foreach (AttackBehaviorGroup.AttackGroupItem attackGroupItem in DraGunNearDeathBehavior.Attacks.AttackBehaviors)
            {

				ShootBehavior shoot = attackGroupItem.Behavior as ShootBehavior;
				shoot.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunNegativeSpace1));
				//ETGModConsole.Log(shoot.BulletScript.scriptTypeName.ToString());
			}


			DraGunRPGBehavior DraGunRPGBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as DraGunRPGBehavior;
			DraGunRPGBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunRocket1));

			SimultaneousAttackBehaviorGroup SimultaneousAttackBehaviorGroup1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as SimultaneousAttackBehaviorGroup;
			foreach (AttackBehaviorBase attackBehaviorBase in SimultaneousAttackBehaviorGroup1.AttackBehaviors)
			{
				if (attackBehaviorBase is DraGunThrowKnifeBehavior knifeBehavior)
                {
					knifeBehavior.delay = 0;
					knifeBehavior.AttackCooldown *= 2;
				}
            }


			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunFlameBreath1));

			ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
			ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunBigNoseShot));

			ShootBehavior ShootBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[4].Behavior as ShootBehavior;
			ShootBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunWingFlap1));
			ShootBehavior3.AttackCooldown = (ShootBehavior3.AttackCooldown*2)+0.5f;
			DraGunHeadShootBehavior DraGunHeadShootBehavior1= behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as DraGunHeadShootBehavior;
			DraGunHeadShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunSweepFlameBreath1));


			SimultaneousAttackBehaviorGroup SimultaneousAttackBehaviorGroup2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[6].Behavior as SimultaneousAttackBehaviorGroup;
			foreach (AttackBehaviorBase attackBehaviorBase in SimultaneousAttackBehaviorGroup2.AttackBehaviors)
			{
				//ETGModConsole.Log(attackBehaviorBase.GetType().ToString());
				if (attackBehaviorBase is DraGunMac10Behavior behav)
				{
					behav.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunMac10Burst1));
					//ETGModConsole.Log(behav.BulletScript.scriptTypeName);
				}
				//ETGModConsole.Log("-----------------");
			}
			//ETGModConsole.Log("=========");
			SimultaneousAttackBehaviorGroup SimultaneousAttackBehaviorGroup3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[7].Behavior as SimultaneousAttackBehaviorGroup;

			if (SimultaneousAttackBehaviorGroup3.AttackBehaviors[0] is DraGunGlockBehavior ae)
			{
				foreach (DraGunGlockBehavior.GlockAttack a in ae.attacks)
				{
					a.bulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunGlockDirected1Left1));
					//ETGModConsole.Log(a.bulletScript.scriptTypeName);
				}
			}
			if (SimultaneousAttackBehaviorGroup3.AttackBehaviors[1] is DraGunGlockBehavior ea)
			{
				foreach (DraGunGlockBehavior.GlockAttack a in ea.attacks)
				{
					a.bulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunGlockDirected1Right1));
					//ETGModConsole.Log(a.bulletScript.scriptTypeName);
				}
			}

			
			//ETGModConsole.Log("=========");
			SimultaneousAttackBehaviorGroup SimultaneousAttackBehaviorGroup4 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[8].Behavior as SimultaneousAttackBehaviorGroup;
			foreach (AttackBehaviorBase attackBehaviorBase in SimultaneousAttackBehaviorGroup4.AttackBehaviors)
			{
				//ETGModConsole.Log(attackBehaviorBase.GetType().ToString());
				if (attackBehaviorBase is DraGunGlockBehavior behav)
				{
					foreach (DraGunGlockBehavior.GlockAttack a in behav.attacks)
					{
						//ETGModConsole.Log(a.bulletScript.scriptTypeName);
					}
				}
				//ETGModConsole.Log("-----------------");
			}
			//ETGModConsole.Log("=========");
			SimultaneousAttackBehaviorGroup SimultaneousAttackBehaviorGroup5 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[9].Behavior as SimultaneousAttackBehaviorGroup;
			foreach (AttackBehaviorBase attackBehaviorBase in SimultaneousAttackBehaviorGroup5.AttackBehaviors)
			{
				//ETGModConsole.Log(attackBehaviorBase.GetType().ToString());
				if (attackBehaviorBase is DraGunGlockBehavior behav)
				{
					foreach (DraGunGlockBehavior.GlockAttack a in behav.attacks)
					{
						//ETGModConsole.Log(a.bulletScript.scriptTypeName);
					}
				}
				//ETGModConsole.Log("-----------------");
			}
			//ETGModConsole.Log("=========");
			
			/*
			/*
			BeholsterLaserBehavior shootLaser = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[4].Behavior as BeholsterLaserBehavior;
			shootLaser.trackingType = BeholsterLaserBehavior.TrackingType.ConstantTurn;
			shootLaser.maxTurnRate = 144;
			*/
			//behaviorSpec.aiActor.CanTargetPlayers = true;
			//behaviorSpec.aiActor.CanTargetEnemies = false;

			// For this first change, we're just going to increase the lead amount of the bullet kin's ShootGunBehavior so its shots fire like veteran kin.
			//ShootGunBehavior shootGunBehavior = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as ShootGunBehavior;
			//ShootBehavior shootGunBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as ShootBehavior;
			//ShootBehavior shootGunBehavior = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
			//ShootBehavior shootGunBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as ShootBehavior;
			//ShootGunBehavior shootGunBehavior = behaviorSpec.AttackBehaviors[0] as ShootGunBehavior;
			//base.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
			//base.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));
			//yah.chargeSpeed = 30;
			//yah.chargeRange = 30;
			//yah.AttackCooldown = 1f;
			//yah.delayWallRecoil = false;
			//yah.wallRecoilForce = 160000;	
			//shootGunBehavior.WeaponType = WeaponType.BulletScript;

			//shootGunBehavior.BulletScript = new CustomBulletScriptSelector(typeof(Bullettest));



			// Next, we're going to change another few things on the ShootGunBehavior so that it has a custom BulletScript.
			// Makes it so the bullet kin will shoot our bullet script instead of his own gun shot.
			//shootGunBehavior.BulletScript = new CustomBulletScriptSelector(typeof(Death)); // Sets the bullet kin's bullet script to our custom bullet script.
			//behaviorSpec.aiActor.get
		}
		//public AIBulletBank.Entry ThesporeEntry;





		public class ModifiedDraGunNegativeSpace1 : ScriptLite
		{
			private List<bool> m_centerBullets;

			public override void Start()
			{
				this.m_centerBullets = new List<bool>();
				J = new List<Vector2>();
				this.ActivePlatformRadius = ((!ChallengeManager.CHALLENGE_MODE_ACTIVE) ? 4.5f : 4f);
				int num = 10;
				this.m_platformCenters = new List<Vector2>(num);
				this.m_platformCenters.Add(new Vector2(UnityEngine.Random.Range(-17f, 17f), 0f));
				for (int i = 1; i < num; i++)
				{
					Vector2 a = this.m_platformCenters[i - 1];
					ModifiedDraGunNegativeSpace1.s_validPlatformIndices.Clear();
					if (i % 3 == 0 && !ChallengeManager.CHALLENGE_MODE_ACTIVE)
					{
						ModifiedDraGunNegativeSpace1.s_validPlatformIndices.Add(2);
					}
					else
					{
						for (int j = 0; j < ModifiedDraGunNegativeSpace1.PlatformAngles.Length; j++)
						{
							if (j != 2)
							{
								Vector2 vector = a + BraveMathCollege.DegreesToVector(ModifiedDraGunNegativeSpace1.PlatformAngles[j], 2f * this.ActivePlatformRadius + ModifiedDraGunNegativeSpace1.PlatformDistances[j]);
								if (vector.x > -17f && vector.x < 17f)
								{
									ModifiedDraGunNegativeSpace1.s_validPlatformIndices.Add(j);
									
								}
							}
						}
					}
					int num2 = BraveUtility.RandomElement<int>(ModifiedDraGunNegativeSpace1.s_validPlatformIndices);
					this.m_platformCenters.Add(a + BraveMathCollege.DegreesToVector(ModifiedDraGunNegativeSpace1.PlatformAngles[num2], 2f * this.ActivePlatformRadius + ModifiedDraGunNegativeSpace1.PlatformDistances[num2]));
					
				}
				this.m_verticalGap = 1.6f;
				this.m_lastCenterHeight = this.m_platformCenters[this.m_platformCenters.Count - 1].y;
				this.m_rowHeight = 0f;
				this.m_centerBullets.Clear();

				for (int l = 0; l < this.m_platformCenters.Count; l++)
				{
					this.m_centerBullets.Add(false);
				}
			}

			public List<Vector2> J = new List<Vector2>();
			public override int Update(ref int state)
			{
				if (state != 0)
				{
					return base.Done();
				}
				if (this.m_rowHeight < this.m_lastCenterHeight)
				{
					for (int i = 0; i < 19; i++)
					{
						float num = base.SubdivideRange(-17f, 17f, 19, i, false);
						Vector2 a = new Vector2(num, this.m_rowHeight);
						bool suppressOffset = false;
						for (int j = 0; j < this.m_platformCenters.Count; j++)
						{
							if (Vector2.Distance(a, this.m_platformCenters[j]) < this.ActivePlatformRadius)
							{
								Vector2 vector;
								Vector2 vector2;
								int num2 = BraveMathCollege.LineCircleIntersections(this.m_platformCenters[j], this.ActivePlatformRadius, new Vector2(-17f, this.m_rowHeight), new Vector2(17f, this.m_rowHeight), out vector, out vector2);
								if (num2 == 1)
								{
									num = vector.x;
								}
								else
								{
									num = ((Mathf.Abs(num - vector.x) >= Mathf.Abs(num - vector2.x)) ? vector2.x : vector.x);
								}
								suppressOffset = true;
							}
							
						}
						base.Fire(new Offset(num, 18f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(4.66f, SpeedType.Absolute), new ModifiedDraGunNegativeSpace1.WiggleBullet(suppressOffset));
					}
					for (int k = 0; k < this.m_platformCenters.Count; k++)
					{
						if (!this.m_centerBullets[k] && this.m_platformCenters[k].y < this.m_rowHeight - 2f)
						{
							base.Fire(new Offset(this.m_platformCenters[k].x, (20.5f - this.m_rowHeight + this.m_platformCenters[k].y)-2, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(4.66f, SpeedType.Absolute), new ModifiedDraGunNegativeSpace1.WiggleBullet(true, StaticUndodgeableBulletEntries.UndodgeableFrogger.Name));
							this.m_centerBullets[k] = true;
						}
					}

					this.m_rowHeight += this.m_verticalGap;
					return base.Wait(16);
				}
				state++;
				return base.Wait(120);
			}

			public List<Vector2> spawnedCenters;

			static ModifiedDraGunNegativeSpace1()
			{
				ModifiedDraGunNegativeSpace1.PlatformAngles = new float[]
				{
			165f,
			135f,
			90f,
			65f,
			35,
				};
				ModifiedDraGunNegativeSpace1.PlatformDistances = new float[]
				{
			2f,
			2.33f,
			2.66f,
			2.33f,
			2
				};
				ModifiedDraGunNegativeSpace1.s_validPlatformIndices = new List<int>();
			}

			private const int NumPlatforms = 10;
			private const int NumBullets = 19;
			private const int RowDelay = 16;
			private const float HalfRoomWidth = 17f;
			private const int PlatformRadius = 4;
			private static float[] PlatformAngles;
			private static float[] PlatformDistances;
			private static List<int> s_validPlatformIndices;
			private float ActivePlatformRadius;
			private List<Vector2> m_platformCenters;
			private float m_verticalGap;
			private float m_lastCenterHeight;
			private float m_rowHeight;
			public class WiggleBullet : BulletLite
			{
				public WiggleBullet(bool suppressOffset, string bankName = "default_novfx") : base(bankName, false, false)
				{
					this.m_suppressOffset = suppressOffset;
				}

				public override void Start()
				{
					
					//string str = this.Projectile.GetComponentInChildren<MarkForUndodgeAbleBullet>() != null ? "blue bullet" : "normal Bullet";
					//ETGModConsole.Log(str + " sort order:");
					//ETGModConsole.Log("projectile sort order: "+ this.Projectile.renderer.sortingOrder != null, this.Projectile.GetComponent<MarkForUndodgeAbleBullet>() != null ? true : false);
					//ETGModConsole.Log("sprite sort order: " + this.Projectile.sprite.renderer.sortingOrder, this.Projectile.GetComponent<MarkForUndodgeAbleBullet>() != null ? true : false);
					//ETGModConsole.Log("sprite sort order: " + this.Projectile.sprite.renderer.sortingLayerID, this.Projectile.GetComponent<MarkForUndodgeAbleBullet>() != null ? true : false);

					base.ManualControl = true;
					this.m_truePosition = base.Position;
					this.m_offset = Vector2.zero;
					this.m_xMagnitude = UnityEngine.Random.Range(0f, 0.6f);
					this.m_xPeriod = UnityEngine.Random.Range(1f, 2.5f);
					this.m_yMagnitude = UnityEngine.Random.Range(0f, 0.4f);
					this.m_yPeriod = UnityEngine.Random.Range(1f, 2.5f);
					this.m_delta = BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
				}

				public override int Update(ref int state)
				{
					if (base.Tick >= 450)
					{
						base.Vanish(false);
						return base.Done();
					}
					if (!this.m_suppressOffset)
					{
						float num = 0.5f + (float)base.Tick / 60f * this.m_xPeriod;
						num = Mathf.Repeat(num, 2f);
						float num2 = 1f - Mathf.Abs(num - 1f);
						num2 = Mathf.Clamp01(num2);
						num2 = (float)(-2.0 * (double)num2 * (double)num2 * (double)num2 + 3.0 * (double)num2 * (double)num2);
						this.m_offset.x = (float)((double)this.m_xMagnitude * (double)num2 + (double)(-(double)this.m_xMagnitude) * (1.0 - (double)num2));
						float num3 = 0.5f + (float)base.Tick / 60f * this.m_yPeriod;
						num3 = Mathf.Repeat(num3, 2f);
						float num4 = 1f - Mathf.Abs(num3 - 1f);
						num4 = Mathf.Clamp01(num4);
						num4 = (float)(-2.0 * (double)num4 * (double)num4 * (double)num4 + 3.0 * (double)num4 * (double)num4);
						this.m_offset.y = (float)((double)this.m_yMagnitude * (double)num4 + (double)(-(double)this.m_yMagnitude) * (1.0 - (double)num4));
					}
					this.m_truePosition += this.m_delta;
					base.Position = this.m_truePosition + this.m_offset;
					return base.Wait(1);
				}

				private bool m_suppressOffset;
				private Vector2 m_truePosition;
				private Vector2 m_offset;
				private float m_xMagnitude;
				private float m_xPeriod;
				private float m_yMagnitude;
				private float m_yPeriod;
				private Vector2 m_delta;
			}
		}


		public class ModifiedDraGunGlockDirected1Right1 : ModifiedDraGunGlockDirected1
		{
			protected override string BulletName
			{
				get
				{
					return "glockRight";
				}
			}
			protected override bool IsHard
			{
				get
				{
					return true;
				}
			}
		}
		public class ModifiedDraGunGlockDirected1Left1 : ModifiedDraGunGlockDirected1
		{

			protected override string BulletName
			{
				get
				{
					return "glockLeft";
				}
			}
			protected override bool IsHard
			{
				get
				{
					return true;
				}
			}
		}


		public class ModifiedDraGunGlockDirected1 : Script
		{
			protected virtual string BulletName
			{
				get
				{
					return "glock";
				}
			}

			protected virtual bool IsHard
			{
				get
				{
					return false;
				}
			}

			protected override IEnumerator Top()
			{
				float num = BraveMathCollege.ClampAngle180(this.Direction);
				if (num > -91f && num < -89f)
				{
					int num2 = 8;
					float startAngle = -170f;
					for (int i = 0; i < num2; i++)
					{
						base.Fire(new Direction(base.SubdivideArc(startAngle, 160f, num2, i, false), DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new Bullet(this.BulletName, false, false, false));
					}
					if (this.IsHard)
					{
						for (int j = 0; j < num2 - 1; j++)
						{
							base.Fire(new Direction(base.SubdivideArc(startAngle, 160f, num2, j, true), DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(this.BulletName, 9f, 60, -1, false));
						}
					}
					float aimDirection = base.AimDirection;
					if (BraveMathCollege.AbsAngleBetween(aimDirection, -90f) <= 90f)
					{
						base.Fire(new Direction(base.AimDirection, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new Bullet(this.BulletName, false, false, false));
					}
					float startAngle3 = base.RandomAngle();
					for (int i = 0; i < 7; i++)
					{
						base.Fire(new Direction(base.SubdivideCircle(startAngle3, 7, i, 1f, false), DirectionType.Absolute, -1f), new Speed(3f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 8, 90));
					}
				}
				else
				{
					int num3 = 12;
					float startAngle2 = base.RandomAngle();
					for (int k = 0; k < num3; k++)
					{
						base.Fire(new Direction(base.SubdivideCircle(startAngle2, num3, k, 1f, false), DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new Bullet(this.BulletName + "_spin", false, false, false));
					}
					if (this.IsHard)
					{
						for (int l = 0; l < num3; l++)
						{
							base.Fire(new Direction(base.SubdivideCircle(startAngle2, num3, l, 1f, true), DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(this.BulletName + "_spin", 9f, 60, -1, false));
						}
					}
					base.Fire(new Direction(base.AimDirection, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new Bullet(this.BulletName, false, false, false));
					float startAngle3 = base.RandomAngle();
					for (int i = 0; i < 7; i++)
					{
						base.Fire(new Direction(base.SubdivideCircle(startAngle3, 7, i, 1f, false), DirectionType.Absolute, -1f), new Speed(3f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 8, 90));
					}
				}
				return null;
			}
		}



		public class ModifiedDraGunMac10Burst1 : Script
		{
			protected override IEnumerator Top()
			{
				int i = 0;
				for (; ; )
				{
					i++;
					if (i % 5 == 0)
					{
						this.Fire(new Direction(0, DirectionType.Relative, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 10, 180));
					}
					this.Fire(new Direction(UnityEngine.Random.Range(-30f, 30f), DirectionType.Relative, -1f), new Speed(12f, SpeedType.Absolute), new Bullet("UziBurst", false, false, false));
					yield return this.Wait(UnityEngine.Random.Range(2, 3));
				}
			}
		}

		public class ModifiedDraGunSweepFlameBreath1 : Script
		{
			protected override IEnumerator Top()
			{
				int i =0;
				for (; ; )
				{
					i++;
					if (i % 4 == 0) 
					{
						this.Fire(new Direction(UnityEngine.Random.Range(-22.5f, 22.5f), DirectionType.Relative, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 12, 180));
					}
					this.Fire(new Direction(UnityEngine.Random.Range(-45f, 45f), DirectionType.Relative, -1f), new Speed(14f, SpeedType.Absolute), new Bullet("Sweep", false, false, false));
					yield return this.Wait(1);
				}
			}
		}
		public class ModifiedDraGunWingFlap1 : Script
		{
			protected override IEnumerator Top()
			{
				this.EndOnBlank = true;
				for (int i = 0; i < 24; i++)
				{
					float y1 = UnityEngine.Random.Range(0f, 5f);
					float y2 = UnityEngine.Random.Range(0f, 5f);
					this.Fire(new Offset(-17f + y1, 18f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new ModifiedDraGunWingFlap1.WindProjectile(1f));
					this.Fire(new Offset(17f - y2, 18f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new ModifiedDraGunWingFlap1.WindProjectile(-1f));

					this.Fire(new Offset(-17f, 18f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new ModifiedDraGunWingFlap1.DodgeRollCheesePreventionProjectile(1f, UnityEngine.Random.Range(119, 126)));
					this.Fire(new Offset(17f, 18f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new ModifiedDraGunWingFlap1.DodgeRollCheesePreventionProjectile(-1f, UnityEngine.Random.Range(119, 126)));

					yield return this.Wait(10);
				}
				yield return this.Wait(60);
				yield break;
			}

			private const int NumBullets = 30;

			public class WindProjectile : Bullet
			{
				public WindProjectile(float sign) : base(null, false, false, false)
				{
					this.m_sign = sign;
				}

				protected override IEnumerator Top()
				{
					yield return this.Wait(UnityEngine.Random.Range(60, 126));
					this.ChangeDirection(new Direction(-90f + this.m_sign * 90f, DirectionType.Absolute, -1f), 30);
					this.ChangeSpeed(new Brave.BulletScript.Speed(6), 90);
					yield break;
				}
				private float m_sign;
			}
			public class DodgeRollCheesePreventionProjectile : Bullet
			{
				public DodgeRollCheesePreventionProjectile(float sign, float wait) : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false)
				{
					this.m_sign = sign;
					this.WaitTime = wait;
				}

				protected override IEnumerator Top()
				{
					yield return this.Wait(WaitTime);
					this.ChangeDirection(new Direction(-90f + this.m_sign * 90f, DirectionType.Absolute, -1f), 30);
					yield break;
				}
				private float m_sign;
				private float WaitTime;

			}

		}


		public class ModifiedDraGunBigNoseShot : Script
		{
			protected override IEnumerator Top()
			{
				base.Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableSkullAudio.Name, false, false, false));
				base.Fire(new Direction(-110f, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableSkullAudio.Name, false, false, false));
				base.Fire(new Direction(-130f, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableSkullAudio.Name, false, false, false));
				base.Fire(new Direction(-70f, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableSkullAudio.Name, false, false, false));
				base.Fire(new Direction(-50f, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableSkullAudio.Name, false, false, false));
				if (ChallengeManager.CHALLENGE_MODE_ACTIVE)
				{
					base.Fire(new Direction(-60f, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableSkullAudio.Name, false, false, false));
					base.Fire(new Direction(-80f, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableSkullAudio.Name, false, false, false));
					base.Fire(new Direction(-100f, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableSkullAudio.Name, false, false, false));
					base.Fire(new Direction(-120f, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableSkullAudio.Name, false, false, false));
				}
				return null;
			}
		}

		public class ModifiedDraGunFlameBreath1 : Script
		{
			protected override IEnumerator Top()
			{
				bool q = true;
				for (int i = 0; i < 120; i++)
				{
					if (i % 3 == 0)
                    {
						this.Fire(new Direction(UnityEngine.Random.Range(-24f, 24f), DirectionType.Aim, -1f), new Speed(11.5f, SpeedType.Absolute), new SpeedChangingBullet("Breath", 6, 180));
					}
					if (i % 2 == 0)
                    {
						if (q == true)
                        {
							this.Fire(new Direction(UnityEngine.Random.Range(-23f, -32f), DirectionType.Aim, -1f), new Speed(14f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false));
						}
						else
                        {
							this.Fire(new Direction(UnityEngine.Random.Range(23f, 32f), DirectionType.Aim, -1f), new Speed(14f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false));
						}
						q = !q;
					}
					yield return this.Wait(2);
				}
				yield break;
			}
			private const int NumBullets = 80;
		}


		public class ModifiedDraGunRocket1 : Script
		{
			protected override IEnumerator Top()
			{
				if (ChallengeManager.CHALLENGE_MODE_ACTIVE)
				{
					if (UnityEngine.Random.value < 0.5f)
					{
						base.Fire(new Direction(-60f, DirectionType.Absolute, -1f), new Speed(40f, SpeedType.Absolute), new ModifiedDraGunRocket1.Rocket());
						base.Fire(new Direction(-120f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new ModifiedDraGunRocket1.Rocket());
					}
					else
					{
						base.Fire(new Direction(-60f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new ModifiedDraGunRocket1.Rocket());
						base.Fire(new Direction(-120f, DirectionType.Absolute, -1f), new Speed(40f, SpeedType.Absolute), new ModifiedDraGunRocket1.Rocket());
					}
				}
				else
				{
					base.Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(40f, SpeedType.Absolute), new ModifiedDraGunRocket1.Rocket());
				}
				return null;
			}

			private const int NumBullets = 42;

			public class Rocket : Bullet
			{
				public Rocket() : base("rocket", false, false, false)
				{
				}

				protected override IEnumerator Top()
				{
					return null;
				}

				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					int r = 0;
					bool q = UnityEngine.Random.value > 0.5f ? true : false;
					for (int i = 0; i < 44; i++)
					{
						string a = q == true ? StaticUndodgeableBulletEntries.UndodgeableFrogger.Name : "default_novfx";
						base.Fire(new Direction(base.SubdivideArc(-10f, 200f, 44, i, false), DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new Bullet(a, false, false, false));
						if (i < 41)
						{
							base.Fire(new Direction(base.SubdivideArc(-10f, 200f, 44, i, true), DirectionType.Absolute, -1f), new Speed(5.5f, SpeedType.Absolute), new SpeedChangingBullet(a, 9f, 70, -1, false));
						}
						base.Fire(new Direction(base.SubdivideArc(-10f, 200f, 44, i, false), DirectionType.Absolute, -1f), new Speed(2.5f, SpeedType.Absolute), new SpeedChangingBullet(a, 9f, 70, -1, false));
						r++;
						if (r > 3) { r = 0; q = !q; }
					}
					
					for (int j = 0; j < 12; j++)
					{
						//base.Fire(new Direction(UnityEngine.Random.Range(20f, 160f), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(10f, 16f), SpeedType.Absolute), new ModifiedDraGunRocket1.ShrapnelBullet());
					}
				}
			}

			public class ShrapnelBullet : Bullet
			{
				public ShrapnelBullet() : base("shrapnel", false, false, false)
				{
				}

				protected override IEnumerator Top()
				{
					this.ManualControl = true;
					yield return this.Wait(UnityEngine.Random.Range(0, 10));
					Vector2 truePosition = this.Position;
					float trueDirection = this.Direction;
					for (int i = 0; i < 360; i++)
					{
						float offsetMagnitude = Mathf.SmoothStep(-0.75f, 0.75f, Mathf.PingPong(0.5f + (float)i / 60f * 3f, 1f));
						Vector2 lastPosition = truePosition;
						truePosition += BraveMathCollege.DegreesToVector(trueDirection, this.Speed / 60f);
						this.Position = truePosition + BraveMathCollege.DegreesToVector(trueDirection - 90f, offsetMagnitude);
						this.Direction = (truePosition - lastPosition).ToAngle();
						this.Projectile.transform.rotation = Quaternion.Euler(0f, 0f, this.Direction);
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				private const float WiggleMagnitude = 0.75f;

				private const float WigglePeriod = 3f;
			}
		}

	}
}

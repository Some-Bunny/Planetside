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
    public class KillPillarsChanges
    {
		public class KillPillarChanges : OverrideBehavior
		{
			public override string OverrideAIActorGUID => "3f11bbbc439c4086a180eb0fb9990cb4"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																							  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
			public override void DoOverride()
			{
				AttackBehaviorGroup attackBehaviorGroup = this.behaviorSpec.AttackBehaviorGroup;
				//ToolsEnemy.DebugInformation(behaviorSpec);

				foreach (AttackBehaviorGroup.AttackGroupItem attackGroupItem in attackBehaviorGroup.AttackBehaviors)
				{
					AttackBehaviorBase behavior = attackGroupItem.Behavior;
					BossStatuesCircleBehavior bossStatuesCircleBehavior2;
					BossStatuesCircleBehavior bossStatuesCircleBehavior = ((bossStatuesCircleBehavior2 = (behavior as BossStatuesCircleBehavior)) != null) ? bossStatuesCircleBehavior2 : null;
					string nickName = attackGroupItem.NickName;
					if (!(nickName == "Shoot Circle (2-4)"))
					{
						if (!(nickName == "Shoot Circle (3)"))
						{
							if (!(nickName == "Shoot Line"))
							{
								if (!(nickName == "Shoot Chaos"))
								{
									if (!(nickName == "Crosshair"))
									{
										if (nickName == "Chase")
										{
											for (int i = 0; i < (bossStatuesCircleBehavior.attackType as BossStatuesPatternBehavior.ConstantAttacks).attacks.Count; i++)
											{
												BossStatuesPatternBehavior.ConstantAttacks.ConstantAttackGroup constantAttackGroup = (bossStatuesCircleBehavior.attackType as BossStatuesPatternBehavior.ConstantAttacks).attacks[i];
												for (int j = 0; j < constantAttackGroup.bulletScript.Count; j++)
												{
													BulletScriptSelector bulletScriptSelector = constantAttackGroup.bulletScript[j];
													bool flag = !bulletScriptSelector.IsNull;
													if (flag)
													{
														constantAttackGroup.bulletScript[j] = new CustomBulletScriptSelector(typeof(ModifiedBossStatuesDirectionalWaveAllSimple));
													}
												}
											}
										}
									}
									else
									{
										(behavior as BossStatuesCrosshairBehavior).BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBossStatuesCrosshair));
									}
								}
								else
								{
									for (int k = 0; k < (bossStatuesCircleBehavior.attackType as BossStatuesPatternBehavior.ConstantAttacks).attacks.Count; k++)
									{
										BossStatuesPatternBehavior.ConstantAttacks.ConstantAttackGroup constantAttackGroup2 = (bossStatuesCircleBehavior.attackType as BossStatuesPatternBehavior.ConstantAttacks).attacks[k];
										for (int l = 0; l < constantAttackGroup2.bulletScript.Count; l++)
										{
											BulletScriptSelector bulletScriptSelector2 = constantAttackGroup2.bulletScript[l];
											//	constantAttackGroup2.bulletScript[l] = new CustomBulletScriptSelector(typeof(KillPillarsScripts.BossStatuesDirectionalWaveAllVerySimple));
										}
									}
								}
							}
							else
							{
								for (int m = 0; m < ((behavior as BossStatuesLineBehavior).attackType as BossStatuesPatternBehavior.ConstantAttacks).attacks.Count; m++)
								{
									BossStatuesPatternBehavior.ConstantAttacks.ConstantAttackGroup constantAttackGroup3 = ((behavior as BossStatuesLineBehavior).attackType as BossStatuesPatternBehavior.ConstantAttacks).attacks[m];
									for (int n = 0; n < constantAttackGroup3.bulletScript.Count; n++)
									{
										BulletScriptSelector bulletScriptSelector3 = constantAttackGroup3.bulletScript[n];
										bool flag2 = !bulletScriptSelector3.IsNull;
										if (flag2)
										{
											constantAttackGroup3.bulletScript[n] = new CustomBulletScriptSelector(typeof(ModifiedBossStatuesDirectionalWaveAllSimple));
										}
									}
								}
							}
						}
						else
						{
							for (int num = 0; num < (bossStatuesCircleBehavior.attackType as BossStatuesPatternBehavior.TimedAttacks).attacks.Count; num++)
							{
								BossStatuesPatternBehavior.TimedAttacks.TimedAttack timedAttack = (bossStatuesCircleBehavior.attackType as BossStatuesPatternBehavior.TimedAttacks).attacks[num];
								//timedAttack.bulletScript = new CustomBulletScriptSelector(typeof(KillPillarsScripts.BossStatuesDirectionalWaveAll));
							}
						}
					}
					else
					{
						for (int num2 = 0; num2 < (bossStatuesCircleBehavior.attackType as BossStatuesPatternBehavior.TimedAttacks).attacks.Count; num2++)
						{
							BossStatuesPatternBehavior.TimedAttacks.TimedAttack timedAttack2 = (bossStatuesCircleBehavior.attackType as BossStatuesPatternBehavior.TimedAttacks).attacks[num2];
							//timedAttack2.bulletScript = new CustomBulletScriptSelector(typeof(KillPillarsScripts.BossStatuesDirectionalWaveAll));
						}
					}
				}
				BossStatuesStompBehavior bossStatuesStompBehavior = this.behaviorSpec.AttackBehaviors[1] as BossStatuesStompBehavior;
				bossStatuesStompBehavior.HangTime = 0.5f;
				bossStatuesStompBehavior.Cooldown = 0.75f;
				for (int num3 = 0; num3 < (bossStatuesStompBehavior.attackType as BossStatuesPatternBehavior.ConstantAttacks).attacks.Count; num3++)
				{
					BossStatuesPatternBehavior.ConstantAttacks.ConstantAttackGroup constantAttackGroup4 = (bossStatuesStompBehavior.attackType as BossStatuesPatternBehavior.ConstantAttacks).attacks[num3];
					for (int num4 = 0; num4 < constantAttackGroup4.bulletScript.Count; num4++)
					{
						BulletScriptSelector bulletScriptSelector4 = constantAttackGroup4.bulletScript[num4];
						bool flag3 = !bulletScriptSelector4.IsNull;
						if (flag3)
						{
							constantAttackGroup4.bulletScript[num4] = new CustomBulletScriptSelector(typeof(ModifiedBossStatuesKaliSlam1));
						}
					}
				}
			}
			public AIBulletBank.Entry lol;



			public class ModifiedBossStatuesDirectionalWaveAllSimple : Script
			{
				protected override IEnumerator Top()
				{
					this.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBullet);

					base.Fire(new Offset("top 1"), new Direction(90f, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new BossStatuesDirectionalWaveAllSimple.EggBullet());
					base.Fire(new Offset("right 1"), new Direction(0f, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new BossStatuesDirectionalWaveAllSimple.EggBullet());
					base.Fire(new Offset("bottom 1"), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new BossStatuesDirectionalWaveAllSimple.EggBullet());
					base.Fire(new Offset("left 1"), new Direction(180f, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new BossStatuesDirectionalWaveAllSimple.EggBullet());

					for (int i = 0; i < 8; i++)
                    {
						Vector2 fixedPosition = this.Position;
						this.Fire(Offset.OverridePosition(fixedPosition), new Direction(45*i, DirectionType.Aim, -1f), new Speed(5, SpeedType.Absolute), new BBullet());
					}
					BossStatuesChaos1.AntiCornerShot(this);
					return null;
				}

				public class BBullet : SpeedChangingBullet
				{
					public BBullet() : base(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBullet.Name, 10, 90)
					{
					}

					protected override IEnumerator Top()
					{
						yield return this.Wait(300);
						this.Vanish(false);
						yield break;
					}
				}

				public class EggBullet : Bullet
				{
					public EggBullet() : base("egg", false, false, false)
					{
					}

					protected override IEnumerator Top()
					{
						this.ChangeSpeed(new Speed(9f, SpeedType.Absolute), 120);
						yield return this.Wait(600);
						this.Vanish(false);
						yield break;
					}
				}
			}


			public class ModifiedBossStatuesKaliSlam1 : Script
			{
				protected override IEnumerator Top()
				{
					this.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableQuickHoming);
					Vector2 fixedPosition = this.Position;
					for (int k = 0; k < 8; k++)
					{
						for (int p = 0; p < 4; p++)
                        {
							this.Fire(Offset.OverridePosition(fixedPosition), new Direction((float)(k * 45), DirectionType.Aim, -1f), new Speed(7f + (p*1.25f), SpeedType.Absolute), new SpeedChangingBullet("egg", 12, 90));
						}
					}


					for (int i = 0; i < 12; i++)
					{
						this.Fire(Offset.OverridePosition(fixedPosition), new Direction((float)(i * 30) + 15, DirectionType.Aim, -1f), new Speed(4, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.undodgeableQuickHoming.Name, 13, 90));
						this.Fire(Offset.OverridePosition(fixedPosition), new Direction((float)(i * 30), DirectionType.Aim, -1f), new Speed(7, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.undodgeableQuickHoming.Name, 10, 60));

					}

					yield break;
				}
				public class SpiralBullet1 : Bullet
				{
					public SpiralBullet1(float flip) : base("spiralbullet1", false, false, false)
					{
						H = flip;
					}

					protected override IEnumerator Top()
					{
						this.ChangeDirection(new Direction(H, DirectionType.Sequence, -1f), 1);
						this.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 180);
						yield return this.Wait(600);
						this.Vanish(false);
						yield break;
					}
					public float H;
				}
				
			}


			public void OverrideAllKillPillars(AIActor actor)
			{
				this.actor = actor;
				this.controller = actor.gameObject.GetComponent<BossStatueController>();
				this.allcontroller = this.controller.transform.parent.GetComponent<BossStatuesController>();
				this.behaviorSpec = this.allcontroller.behaviorSpeculator;
				this.DoOverride();
			}

			protected BossStatueController controller;
			protected BossStatuesController allcontroller;




			public class ModifiedBossStatuesCrosshair : Script
			{
				protected override IEnumerator Top()
				{
					//this.EndOnBlank = true;
					this.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableFrogger);
					AkSoundEngine.PostEvent("Play_ENM_statue_ring_01", this.BulletBank.gameObject);

					this.FireSpinningLine(0f);
					this.FireCircleSegment(0f);
					this.FireSpinningLine(120f);
					this.FireCircleSegment(120f);
					this.FireSpinningLine(240f);
					this.FireCircleSegment(240f);

					yield return this.Wait(ModifiedBossStatuesCrosshair.SetupTime + ModifiedBossStatuesCrosshair.PulseInitialDelay - ModifiedBossStatuesCrosshair.PulseDelay);
					for (int i = 0; i < ModifiedBossStatuesCrosshair.PulseCount; i++)
					{
						//this.EndOnBlank = true;
						yield return this.Wait(ModifiedBossStatuesCrosshair.PulseDelay);
						this.FirePulse();
					}
					yield return this.Wait(ModifiedBossStatuesCrosshair.SpinTime - (ModifiedBossStatuesCrosshair.PulseDelay * (ModifiedBossStatuesCrosshair.PulseCount - 1) + ModifiedBossStatuesCrosshair.PulseInitialDelay));
					yield break;
				}

				private void FireSpinningLine(float dir)
				{
					float num = (float)ModifiedBossStatuesCrosshair.SkipSetupBulletNum * (ModifiedBossStatuesCrosshair.Radius * 2f * (60f / (float)ModifiedBossStatuesCrosshair.SetupTime) / (float)ModifiedBossStatuesCrosshair.BulletCount);
					float num2 = ModifiedBossStatuesCrosshair.Radius * 2f * (60f / (float)ModifiedBossStatuesCrosshair.SetupTime) / (float)ModifiedBossStatuesCrosshair.BulletCount;
					for (int i = 0; i < ModifiedBossStatuesCrosshair.BulletCount + ModifiedBossStatuesCrosshair.ExtraSetupBulletNum - ModifiedBossStatuesCrosshair.SkipSetupBulletNum; i++)
					{
						base.Fire(new Direction(dir, DirectionType.Absolute, -1f), new Speed(num + num2 * (float)i, SpeedType.Absolute), new ModifiedBossStatuesCrosshair.LineBullet(i + ModifiedBossStatuesCrosshair.SkipSetupBulletNum));
					}
				}

				private void FireCircleSegment(float dir)
				{
					for (int i = 0; i < 40; i++)
					{
						base.Fire(new Direction(dir, DirectionType.Absolute, -1f), new Speed(ModifiedBossStatuesCrosshair.Radius * 2f * (60f / (float)ModifiedBossStatuesCrosshair.SetupTime), SpeedType.Absolute), new ModifiedBossStatuesCrosshair.CircleBullet(i));
					}
				}

				private void FirePulse()
				{
					float num = 22.5f;
					for (int i = 0; i < 16; i++)
					{
						base.Fire(new Direction(((float)i + 0.5f) * num, DirectionType.Absolute, -1f), new Speed(ModifiedBossStatuesCrosshair.Radius / ((float)ModifiedBossStatuesCrosshair.PulseTravelTime / 60f), SpeedType.Absolute), new Bullet("defaultPulse", false, false, true));
						base.Fire(new Direction((((float)i + 0.5f) * num)+ (num/2), DirectionType.Absolute, -1f), new Speed(ModifiedBossStatuesCrosshair.Radius / ((float)ModifiedBossStatuesCrosshair.PulseTravelTime / 50f), SpeedType.Absolute), new Bullet("defaultPulse", false, false, true));
					}
				}

				static ModifiedBossStatuesCrosshair()
				{
					ModifiedBossStatuesCrosshair.QuarterPi = 0.785f;
					ModifiedBossStatuesCrosshair.SkipSetupBulletNum = 0;
					ModifiedBossStatuesCrosshair.SetupTime = 90;
					ModifiedBossStatuesCrosshair.BulletCount = 5;
					ModifiedBossStatuesCrosshair.Radius = 10f;
					ModifiedBossStatuesCrosshair.QuaterRotTime = 90;
					ModifiedBossStatuesCrosshair.SpinTime = 720;
					ModifiedBossStatuesCrosshair.PulseInitialDelay = 120;
					ModifiedBossStatuesCrosshair.PulseDelay = 120;
					ModifiedBossStatuesCrosshair.PulseCount = 5;
					ModifiedBossStatuesCrosshair.PulseTravelTime = 100;
				}

				public static float QuarterPi;

				public static int SkipSetupBulletNum;

				public static int ExtraSetupBulletNum;

				public static int SetupTime;

				public static int BulletCount;

				public static float Radius;

				public static int QuaterRotTime;

				public static int SpinTime;

				public static int PulseInitialDelay;

				public static int PulseDelay;

				public static int PulseCount;

				public static int PulseTravelTime;

				public class LineBullet : Bullet
				{
					public LineBullet(int spawnTime) : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, true)
					{
						this.spawnTime = spawnTime;
					}

					protected override IEnumerator Top()
					{
						this.Projectile.ImmuneToSustainedBlanks = true;
						this.Projectile.ImmuneToBlanks = true;
						this.EndOnBlank = false;
						this.ChangeSpeed(new Speed(0f, SpeedType.Absolute), ModifiedBossStatuesCrosshair.SetupTime);
						yield return this.Wait(ModifiedBossStatuesCrosshair.SetupTime);
						this.ChangeDirection(new Direction(90f, DirectionType.Relative, -1f), 1);
						yield return this.Wait(1);
						this.ChangeSpeed(new Speed((float)this.spawnTime / (float)ModifiedBossStatuesCrosshair.BulletCount * (ModifiedBossStatuesCrosshair.Radius * 2f) * ModifiedBossStatuesCrosshair.QuarterPi * (60f / (float)ModifiedBossStatuesCrosshair.QuaterRotTime), SpeedType.Relative), 1);
						this.ChangeDirection(new Direction(90f / (float)ModifiedBossStatuesCrosshair.QuaterRotTime, DirectionType.Sequence, -1f), ModifiedBossStatuesCrosshair.SpinTime);
						yield return this.Wait(ModifiedBossStatuesCrosshair.SpinTime - 1);
						this.Vanish(false);
						yield break;
					}

					public int spawnTime;
				}

				public class CircleBullet : Bullet
				{
					public CircleBullet(int spawnTime) : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false)
					{
						this.spawnTime = spawnTime;
					}
					protected override IEnumerator Top()
					{
						this.Projectile.ImmuneToSustainedBlanks = true;
						this.Projectile.ImmuneToBlanks = true;
						this.EndOnBlank = false;
						this.ChangeSpeed(new Speed(0f, SpeedType.Absolute), ModifiedBossStatuesCrosshair.SetupTime);
						yield return this.Wait(ModifiedBossStatuesCrosshair.SetupTime);
						this.ChangeDirection(new Direction(90f, DirectionType.Relative, -1f), 1);
						yield return this.Wait(1);
						this.ChangeSpeed(new Speed(ModifiedBossStatuesCrosshair.Radius * 2f * ModifiedBossStatuesCrosshair.QuarterPi * (60f / (float)ModifiedBossStatuesCrosshair.QuaterRotTime), SpeedType.Relative), 1);
						this.ChangeDirection(new Direction(120f / (float)(ModifiedBossStatuesCrosshair.QuaterRotTime*1.33f), DirectionType.Sequence, -1f), (int)(ModifiedBossStatuesCrosshair.QuaterRotTime * 1.33f));
						yield return this.Wait((float)this.spawnTime * ((float)ModifiedBossStatuesCrosshair.QuaterRotTime / 30));
						this.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 1);
						for (int i = 1; i < 7; i++)
						{
							this.Fire(new Direction(-90f, DirectionType.Relative, -1f), new Speed((float)(i * 3), SpeedType.Absolute), new ModifiedBossStatuesCrosshair.CircleExtraBullet(this.spawnTime));
						}
						yield return this.Wait((float)(ModifiedBossStatuesCrosshair.BulletCount - this.spawnTime) * ((float)ModifiedBossStatuesCrosshair.QuaterRotTime / 40));
						yield return this.Wait(ModifiedBossStatuesCrosshair.SpinTime - ModifiedBossStatuesCrosshair.QuaterRotTime);
						this.Vanish(false);
						yield break;
					}

					public int spawnTime;
				}

				public class CircleExtraBullet : Bullet
				{
					public CircleExtraBullet(int spawnTime) : base("defaultCircleExtra", false, false, false)
					{
						this.spawnTime = spawnTime;
					}
					protected override IEnumerator Top()
					{
						this.Projectile.ImmuneToSustainedBlanks = true;
						this.Projectile.ImmuneToBlanks = true;
						this.EndOnBlank = false;
						this.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
						yield return this.Wait((float)(ModifiedBossStatuesCrosshair.BulletCount - this.spawnTime) * ((float)ModifiedBossStatuesCrosshair.QuaterRotTime / (float)ModifiedBossStatuesCrosshair.BulletCount));
						yield return this.Wait(ModifiedBossStatuesCrosshair.SpinTime - ModifiedBossStatuesCrosshair.QuaterRotTime);
						this.ChangeSpeed(new Speed(6f, SpeedType.Absolute), 1);
						yield return this.Wait(120);
						this.Vanish(false);
						yield break;
					}
					public int spawnTime;
				}
			}
		}
	}
}

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


	public class AdvancedDragunChanges : OverrideBehavior
	{

		//2e6223e42e574775b56c6349921f42cb Advanced Dragun Knife
		public override string OverrideAIActorGUID => "05b8afe0b6cc4fffa9dc6036fa24c8ec"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			//ToolsEnemy.DebugInformation(behaviorSpec);

			foreach (Component item in actor.GetComponentsInChildren(typeof(Component)))
			{
				if (item is ParticleSystem particle)
				{
					
					var ts = particle.textureSheetAnimation;
					ts.numTilesX = 4;
					ts.animation = ParticleSystemAnimationType.SingleRow;
					ts.numTilesY = 1;

					var particleRenderer = particle.gameObject.GetComponent<ParticleSystemRenderer>();
					particleRenderer.material = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
					particleRenderer.material.SetFloat("_EmissivePower", 100);
					particleRenderer.material.SetFloat("_EmissiveColorPower", 10);

					particleRenderer.material.mainTexture = StaticTextures.AdvancedParticleBlue;

					var tsa = particle.textureSheetAnimation;
					tsa.animation = ParticleSystemAnimationType.SingleRow;
					tsa.numTilesX = 5;
					tsa.numTilesY = 1;
					tsa.enabled = true;
					tsa.cycleCount = 1;
					tsa.frameOverTime = new ParticleSystem.MinMaxCurve(0, 1);
					tsa.frameOverTimeMultiplier = 1.2f;


					particleRenderer.UpdateGIMaterials();
					DepthLookupManager.AssignRendererToSortingLayer(particleRenderer, DepthLookupManager.GungeonSortingLayer.FOREGROUND);
					DepthLookupManager.UpdateRenderer(particleRenderer);

					var main = particle.main;
					main.startColor = new ParticleSystem.MinMaxGradient(Color.blue, Color.cyan);
					var cbs = particle.colorBySpeed;
					cbs.color = new ParticleSystem.MinMaxGradient(Color.blue, Color.cyan);
					var col = particle.colorOverLifetime;
					col.color = new ParticleSystem.MinMaxGradient(Color.cyan, Color.black);
					var sE = particle.subEmitters;
					sE.enabled = false;
			
				}
			}
			foreach (Component item in actor.GetComponents(typeof(Component)))
			{
				if (item is ParticleSystem particle)
				{
					var main = particle.main;
					main.startColor = new ParticleSystem.MinMaxGradient(Color.blue, Color.cyan);
					var cbs = particle.colorBySpeed;
					cbs.color = new ParticleSystem.MinMaxGradient(Color.blue, Color.cyan);
					var col = particle.colorOverLifetime;
					col.color = new ParticleSystem.MinMaxGradient(Color.cyan, Color.black);
					var sE = particle.subEmitters;
					sE.enabled = false;
				}
			}





			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableFrogger);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSkull);


			DraGunRPGBehavior DraGunRPGBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as DraGunRPGBehavior;
			DraGunRPGBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunRocket1));
			DraGunRPGBehavior1.AttackCooldown += 3.5f;

			SimultaneousAttackBehaviorGroup SimultaneousAttackBehaviorGroup1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as SimultaneousAttackBehaviorGroup;
			foreach (AttackBehaviorBase attackBehaviorBase in SimultaneousAttackBehaviorGroup1.AttackBehaviors)
			{
				if (attackBehaviorBase is DraGunThrowKnifeBehavior knifeBehavior)
				{
					knifeBehavior.delay = 0;
					knifeBehavior.AttackCooldown *= 2;
				}
			}
			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[4].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunFlameBreath2));
			ShootBehavior1.AttackCooldown = (ShootBehavior1.AttackCooldown) + 1.5f;

			ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[6].Behavior as ShootBehavior;
			ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunSpotlight1));
			ShootBehavior2.AttackCooldown = (ShootBehavior2.AttackCooldown) + 1f;


			ShootBehavior ShootBehavior6 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as ShootBehavior;
			ShootBehavior6.BulletScript = new CustomBulletScriptSelector(typeof(DraGunBigNoseShot2Modified));
			ShootBehavior6.AttackCooldown = (ShootBehavior2.AttackCooldown) + 1.5f;

			DraGunHeadShootBehavior DraGunHeadShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[8].Behavior as DraGunHeadShootBehavior;
			DraGunHeadShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunSweepFlameBreath2));
			DraGunHeadShootBehavior1.AttackCooldown = (DraGunHeadShootBehavior1.AttackCooldown) + 0.5f;

			SimultaneousAttackBehaviorGroup SimultaneousAttackBehaviorGroup2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[9].Behavior as SimultaneousAttackBehaviorGroup;
			foreach (AttackBehaviorBase attackBehaviorBase in SimultaneousAttackBehaviorGroup2.AttackBehaviors)
			{
				//ETGModConsole.Log(attackBehaviorBase.GetType().ToString());
				if (attackBehaviorBase is DraGunMac10Behavior behav)
				{
					behav.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunMac10Burst2));
					behav.AttackCooldown = (behav.AttackCooldown) + 0.5f;

					//ETGModConsole.Log(behav.BulletScript.scriptTypeName);
				}
				//ETGModConsole.Log("-----------------");
			}



			SimultaneousAttackBehaviorGroup SimultaneousAttackBehaviorGroup3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[10].Behavior as SimultaneousAttackBehaviorGroup;

			if (SimultaneousAttackBehaviorGroup3.AttackBehaviors[0] is DraGunGlockBehavior ae)
			{
				foreach (DraGunGlockBehavior.GlockAttack a in ae.attacks)
				{
					a.bulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunGlockDirectedHardLeft2));
					//ETGModConsole.Log(a.bulletScript.scriptTypeName);
				}
			}
			if (SimultaneousAttackBehaviorGroup3.AttackBehaviors[1] is DraGunGlockBehavior ea)
			{
				foreach (DraGunGlockBehavior.GlockAttack a in ea.attacks)
				{
					a.bulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunGlockDirectedHardRight2));
				}
			}
	
			SimultaneousAttackBehaviorGroup SimultaneousAttackBehaviorGroup4 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[11].Behavior as SimultaneousAttackBehaviorGroup;
			foreach (AttackBehaviorBase attackBehaviorBase in SimultaneousAttackBehaviorGroup4.AttackBehaviors)
			{
				//ETGModConsole.Log(attackBehaviorBase.GetType().ToString());
				if (attackBehaviorBase is DraGunGlockBehavior behav)
				{
					DraGunGlockBehavior.GlockAttack glockAttack1 = behav.attacks[0] as DraGunGlockBehavior.GlockAttack;
					foreach (DraGunGlockBehavior.GlockAttack a in behav.attacks)
					{
						a.bulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunGlockDirectedLeft2));
					}
					DraGunGlockBehavior.GlockAttack glockAttack2 = behav.attacks[1] as DraGunGlockBehavior.GlockAttack;
					foreach (DraGunGlockBehavior.GlockAttack a in behav.attacks)
					{

						a.bulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunGlockDirectedRight2));
					}
				}
				//ETGModConsole.Log("-----------------");
			}
			ShootBehavior ShootBehavior4 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[12].Behavior as ShootBehavior;
			ShootBehavior4.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDraGunNegativeSpace2));
			ShootBehavior4.AttackCooldown = (ShootBehavior4.AttackCooldown) + 2f;

		}


		public class DraGunBigNoseShot2Modified : DraGunBigNoseShot2
		{
			public DraGunBigNoseShot2Modified()
			{
				this.NumTraps = 3;
			}
		}


			public class ModifiedDraGunNegativeSpace2 : ScriptLite
		{
			public ModifiedDraGunNegativeSpace2()
			{
				this.m_centerBullets = new List<bool>();
			}

			public override void Start()
			{
				this.ActivePlatformRadius = 5f;
				int num = 8;
				this.m_platformCenters = new List<Vector2>(10);
				this.m_platformCenters.Add(new Vector2(UnityEngine.Random.Range(-17f, 17f) / 2f, 0f));
				for (int i = 1; i < 10; i++)
				{
					Vector2 a = this.m_platformCenters[this.m_platformCenters.Count - 1];
					ModifiedDraGunNegativeSpace2.s_validPlatformIndices.Clear();
					for (int j = 0; j < ModifiedDraGunNegativeSpace2.PlatformAngles.Length; j++)
					{
						Vector2 a2 = a + BraveMathCollege.DegreesToVector(ModifiedDraGunNegativeSpace2.PlatformAngles[j], 2f * this.ActivePlatformRadius + ModifiedDraGunNegativeSpace2.PlatformDistances[j]);
						if (a2.x > -17f && a2.x < 17f && Vector2.Distance(a2, this.m_platformCenters[this.m_platformCenters.Count - 1]) > (float)num && (i < 2 || Vector2.Distance(a2, this.m_platformCenters[this.m_platformCenters.Count - 2]) > (float)num) && (i < 3 || Vector2.Distance(a2, this.m_platformCenters[this.m_platformCenters.Count - 3]) > (float)num))
						{
							ModifiedDraGunNegativeSpace2.s_validPlatformIndices.Add(j);
						}
					}
					if (ModifiedDraGunNegativeSpace2.s_validPlatformIndices.Count == 0)
					{
						ModifiedDraGunNegativeSpace2.s_validPlatformIndices.Add(2);
					}
					int num2 = BraveUtility.RandomElement<int>(ModifiedDraGunNegativeSpace2.s_validPlatformIndices);
					this.m_platformCenters.Add(a + BraveMathCollege.DegreesToVector(ModifiedDraGunNegativeSpace2.PlatformAngles[num2], 2f * this.ActivePlatformRadius + ModifiedDraGunNegativeSpace2.PlatformDistances[num2]));
					if (i % 2 == 1)
					{
						ModifiedDraGunNegativeSpace2.s_validPlatformIndices.Remove(num2);
						for (int k = ModifiedDraGunNegativeSpace2.s_validPlatformIndices.Count - 1; k >= 0; k--)
						{
							int num3 = ModifiedDraGunNegativeSpace2.s_validPlatformIndices[k];
							Vector2 a3 = a + BraveMathCollege.DegreesToVector(ModifiedDraGunNegativeSpace2.PlatformAngles[num3], 2f * this.ActivePlatformRadius + ModifiedDraGunNegativeSpace2.PlatformDistances[num3]);
							if (Vector2.Distance(a3, this.m_platformCenters[this.m_platformCenters.Count - 1]) < (float)num || Vector2.Distance(a3, this.m_platformCenters[this.m_platformCenters.Count - 2]) < (float)num)
							{
								ModifiedDraGunNegativeSpace2.s_validPlatformIndices.RemoveAt(k);
							}
						}
						if (ModifiedDraGunNegativeSpace2.s_validPlatformIndices.Count > 0)
						{
							num2 = BraveUtility.RandomElement<int>(ModifiedDraGunNegativeSpace2.s_validPlatformIndices);
							this.m_platformCenters.Add(a + BraveMathCollege.DegreesToVector(ModifiedDraGunNegativeSpace2.PlatformAngles[num2], 2f * this.ActivePlatformRadius + ModifiedDraGunNegativeSpace2.PlatformDistances[num2]));
						}
					}
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
						base.Fire(new Offset(num, 18f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(4.33f, SpeedType.Absolute), new ModifiedDraGunNegativeSpace2.WiggleBullet(suppressOffset));
					}
					this.m_rowHeight += this.m_verticalGap;
					for (int k = 0; k < this.m_platformCenters.Count; k++)
					{
						if (!this.m_centerBullets[k] && this.m_platformCenters[k].y < this.m_rowHeight - 2f)
						{
							base.Fire(new Offset(this.m_platformCenters[k].x, (20.5f - this.m_rowHeight + this.m_platformCenters[k].y)-0.33f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(4.33f, SpeedType.Absolute), new ModifiedDraGunNegativeSpace2.WiggleBullet(true, StaticUndodgeableBulletEntries.UndodgeableFrogger.Name));
							base.Fire(new Offset(this.m_platformCenters[k].x + 0.33f, (20.5f - this.m_rowHeight + this.m_platformCenters[k].y) - 0.66f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(4.33f, SpeedType.Absolute), new ModifiedDraGunNegativeSpace2.WiggleBullet(true, StaticUndodgeableBulletEntries.UndodgeableFrogger.Name));
							base.Fire(new Offset(this.m_platformCenters[k].x - 0.33f, (20.5f - this.m_rowHeight + this.m_platformCenters[k].y) - 0.66f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(4.33f, SpeedType.Absolute), new ModifiedDraGunNegativeSpace2.WiggleBullet(true, StaticUndodgeableBulletEntries.UndodgeableFrogger.Name));
							base.Fire(new Offset(this.m_platformCenters[k].x, (20.5f - this.m_rowHeight + this.m_platformCenters[k].y)-0.66f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(4.33f, SpeedType.Absolute), new ModifiedDraGunNegativeSpace2.WiggleBullet(true, StaticUndodgeableBulletEntries.UndodgeableFrogger.Name));
							base.Fire(new Offset(this.m_platformCenters[k].x, (20.5f - this.m_rowHeight + this.m_platformCenters[k].y)-1f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(4.33f, SpeedType.Absolute), new ModifiedDraGunNegativeSpace2.WiggleBullet(true, StaticUndodgeableBulletEntries.UndodgeableFrogger.Name));

							this.m_centerBullets[k] = true;
						}
					}
					return base.Wait(16);
				}
				state++;
				return base.Wait(120);
			}

			static ModifiedDraGunNegativeSpace2()
			{
				ModifiedDraGunNegativeSpace2.PlatformAngles = new float[]
				{
			135f,
			120f,
			90f,
			60f,
			45f
				};
				ModifiedDraGunNegativeSpace2.PlatformDistances = new float[]
				{
			2f,
			2.5f,
			3f,
			2.5f,
			2f
				};
				ModifiedDraGunNegativeSpace2.s_validPlatformIndices = new List<int>();
			}

			// Token: 0x0400059D RID: 1437
			private const int NumPlatforms = 10;

			// Token: 0x0400059E RID: 1438
			private const int NumBullets = 19;

			// Token: 0x0400059F RID: 1439
			private const int RowDelay = 16;

			// Token: 0x040005A0 RID: 1440
			private const float HalfRoomWidth = 17f;

			// Token: 0x040005A1 RID: 1441
			private const int PlatformRadius = 4;

			// Token: 0x040005A2 RID: 1442
			private static float[] PlatformAngles;

			// Token: 0x040005A3 RID: 1443
			private static float[] PlatformDistances;

			// Token: 0x040005A4 RID: 1444
			private static List<int> s_validPlatformIndices;

			// Token: 0x040005A5 RID: 1445
			private float ActivePlatformRadius;

			// Token: 0x040005A6 RID: 1446
			private List<Vector2> m_platformCenters;

			// Token: 0x040005A7 RID: 1447
			private List<bool> m_centerBullets;

			// Token: 0x040005A8 RID: 1448
			private float m_verticalGap;

			// Token: 0x040005A9 RID: 1449
			private float m_lastCenterHeight;

			// Token: 0x040005AA RID: 1450
			private float m_rowHeight;

			// Token: 0x02000184 RID: 388
			public class WiggleBullet : BulletLite
			{
				public WiggleBullet(bool suppressOffset, string bankName = "default_novfx") : base(bankName, false, false)
				{
					this.m_suppressOffset = suppressOffset;
				}

				public override void Start()
				{
					base.ManualControl = true;
					this.m_truePosition = base.Position;
					this.m_offset = Vector2.zero;
					this.m_xMagnitude = UnityEngine.Random.Range(0f, 0.5f);
					this.m_xPeriod = UnityEngine.Random.Range(1f, 2f);
					this.m_yMagnitude = UnityEngine.Random.Range(0f, 0.3f);
					this.m_yPeriod = UnityEngine.Random.Range(1f, 1.75f);
					this.m_delta = BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
				}

				public override int Update(ref int state)
				{
					if (base.Tick >= 600)
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


		public class ModifiedDraGunGlockDirectedRight2 : ModifiedDraGunGlockDirected2
		{
			protected override string BulletName
			{
				get
				{
					return "glockLeft";
				}
			}
		}


		public class ModifiedDraGunGlockDirectedLeft2 : ModifiedDraGunGlockDirected2
		{
			// Token: 0x1700013E RID: 318
			protected override string BulletName
			{
				get
				{
					return "glockLeft";
				}
			}
		}


		public class ModifiedDraGunGlockDirectedHardRight2 : ModifiedDraGunGlockDirected2
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

		public class ModifiedDraGunGlockDirectedHardLeft2 : ModifiedDraGunGlockDirected2
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


		public class ModifiedDraGunGlockDirected2 : Script
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


				float le = RandomAngle();
				float delta = IsHard == true ? 45 : 60;
				float amount = IsHard == true ? 8 : 6;
				float amount2 = IsHard == true ? 4 : 3;

				for (int l = 0; l < amount; l++)
				{
					for (int n = 0; n < amount2; n++)
                    {
						this.Fire(new Direction(le + (delta * l), DirectionType.Absolute, -1f), new Speed(1f + n, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 10f, 80, -1, false));
						this.Fire(new Direction((le + (delta * l)) + delta/2, DirectionType.Absolute, -1f), new Speed(1f + (n*2), SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 11f, 120, -1, false));
					}
				}
				yield break;
			}
		}




		public class ModifiedDraGunMac10Burst2 : Script
		{
			protected override IEnumerator Top()
			{
				yield return this.Wait(1);
				Vector2 lastPosition = this.Position;
				this.PostWwiseEvent("Play_BOSS_Dragun_Uzi_01", null);
				int r = 0;
				bool i = false; 
				for (; ; )
				{

					if (Vector2.Distance(lastPosition, this.Position) > 0.66f)
					{
						this.Fire(new Offset((lastPosition - this.Position) * 0.33f, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Relative, -1f), new ModifiedDraGunMac10Burst2.UziBullet(i));
						this.Fire(new Offset((lastPosition - this.Position) * 0.66f, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Relative, -1f), new ModifiedDraGunMac10Burst2.UziBullet(i));
					}
					this.Fire(new Direction(0f, DirectionType.Relative, -1f), new ModifiedDraGunMac10Burst2.UziBullet(i));
					r++;
					if (r > (i == true? 1 : 4)) { r = 0; i = !i; }
					lastPosition = this.Position;
					yield return this.Wait(3);
				}
				//yield break;
			}

			public class UziBullet : Bullet
			{
				public UziBullet(bool s) : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false)
				{
					Shoot = s;
				}
				private bool Shoot;
				protected override IEnumerator Top()
				{
					yield return this.Wait(60);
					if (Shoot == true) 
					{
						float r = RandomAngle();
						this.Fire(new Direction(0f + r, DirectionType.Relative, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 9, 90));
						this.Fire(new Direction(120f + r, DirectionType.Relative, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 9, 90));
						this.Fire(new Direction(240f + r, DirectionType.Relative, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 9, 90));
					}
					yield return this.Wait(75);
					if (Shoot == true) 
					{
						float r = RandomAngle();
						this.Fire(new Direction(0f+r, DirectionType.Relative, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 9, 90));
						this.Fire(new Direction(120f+r, DirectionType.Relative, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 9, 90));
						this.Fire(new Direction(240f +r, DirectionType.Relative, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 9, 90));

					}
					yield return this.Wait(75);
					this.ChangeSpeed(new Brave.BulletScript.Speed(10), 150);
					this.Direction = this.RandomAngle();
					yield break;
				}
			}
		}

		public class ModifiedDraGunSweepFlameBreath2 : Script
		{
			protected override IEnumerator Top()
			{
				int r = 0;
				bool i = BraveUtility.RandomBool();
				for (; ; )
				{
					r++;
					this.Fire(new Direction(UnityEngine.Random.Range(-45f, 45f), DirectionType.Relative, -1f), new Speed(15f, SpeedType.Absolute), new Bullet("Sweep", false, false, false));
					this.Fire(new Direction(UnityEngine.Random.Range(-15f, 15f), DirectionType.Relative, -1f), new Speed((float)UnityEngine.Random.Range(1, 2), SpeedType.Absolute), new DieBullet(i));
					if (r > 6) { r = 0; i = !i; }
					yield return this.Wait(1);
				}
			}
			public class DieBullet : SpeedChangingBullet
			{
				public DieBullet(bool dies) : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 10, 180)
				{
					die = dies;
				}
				private bool die;
				protected override IEnumerator Top()
				{
					this.ChangeSpeed(new Brave.BulletScript.Speed(8), 150);
					if (die == true)
                    {
						yield return this.Wait(UnityEngine.Random.Range(30, 60));
						base.Vanish(false);
					}
					yield break;
				}
			}
		}


		public class ModifiedDraGunSpotlight1 : Script
		{
			protected override IEnumerator Top()
			{
				GameManager.Instance.Dungeon.PreventPlayerLightInDarkTerrifyingRooms = true;
				DraGunController dragunController = this.BulletBank.GetComponent<DraGunController>();
				dragunController.aiActor.ParentRoom.BecomeTerrifyingDarkRoom(0.5f, 0.1f, 1f, "Play_ENM_darken_world_01");
				dragunController.HandleDarkRoomEffects(true, 3f);
				yield return this.Wait(30);
				dragunController.SpotlightPos = this.BulletBank.aiActor.transform.position + new Vector3(4f, 1f);
				dragunController.SpotlightSpeed = 7.25f;
				dragunController.SpotlightSmoothTime = 0.2f;
				dragunController.SpotlightVelocity = Vector2.zero;
				dragunController.SpotlightEnabled = true;
				this.StartTask(this.UpdateSpotlightShrink());
				while (this.Tick < 480)
				{
					float dist = Vector2.Distance(this.BulletManager.PlayerPosition(), dragunController.SpotlightPos);
					dragunController.SpotlightSpeed = Mathf.Lerp(6f, 14f, Mathf.InverseLerp(3f, 10f, dist));
					if (dist <= dragunController.SpotlightRadius)
					{
						float t = UnityEngine.Random.value;
						float speed = Mathf.Lerp(5f, 10f, t);
						Vector2 target = (!BraveUtility.RandomBool()) ? this.GetPredictedTargetPositionExact(1f, speed) : this.BulletManager.PlayerPosition();
						this.Fire(new Direction(75, DirectionType.Aim, -1f), new Speed(speed, SpeedType.Absolute), new DraGunSpotlight1.ArcBullet(target, t));
						this.Fire(new Direction(-75, DirectionType.Aim, -1f), new Speed(speed, SpeedType.Absolute), new DraGunSpotlight1.ArcBullet(target, t));

						this.Fire(new Direction(UnityEngine.Random.Range(-120, 120), DirectionType.Aim, -1f), new Speed(speed, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableSkull.Name));

						yield return this.Wait(30);
					}
					yield return this.Wait(1);
				}
				dragunController.SpotlightEnabled = false;
				dragunController.aiActor.ParentRoom.EndTerrifyingDarkRoom(0.5f, 0.1f, 1f, "Play_ENM_lighten_world_01");
				dragunController.HandleDarkRoomEffects(false, 3f);
				yield return this.Wait(30);
				GameManager.Instance.Dungeon.PreventPlayerLightInDarkTerrifyingRooms = false;
				yield break;
			}

			private IEnumerator UpdateSpotlightShrink()
			{
				DraGunController dragunController = this.BulletBank.GetComponent<DraGunController>();
				int startTick = this.Tick;
				while (this.Tick < 480)
				{
					if (this.Tick - startTick < 10)
					{
						dragunController.SpotlightShrink = (float)(this.Tick - startTick) / 9f;
					}
					else if (this.Tick > 470)
					{
						int num = 480 - this.Tick - 1;
						dragunController.SpotlightShrink = (float)num / 9f;
					}
					yield return this.Wait(1);
				}
				yield break;
			}

			public override void OnForceEnded()
			{
				DraGunController component = base.BulletBank.GetComponent<DraGunController>();
				component.SpotlightEnabled = false;
				component.aiActor.ParentRoom.EndTerrifyingDarkRoom(0.5f, 0.1f, 1f, "Play_ENM_lighten_world_01");
				component.HandleDarkRoomEffects(false, 3f);
			}

			public const int ChaseTime = 480;

			public class ArcBullet : Bullet
			{
				public ArcBullet(Vector2 target, float t) : base("triangle", false, false, false)
				{
					this.m_target = target;
					this.m_t = t;
				}

				protected override IEnumerator Top()
				{
					Vector2 toTarget = this.m_target - this.Position;
					float travelTime = toTarget.magnitude / this.Speed * 60f - 1f;
					float magnitude = BraveUtility.RandomSign() * (1f - this.m_t) * 8f;
					Vector2 offset = magnitude * toTarget.Rotate(90f).normalized;
					this.ManualControl = true;
					Vector2 truePosition = this.Position;
					Vector2 lastPosition = this.Position;
					int i = 0;
					while ((float)i < travelTime)
					{
						this.UpdateVelocity();
						truePosition += this.Velocity / 60f;
						lastPosition = this.Position;
						this.Position = truePosition + offset * Mathf.Sin((float)this.Tick / travelTime * 3.1415927f);
						yield return this.Wait(1);
						i++;
					}
					Vector2 v = (this.Position - lastPosition) * 60f;
					this.Speed = v.magnitude;
					this.Direction = v.ToAngle();
					this.ManualControl = false;
					yield break;
				}

				private Vector2 m_target;

				private float m_t;
			}
		}


		public class ModifiedDraGunFlameBreath2 : Script
		{
			protected override IEnumerator Top()
			{
				ModifiedDraGunFlameBreath2.StopYHeight = this.BulletBank.aiActor.ParentRoom.area.UnitBottomLeft.y + 21f;
				int pocketResetTimer = 0;
				float pocketAngle = 0f;
				float pocketSign = BraveUtility.RandomSign();
				bool q = BraveUtility.RandomBool();

				for (int i = 0; i < 120; i++)
				{
					if (i % 40 == 27)
					{
						for (int j = 0; j < 7; j++)
						{
							this.Fire(new Direction(this.SubdivideArc(-30f, 60f, 7, j, false), DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 7, 120));
						}
					}
					float direction = UnityEngine.Random.Range(-44f, 44f);
					if (pocketResetTimer == 0)
					{
						pocketAngle = pocketSign * UnityEngine.Random.Range(0f, 15f);
						pocketSign *= -1f;
						pocketResetTimer = 30;
					}
					pocketResetTimer--;
					if (direction >= pocketAngle - 5f && direction <= pocketAngle)
					{
						direction -= 5f;
					}
					else if (direction <= pocketAngle + 5f && direction >= pocketAngle)
					{
						direction += 5f;
					}
					this.Fire(new Direction(direction, DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new DraGunFlameBreath2.FlameBullet());

					if (q == true)
					{
						this.Fire(new Direction(UnityEngine.Random.Range(-32f, -40f), DirectionType.Aim, -1f), new Speed(11f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false));
					}
					else
					{
						this.Fire(new Direction(UnityEngine.Random.Range(32f, 40f), DirectionType.Aim, -1f), new Speed(11f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false));
					}
					q = !q;

					yield return this.Wait(2);
				}
				yield break;
			}

			private const int NumBullets = 120;
			private const int NumWaveBullets = 12;
			private const float Spread = 30f;
			private const int PocketResetTime = 30;
			private const float PocketWidth = 5f;
			protected static float StopYHeight;

			public class FlameBullet : Bullet
			{
				public FlameBullet(string BankName = "Breath") : base(BankName, false, false, false)
				{
				}

				protected override IEnumerator Top()
				{
					
					this.ChangeSpeed(new Speed(5f, SpeedType.Absolute), 180);
					yield return this.Wait(60);
					this.Vanish(false);
					yield break;
				}
			}
		}


		public class ModifiedDraGunRocket1 : Script
		{
			protected override IEnumerator Top()
			{
				base.Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(50f, SpeedType.Absolute), new ModifiedDraGunRocket1.Rocket());

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
						base.Fire(new Direction(base.SubdivideArc(-10f, 200f, 44, i, false), DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new Bullet(a, false, false, false));
						base.Fire(new Direction(base.SubdivideArc(-10f, 200f, 44, i, false), DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new SpeedChangingBullet(a, 7, 60));

						r++;
						if (r > 3) { r = 0; q = !q; }
					}
					q = UnityEngine.Random.value > 0.5f ? true : false;

					for (int i = 0; i < 44; i++)
					{

						string a = q == true ? StaticUndodgeableBulletEntries.UndodgeableFrogger.Name : "default_novfx";
						base.Fire(new Direction(base.SubdivideArc(-10f, 200f, 44, i, false), DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), new RocketBullet(a, 90));
						base.Fire(new Direction(base.SubdivideArc(-10f, 200f, 44, i, false), DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), new RocketBullet(a, 100));//SpeedChangingBullet(a, 7f, 210, -1, false));
						r++;
						if (r > 3) { r = 0; q = !q; }
					}
					q = UnityEngine.Random.value > 0.5f ? true : false;


					
					for (int i = 0; i < 44; i++)
					{

						string a = q == true ? StaticUndodgeableBulletEntries.UndodgeableFrogger.Name : "default_novfx";
						base.Fire(new Direction(base.SubdivideArc(-10f, 200f, 44, i, false), DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), new RocketBullet(a, 190));
						base.Fire(new Direction(base.SubdivideArc(-10f, 200f, 44, i, false), DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), new RocketBullet(a, 200));//SpeedChangingBullet(a, 7f, 210, -1, false));
						r++;
						if (r > 3) { r = 0; q = !q; }
					}
					
					
				}
			}


			public class RocketBullet : Bullet
			{
				public RocketBullet(string BankName, float W) : base(BankName, false, false, false)
				{
					WaitTime = W;
				}
				protected override IEnumerator Top()
				{
					yield return this.Wait(WaitTime);
					this.ChangeSpeed(new Brave.BulletScript.Speed(7), 90);
				}
				private float WaitTime;
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

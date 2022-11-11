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


	public class OphanaimChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => Ophanaim.guid; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			actor.MovementSpeed *= 0.8f; // Doubles the enemy movement speed

	
			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableFrogger);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBig);

			/*
			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedFireCircles));

			ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
			ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMithrixSlam));

			ShootBehavior ShootBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as ShootBehavior;
			ShootBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBouncyCicleTheShoot));
			*/
		}

		/*
		public class ModifiedBouncyCicleTheShoot : Script
		{
			protected override IEnumerator Top()
			{
				yield return base.Wait(30);

				bool IsSXecondPhase = base.BulletBank.aiActor.GetComponent<Ophanaim.EyeEnemyBehavior>().Phase2Check;
				DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(base.BulletBank.aiActor.sprite.WorldCenter, 25);
				base.EndOnBlank = true;

				base.PostWwiseEvent("Play_BigSlam", null);
				for (int A = 0; A < 8; A++)
				{
					float num = 45 * A;
					base.Fire(new Direction(num, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(5, SpeedType.Absolute), new ModifiedBouncyCicleTheShoot.WaveBullet());
					base.Fire(new Direction(num + 22.5f, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(4, SpeedType.Absolute), new ModifiedBouncyCicleTheShoot.WaveBullet());
					if (IsSXecondPhase == true)
					{
						base.Fire(new Direction(num, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(7, SpeedType.Absolute), new ModifiedBouncyCicleTheShoot.WaveBullet());
					}
				}


				yield return base.Wait(60);
				float Am = IsSXecondPhase == false ? 3 : 5;
				float Wa = IsSXecondPhase == false ? 70 : 50;
				for (int A = 0; A < Am; A++)
				{
					Vector2 vector2 = this.BulletManager.PlayerPosition();
					Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2, this.BulletManager.PlayerVelocity(), this.Position, 18f);
					base.PostWwiseEvent("Play_ENM_kali_shockwave_01", null);
					base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(5, SpeedType.Absolute), new ModifiedBouncyCicleTheShoot.Superball());
					yield return base.Wait(Wa);
				}
				yield return base.Wait(90);
				yield break;
			}


			public class Flames1 : Bullet { public Flames1() : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false) { } }


			public class Superball : Bullet
			{
				public Superball() : base(StaticUndodgeableBulletEntries.undodgeableBig.Name, false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(17, SpeedType.Absolute), 120);
					for (int i = 0; i < 300; i++)
					{
						Vector2 Point2 = MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(0.5f, 1.25f));
						base.Fire(new Offset(Point2), new Direction(180, Brave.BulletScript.DirectionType.Relative, -1f), new Speed(2f, SpeedType.Absolute), new ModifiedBouncyCicleTheShoot.Spore(240));
						yield return this.Wait(5f);

					}
					yield break;
				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (!preventSpawningProjectiles)
					{
						base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);
						for (int i = 0; i < 12; i++)
						{
							base.Fire(new Direction(30f * i, Brave.BulletScript.DirectionType.Relative, -1f), new Speed(5, SpeedType.Absolute), new ModifiedBouncyCicleTheShoot.Flames1());
						}
						return;
					}
				}
			}
			private class WaveBullet : Bullet
			{
				public WaveBullet() : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false) { }

				protected override IEnumerator Top()
				{
					base.ManualControl = true;
					Vector2 truePosition = base.Position;
					for (int e = 0; e < 8; e++)
					{
						for (int i = 0; i < 75; i++)
						{
							base.UpdateVelocity();
							truePosition += this.Velocity / 150f;
							base.Position = truePosition + new Vector2(0f, Mathf.Sin((float)base.Tick / 60f / 0.75f * 3.14159274f) * 1.5f);
							yield return base.Wait(1);
						}
					}
					base.Vanish(false);
					yield break;
				}
				private const float SinPeriod = 0.75f;
				private const float SinMagnitude = 1.5f;
			}

			public class Spore : Bullet
			{
				public Spore(float Airtime) : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false)
				{
					this.AirTime = Airtime;
				}

				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0, SpeedType.Absolute), UnityEngine.Random.Range(60, 240));
					yield return this.Wait(AirTime);
					base.Vanish(false);
					yield break;
				}
				public float AirTime;
			}
		}


		public class ModifiedMithrixSlam : Script
		{
			private const int SpinTime = 450;

			public override void OnForceEnded()
			{
				base.OnForceEnded();
				Ophanaim.EyeEnemyBehavior vfx = base.BulletBank.GetComponent<Ophanaim.EyeEnemyBehavior>();
				for (int e = 0; e < vfx.extantReticles.Count; e++)
				{
					UnityEngine.Object.Destroy(vfx.extantReticles[e]);
				}
			}

			protected override IEnumerator Top()
			{
				CellArea area = base.BulletBank.aiActor.ParentRoom.area;
				base.PostWwiseEvent("Play_EyeRoar", null);
				yield return this.Wait(30f);
				float Amount = 4;
				float WaitTime = 75;
				float delta = 45;
				float Lines = 8;
				if (base.BulletBank.aiActor.GetComponent<Ophanaim.EyeEnemyBehavior>().Phase2Check == true)
				{
					Amount = 5;
					WaitTime = 60f;
					delta = 30;
					Lines = 12;
				}

				for (int e = 0; e < Amount; e++)
				{
					List<float> log = new List<float>() { };
					Ophanaim.EyeEnemyBehavior vfx = base.BulletBank.GetComponent<Ophanaim.EyeEnemyBehavior>();
					vfx.LaserShit = RandomPiecesOfStuffToInitialise.LaserReticle;
					float fuck = (UnityEngine.Random.Range(-180, 180));
					for (int i = 0; i < Lines; i++)
					{
						float num2 = delta;
						float angle = base.AimDirection + fuck + (delta * i);
						Vector2 zero = Vector2.zero;
						if (BraveMathCollege.LineSegmentRectangleIntersection(this.Position, this.Position + BraveMathCollege.DegreesToVector(angle, 60f), new Vector2(-40, -40), new Vector2(40, 40), ref zero))
						{
							num2 = (zero - this.Position).magnitude;
						}

						GameObject gameObject = SpawnManager.SpawnVFX(vfx.LaserShit, false);
						tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
						component2.dimensions = new Vector2((num2) * 50f, 1f);
						component2.UpdateZDepth();

						component2.HeightOffGround = -2;
						component2.gameObject.layer = 28;

						component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
						component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 30);
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
						component2.sprite.renderer.material.SetColor("_OverrideColor", Color.cyan);
						component2.sprite.renderer.material.SetColor("_EmissiveColor", Color.cyan);

						vfx.extantReticles.Add(gameObject);

					}
					bool LeftOrRight = (UnityEngine.Random.value > 0.5f) ? false : true;
					base.PostWwiseEvent("Play_BOSS_lichB_charge_02", null);
					for (int i = 0; i < 60; i++)
					{
						foreach (GameObject obj in vfx.extantReticles)
						{
							tk2dTiledSprite component2 = obj.GetComponent<tk2dTiledSprite>();
							float ix = component2.transform.localRotation.eulerAngles.x;
							float wai = component2.transform.localRotation.eulerAngles.y;
							if (base.BulletBank.aiActor != null)
							{
								ix = component2.transform.localRotation.eulerAngles.x + base.BulletBank.aiActor.transform.localRotation.x;
								wai = component2.transform.localRotation.eulerAngles.y + base.BulletBank.aiActor.transform.localRotation.y;
							}
							float zee = component2.transform.localRotation.eulerAngles.z;
							component2.transform.position.WithZ(component2.transform.position.z + 99999);
							component2.transform.position = this.Position.ToVector3ZisY(0);
							component2.HeightOffGround = -2;
							component2.gameObject.layer = 28;

							if (i < 20 || i == 20)
							{
								component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
								component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
								component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * i);
								component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
							}
							if (i > 20)
							{
								component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
								component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
								component2.sprite.renderer.material.SetFloat("_EmissivePower", 200 + (i - 20));
								component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f + 1 / (i - 5));
							}
							if (LeftOrRight == true)
							{
								component2.transform.localRotation = Quaternion.Euler(ix, wai, zee + ((3f - (i / 20) + 3600)));
							}
							else
							{
								component2.transform.localRotation = Quaternion.Euler(ix, wai, zee - ((3f - (i / 20) + 3600)));
							}
							component2.UpdateZDepth();
						}
						yield return this.Wait(1f);

					}
					foreach (GameObject obj in vfx.extantReticles)
					{
						log.Add(obj.GetComponent<tk2dTiledSprite>().transform.localRotation.eulerAngles.z);
					}
						yield return this.Wait(15f);
					this.CleanupReticles();
					base.PostWwiseEvent("Play_BigSlam", null);
					base.PostWwiseEvent("Play_BOSS_spacebaby_explode_01", null);

					Exploder.DoDistortionWave(base.BulletBank.aiActor.transform.position, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
					foreach (float h in log)
					{
						base.Fire(new Direction(h, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(40f, SpeedType.Absolute), new ModifiedMithrixSlam.Flameert());
					}
					for (int i = 0; i < 10; i++)
					{
						base.Fire(new Direction(36f * i, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 11, 60));
						base.Fire(new Direction((36f * i)+15f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(4f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 14, 150));

					}
					yield return this.Wait(WaitTime);
				}

				yield break;
			}

			public float distortionMaxRadius = 30f;
			public float distortionDuration = 0.5f;
			public float distortionIntensity = 0.9f;
			public float distortionThickness = 0.4f;
			public void CleanupReticles()
			{
				Ophanaim.EyeEnemyBehavior controller = base.BulletBank.aiActor.GetComponent<Ophanaim.EyeEnemyBehavior>();
				if (controller)
				{
					for (int i = 0; i < controller.extantReticles.Count; i++)
					{
						SpawnManager.Despawn(controller.extantReticles[i]);
						UnityEngine.Object.Destroy(controller.extantReticles[i]);
					}
					controller.extantReticles.Clear();
				}
			}
			
			public class Flameert : Bullet
			{
				public Flameert() : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					for (int i = 0; i < 200; i++)
					{
						base.Fire(new Direction(UnityEngine.Random.Range(140, 200), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedMithrixSlam.Flames1(75 - i));
						yield return this.Wait(1f);

					}
					yield break;
				}
			}
			public class Flames1 : Bullet
			{
				public Flames1(float timetilldie) : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false)
				{
					TimeTillDeath = timetilldie;
				}
				protected override IEnumerator Top()
				{
					this.ManualControl = true;
					Vector2 truePosition = this.Position;

					for (int i = 0; i < TimeTillDeath; i++)
					{
						this.UpdateVelocity();
						truePosition += this.Velocity / 75f;
						this.Position = truePosition + new Vector2(0f, Mathf.Sin((float)this.Tick / 45f / 1.625f * 3.14159274f) * 1.5f);
						yield return this.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}
				private float TimeTillDeath;
			}
		}
		public class ModifiedFireCircles : Script
		{
			protected override IEnumerator Top()
			{
				base.EndOnBlank = true;

				float floatDirection = base.AimDirection + UnityEngine.Random.Range(10f, -10f);
				base.PostWwiseEvent("Play_BOSS_Rat_Kunai_Prep_01", null);
				GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(base.Position + new Vector2(2.125f, -1.3125f), 10, 0.1f, 30, 0.666f));
				yield return base.Wait(40f);

				base.PostWwiseEvent("Play_BigSlam", null);
				Vector2 floatVelocity = BraveMathCollege.DegreesToVector(floatDirection, 6f);
				for (int j = 0; j < 8; j++)
				{
					this.Fire(new Offset(new Vector2(2.125f, -1.3125f)), new Direction(this.SubdivideCircle(0, 8, j, 1f, false), Brave.BulletScript.DirectionType.Aim, -1f), new Speed(3.5f, SpeedType.Absolute), new ModifiedFireCircles.BurstBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, floatVelocity));
				}
				yield return base.Wait(20f);

				base.PostWwiseEvent("Play_BOSS_Rat_Kunai_Prep_01", null);
				GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(base.Position + new Vector2(-2.125f, -1.3125f), 10, 0.1f, 30, 0.666f));
				yield return base.Wait(40f);
				floatDirection = base.AimDirection + UnityEngine.Random.Range(10f, -10f);
				base.PostWwiseEvent("Play_BigSlam", null);
				floatVelocity = BraveMathCollege.DegreesToVector(floatDirection, 6);
				for (int j = 0; j < 10; j++)
				{
					this.Fire(new Offset(new Vector2(-2.125f, -1.3125f)), new Direction(this.SubdivideCircle(0, 8, j, 1f, false), Brave.BulletScript.DirectionType.Aim, -1f), new Speed(3.5f, SpeedType.Absolute), new ModifiedFireCircles.BurstBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, floatVelocity));
				}

				if (base.BulletBank.aiActor.GetComponent<Ophanaim.EyeEnemyBehavior>().Phase2Check == true)
				{
					base.PostWwiseEvent("Play_BOSS_Rat_Kunai_Prep_01", null);
					GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(base.Position + new Vector2(2.125f, -1.3125f), 10, 0.1f, 40, 1f));
					yield return base.Wait(60f);
					floatDirection = base.AimDirection + UnityEngine.Random.Range(10f, -10f);
					base.PostWwiseEvent("Play_BigSlam", null);
					floatVelocity = BraveMathCollege.DegreesToVector(floatDirection, 4f);
					for (int j = 0; j < 16; j++)
					{
						this.Fire(new Direction(this.SubdivideCircle(0, 16, j, 1f, false), Brave.BulletScript.DirectionType.Aim, -1f), new Speed(2f, SpeedType.Absolute), new ModifiedFireCircles.BurstBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, floatVelocity));
					}
					yield return base.Wait(60f);
				}
				yield break;
			}

			public class BurstBullet : Bullet
			{
				public BurstBullet(string bulletname, Vector2 additionalVelocity) : base(bulletname, true, false, false)
				{
					this.m_addtionalVelocity = additionalVelocity;
				}

				protected override IEnumerator Top()
				{
					this.ManualControl = true;
					for (int i = 0; i < 300; i++)
					{
						this.UpdateVelocity();
						this.Velocity += this.m_addtionalVelocity * Mathf.Min(10f, (float)i / 60f);
						this.UpdatePosition();
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				private Vector2 m_addtionalVelocity;
			}
		}
		*/
	}
}

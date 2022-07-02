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
	public class FungannonChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => Fungannon.guid; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			actor.MovementSpeed *= 0.75f; // Doubles the enemy movement speed
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBig);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBigBullet);

			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);


			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMegaCannon));

			ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as ShootBehavior;
			ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedPrimaryCannonScript));

			ShootBehavior ShootBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[4].Behavior as ShootBehavior;
			ShootBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBigEverywhereAttack));

		
		}


		public class ModifiedBigEverywhereAttack : Script
		{
			protected override IEnumerator Top()
			{
				CellArea area = base.BulletBank.aiActor.ParentRoom.area;
				float delta = 36f;
				float startDirection = AimDirection;
				base.PostWwiseEvent("Play_BOSS_Rat_Cheese_Summon_01", null);
				for (int j = 0; j < 10; j++)
				{
					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedBigEverywhereAttack.TheGear(15, 25, 240, "spore2", this, (startDirection + (float)j * delta) + 180, 0.04f));
					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedBigEverywhereAttack.TheGear(-15, 75, 360, "spore1", this, (startDirection + (float)j * delta) + 180, 0.04f));
					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedBigEverywhereAttack.TheGear(15, 50, 480, "spore2", this, (startDirection + (float)j * delta) + 180, 0.04f));
					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedBigEverywhereAttack.TheGear(-15, 100, 600, "spore1", this, (startDirection + (float)j * delta) + 180, 0.04f));

					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedBigEverywhereAttack.TheGear(-15, 25, 240, "spore2", this, (startDirection + (float)j * delta) + 180, 0.04f));
					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedBigEverywhereAttack.TheGear(15, 75, 360, "spore1", this, (startDirection + (float)j * delta) + 180, 0.04f));
					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedBigEverywhereAttack.TheGear(-15, 50, 480, "spore2", this, (startDirection + (float)j * delta) + 180, 0.04f));
					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedBigEverywhereAttack.TheGear(15, 100, 600, "spore1", this, (startDirection + (float)j * delta) + 180, 0.04f));

				}

				for (int j = 0; j < 6; j++)
				{
					yield return this.Wait(70f);
					List<float> log = new List<float>()
					{


					};
					FungannonController vfx = base.BulletBank.GetComponent<FungannonController>();

					vfx.LaserShit = EnemyDatabase.GetOrLoadByGuid("6868795625bd46f3ae3e4377adce288b").GetComponent<ResourcefulRatController>().ReticleQuad;
					vfx.name = "LaserTell" + j.ToString();
					base.PostWwiseEvent("Play_BOSS_RatMech_Barrel_01", null);
					int Amount = 8 + (j*2);
					for (int e = 0; e < Amount; e++)
					{
						float anim = (360 / Amount) * e;
						float angle = base.AimDirection + anim;
						log.Add(angle);
					}
					List<float> list = log.Shuffle<float>();
					for (int i = 0; i < Amount; i++)
					{
						float num2 = 20f;
						Vector2 zero = Vector2.zero;
						if (BraveMathCollege.LineSegmentRectangleIntersection(this.Position, this.Position + BraveMathCollege.DegreesToVector(list[i], 60f), area.UnitBottomLeft, area.UnitTopRight, ref zero))
						{
							num2 = (zero - this.Position).magnitude;
						}
						if (vfx == null)
						{
							vfx.LaserShit = EnemyDatabase.GetOrLoadByGuid("6868795625bd46f3ae3e4377adce288b").GetComponent<ResourcefulRatController>().ReticleQuad;
						}
						GameObject gameObject = SpawnManager.SpawnVFX(vfx.LaserShit, false);
						tk2dSlicedSprite component2 = gameObject.GetComponent<tk2dSlicedSprite>();
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, this.Position.y) + BraveMathCollege.DegreesToVector(list[i], 0f).ToVector3ZUp(0);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, list[i]);
						component2.dimensions = new Vector2((num2) * 16f, 5f);
						component2.UpdateZDepth();
						component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
						component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 30);
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
						component2.sprite.renderer.material.SetColor("_OverrideColor", Color.cyan);
						component2.sprite.renderer.material.SetColor("_EmissiveColor", Color.cyan);

						vfx.extantReticles.Add(gameObject);
						yield return this.Wait(1);
					}
					yield return this.Wait(20);
					this.CleanupReticles();
					base.PostWwiseEvent("Play_ENM_cannonball_blast_01", null);
					ExplosionData aww = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
					GameManager.Instance.MainCameraController.DoScreenShake(aww.ss, new Vector2?(base.Position), false);
					foreach (float h in log)
					{
						string bankName = (UnityEngine.Random.value > 0.33f) ? StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name : StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name;
						base.Fire(new Direction(h, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(35f, SpeedType.Absolute), new ModifiedBigEverywhereAttack.Cannonball());
						if (h > 0)
						{
							base.EndOnBlank = true;
						}
					}
					base.EndOnBlank = false;
				}
				yield break;
			}
			public class Cannonball : Bullet
			{
				public Cannonball() : base(StaticUndodgeableBulletEntries.undodgeableBigBullet.Name, false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					for (int i = 0; i < 600; i++)
					{
						string bankName = (UnityEngine.Random.value > 0.33f) ? StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name : StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name;
						base.Fire(new Direction(UnityEngine.Random.Range(150, 210), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(0.6f, SpeedType.Absolute), new ModifiedPrimaryCannonScript.Spore(bankName, UnityEngine.Random.Range(75, 105)));
						yield return this.Wait(1f);

					}
					yield break;
				}
			}
			public class SporeSmall : Bullet
			{
				public SporeSmall(string bulletname, float Airtime) : base(bulletname, false, false, false)
				{
					this.BulletName = bulletname;
					this.AirTime = Airtime;
				}

				protected override IEnumerator Top()
				{
					if (this.BulletName == "spore2")
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 180);
					}
					else
					{
						base.ChangeSpeed(new Speed(1, SpeedType.Absolute), 120);
					}
					yield return this.Wait(AirTime);
					base.Vanish(false);
					yield break;
				}
				public string BulletName;
				public float AirTime;
			}
			public void CleanupReticles()
			{
				FungannonController controller = base.BulletBank.aiActor.GetComponent<FungannonController>();
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

			public class TheGear : Bullet
			{
				public TheGear(float spinspeed, float RevUp, float StartSpeenAgain, string BulletType, ModifiedBigEverywhereAttack parent, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
				{
					this.m_spinSpeed = spinspeed;
					this.TimeToRevUp = RevUp;
					this.StartAgain = StartSpeenAgain;

					this.m_parent = parent;
					this.m_angle = angle;
					this.m_radius = aradius;
					this.m_bulletype = BulletType;
					this.SuppressVfx = true;
				}

				protected override IEnumerator Top()
				{
					base.ManualControl = true;
					this.Projectile.collidesOnlyWithPlayerProjectiles = true;
					this.Projectile.collidesWithProjectiles = true;
					this.Projectile.UpdateCollisionMask();
					Vector2 centerPosition = base.Position;
					float radius = 0f;
					for (int i = 0; i < 2400; i++)
					{
						if (i < TimeToRevUp)
						{
							radius += m_radius;
						}
						if (StartAgain < i)
						{
							radius += m_radius * 2;
						}
						if (i == StartAgain)
						{
							this.Projectile.spriteAnimator.Play();
						}
						centerPosition += this.Velocity / 60f;
						base.UpdateVelocity();
						this.m_angle += this.m_spinSpeed / 60f;
						base.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}

				private IEnumerator ChangeSpinSpeedTask(float newSpinSpeed, int term)
				{
					float delta = (newSpinSpeed - this.m_spinSpeed) / (float)term;
					for (int i = 0; i < term; i++)
					{
						this.m_spinSpeed += delta;
						yield return base.Wait(1);
					}
					yield break;
				}
				private const float ExpandSpeed = 4.5f;
				private const float SpinSpeed = 40f;
				private ModifiedBigEverywhereAttack m_parent;
				private float m_angle;
				private float m_spinSpeed;
				private float m_radius;
				private string m_bulletype;
				private float TimeToRevUp;
				private float StartAgain;


			}
			public class Superball : Bullet
			{
				public Superball() : base(StaticUndodgeableBulletEntries.undodgeableBig.Name, false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					for (int i = 0; i < 90; i++)
					{
						float Speed = base.Speed;
						base.ChangeSpeed(new Speed(Speed * 0.98f, SpeedType.Absolute), 0);

						float aim = this.GetAimDirection(5f, UnityEngine.Random.Range(6f, 6f));
						float delta = BraveMathCollege.ClampAngle180(aim - this.Direction);
						if (Mathf.Abs(delta) > 100f)
						{
							yield break;
						}
						this.Direction += Mathf.MoveTowards(0f, delta, 3f);
						yield return this.Wait(1);
					}

					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 1);
					base.Vanish(true);
					yield break;
				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (!preventSpawningProjectiles)
					{
						base.PostWwiseEvent("Play_BOSS_Rat_Cheese_Burst_02", null);
						for (int i = 0; i < 10; i++)
						{
							string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
							float speed = 1f;
							if (bankName == "spore2")
							{
								speed *= UnityEngine.Random.Range(1.5f, 2f);
							}
							else
							{
								speed *= UnityEngine.Random.Range(0.6f, 1.4f);

							}
							base.Fire(new Direction(UnityEngine.Random.Range(-180, 180), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f * speed, SpeedType.Absolute), new ModifiedPrimaryCannonScript.Spore(bankName, 180));
						}
						return;
					}
				}
			}

			public class Spore : Bullet
			{
				public Spore(string bulletname) : base(bulletname, false, false, false)
				{
					this.BulletName = bulletname;
				}

				protected override IEnumerator Top()
				{
					if (this.BulletName == StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name)
					{
						base.ChangeSpeed(new Speed(22, SpeedType.Absolute), 60);
					}
					else
					{
						base.ChangeSpeed(new Speed(18, SpeedType.Absolute), 60);
					}
					yield break;
				}
				public string BulletName;
			}
		}


		public class ModifiedPrimaryCannonScript : Script
		{
			protected override IEnumerator Top()
			{
				base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
				ExplosionData aww = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
				GameManager.Instance.MainCameraController.DoScreenShake(aww.ss, new Vector2?(base.Position), false);
				base.Fire(new Offset("CannonNorth"), new Direction(45, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), new ModifiedPrimaryCannonScript.Cannonball());
				base.Fire(new Offset("CannonSouth"), new Direction(225, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), new ModifiedPrimaryCannonScript.Cannonball());
				base.Fire(new Offset("CannonEast"), new Direction(135, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), new ModifiedPrimaryCannonScript.Cannonball());
				base.Fire(new Offset("CannonWest"), new Direction(315, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), new ModifiedPrimaryCannonScript.Cannonball());
			
				yield break;
			}


			public class Cannonball : Bullet
			{
				public Cannonball() : base(StaticUndodgeableBulletEntries.undodgeableBigBullet.Name, false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(22f, SpeedType.Absolute), 60);
					for (int i = 0; i < 600; i++)
					{
						string bankName = (UnityEngine.Random.value > 0.33f) ? StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name : StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name;
						base.Fire(new Direction(UnityEngine.Random.Range(150, 210), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(1f, SpeedType.Absolute), new ModifiedPrimaryCannonScript.Spore(bankName, UnityEngine.Random.Range(360, 900)));
						yield return this.Wait(8f);

					}
					yield break;
				}
			}
			public class Spore : Bullet
			{
				public Spore(string bulletname, float Airtime) : base(bulletname, false, false, false)
				{
					this.BulletName = bulletname;
					this.AirTime = Airtime;
				}

				protected override IEnumerator Top()
				{
					if (this.BulletName == StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name)
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 150);
					}
					else
					{
						base.ChangeSpeed(new Speed(1, SpeedType.Absolute), 210);
					}
					yield return this.Wait(AirTime);
					base.Vanish(false);
					yield break;
				}
				public string BulletName;
				public float AirTime;
			}
		}





		public class ModifiedMegaCannon : Script
		{
			protected override IEnumerator Top()
			{
				float ANim = base.AimDirection;
				base.PostWwiseEvent("Play_ENM_hammer_target_01", null);
				yield return this.Wait(20f);
				for (int i = 0; i < 30; i++)
				{
					string bankName = (UnityEngine.Random.value > 0.33f) ? StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name : StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name;
					base.Fire(new Direction(UnityEngine.Random.Range(-180, 180), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(UnityEngine.Random.Range(4, 8), SpeedType.Absolute), new ModifiedMegaCannon.Spore(bankName, UnityEngine.Random.Range(30, 120)));

				}
				base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
				base.Fire(new Direction(ANim, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(25, SpeedType.Absolute), new ModifiedMegaCannon.Superball());


				yield break;
			}

			public class Superball : Bullet
			{
				public Superball() : base(StaticUndodgeableBulletEntries.undodgeableBig.Name, false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					for (int i = 0; i < 100; i++)
					{
						base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedMegaCannon.Cannonball());
						yield return this.Wait(3f);

					}
					yield break;
				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (!preventSpawningProjectiles)
					{
						for (int i = 0; i < 60; i++)
						{
							string bankName = (UnityEngine.Random.value > 0.33f) ? StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name : StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name;
							base.Fire(new Direction(UnityEngine.Random.Range(-180, 180), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(UnityEngine.Random.Range(7, 11), SpeedType.Absolute), new ModifiedMegaCannon.Spore(bankName, UnityEngine.Random.Range(30, 120)));

						}
						return;
					}
				}
			}
			public class Cannonball : Bullet
			{
				public Cannonball() : base(StaticUndodgeableBulletEntries.undodgeableBigBullet.Name, false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					yield return this.Wait(180f);
					for (int i = 0; i < 4; i++)
					{
						string bankName = (UnityEngine.Random.value > 0.33f) ? StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name : StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name;
						base.Fire(new Direction(UnityEngine.Random.Range(-180, 180), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(UnityEngine.Random.Range(2, 7), SpeedType.Absolute), new ModifiedMegaCannon.Spore(bankName, UnityEngine.Random.Range(180, 690)));
					}
					base.Vanish(false);
					yield break;
				}
			}
			public class Spore : Bullet
			{
				public Spore(string bulletname, float Airtime) : base(bulletname, false, false, false)
				{
					this.BulletName = bulletname;
					this.AirTime = Airtime;
				}

				protected override IEnumerator Top()
				{
					if (this.BulletName == StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name)
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 90);
					}
					else
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 150);
					}
					yield return this.Wait(AirTime);
					base.Vanish(false);
					yield break;
				}
				public string BulletName;
				public float AirTime;
			}
		}

	}
}

using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;
using Random = System.Random;
using Pathfinding;


namespace Planetside
{
    public class PrisonerSubPhase3Attacks
    {
		public class ChainRotatorsThree : Script
		{
			public const int NumBullets = 9;
			public const int BaseTurnSpeed = 540;
			public const float MaxDist = 6f;
			public const int ExtendTime = 30;
			public const int Lifetime = 120;
			public const int ContractTime = 45;
			public const int TellTime = 30;
			public float TurnSpeed;
			public int TicksRemaining;
			public static string[] Transforms;
			public float Divider;
			public float TempaletAngle;

			static ChainRotatorsThree()
			{
				ChainRotatorsThree.Transforms = new string[]
				{
				"bullet hand",
				"bullet limb 1",
				"bullet limb 2",
				"bullet limb 3",
				"bullet limb 4",
				"bullet limb 5",
				"bullet limb 6",
				"bullet limb 7",
				};
			}
			public override void OnForceEnded()
			{
				base.OnForceEnded();
				List<List<ChainRotatorsThree.SpinBullet>> lists = new List<List<ChainRotatorsThree.SpinBullet>>
				{
					bullets,
					bullets2,
					bullets3,
					bullets4,
					bullets5,
					bullets6,
					bullets7,
					bullets8,
				};
				this.ClearAllLists(lists);
			}
			private List<ChainRotatorsThree.SpinBullet> bullets;
			private List<ChainRotatorsThree.SpinBullet> bullets2;
			private List<ChainRotatorsThree.SpinBullet> bullets3;
			private List<ChainRotatorsThree.SpinBullet> bullets4;
			
			private List<ChainRotatorsThree.SpinBullet> bullets5;
			private List<ChainRotatorsThree.SpinBullet> bullets6;
			private List<ChainRotatorsThree.SpinBullet> bullets7;
			private List<ChainRotatorsThree.SpinBullet> bullets8;

			private void SpawnChainsOf(List<ChainRotatorsThree.SpinBullet> ListToUse, float Angle, int Delay, bool isTemplateAngle = false)
			{
				base.PostWwiseEvent("Play_ChainBreak_01", null);
				for (int i = 0; i < 60; i++)
				{
					if (i % 4 == 0)
                    {
						float num = ((float)i + 0.5f) / 10f;
						int num2 = Mathf.CeilToInt(Mathf.Lerp((float)(ChainRotatorsThree.Transforms.Length - 1), 0f, num));
						ChainRotatorsThree.SpinBullet spinBullet = new ChainRotatorsThree.SpinBullet(this, num * 6f, Angle, Delay, isTemplateAngle, i % 6 == 1);
						this.Fire(new Offset(ChainRotatorsThree.Transforms[num2]), new Speed(0f, SpeedType.Absolute), spinBullet);
						ListToUse.Add(spinBullet);
					}		
				}
			}

			public override IEnumerator Top()
			{
				PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));

				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("463d16121f884984abe759de38418e48").bulletBank.GetBullet("link"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("463d16121f884984abe759de38418e48").bulletBank.GetBullet("ball"));
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBulletSoundless);

				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableChainLink);

				Divider = 60;
				this.EndOnBlank = true;
				float turnSign = (float)((BraveMathCollege.AbsAngleBetween(this.BulletBank.aiAnimator.FacingDirection, 0f) <= 90f) ? -1 : 1);

				this.TurnSpeed = 60f * turnSign;
				this.bullets = new List<ChainRotatorsThree.SpinBullet>(60);
				this.bullets2 = new List<ChainRotatorsThree.SpinBullet>(60);
				this.bullets3 = new List<ChainRotatorsThree.SpinBullet>(60);
				this.bullets4 = new List<ChainRotatorsThree.SpinBullet>(60);
				this.bullets5 = new List<ChainRotatorsThree.SpinBullet>(60);
				this.bullets6 = new List<ChainRotatorsThree.SpinBullet>(60);
				this.bullets7 = new List<ChainRotatorsThree.SpinBullet>(60);
				this.bullets8 = new List<ChainRotatorsThree.SpinBullet>(60);

				TempaletAngle = 90;
				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				GameManager.Instance.StartCoroutine(SpawnReticle(0, controller, this, false));
				GameManager.Instance.StartCoroutine(SpawnReticle(180, controller, this, false));
				this.SpawnChainsOf(bullets, 90, 60, true);
				this.SpawnChainsOf(bullets2,180, 60, false);

				for (int i = 0; i < 60; i++)
				{
					TempaletAngle += (1 * turnSign);
					yield return this.Wait(1);
				}
				controller.MoveTowardsCenterMethod(4);

				for (int i = 0; i < 3; i++)
				{
					FireQuickBurst();
					yield return this.Wait(90);
				}
				FireQuickBurst();
				GameManager.Instance.StartCoroutine(SpawnReticle(270, controller, this, false));
				GameManager.Instance.StartCoroutine(SpawnReticle(90, controller, this, false));


				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				for (int i = 0; i < 60; i++)
				{
					float t = (float)i / (float)60;
					this.TurnSpeed = Mathf.SmoothStep(60, 40, t) * turnSign;
					yield return this.Wait(1);
				}

				this.SpawnChainsOf(bullets3, 270 + this.TempaletAngle, 400);
				this.SpawnChainsOf(bullets4, 90 + this.TempaletAngle, 400);

				for (int i = 0; i < 3; i++)
				{
					FireQuickBurst();
					yield return this.Wait(90);
				}
				FireQuickBurst();
				GameManager.Instance.StartCoroutine(SpawnReticle(45, controller, this, false));
				GameManager.Instance.StartCoroutine(SpawnReticle(135, controller, this, false));
				GameManager.Instance.StartCoroutine(SpawnReticle(225, controller, this, false));
				GameManager.Instance.StartCoroutine(SpawnReticle(315, controller, this, false));

				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				for (int i = 0; i < 60; i++)
				{
					float t = (float)i / (float)60;
					this.TurnSpeed = Mathf.SmoothStep(40, 25, t) * turnSign;
					yield return this.Wait(1);

				}
				controller.extantReticles.Clear();
				this.SpawnChainsOf(bullets5, 45 + this.TempaletAngle, 730);
				this.SpawnChainsOf(bullets6, 135 + this.TempaletAngle, 730);
				this.SpawnChainsOf(bullets7, 225 + this.TempaletAngle, 730);
				this.SpawnChainsOf(bullets8, 315 + this.TempaletAngle, 730);

				for (int i = 0; i < 4; i++)
				{
					FireQuickBurst();
					yield return this.Wait(90);
				}
				FireQuickBurst();
				for (int i = 0; i < 90; i++)
				{
					float t = (float)i / (float)90;
					this.TurnSpeed = Mathf.SmoothStep(25, 0, t) * turnSign;
					yield return this.Wait(1);
				}
				FireQuickBurst();
				
				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				yield return this.Wait(15);

				List<List<ChainRotatorsThree.SpinBullet>> lists = new List<List<ChainRotatorsThree.SpinBullet>>
				{
					bullets,
					bullets2,
					bullets3,
					bullets4,
					bullets5,
					bullets6,
					bullets7,
					bullets8,
				};
				this.ClearAllLists(lists);
				yield break;
			}

			private IEnumerator SpawnReticle(float Offset, PrisonerPhaseOne.PrisonerController controller, ChainRotatorsThree parent, bool ISDodgeAble = true)
			{
				GameObject reticle = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = reticle.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, 90 + this.TempaletAngle);
				component2.dimensions = new Vector2(1f, 1f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laser = new Color(0f, 1f, 1f, 1f);
				Color laserRed = new Color(1f, 0f, 0f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", ISDodgeAble == false ? laser : laserRed);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", ISDodgeAble == false ? laser : laserRed);
				controller.extantReticles.Add(reticle);
				tk2dTiledSprite tiledsprite = reticle.GetComponent<tk2dTiledSprite>();
				float elapsed = 0;
				float Time = 60f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					if (parent.Destroyed || parent.IsEnded)
					{
						UnityEngine.Object.Destroy(reticle);
						yield break;
					}
					if (reticle != null)
					{
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);

						tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (250 * t));
						tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (10 * t));
						tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, this.TempaletAngle + Offset);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.dimensions = new Vector2(1000f * t, 1f);
						tiledsprite.UpdateZDepth();
					}
					elapsed++;
					yield return this.Wait(1);
				}

				elapsed = 30;
				Time = 0f;
				while (elapsed > Time)
				{
					float t = (float)elapsed / (float)30;
					if (parent.Destroyed || parent.IsEnded)
					{
						UnityEngine.Object.Destroy(reticle);
						yield break;
					}
					if (reticle != null)
					{
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);

						tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 2500);
						tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 10.5f);
						tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, this.TempaletAngle + Offset);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.dimensions = new Vector2(1000f * t, 1f);
						tiledsprite.UpdateZDepth();
					}
					elapsed--;
					yield return this.Wait(1);
				}
				UnityEngine.Object.Destroy(reticle);
				yield break;
			}

			private void ClearAllLists(List<List<ChainRotatorsThree.SpinBullet>> bulletLists)
			{
				if (bulletLists != null)
				{
					for (int e = 0; e < bulletLists.Count; e++)
					{
						for (int k = 0; k < bulletLists[e].Count; k++)
						{
							if (bulletLists[e][k] != null)
							{
								bulletLists[e][k].Vanish();
							}
						}
						bulletLists[e] = null;
					}
				}
			}

			public void FireQuickBurst()
			{
				base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
				for (int e = 0; e < 8; e++)
				{
					string str = StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBulletSoundless.Name;
                    base.Fire(new Direction((45 * e), DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new ChainRotatorsThree.TheGear(10, str, this, 45 * e, 0.0375f));
					base.Fire(new Direction((45 * e), DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new ChainRotatorsThree.TheGear(-10, str, this,  45 * e, 0.0375f));
                }
			}

            public class TheGear : Bullet
            {
                public TheGear(float spinspeed, string BulletType, ChainRotatorsThree parent, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
                {
                    this.m_parent = parent;
                    this.m_angle = angle;
                    this.m_radius = aradius;
                    this.m_bulletype = BulletType;
                    this.SuppressVfx = true;
                    this.m_spinSpeed = spinspeed;
                }

                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    Vector2 centerPosition = base.Position;
                    float radius = 0f;
                    for (int i = 0; i < 300; i++)
                    {
                        
                   
                        base.UpdateVelocity();
                        centerPosition += this.Velocity / 60f;
                        radius += m_radius;

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
                private ChainRotatorsThree m_parent;
                private float m_angle;
                private float m_spinSpeed;
                private float m_radius;
                private string m_bulletype;

            }


            private class SpinBullet : Bullet
			{
				public SpinBullet(ChainRotatorsThree parentScript, float maxDist, float angleOffset, int Delay, bool isAngleDecider = false, bool SpawnsUnDodgeablesOnDeath = false) : base("UndodgeableLink", false, false, false)
				{
					this.m_parentScript = parentScript;
					this.m_maxDist = maxDist;
					this.m_angOffset = angleOffset;
					this.Delay = Delay;
					this.isDecider = isAngleDecider;
					this.spawnsUndodgeables = SpawnsUnDodgeablesOnDeath;
				}

				public override IEnumerator Top()
				{
					//this.lifeTime = 0;
					this.ManualControl = true;
					this.Projectile.specRigidbody.CollideWithTileMap = false;
					this.Projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
					float startDist = Vector2.Distance(this.Position, this.m_parentScript.Position);
					float Lifetime = 0;
					float Addon = this.isDecider == true ? -90 : 0;
					this.Direction = m_angOffset + (Addon + 3);
					while (!this.m_parentScript.Destroyed && !this.m_parentScript.IsEnded)
					{

						Lifetime++;
						if (this.m_parentScript.BulletBank.healthHaver.IsDead)
						{
							this.Vanish(false);
							yield break;
						}
						this.Direction += this.m_parentScript.TurnSpeed / 60;
						if (isDecider == true) { this.m_parentScript.TempaletAngle = this.Direction; }
						float dist;
						dist = Mathf.Lerp(startDist, this.m_maxDist, (float)(this.m_parentScript.Tick - Delay) / 120f);
						this.Position = this.m_parentScript.Position + BraveMathCollege.DegreesToVector(this.Direction, dist);
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (!preventSpawningProjectiles && spawnsUndodgeables == true)
					{
						//base.Fire(new Direction(-90, DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new ChainRotators.BasicBullet(12));
						//base.Fire(new Direction(90, DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new ChainRotators.BasicBullet(12));
					}
				}

				private ChainRotatorsThree m_parentScript;
				private int Delay;

				private bool isDecider;
				private float m_angOffset;
				private float m_maxDist;
				private bool spawnsUndodgeables;
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet(float SpeedIncrease = 7) : base(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBulletSoundless.Name, false, false, false)
				{
					this.Inc = SpeedIncrease;
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(base.Speed + Inc, SpeedType.Absolute), 60);
					yield break;
				}
				private float Inc;
			}
		}
		public class BasicLaserAttackTellThree : Script
		{
			public override IEnumerator Top()
			{
				bool ISDodgeAble = false;
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
                base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableChainLink);

                base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
                yield return this.Wait(30);
                base.PostWwiseEvent("Play_ChainBreak_01", null);
                for (int i = 0; i < 12; i++)
				{
					for (int e = 0; e < 3; e++)
					{
                        base.Fire(new Direction(30 * i, DirectionType.Absolute, -1f), new Speed(4 + (e*2.25f), SpeedType.Absolute), new LerpBullet(120));
                    }
                }
                yield return this.Wait(45);

                PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();			
				for (int e = 0; e < 4; e++)
				{
					float aimDir = base.AimDirection;
					float m = 20f;
					for (int i = -7; i < 8; i++)
					{
						float Angle = (m * i);
						GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);

						tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
						component2.HeightOffGround = -2;
						Color laser = new Color(0f, 1f, 1f, 1f);
						Color laserRed = new Color(1f, 0f, 0f, 1f);
						component2.sprite.usesOverrideMaterial = true;
						component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
						component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
						component2.sprite.renderer.material.SetColor("_OverrideColor", ISDodgeAble == false ? laser : laserRed);
						component2.sprite.renderer.material.SetColor("_EmissiveColor", ISDodgeAble == false ? laser : laserRed);
						GameManager.Instance.StartCoroutine(FlashReticles(component2, ISDodgeAble, aimDir, Angle, this, ISDodgeAble == false ? "sniperUndodgeable" : "directedfire", ISDodgeAble == false ? 3 : 2, 22f, 0.75f));
						controller.extantReticles.Add(gameObject);
					}
                    m = 5f;
                    for (int i = -3; i < 4; i++)
                    {
                        float Angle = (m * i);
                        GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);

                        tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                        component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
                        component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
                        component2.dimensions = new Vector2(1000f, 1f);
                        component2.UpdateZDepth();
                        component2.HeightOffGround = -2;
                        Color laser = new Color(0f, 1f, 1f, 1f);
                        Color laserRed = new Color(1f, 0f, 0f, 1f);
                        component2.sprite.usesOverrideMaterial = true;
                        component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                        component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                        component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                        component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
                        component2.sprite.renderer.material.SetColor("_OverrideColor", ISDodgeAble == false ? laser : laserRed);
                        component2.sprite.renderer.material.SetColor("_EmissiveColor", ISDodgeAble == false ? laser : laserRed);
                        GameManager.Instance.StartCoroutine(FlashReticles(component2, ISDodgeAble, aimDir, Angle, this, ISDodgeAble == false ? "sniperUndodgeable" : "directedfire", ISDodgeAble == false ? 3 : 2, 22f, 0.75f));
                        controller.extantReticles.Add(gameObject);
                    }


                    yield return this.Wait(90);
				}
				yield return this.Wait(60);
				yield break;
			}
			private IEnumerator FlashReticles(tk2dTiledSprite tiledspriteObject, bool isDodgeAble, float InitialAngle ,float Angle, BasicLaserAttackTellThree parent, string BulletType, float bulletAmount = 6, float speed = 12, float Time = 0.5f)
			{
				tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
				float elapsed = 0;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;

					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
						yield break;
					}
					if (tiledspriteObject != null)
					{
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);


						float math = isDodgeAble == true ? 250 : 25;
						tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (math * t));
						tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (10 * t));
						tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(InitialAngle, InitialAngle + Angle, MathToolbox.SinLerpTValue(t)));
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.dimensions = new Vector2(1000f, 1f);
						tiledsprite.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.35f;
				base.PostWwiseEvent("Play_FlashTell");
				while (elapsed < Time)
				{

					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
						yield break;
					}
					float t = (float)elapsed / (float)Time;
					if (tiledspriteObject != null)
					{
						float math = isDodgeAble == true ? 350 : 35;
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);
						tiledsprite.dimensions = new Vector2(1000f, 1f);
						tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (math * t));
						tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (20 * t));
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				UnityEngine.Object.Destroy(tiledspriteObject.gameObject);

				base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>().extantReticles.Clear();
				if (isDodgeAble == false)
				{
					base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
				}
				for (int i = 0; i < bulletAmount; i++)
				{
					base.Fire(new Direction(InitialAngle + Angle, DirectionType.Absolute, -1f), new Speed(speed + (i * 1.5f), SpeedType.Absolute), new WallBullet(BulletType));

				}
				yield break;
			}
			public class WallBullet : Bullet
			{
				public WallBullet(string BulletType) : base(BulletType, false, false, false)
				{
				}
				public override IEnumerator Top()
				{

					yield break;
				}
			}

            public class LerpBullet : Bullet
            {
                public LerpBullet(int f) : base(StaticUndodgeableBulletEntries.undodgeableChainLink.Name, false, false, false)
                {
					term = f;
                }
                public override IEnumerator Top()
                {
					this.ChangeSpeed(new Brave.BulletScript.Speed(0, SpeedType.Absolute), term);
                    yield return this.Wait(450);
					base.Vanish(false);
                    yield break;
                }
				private int term;
            }

        }
		public class WallSweepThree : Script
		{
			public override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
				Vector2 TopRight = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitTopRight;
				Vector2 BottomLeft = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitBottomLeft;
				Dictionary<Vector2, Vector2> wallcornerPositions = new Dictionary<Vector2, Vector2>()
				{
					{new Vector2(BottomLeft.x, TopRight.y), TopRight },//Bottom wall
					{new Vector2(TopRight.x, BottomLeft.y), BottomLeft },//Top wall
					{BottomLeft, new Vector2(BottomLeft.x, TopRight.y) },//Left wall
					{TopRight, new Vector2(TopRight.x, BottomLeft.y) },//Right wall
				};
				PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();
				if (Vector2.Distance(base.BulletBank.aiActor.transform.position, ((Vector2)base.BulletBank.aiActor.ParentRoom.GetCenterCell())) < 8)
                {
					controller.MoveTowardsPositionMethod(4f, 4);
				}
				for (int l = 0; l < 5; l++)
				{
					//controller.MoveTowardsPositionMethod(2f, 3);
					bool ISDodgeAble = false;
					Dictionary<Vector2, string> wallcornerstrings = new Dictionary<Vector2, string>()
					{
						{new Vector2(BottomLeft.x, TopRight.y), "bottom" },//Bottom wall
						{new Vector2(TopRight.x, BottomLeft.y), "top" },//Top wall
						{BottomLeft, "left" },//Left wall
						{TopRight, "right" },//Right wall
					};
					List<Vector2> keyList = new List<Vector2>(wallcornerPositions.Keys);
					Random rand = new Random();
					Vector2 randomKey = keyList[rand.Next(keyList.Count)];
					Vector2 RandomValue = new Vector2();
					wallcornerPositions.TryGetValue(randomKey, out RandomValue);

					if (UnityEngine.Random.value <= 0.66f)
					{
						wallcornerPositions.Remove(randomKey);
					}
					Dictionary<GameObject, Dictionary<Vector2, Vector2>> list = new Dictionary<GameObject, Dictionary<Vector2, Vector2>>();
					for (int i = 0; i < 2; i++)
					{
						float angle = i == 0 ? (base.Position - randomKey).ToAngle() : (base.Position - RandomValue).ToAngle();
						GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
						tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
						component2.dimensions = new Vector2(1f, 1f);
						component2.UpdateZDepth();
						component2.HeightOffGround = -2;
						Color laser = new Color(0f, 1f, 1f, 1f);
						Color laserRed = new Color(1f, 0f, 0f, 1f);
						component2.sprite.usesOverrideMaterial = true;
						component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
						component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
						component2.sprite.renderer.material.SetColor("_OverrideColor", ISDodgeAble == false ? laser : laserRed);
						component2.sprite.renderer.material.SetColor("_EmissiveColor", ISDodgeAble == false ? laser : laserRed);
						controller.extantReticles.Add(gameObject);
						if (i == 0) { gameObject.name = "CanSpawn"; }
						list.Add(gameObject, i == 0 ? new Dictionary<Vector2, Vector2>() { { randomKey, RandomValue } } : new Dictionary<Vector2, Vector2>() { { RandomValue, randomKey } });
					}
					string String = "NULL";
					wallcornerstrings.TryGetValue(randomKey, out String);
					bool JustForOne = false;
					foreach (var listObj in list)
					{
						if (listObj.Key.name == "CanSpawn") { JustForOne = true; } else { JustForOne = false; }
						GameManager.Instance.StartCoroutine(MoveReticlesSmoothly(listObj.Key, listObj.Value, String, JustForOne, ISDodgeAble));
					}
					yield return this.Wait(90f);
				}
				yield break;
			}

			private IEnumerator MoveReticlesSmoothly(GameObject tiledspriteObject, Dictionary<Vector2, Vector2> posDictionary, string placement, bool CanSpawn, bool isDodgeAble)
			{
				Vector2 TopRight = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitTopRight;
				Vector2 BottomLeft = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitBottomLeft;
				/*
				Dictionary<Vector2, Vector2> wallcornerPositions = new Dictionary<Vector2, Vector2>()
				{
					{new Vector2(BottomLeft.x, TopRight.y), TopRight },//Bottom wall
					{new Vector2(TopRight.x, BottomLeft.y), BottomLeft },//Top wall
					{BottomLeft, new Vector2(BottomLeft.x, TopRight.y) },//Left wall
					{TopRight, new Vector2(TopRight.x, BottomLeft.y) },//Right wall
				};
				*/
				float elapsed = 0;
				float Time = 0.66f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					if (tiledspriteObject != null)
					{
						tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
						float ix = tiledsprite.transform.localRotation.eulerAngles.x;
						float wai = tiledsprite.transform.localRotation.eulerAngles.y;
						if (base.BulletBank.aiActor != null)
						{
							ix = tiledsprite.transform.localRotation.eulerAngles.x + base.BulletBank.aiActor.transform.localRotation.x;
							wai = tiledsprite.transform.localRotation.eulerAngles.y + base.BulletBank.aiActor.transform.localRotation.y;
						}
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.transform.position.WithZ(tiledsprite.transform.position.z + 99999);
						tiledsprite.UpdateZDepth();
						Dictionary<Vector2, Vector2> Pos = posDictionary;
						foreach (var yes in Pos)
						{
							float Dist = Vector2.Distance(tiledsprite.transform.position, yes.Key) + 4;
							Vector2 vector3 = Vector2.Lerp(yes.Key, yes.Value, t);
							float angle = (base.Position - vector3).ToAngle();
							tiledsprite.transform.localRotation = Quaternion.Euler(ix, wai, angle);
							tiledsprite.dimensions = new Vector2((Dist * 16) * t, 1f);
							tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (5 * t));
							ImprovedAfterImageForTiled yes1 = tiledsprite.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
							yes1.spawnShadows = true;
							yes1.shadowLifetime = 0.2f;
							yes1.shadowTimeDelay = 0.005f;
							yes1.dashColor = isDodgeAble == false ? new Color(0f, 1f, 1f, 1f) : new Color(1f, 0f, 0f, 1f);
						}
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.33f;
				base.PostWwiseEvent("Play_FlashTell");
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					if (tiledspriteObject != null)
					{
						tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
						float ix = tiledsprite.transform.localRotation.eulerAngles.x;
						float wai = tiledsprite.transform.localRotation.eulerAngles.y;
						if (base.BulletBank.aiActor != null)
						{
							ix = tiledsprite.transform.localRotation.eulerAngles.x + base.BulletBank.aiActor.transform.localRotation.x;
							wai = tiledsprite.transform.localRotation.eulerAngles.y + base.BulletBank.aiActor.transform.localRotation.y;
						}
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.transform.position.WithZ(tiledsprite.transform.position.z + 99999);
						tiledsprite.UpdateZDepth();
						Dictionary<Vector2, Vector2> Pos = posDictionary;
						foreach (var yes in Pos)
						{
							Vector2 vector3 = Vector2.Lerp(yes.Value, yes.Key, t);
							float angle = (base.Position - vector3).ToAngle();
							tiledsprite.transform.localRotation = Quaternion.Euler(ix, wai, angle);
							ImprovedAfterImageForTiled yes1 = tiledsprite.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
						}
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				foreach (GameObject reticles in base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>().extantReticles)
				{
					UnityEngine.Object.Destroy(reticles);
				}
				base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>().extantReticles.Clear();
				if (CanSpawn == true)
				{
					this.FireWallBullets(placement, isDodgeAble);
				}
				yield break;
			}

			private void FireWallBullets(string Placement, bool IsDodgeAble)
			{
				Vector2 TopRight = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitTopRight;
				Vector2 BottomLeft = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitBottomLeft;
				Dictionary<Vector2, Vector2> wallcornerPositions = new Dictionary<Vector2, Vector2>()
				{
					{new Vector2(BottomLeft.x, TopRight.y), TopRight },//Bottom wall
					{new Vector2(TopRight.x, BottomLeft.y), BottomLeft },//Top wall
					{BottomLeft, new Vector2(BottomLeft.x, TopRight.y) },//Left wall
					{TopRight, new Vector2(TopRight.x, BottomLeft.y) },//Right wall
				};
				//All of these are flipped. IDFK WHY BUT I NEED TO GET THE "TOP" to spawn it at THE BOTTOM AAAAAAAAAAAAAAAAA
				Dictionary<string, Vector2> wallcornerstrings = new Dictionary<string, Vector2>()
				{
					{ "bottom" ,new Vector2(TopRight.x, BottomLeft.y)},
					{ "top" ,new Vector2(BottomLeft.x, TopRight.y)},
					{ "left" ,TopRight},
					{ "right" ,BottomLeft}
				};

				Vector2 OneCorner = new Vector2();
				wallcornerstrings.TryGetValue(Placement, out OneCorner);
				Vector2 OtherCorner = new Vector2();
				wallcornerPositions.TryGetValue(OneCorner, out OtherCorner);
				float Tiles = Vector2.Distance(OneCorner, OtherCorner);
				float facingDir = 0;
				if (Placement == "bottom") { facingDir = 90; }
				if (Placement == "top") { facingDir = 270; }
				if (Placement == "left") { facingDir = 180; }
				if (Placement == "right") { facingDir = 0; }

				base.PostWwiseEvent("Play_RockBreaking", null);
				for (int l = 0; l < Tiles; l++)
				{
					float t = (float)l / (float)Tiles;
					Vector2 SpawnPos = Vector2.Lerp(OneCorner, OtherCorner, t);
					if (Placement == "top") { SpawnPos = SpawnPos + new Vector2(0, 1.5f); }

					if (UnityEngine.Random.value <= 0.15f)
					{
						GameObject dragunBoulder = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
						foreach (Component item in dragunBoulder.GetComponentsInChildren(typeof(Component)))
						{
							if (item is SkyRocket laser)
							{
								ExplosionData explosionData = laser.ExplosionData;
								GameObject vfx = UnityEngine.Object.Instantiate<GameObject>(explosionData.effect, SpawnPos, Quaternion.identity);
								tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
								component.PlaceAtPositionByAnchor(SpawnPos, tk2dBaseSprite.Anchor.MiddleCenter);
								component.HeightOffGround = 35f;
								component.transform.rotation = Quaternion.Euler(0f, 0f, facingDir + 90);
								component.UpdateZDepth();
								tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
								if (component2 != null)
								{
									component.usesOverrideMaterial = true;
									if (IsDodgeAble == false) { component2.renderer.material.shader = PlanetsideModule.ModAssets.LoadAsset<Shader>("inverseglowshader"); }
									component2.ignoreTimeScale = true;
									component2.AlwaysIgnoreTimeScale = true;
									component2.AnimateDuringBossIntros = true;
									component2.alwaysUpdateOffscreen = true;
									component2.playAutomatically = true;
								}
							}
						}
					}
					if (l % 3 == 1)
                    {
						int travelTime = UnityEngine.Random.Range(90, 360);
						base.Fire(Offset.OverridePosition(SpawnPos), new Direction(facingDir, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(15f, SpeedType.Absolute), new WallSweepThree.WallBullets(this, "undodgeableSpore", facingDir, IsDodgeAble, travelTime));
						//base.Fire(Offset.OverridePosition(SpawnPos), new Direction(facingDir, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(17f, SpeedType.Absolute), new WallSweepTwo.WallBullets(this, "spore2", facingDir, IsDodgeAble, travelTime));

					}
				}
			}
			public class WallBullets : Bullet
			{
				public WallBullets(WallSweepThree parent, string bulletName, float angle, bool IsDodgeAble, int travelTime) : base(bulletName, true, false, false)
				{
					this.m_parent = parent;
					this.Angle = angle;
					this.IsDoge = IsDodgeAble;
					this.travel = travelTime;
				}
				public override IEnumerator Top()
				{
				
					this.Projectile.IgnoreTileCollisionsFor(180f);
					this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
					base.ChangeSpeed(new Speed(3f, SpeedType.Absolute), UnityEngine.Random.Range(75, 300));
					yield return base.Wait(travel);
					base.Vanish(false);
					yield break;
				}
				private int travel;
				private WallSweepThree m_parent;
				private float Angle;
				private bool IsDoge;
			}
		}
		public class SimpleBlastsThree : Script
		{
			public override IEnumerator Top()
			{
				this.EndOnBlank = true;
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("icicle"));

				PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();
				controller.MoveTowardsCenterMethod(4.5f);
				yield return this.Wait(120f);

				for (int l = 0; l < 11; l++)
				{
					base.PostWwiseEvent("Play_ENM_blobulord_bubble_01", null);
					float RNGSPIN = UnityEngine.Random.Range(0, 60);
					for (int j = 0; j < 1; j++)
					{
						for (int e = 0; e < 8; e++)
						{
							base.Fire(new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SimpleBlastsThree.RotatedBullet(-10, 0, 0, "icicle", this, (0 + (float)e * 45 + ((-10) / 12) * j), 0.06f, false));
						}
						for (int e = 0; e < 8; e++)
						{
							base.Fire(new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SimpleBlastsThree.RotatedBullet(10, 0, 0, "icicle", this, (0 + (float)e * 45 + ((10) / 12) * j), 0.06f, false));
						}
					}
					yield return this.Wait(50f);
				}

				yield break;
			}
			public class RotatedBullet : Bullet
			{
				public RotatedBullet(float spinspeed, float RevUp, float StartSpeenAgain, string BulletType, SimpleBlastsThree parent, float angle = 0f, float aradius = 0, bool IsdodgeAble = true) : base(BulletType, false, false, false)
				{
					this.m_spinSpeed = spinspeed;
					this.TimeToRevUp = RevUp;
					this.StartAgain = StartSpeenAgain;

					this.m_parent = parent;
					this.m_angle = angle;
					this.m_radius = aradius;
					this.m_bulletype = BulletType;
					this.SuppressVfx = true;
					this.isDodgeable = IsdodgeAble;
				}

				public override IEnumerator Top()
				{
					if (isDodgeable == false)
					{
						base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
						SpawnManager.PoolManager.Remove(base.Projectile.transform);
					}
					base.Projectile.transform.localRotation = Quaternion.Euler(0f, 0f, this.m_angle);
					base.ManualControl = true;
					Vector2 centerPosition = base.Position;
					float radius = 0f;
					for (int i = 0; i < 2400; i++)
					{
						radius += m_radius;
						centerPosition += this.Velocity / 60f;
						base.UpdateVelocity();
						this.m_angle += this.m_spinSpeed / 60f;
						base.Projectile.shouldRotate = true;
						base.Direction = this.m_angle;
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
				private SimpleBlastsThree m_parent;
				private float m_angle;
				private float m_spinSpeed;
				private float m_radius;
				private string m_bulletype;
				private float TimeToRevUp;
				private float StartAgain;
				bool isDodgeable;
			}
		}
		public class SweepJukeAttackThree : Script
		{
			public override IEnumerator Top()
			{
				this.EndOnBlank = false;
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
                base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableDoorLordBurst);

                base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBulletSoundless);
                base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableHitscan);


                base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));

				PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();
				if (Vector2.Distance(base.BulletBank.aiActor.transform.position, ((Vector2)base.BulletBank.aiActor.ParentRoom.GetCenterCell())) < 8)
				{
				}

				Vector2 vector2 = this.BulletManager.PlayerPosition();
				Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2, this.BulletManager.PlayerVelocity(), this.Position, 18f);
				float CentreAngle = (predictedPosition - this.Position).ToAngle();
				base.PostWwiseEvent("Play_BOSS_omegaBeam_charge_01");
				float f = 75;
                for (int e = 0; e < 15; e++)
				{
                    bool b = BraveUtility.RandomBool();
                    base.PostWwiseEvent("Play_AbyssBlast");
                    for (int j = 0; j < 12; j++)
                    {
                        base.Fire(Offset.OverridePosition(this.Position + MathToolbox.GetUnitOnCircle(30*j, 9)), new Direction(30 * j, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new TheGear(b == true ? 22.5f : -22.5f, StaticUndodgeableBulletEntries.UndodgeableDoorLordBurst.Name, this, this.Position, (float)j * 30, 0.05f));
                    }
					if (e == 1 | e == 6| e == 11)
					{
						float F = this.AimDirection;
                        base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(this.Position, F, this, 120, false, 0.5f));
                        base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(this.Position, F, this, 240, false, 0.5f));
                        base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(this.Position, F, this, 360, false, 0.5f));
                    }
					
                    yield return this.Wait(Mathf.Max(50, f - (5f * e)));
                }
                for (int r = 0; r < 5; r++)
				{
                    float F2 = this.AimDirection;
                    for (int e = 0; e < 3; e++)
                    {
                        base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(this.Position, F2, this, 120 * e, false, 0.75f));
                    }
                    yield return this.Wait(15);

                }
                yield return this.Wait(120);
				yield break;
			}


            public class TheGear : Bullet
            {
                public TheGear(float spinspeed, string BulletType, SweepJukeAttackThree parent, Vector2 center, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
                {
                    this.m_parent = parent;
                    this.m_angle = angle;
                    this.m_radius = aradius;
                    this.m_bulletype = BulletType;
                    this.SuppressVfx = true;
                    this.m_spinSpeed = spinspeed;
					this.trueCenter = center;
                }

                public override IEnumerator Top()
                {
                    this.Projectile.IgnoreTileCollisionsFor(180f);
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                    base.ManualControl = true;
                    Vector2 centerPosition = this.trueCenter;
                    float radius = 10f;
                    for (int i = 0; i < 300; i++)
                    {
                        if (i == 40)
                        {
                            //base.ChangeSpeed(new Speed(base.Speed + 11f, SpeedType.Absolute), 120);
                            //base.ChangeDirection(new Direction(this.m_parent.GetAimDirection(1f, 10f), DirectionType.Absolute, -1f), 20);
                            //base.StartTask(this.ChangeSpinSpeedTask(90f, 120));
                        }

                        base.UpdateVelocity();
                        centerPosition += this.Velocity / 60f;
                        if (i > 50)
                        {
                            radius -= m_radius;
                        }
                        if (radius < 0)
                        {
                            base.Vanish(false);
                        }

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
                private SweepJukeAttackThree m_parent;
                private float m_angle;
                private float m_spinSpeed;
                private Vector2 trueCenter;

                private float m_radius;
                private string m_bulletype;

            }


            private IEnumerator QuickscopeNoob(Vector2 startPos, float aimDir, SweepJukeAttackThree parent, float rotSet, bool isRed, float chargeTime = 0.5f, float BulletSpeed = 20, float BulletAmount = 10)
            {

                GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
                tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                component2.transform.position = new Vector3(startPos.x, startPos.y, 99999);
                component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
                component2.dimensions = new Vector2(1000f, 1f);
                component2.UpdateZDepth();
                component2.HeightOffGround = -2;
                Color laser = isRed == true ? new Color(1, 0, 0) : new Color(0f, 1f, 1f, 1f);
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
                float elapsed = 0;
                float Time = chargeTime;
                while (elapsed < Time)
                {
                    float t = (float)elapsed / (float)Time;

                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(component2.gameObject);
                        yield break;
                    }
                    if (component2 != null)
                    {
                        float throne1 = Mathf.Sin(t * (Mathf.PI / 2));

                        component2.transform.position = new Vector3(startPos.x, startPos.y, 0);
                        component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
                        component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
                        component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir + rotSet);
                        component2.HeightOffGround = -2;
                        component2.renderer.gameObject.layer = 23;
                        component2.dimensions = new Vector2(1000f, 1f);
                        component2.UpdateZDepth();
                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }

                elapsed = 0;
                Time = 0.5f;
                base.PostWwiseEvent("Play_FlashTell");
                while (elapsed < Time)
                {
                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(component2.gameObject);
                        yield break;
                    }
                    float t = (float)elapsed / (float)Time;
                    if (component2 != null)
                    {
                        component2.transform.position = new Vector3(startPos.x, startPos.y, 0);
                        component2.dimensions = new Vector2(1000f, 1f);
                        component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (60 * t));
                        component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (20 * t));
                        component2.HeightOffGround = -2;
                        component2.renderer.gameObject.layer = 23;
                        component2.UpdateZDepth();
                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                UnityEngine.Object.Destroy(component2.gameObject);
                base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
                base.Fire(Offset.OverridePosition(startPos), new Direction(aimDir + rotSet, DirectionType.Absolute, -1f), new Speed(600, SpeedType.Absolute), new HitScan());

                yield break;
            }


            public class HitScan : Bullet
            {
                public HitScan() : base(StaticUndodgeableBulletEntries.UndodgeableHitscan.Name, false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    SpawnManager.PoolManager.Remove(this.Projectile.gameObject.transform);
                    this.Projectile.BulletScriptSettings.preventPooling = true;

                    yield break;
                }
            }


            public class WallBulletNoDodge : Bullet
			{
				public WallBulletNoDodge(string BulletType, float Angle) : base(BulletType, false, false, false)
				{
					ang = Angle;
				}
				public override IEnumerator Top()
				{

					yield return this.Wait(40);
					this.ChangeSpeed(new Brave.BulletScript.Speed(1), 30);
					yield return this.Wait(60);
					this.ChangeSpeed(new Brave.BulletScript.Speed(10), 60);
					yield break;
				}
				private float ang;
			}
		}
		public class LaserCrossThree : Script
		{
			public override IEnumerator Top()
			{
				PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
                base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableHitscan);

				Vector2 Position = base.BulletBank.aiActor.sprite.WorldCenter;

                if (Vector2.Distance(base.BulletBank.aiActor.transform.position, ((Vector2)base.BulletBank.aiActor.ParentRoom.GetCenterCell())) < 8)
				{
					controller.MoveTowardsPositionMethod(3f, 3);
				}
				float f11 = this.RandomAngle();
				float u11 = UnityEngine.Random.Range(60, 120);
				float M11 = UnityEngine.Random.value < 0.5f ? u11 : -u11;
                Position = base.BulletBank.aiActor.sprite.WorldCenter;
                for (int i = 0; i < 8; i++)
				{
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoobLerpPosition(Position, Position + MathToolbox.GetUnitOnCircle(f11 + (i * 45), Vector2.Distance(Position, Position + (Vector2.one * 2.5f))), (45 * i) + 0, this, M11, 1f));
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoobLerpPosition(Position, Position + MathToolbox.GetUnitOnCircle(f11 + (i * 45), Vector2.Distance(Position, Position + (Vector2.one * 2.5f))), (45 * i) + 120, this, M11, 1f));
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoobLerpPosition(Position, Position + MathToolbox.GetUnitOnCircle(f11 + (i * 45), Vector2.Distance(Position, Position + (Vector2.one * 2.5f))), (45 * i) + 240, this, M11, 1f));
				}
				yield return this.Wait(100);
				int AM = 6;
				for (int q = 0; q < 4; q++)
				{
					float f1 = this.RandomAngle();
					float u1 = UnityEngine.Random.Range(22.5f, 45);
					float M1 = UnityEngine.Random.value < 0.5f ? u1 : -u1;
                    Position = base.BulletBank.aiActor.sprite.WorldCenter;
                    for (int e = 0; e < AM; e++)
					{
						base.BulletBank.aiActor.StartCoroutine(QuickscopeNoobLerpPosition(Position, Position + MathToolbox.GetUnitOnCircle(f1 + (e * (360/AM)), Vector2.Distance(Position, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter)), ((360 / AM) * e) + 0, this, M1, 1f));
						base.BulletBank.aiActor.StartCoroutine(QuickscopeNoobLerpPosition(Position, Position + MathToolbox.GetUnitOnCircle(f1 + (e * (360 / AM)), Vector2.Distance(Position, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter)), ((360 / AM) * e) + 120, this, M1, 1f));
						base.BulletBank.aiActor.StartCoroutine(QuickscopeNoobLerpPosition(Position, Position + MathToolbox.GetUnitOnCircle(f1 + (e * (360 / AM)), Vector2.Distance(Position, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter)), ((360 / AM) * e) + 240, this, M1, 1f));
					}
					AM++;
					yield return this.Wait(80);
				}
				//controller.MoveTowardsPositionMethod(4f, 4);
				yield return this.Wait(60);
				yield break;
			}


            public class HitScan : Bullet
            {
                public HitScan() : base(StaticUndodgeableBulletEntries.UndodgeableHitscan.Name, false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    SpawnManager.PoolManager.Remove(this.Projectile.gameObject.transform);
                    this.Projectile.BulletScriptSettings.preventPooling = true;

                    yield break;
                }
            }

            private IEnumerator QuickscopeNoobLerpPosition(Vector2 startPos, Vector3 endPos, float aimDir, LaserCrossThree parent, float rotSet, float chargeTime = 0.5f)
			{

				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(startPos.x, startPos.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
				component2.dimensions = new Vector2(1000f, 1f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laser = new Color(0f, 1f, 1f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
				float elapsed = 0;
				float Time = chargeTime;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;

					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					if (component2 != null)
					{
						float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
						float Q = Mathf.Lerp(0, rotSet, throne1);

						component2.transform.position = Vector3.Lerp(new Vector3(startPos.x, startPos.y, 0), endPos, MathToolbox.SinLerpTValue(t));
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir + Q);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}

				elapsed = 0;
				Time = 0.5f;
				base.PostWwiseEvent("Play_FlashTell");
				while (elapsed < Time)
				{
					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					float t = (float)elapsed / (float)Time;
					if (component2 != null)
					{
						component2.transform.position = new Vector3(endPos.x, endPos.y, 0);
						component2.dimensions = new Vector2(1000f, 1f);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (60 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (20 * t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				UnityEngine.Object.Destroy(component2.gameObject);
				base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
                base.Fire(Offset.OverridePosition(endPos), new Direction(aimDir + rotSet, DirectionType.Absolute, -1f), new Speed(600f, SpeedType.Absolute), new HitScan());

                yield break;
			}




			private IEnumerator QuickscopeNoob(Vector2 startPos, float aimDir, LaserCrossThree parent, float rotSet,string BulletType, bool isRed ,float chargeTime = 0.5f, float BulletSpeed = 20, float BulletAmount = 10)
			{

				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(startPos.x, startPos.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
				component2.dimensions = new Vector2(1000f, 1f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laser = isRed == true ? new Color(1,0,0) : new Color(0f, 1f, 1f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
				float elapsed = 0;
				float Time = chargeTime;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;

					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					if (component2 != null)
					{
						float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
						float Q = Mathf.Lerp(0, rotSet, throne1);

						component2.transform.position = new Vector3(startPos.x, startPos.y, 0);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir + Q);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}

				elapsed = 0;
				Time = 0.5f;
				base.PostWwiseEvent("Play_FlashTell");
				while (elapsed < Time)
				{
					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					float t = (float)elapsed / (float)Time;
					if (component2 != null)
					{
						component2.transform.position = new Vector3(startPos.x, startPos.y, 0);
						component2.dimensions = new Vector2(1000f, 1f);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (60 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (20 * t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				UnityEngine.Object.Destroy(component2.gameObject);
				base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
				for (int i = 0; i < BulletAmount; i++)
				{
					base.Fire(Offset.OverridePosition(startPos), new Direction(aimDir + rotSet, DirectionType.Absolute, -1f), new Speed(BulletSpeed, SpeedType.Absolute), new WallBulletNoDodge(BulletType));
					yield return new WaitForSeconds(0.025f);
				}
				yield break;
			}

			public class WallBulletNoDodge : Bullet
			{
				public WallBulletNoDodge(string BulletType) : base(BulletType, false, false, false)
				{
				}
				public override IEnumerator Top()
				{
					this.Projectile.IgnoreTileCollisionsFor(300f);
					yield break;
				}
			}
			public class WallBulletDodge : Bullet
			{
				public WallBulletDodge(string BulletType) : base(BulletType, false, false, false)
				{
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(8f, SpeedType.Absolute), 30);
					//base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
					//SpawnManager.PoolManager.Remove(base.Projectile.transform);
					yield break;
				}
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet() : base("sniperUndodgeable", false, false, false)
				{

				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(base.Speed + 11, SpeedType.Absolute), 60);
					yield break;
				}
			}
			public class MegaBullet : Bullet
			{
				public MegaBullet() : base("big", false, false, false)
				{

				}
				public override IEnumerator Top()
				{
					base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
					SpawnManager.PoolManager.Remove(base.Projectile.transform);
					base.ChangeSpeed(new Speed(30f, SpeedType.Absolute), 150);
					yield break;
				}
			}
		}
	}
}

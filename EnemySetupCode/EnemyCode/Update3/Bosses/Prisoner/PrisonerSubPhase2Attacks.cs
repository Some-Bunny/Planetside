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
    public class PrisonerSubPhase2Attacks
    {
		public class ChainRotatorsTwo : Script
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

			static ChainRotatorsTwo()
			{
				ChainRotatorsTwo.Transforms = new string[]
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
				List<List<ChainRotatorsTwo.SpinBullet>> lists = new List<List<ChainRotatorsTwo.SpinBullet>>
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
			private List<ChainRotatorsTwo.SpinBullet> bullets;
			private List<ChainRotatorsTwo.SpinBullet> bullets2;
			private List<ChainRotatorsTwo.SpinBullet> bullets3;
			private List<ChainRotatorsTwo.SpinBullet> bullets4;
			
			private List<ChainRotatorsTwo.SpinBullet> bullets5;
			private List<ChainRotatorsTwo.SpinBullet> bullets6;
			private List<ChainRotatorsTwo.SpinBullet> bullets7;
			private List<ChainRotatorsTwo.SpinBullet> bullets8;

			private void SpawnChainsOf(List<ChainRotatorsTwo.SpinBullet> ListToUse, float Angle, int Delay, bool isTemplateAngle = false)
			{
				base.PostWwiseEvent("Play_ChainBreak_01", null);
				for (int i = 0; i < 60; i++)
				{
					float num = ((float)i + 0.5f) / 10f;
					int num2 = Mathf.CeilToInt(Mathf.Lerp((float)(ChainRotatorsTwo.Transforms.Length - 1), 0f, num));
					ChainRotatorsTwo.SpinBullet spinBullet = new ChainRotatorsTwo.SpinBullet(this, num * 6f, Angle, Delay, isTemplateAngle, i % 6 == 1);
					this.Fire(new Offset(ChainRotatorsTwo.Transforms[num2]), new Speed(0f, SpeedType.Absolute), spinBullet);
					ListToUse.Add(spinBullet);
				}
			}

			protected override IEnumerator Top()
			{
				PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("463d16121f884984abe759de38418e48").bulletBank.GetBullet("link"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("463d16121f884984abe759de38418e48").bulletBank.GetBullet("ball"));
				Divider = 60;
				this.EndOnBlank = true;
				float turnSign = (float)((BraveMathCollege.AbsAngleBetween(this.BulletBank.aiAnimator.FacingDirection, 0f) <= 90f) ? -1 : 1);

				this.TurnSpeed = 60f * turnSign;
				this.bullets = new List<ChainRotatorsTwo.SpinBullet>(60);
				this.bullets2 = new List<ChainRotatorsTwo.SpinBullet>(60);
				this.bullets3 = new List<ChainRotatorsTwo.SpinBullet>(60);
				this.bullets4 = new List<ChainRotatorsTwo.SpinBullet>(60);
				this.bullets5 = new List<ChainRotatorsTwo.SpinBullet>(60);
				this.bullets6 = new List<ChainRotatorsTwo.SpinBullet>(60);
				this.bullets7 = new List<ChainRotatorsTwo.SpinBullet>(60);
				this.bullets8 = new List<ChainRotatorsTwo.SpinBullet>(60);

				TempaletAngle = 90;
				controller.MoveTowardsCenterMethod(1.5f);
				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				GameManager.Instance.StartCoroutine(SpawnReticle(0, controller, this));
				GameManager.Instance.StartCoroutine(SpawnReticle(180, controller, this));
				this.SpawnChainsOf(bullets, 90, 60, true);
				this.SpawnChainsOf(bullets2,180, 60, false);

				for (int i = 0; i < 60; i++)
				{
					TempaletAngle += (1 * turnSign);
					yield return this.Wait(1);
				}
				for (int i = 0; i < 2; i++)
				{
					FireQuickBurst();
					yield return this.Wait(60);
				}
				FireQuickBurst();
				GameManager.Instance.StartCoroutine(SpawnReticle(270, controller, this));
				GameManager.Instance.StartCoroutine(SpawnReticle(90, controller, this));


				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				for (int i = 0; i < 60; i++)
				{
					float t = (float)i / (float)60;
					this.TurnSpeed = Mathf.SmoothStep(60, 45, t) * turnSign;
					yield return this.Wait(1);
				}

				this.SpawnChainsOf(bullets3, 270 + this.TempaletAngle, 250);
				this.SpawnChainsOf(bullets4, 90 + this.TempaletAngle, 250);

				for (int i = 0; i < 2; i++)
				{
					FireQuickBurst();
					yield return this.Wait(60);
				}
				FireQuickBurst();
				GameManager.Instance.StartCoroutine(SpawnReticle(45, controller, this));
				GameManager.Instance.StartCoroutine(SpawnReticle(135, controller, this));
				GameManager.Instance.StartCoroutine(SpawnReticle(225, controller, this));
				GameManager.Instance.StartCoroutine(SpawnReticle(315, controller, this));

				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				for (int i = 0; i < 60; i++)
				{
					float t = (float)i / (float)60;
					this.TurnSpeed = Mathf.SmoothStep(45, 30, t) * turnSign;
					yield return this.Wait(1);

				}
				controller.extantReticles.Clear();
				this.SpawnChainsOf(bullets5, 45 + this.TempaletAngle, 430);
				this.SpawnChainsOf(bullets6, 135 + this.TempaletAngle, 430);
				this.SpawnChainsOf(bullets7, 225 + this.TempaletAngle, 430);
				this.SpawnChainsOf(bullets8, 315 + this.TempaletAngle, 430);


				for (int i = 0; i < 6; i++)
				{
					FireQuickBurst();
					yield return this.Wait(60);
				}
				FireQuickBurst();
				for (int i = 0; i < 60; i++)
				{
					float t = (float)i / (float)60;
					this.TurnSpeed = Mathf.SmoothStep(30, -15, t) * turnSign;
					yield return this.Wait(1);
				}
				FireQuickBurst();
				for (int i = 0; i < 60; i++)
				{
					float t = (float)i / (float)60;
					this.TurnSpeed = Mathf.SmoothStep(-15, -30, t) * turnSign;
					yield return this.Wait(1);
				}
				for (int i = 0; i < 3; i++)
				{
					FireQuickBurst();
					yield return this.Wait(60);
				}
				for (int i = 0; i < 60; i++)
				{
					float t = (float)i / (float)60;
					this.TurnSpeed = Mathf.SmoothStep(-30, 0, t) * turnSign;
					yield return this.Wait(1);
				}
				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				yield return this.Wait(15);

				List<List<ChainRotatorsTwo.SpinBullet>> lists = new List<List<ChainRotatorsTwo.SpinBullet>>
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

			private IEnumerator SpawnReticle(float Offset, PrisonerPhaseOne.PrisonerController controller, ChainRotatorsTwo parent, bool ISDodgeAble = true)
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

			private void ClearAllLists(List<List<ChainRotatorsTwo.SpinBullet>> bulletLists)
			{
				if (bulletLists != null)
				{
					for (int e = 0; e < bulletLists.Count; e++)
					{
						for (int k = 0; k < bulletLists[e].Count; k++)
						{
							if (bulletLists[e][k] != null)
							{
								if (k % 6 == 0)
                                {
									base.Fire(Offset.OverridePosition(bulletLists[e][k].Position), new Direction(0, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("link", 10, 90));
								}
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
				for (int e = 0; e < 12; e++)
				{
					base.Fire(new Direction((30 * e)-3, DirectionType.Aim, -1f), new Speed(1.2f, SpeedType.Absolute), new ChainRotatorsTwo.BasicBullet());
					base.Fire(new Direction((30 * e)+3, DirectionType.Aim, -1f), new Speed(1.2f, SpeedType.Absolute), new ChainRotatorsTwo.BasicBullet());
					base.Fire(new Direction((30 * e), DirectionType.Aim, -1f), new Speed(1.6f, SpeedType.Absolute), new ChainRotatorsTwo.BasicBullet());
				}
			}

			private class SpinBullet : Bullet
			{
				public SpinBullet(ChainRotatorsTwo parentScript, float maxDist, float angleOffset, int Delay, bool isAngleDecider = false, bool SpawnsUnDodgeablesOnDeath = false) : base("link", false, false, false)
				{
					this.m_parentScript = parentScript;
					this.m_maxDist = maxDist;
					this.m_angOffset = angleOffset;
					this.Delay = Delay;
					this.isDecider = isAngleDecider;
					this.spawnsUndodgeables = SpawnsUnDodgeablesOnDeath;
				}

				protected override IEnumerator Top()
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

				private ChainRotatorsTwo m_parentScript;
				private int Delay;

				private bool isDecider;
				private float m_angOffset;
				private float m_maxDist;
				private bool spawnsUndodgeables;
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet(float SpeedIncrease = 7) : base("sniper", false, false, false)
				{
					this.Inc = SpeedIncrease;
				}
				protected override IEnumerator Top()
				{
					base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
					SpawnManager.PoolManager.Remove(base.Projectile.transform);
					base.ChangeSpeed(new Speed(base.Speed + Inc, SpeedType.Absolute), 60);
					yield break;
				}
				private float Inc;
			}
		}
		public class BasicLaserAttackTellTwo : Script
		{
			protected override IEnumerator Top()
			{
				bool ISDodgeAble = false;
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
				PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();
				controller.MoveTowardsPositionMethod(1f, 4);
				for (int i = -4; i < 5; i++)
				{
					float OffsetPhase = 18;
					float Angle = base.AimDirection + (OffsetPhase * i);
					float Offset = (OffsetPhase * i);
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
					GameManager.Instance.StartCoroutine(FlashReticles(component2, ISDodgeAble, Angle, Offset, this, ISDodgeAble == false ? "sniperUndodgeable" : "sniperUndodgeable"));
					controller.extantReticles.Add(gameObject);

				}
				yield return this.Wait(45);
				ISDodgeAble = false;
				for (int i = -2; i < 3; i++)
				{
					float Angle = base.AimDirection + (45 * i);
					float Offset = (45 * i);
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
					GameManager.Instance.StartCoroutine(FlashReticles(component2, ISDodgeAble, Angle, Offset, this, "sniperUndodgeable"));
					controller.extantReticles.Add(gameObject);
				}
				yield return this.Wait(30);
				ISDodgeAble = false;
				for (int i = -1; i < 2; i++)
				{
					float Angle = base.AimDirection + (5 * i);
					float Offset = (5 * i);
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
					GameManager.Instance.StartCoroutine(FlashReticles(component2, ISDodgeAble, Angle, Offset, this, "sniperUndodgeable"));
					controller.extantReticles.Add(gameObject);
				}


				yield return this.Wait(60);
				yield break;
			}
			private IEnumerator FlashReticles(tk2dTiledSprite tiledspriteObject, bool isDodgeAble, float Angle, float Offset, BasicLaserAttackTellTwo parent, string BulletType)
			{
				tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
				float elapsed = 0;
				float Time = 0.4f;
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
						tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, base.AimDirection + Offset);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.dimensions = new Vector2(1000f, 1f);
						tiledsprite.UpdateZDepth();

						Angle = base.AimDirection + Offset;
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
				for (int i = 0; i < 6; i++)
				{
					base.Fire(new Direction(Angle, DirectionType.Absolute, -1f), new Speed(12f + (i * 1.5f), SpeedType.Absolute), new WallBullet(BulletType));

				}
				yield break;
			}
			public class WallBullet : Bullet
			{
				public WallBullet(string BulletType) : base(BulletType, false, false, false)
				{
				}
				protected override IEnumerator Top()
				{

					yield break;
				}
			}
		}
		public class WallSweepTwo : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
				Vector2 TopRight = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitTopRight;
				Vector2 BottomLeft = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitBottomLeft;
				Dictionary<Vector2, Vector2> wallcornerPositions = new Dictionary<Vector2, Vector2>()
				{
					{new Vector2(BottomLeft.x, TopRight.y), TopRight },//Bottom wall
					{new Vector2(TopRight.x, BottomLeft.y), BottomLeft },//Top wall
					{BottomLeft, new Vector2(BottomLeft.x, TopRight.y) },//Left wall
					{TopRight, new Vector2(TopRight.x, BottomLeft.y) },//Right wall
				};

				for (int l = 0; l < 4; l++)
				{
					PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();
					controller.MoveTowardsPositionMethod(1f, 7);
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
					yield return this.Wait(105f);
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
					if (l % 2 == 1)
                    {
						int travelTime = UnityEngine.Random.Range(90, 360);
						base.Fire(Offset.OverridePosition(SpawnPos), new Direction(facingDir, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(15f, SpeedType.Absolute), new WallSweepTwo.WallBullets(this, "spore2", facingDir, IsDodgeAble, travelTime));
						base.Fire(Offset.OverridePosition(SpawnPos), new Direction(facingDir, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(16f, SpeedType.Absolute), new WallSweepTwo.WallBullets(this, "spore2", facingDir, IsDodgeAble, travelTime));
						//base.Fire(Offset.OverridePosition(SpawnPos), new Direction(facingDir, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(17f, SpeedType.Absolute), new WallSweepTwo.WallBullets(this, "spore2", facingDir, IsDodgeAble, travelTime));

					}
				}
			}
			public class WallBullets : Bullet
			{
				public WallBullets(WallSweepTwo parent, string bulletName, float angle, bool IsDodgeAble, int travelTime) : base(bulletName, true, false, false)
				{
					this.m_parent = parent;
					this.Angle = angle;
					this.IsDoge = IsDodgeAble;
					this.travel = travelTime;
				}
				protected override IEnumerator Top()
				{
					if (IsDoge == false)
					{
						base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
						SpawnManager.PoolManager.Remove(base.Projectile.transform);
					}
					this.Projectile.IgnoreTileCollisionsFor(180f);
					this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
					base.ChangeSpeed(new Speed(3f, SpeedType.Absolute), UnityEngine.Random.Range(75, 300));
					yield return base.Wait(travel);
					base.Vanish(false);
					yield break;
				}
				private int travel;
				private WallSweepTwo m_parent;
				private float Angle;
				private bool IsDoge;
			}
		}
		public class SimpleBlaststwo : Script
		{
			protected override IEnumerator Top()
			{
				this.EndOnBlank = true;
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("icicle"));

				PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();
				controller.MoveTowardsCenterMethod(2f);
				yield return this.Wait(100f);

				for (int l = 0; l < 10; l++)
				{
					base.PostWwiseEvent("Play_ENM_blobulord_bubble_01", null);
					float RNGSPIN = UnityEngine.Random.Range(0, 60);
					for (int j = 0; j < 3; j++)
					{
						for (int e = 0; e < 12; e++)
						{
							base.Fire(new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SimpleBlaststwo.RotatedBullet(-10, 0, 0, "icicle", this, (0 + (float)e * 30 + ((-10) / 12) * j), 0.07f, false));
						}
						for (int e = 0; e < 12; e++)
						{
							base.Fire(new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SimpleBlaststwo.RotatedBullet(10, 0, 0, "icicle", this, (0 + (float)e * 30 + ((10) / 12) * j), 0.07f,  false));
						}
						yield return this.Wait(6f);
					}
					yield return this.Wait(50f);
				}

				yield break;
			}
			public class RotatedBullet : Bullet
			{
				public RotatedBullet(float spinspeed, float RevUp, float StartSpeenAgain, string BulletType, SimpleBlaststwo parent, float angle = 0f, float aradius = 0, bool IsdodgeAble = true) : base(BulletType, false, false, false)
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

				protected override IEnumerator Top()
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
				private SimpleBlaststwo m_parent;
				private float m_angle;
				private float m_spinSpeed;
				private float m_radius;
				private string m_bulletype;
				private float TimeToRevUp;
				private float StartAgain;
				bool isDodgeable;

			}
		}
		public class SweepJukeAttackTwo : Script
		{
			protected override IEnumerator Top()
			{
				this.EndOnBlank = true;
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableQuickHoming);

				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));

				PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();
				controller.MoveTowardsPositionMethod(2f, 11);
				Vector2 vector2 = this.BulletManager.PlayerPosition();
				Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2, this.BulletManager.PlayerVelocity(), this.Position, 18f);
				float CentreAngle = (predictedPosition - this.Position).ToAngle();
				base.PostWwiseEvent("Play_BOSS_omegaBeam_charge_01");
				base.BulletBank.aiActor.StartCoroutine(FlashReticles(CentreAngle - 180, 168, this));
				base.BulletBank.aiActor.StartCoroutine(FlashReticles(CentreAngle - 180, -168, this));
				for (int e = 0; e < 4; e++)
				{
					yield return this.Wait(30);
					base.PostWwiseEvent("Play_BOSS_omegaBeam_charge_01");
					base.BulletBank.aiActor.StartCoroutine(SwipeLaser(CentreAngle - 180, -168, this, 1.5f - (0.5f * e)));
					base.BulletBank.aiActor.StartCoroutine(SwipeLaser(CentreAngle - 180, 168, this, 1.5f - (0.5f * e)));
				}
				for (int e = 0; e < 8; e++)
				{
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(CentreAngle, this, 0.75f));
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(CentreAngle, this, 0.75f, 15f));
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(CentreAngle, this, 0.75f, -15f));

					yield return this.Wait(60);
				}
				base.BulletBank.aiActor.StartCoroutine(DoBlast(this));
				yield return this.Wait(75);

				yield break;
			}

			private IEnumerator DoBlast(SweepJukeAttackTwo parent)
			{
				StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(this.Position);
				yield return new WaitForSeconds(1);
				base.PostWwiseEvent("Play_BOSS_spacebaby_explode_01");
				for (int i = 0; i < 60; i++)
				{
					int r = 2;
					if (i % 5 == 0)
                    {
						r = 6;
					}
					for (int h = 0; h < r; h++)
                    {
						base.Fire(new Direction(6 * i, DirectionType.Aim, -1f), new Speed(i % 5 == 0 ? 7f + h: 9, SpeedType.Absolute), new SpeedChangingBullet(i % 5 == 0 ? StaticUndodgeableBulletEntries.undodgeableQuickHoming.Name : "sniper", 20, 90));
					}
				}
				yield break;
			}



			private IEnumerator SwipeLaser(float StartAngle, float AddOrSubtract, SweepJukeAttackTwo parent, float Time)
			{
				GameObject reticle = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = reticle.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, StartAngle);
				component2.dimensions = new Vector2(1f, 1000f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color red = new Color(1f, 0f, 0f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", red);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", red);

				ImprovedAfterImageForTiled yes1 = component2.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
				yes1.spawnShadows = true;
				yes1.shadowLifetime = 0.2f;
				yes1.shadowTimeDelay = 0.005f;
				yes1.dashColor = new Color(1f, 0f, 0f, 1f);
				float elapsed = 0;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					if (component2.gameObject != null)
					{
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 0);

						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (50 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (2 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(StartAngle, StartAngle + AddOrSubtract, t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				UnityEngine.Object.Destroy(component2.gameObject);
				yield break;
			}


			private IEnumerator FlashReticles(float StartAngle, float AddOrSubtract, SweepJukeAttackTwo parent)
			{

				GameObject reticle = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = reticle.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, StartAngle);
				component2.dimensions = new Vector2(1f, 1000f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color red = new Color(1f, 0f, 0f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", red);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", red);

				ImprovedAfterImageForTiled yes1 = component2.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
				yes1.spawnShadows = true;
				yes1.shadowLifetime = 0.2f;
				yes1.shadowTimeDelay = 0.005f;
				yes1.dashColor = new Color(1f, 0f, 0f, 1f);

				float elapsed = 0;
				float Time = 1.25f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					if (component2.gameObject != null)
					{
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 0);

						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (50 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (2 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(StartAngle, StartAngle + AddOrSubtract, t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.75f;
				while (elapsed < Time)
				{

					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				UnityEngine.Object.Destroy(component2.gameObject);

				base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>().extantReticles.Clear();
				base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);

				for (int e = 0; e < 25; e++)
				{
					if (parent.IsEnded || parent.Destroyed)
					{
						yield break;
					}
					for (int i = 0; i < 36; i++)
					{
						float t = (float)i / (float)36;
						base.Fire(new Direction(Mathf.Lerp(StartAngle, StartAngle + AddOrSubtract, t), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(28, 32), SpeedType.Absolute), new SweepJukeAttackTwo.BasicBulletDodge());
					}
					base.PostWwiseEvent("Play_BOSS_doormimic_zap_01");
					yield return new WaitForSeconds(0.33f);
				}
				yield break;
			}

			private IEnumerator QuickscopeNoob(float Angle, SweepJukeAttackTwo parent, float delay = 0.25f, float Offset = 0)
			{

				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
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
				float Time = delay;
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

						float Pos = ((base.GetPredictedTargetPosition(0.6f, 34) - base.Position).ToAngle()) + Offset;

						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 0);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, Pos);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();

						Angle = Pos;
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.25f;
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
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 0);
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
				for (int i = 0; i < 3; i++)
				{
					base.Fire(new Direction(Angle, DirectionType.Absolute, -1f), new Speed(40f, SpeedType.Absolute), new WallBulletNoDodge("sniperUndodgeable", Angle));
					yield return new WaitForSeconds(0.025f);

				}
				yield break;
			}


			public class WallBulletNoDodge : Bullet
			{
				public WallBulletNoDodge(string BulletType, float Angle) : base(BulletType, false, false, false)
				{
					ang = Angle;
				}
				protected override IEnumerator Top()
				{
				
					yield break;
				}
				private float ang;
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet(float SpeedIncrease = 7) : base("sniperUndodgeable", false, false, false)
				{
					this.Inc = SpeedIncrease;
				}
				protected override IEnumerator Top()
				{
					yield break;
				}
				private float Inc;
			}
			public class BasicBulletDodge : Bullet
			{
				public BasicBulletDodge(float SpeedIncrease = 7) : base("sniper", false, false, false)
				{
					this.Inc = SpeedIncrease;
				}
				protected override IEnumerator Top()
				{
					yield break;
				}
				private float Inc;
			}
		}
		public class LaserCrossTwo : Script
		{
			protected override IEnumerator Top()
			{
				PrisonerPhaseOne.PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerPhaseOne.PrisonerController>();
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);

				controller.MoveTowardsPositionMethod(3f, 7);
				for (int e = 0; e < GameManager.Instance.AllPlayers.Length; e++)
				{
					float u = UnityEngine.Random.Range(15, 90);
					float Dir = UnityEngine.Random.value > 0.5f ? 0 : 30f;
					float M = UnityEngine.Random.value < 0.5f ? u : -u;
					for (int i = 0; i < 12; i++)
					{
						base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(GameManager.Instance.AllPlayers[e].transform.PositionVector2(), (30 * i) + Dir, this, M, 0.75f));
					}
				}
				yield return this.Wait(90);
				for (int q = 0; q < 3; q++)
				{
					for (int e = 0; e < GameManager.Instance.AllPlayers.Length; e++)
					{
						float u = UnityEngine.Random.Range(15, 60);
						float Dir = UnityEngine.Random.Range(-180, 180);
						float helpme = UnityEngine.Random.Range(180, -180);
						float M = UnityEngine.Random.value < 0.5f ? u : -u;
						for (int i = 0; i < 6; i++)
						{
							base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(GameManager.Instance.AllPlayers[e].transform.PositionVector2() + MathToolbox.GetUnitOnCircle(helpme, 8), (60 * i) + Dir, this, M, 0.75f));
						}
					}
					yield return this.Wait(45);
				}

				float f1 = this.RandomAngle();
				float u1 = UnityEngine.Random.Range(15, 30);
				float M1 = UnityEngine.Random.value < 0.5f ? u1 : -u1;
				Vector2 pos = this.BulletBank.aiActor.sprite.WorldCenter;
				for (int e = 0; e < 10; e++)
				{
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoobLerpPosition(pos, pos + MathToolbox.GetUnitOnCircle(f1 + (e * 36), Vector2.Distance(pos, GameManager.Instance.PrimaryPlayer.sprite.WorldCenter)), (36 * e) + 0, this, M1, 1.5f));
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoobLerpPosition(pos, pos + MathToolbox.GetUnitOnCircle(f1 + (e * 36) , Vector2.Distance(pos, GameManager.Instance.PrimaryPlayer.sprite.WorldCenter)), (36 * e) + 120, this, M1, 1.5f));
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoobLerpPosition(pos, pos + MathToolbox.GetUnitOnCircle(f1 + (e * 36) , Vector2.Distance(pos, GameManager.Instance.PrimaryPlayer.sprite.WorldCenter)), (36 * e) + 240, this, M1, 1.5f));
				}

				controller.MoveTowardsPositionMethod(1f, 5);
				yield return this.Wait(150);
				yield break;
			}

			private IEnumerator QuickscopeNoobLerpPosition(Vector2 startPos,Vector3 endPos ,float aimDir, LaserCrossTwo parent, float rotSet, float chargeTime = 0.5f)
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
				for (int i = 0; i < 10; i++)
				{
					base.Fire(Offset.OverridePosition(endPos), new Direction(aimDir + rotSet, DirectionType.Absolute, -1f), new Speed(20f, SpeedType.Absolute), new WallBulletNoDodge("sniperUndodgeable"));
					yield return new WaitForSeconds(0.025f);
				}
				yield break;
			}

			private IEnumerator QuickscopeNoob(Vector2 startPos, float aimDir, LaserCrossTwo parent, float rotSet, float chargeTime = 0.5f)
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
				for (int i = 0; i < 10; i++)
				{
					base.Fire(Offset.OverridePosition(startPos), new Direction(aimDir + rotSet, DirectionType.Absolute, -1f), new Speed(20f, SpeedType.Absolute), new WallBulletNoDodge("sniperUndodgeable"));
					yield return new WaitForSeconds(0.025f);
				}
				yield break;
			}

			public class WallBulletNoDodge : Bullet
			{
				public WallBulletNoDodge(string BulletType) : base(BulletType, false, false, false)
				{
				}
				protected override IEnumerator Top()
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
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(8f, SpeedType.Absolute), 30);
					yield break;
				}
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet() : base("sniperUndodgeable", false, false, false)
				{

				}
				protected override IEnumerator Top()
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
				protected override IEnumerator Top()
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

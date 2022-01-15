using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.ObjectModel;
using System.Collections;
using Planetside;
using Brave.BulletScript;
using Dungeonator;

public class UmbraController : BraveBehaviour
{
	public UmbraController()
    {
		CanTeleport = false;
		CanDash = false;
	}

	public void Start()
	{

		base.aiActor.sprite.usesOverrideMaterial = true;

		var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\plating.png");
		base.aiActor.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
		base.aiActor.sprite.renderer.material.SetTexture("_EeveeTex", texture);
		//chest2.sprite.renderer.material.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
		//chest2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
		base.aiActor.sprite.renderer.material.SetFloat("_StencilVal", 0);
		base.aiActor.sprite.renderer.material.SetFloat("_FlatColor", 0f);
		base.aiActor.sprite.renderer.material.SetFloat("_Perpendicular", 0);
		base.aiActor.behaviorSpeculator.CooldownScale *= 0.5f;
		base.aiActor.MovementSpeed *= 1.3f; 
		ImprovedAfterImage yeah = base.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
		yeah.dashColor = Color.black;
		yeah.spawnShadows = true;
		yeah.shadowTimeDelay = 0.01f;
		yeah.shadowLifetime = 2.5f;

		base.aiActor.healthHaver.spawnBulletScript = true;
		base.aiActor.healthHaver.chanceToSpawnBulletScript = 1f;
		base.aiActor.healthHaver.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
		base.aiActor.healthHaver.bulletScript = new CustomBulletScriptSelector(typeof(Baboomer));

		//base.aiActor.gameObject.AddComponent<AfterImageTrailController>().spawnShadows = false;
		//base.aiActor.gameObject.AddComponent<tk2dSpriteAttachPoint>();
		//base.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();
		//GameManager.Instance.DungeonMusicController.NotifyEnteredNewRoom(base.aiActor.GetAbsoluteParentRoom());

		base.aiActor.HasShadow = true;
		base.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["hollowpoint"]).ShadowObject;
		if (CanTeleport == true)
        {
			TeleportBehavior tely = new TeleportBehavior()
			{

				AttackableDuringAnimation = false,
				AllowCrossRoomTeleportation = false,
				teleportRequiresTransparency = false,
				hasOutlinesDuringAnim = true,
				ManuallyDefineRoom = false,
				MaxHealthThreshold = 1f,
				StayOnScreen = true,
				AvoidWalls = true,
				GoneTime = 2f,
				OnlyTeleportIfPlayerUnreachable = false,
				MinDistanceFromPlayer = 4f,
				MaxDistanceFromPlayer = -1f,
				AttackCooldown = 1f,
				InitialCooldown = 0f,
				RequiresLineOfSight = false,
				roomMax = new Vector2(0, 0),
				roomMin = new Vector2(0, 0),
				GlobalCooldown = 0.5f,
				Cooldown = 2f,

				CooldownVariance = 1f,
				InitialCooldownVariance = 0f,
				//goneAttackBehavior = null,
				IsBlackPhantom = false,
				GroupName = null,
				GroupCooldown = 0f,
				MinRange = 0,
				Range = 0,
				MinHealthThreshold = 0,
				MaxUsages = 0,
				AccumulateHealthThresholds = true,
				targetAreaStyle = null,
				HealthThresholds = new float[0],
				MinWallDistance = 0,
			};
			//base.aiActor.behaviorSpeculator.AttackBehaviors.Add(tely);
			BehaviorSpeculator nehav = base.aiActor.GetComponent<BehaviorSpeculator>();
			nehav.RegenerateCache();
			nehav.RefreshBehaviors();
			nehav.AttackBehaviors.Add(tely);
			nehav.PostAwakenDelay = 1;
			nehav.InstantFirstTick = false;
			tely.IsReady();
			tely.UpdateEveryFrame();
			
			/*
			foreach (AttackBehaviorBase att in EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["hollowpoint"]).behaviorSpeculator.AttackBehaviors)
			{
				if (att is TeleportBehavior)
				{
					base.aiActor.behaviorSpeculator.AttackBehaviors.Add(att);
				}
			}
			*/
		}
		if (CanDash == true)
        {
			GameObject piss = new GameObject();
			piss.transform.parent = base.aiActor.transform;
			piss.transform.position = base.aiActor.sprite.WorldCenter;

			GameObject m_CachedGunAttachPoint = piss;
			DashBehavior dash = new DashBehavior()
			{
				ShootPoint = m_CachedGunAttachPoint,
				dashDistance = 4.5f,
				dashTime = 0.25f,
				doubleDashChance = 0,
				enableShadowTrail = true,
				Cooldown = 4,
				dashDirection = DashBehavior.DashDirection.Random,
				warpDashAnimLength = false,
				hideShadow = true,
				InitialCooldown = 1f,
				//chargeAnim = null,
				//dashAnim = null,

				//bulletScript = new CustomBulletScriptSelector(typeof(DashAttack)),
				//BulletScript = new CustomBulletScriptSelector(typeof(Wail)),
				//LeadAmount = 0f,
				//AttackCooldown = 5f,
				//InitialCooldown = 4f,
				//TellAnimation = "wail",
				//FireAnimation = "wail",
				RequiresLineOfSight = false,
			};
			base.aiActor.behaviorSpeculator.AttackBehaviors.Add(dash);
			//base.aiActor.behaviorSpeculator.

		}
		try
		{
			/*
			string Name = base.aiActor.encounterTrackable.journalData.PrimaryDisplayName;
			if (Name != null)
			{
				PlanetsideModule.Strings.Enemies.Set("#UMBRABASENAME", "Umbra Of " + Name);
			}
			else
			{
				PlanetsideModule.Strings.Enemies.Set("#UMBRABASENAME", "Umbra Of The Unknown");
			}
			*/
			//string YourOwnGuid = base.aiActor.encounterTrackable.EncounterGuid;
			//base.aiActor.OverrideDisplayName = "Umbra Of A Damned Soul";
		}
		catch
		{
			ETGModConsole.Log("lol no");
		}

		base.healthHaver.OnPreDeath += this.OnPreDeath;
	}

	public void Update()
    {
		if (!base.aiActor.IsBlackPhantom && base.aiActor != null)
        {
			var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\plating.png");
			base.aiActor.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
			base.aiActor.sprite.renderer.material.SetTexture("_EeveeTex", texture);
			base.aiActor.sprite.renderer.material.SetFloat("_StencilVal", 0);
			base.aiActor.sprite.renderer.material.SetFloat("_FlatColor", 0f);
			base.aiActor.sprite.renderer.material.SetFloat("_Perpendicular", 0);
			base.aiActor.BecomeBlackPhantom();
		}
    }
	protected override void OnDestroy()
	{
		if (base.healthHaver)
		{
			base.healthHaver.OnPreDeath -= this.OnPreDeath;
		}
		base.OnDestroy();
	}

	private void OnPreDeath(Vector2 obj)
	{
		base.aiActor.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, false, false, false);
	}
	public class Baboomer : Script
	{
		protected override IEnumerator Top()
		{
			base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);
			base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("4d164ba3f62648809a4a82c90fc22cae").bulletBank.GetBullet("big_one"));
			base.Fire(Offset.OverridePosition(base.Position + new Vector2(0f, 30f)), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new Baboomer.BigBullet());

			yield break;
		}
		private class BigBullet : Bullet
		{
			public BigBullet() : base("big_one", false, false, false)
			{
			}

			public override void Initialize()
			{
				this.Projectile.spriteAnimator.StopAndResetFrameToDefault();
				base.Initialize();
			}

			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
				this.Projectile.specRigidbody.CollideWithTileMap = false;
				this.Projectile.specRigidbody.CollideWithOthers = false;
				yield return base.Wait(60);
				base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);
				this.Speed = 0f;
				this.Projectile.spriteAnimator.Play();
				base.Vanish(true);
				yield break;
			}

			public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				if (!preventSpawningProjectiles)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
					
					float num = base.RandomAngle();
					float Amount = 20;
					float Angle = 360 / Amount;
					for (int i = 0; i < Amount; i++)
					{
						base.Fire(new Direction(num + Angle * (float)i + 10, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new BurstBullet());
					}
					num = base.RandomAngle();
					for (int i = 0; i < Amount; i++)
					{
						base.Fire(new Direction(num + Angle * (float)i + 10, DirectionType.Absolute, -1f), new Speed(4f, SpeedType.Absolute), new BurstBullet());
					}
					num = base.RandomAngle();
					base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);
					return;
				}
			}
			public class BurstBullet : Bullet
			{
				public BurstBullet() : base("reversible", false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(base.Speed+6, SpeedType.Absolute), 120);
					yield break;
				}
			}

		}
	}

	public bool CanTeleport;
	public bool CanDash;
}





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
	public void Start()
	{

		base.aiActor.sprite.usesOverrideMaterial = true;

		var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\plating.png");
		base.aiActor.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
		base.aiActor.sprite.renderer.material.SetTexture("_EeveeTex", texture);
		base.aiActor.sprite.renderer.material.SetFloat("_StencilVal", 0);
		base.aiActor.sprite.renderer.material.SetFloat("_FlatColor", 0f);
		base.aiActor.sprite.renderer.material.SetFloat("_Perpendicular", 0);
		base.aiActor.behaviorSpeculator.CooldownScale *= 0.5f;
		base.aiActor.MovementSpeed *= 1.3f; 
		ImprovedAfterImage yeah = base.aiActor.gameObject.GetOrAddComponent<ImprovedAfterImage>();
		yeah.dashColor = Color.black;
		yeah.spawnShadows = true;
		yeah.shadowTimeDelay = 0.01f;
		yeah.shadowLifetime = 2.5f;
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
		SpawnManager.SpawnBulletScript(base.aiActor.gameActor, new CustomBulletScriptSelector(typeof(Baboomer)));
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





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

		base.aiActor.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
		base.aiActor.sprite.renderer.material.SetTexture("_EeveeTex", StaticTextures.NebulaTexture);
		base.aiActor.sprite.renderer.material.SetFloat("_StencilVal", 0);
		base.aiActor.sprite.renderer.material.SetFloat("_FlatColor", 0f);
		base.aiActor.sprite.renderer.material.SetFloat("_Perpendicular", 0);
		base.aiActor.behaviorSpeculator.CooldownScale *= 0.7f;
		base.aiActor.MovementSpeed *= 1.15f; 
		ImprovedAfterImage yeah = base.aiActor.gameObject.GetOrAddComponent<ImprovedAfterImage>();
		yeah.dashColor = Color.black;
		yeah.spawnShadows = true;
		yeah.shadowTimeDelay = 0.025f;
		yeah.shadowLifetime = 1.5f;
	}

	public void Update()
    {
		if (!base.aiActor.IsBlackPhantom && base.aiActor != null)
        {
			base.aiActor.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
			base.aiActor.sprite.renderer.material.SetTexture("_EeveeTex", StaticTextures.NebulaTexture);
			base.aiActor.sprite.renderer.material.SetFloat("_StencilVal", 0);
			base.aiActor.sprite.renderer.material.SetFloat("_FlatColor", 0f);
			base.aiActor.sprite.renderer.material.SetFloat("_Perpendicular", 0);
			base.aiActor.BecomeBlackPhantom();
		}
    }
	public override void OnDestroy()
	{
		if (base.aiActor)
		{
            base.aiActor.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, false, false, false);
            SpawnManager.SpawnBulletScript(base.aiActor.gameActor, new CustomBulletScriptSelector(typeof(Baboomer)));
        }

        base.OnDestroy();
	}

	public class Baboomer : Script
	{
		public override IEnumerator Top()
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

			public override IEnumerator Top()
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
					float Amount = 16;
					float Angle = 360 / Amount;
					for (int i = 0; i < Amount; i++)
					{
						base.Fire(new Direction((Angle * i) + num, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SpeedChangingBullet(14, 180));
                    }
					num = base.RandomAngle();
					for (int i = 0; i < Amount; i++)
					{
						base.Fire(new Direction((Angle * i) + num, DirectionType.Absolute, -1f), new Speed(3f, SpeedType.Absolute), new SpeedChangingBullet(12, 120));
					}
					num = base.RandomAngle();
					base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);
					return;
				}
			}
			

		}
	}

	public bool CanTeleport;
	public bool CanDash;
}





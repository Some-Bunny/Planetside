using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Planetside;
using BreakAbleAPI;
using Brave.BulletScript;
using System.Collections;

namespace Planetside
{

    public class DeturretController : MonoBehaviour
    {
		public int Speed;
        public void Start()
        {
			SpinBulletsControllerEnemyless enemyless = this.gameObject.AddComponent<SpinBulletsControllerEnemyless>();
			enemyless.ShootPoint = base.gameObject.transform.Find("attachy").gameObject;
			enemyless.OverrideBulletName = "deturretShell";
			enemyless.NumBullets = 5;
			enemyless.BulletMinRadius = 1;
			enemyless.BulletMaxRadius = 3.2f;
			enemyless.BulletCircleSpeed = Speed;
			enemyless.BulletsIgnoreTiles = true;
			enemyless.RegenTimer = 0.02f;
			enemyless.AmountOFLines = 4;
			SpriteOutlineManager.AddOutlineToSprite(base.GetComponent<tk2dBaseSprite>(), Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
		}
	}
    public class Deturrets
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/Enemies/Deturret/";
            string[] idlePaths = new string[]
            {
                defaultPath+"deturret_idle_001.png",
                defaultPath+"deturret_idle_002.png",
                defaultPath+"deturret_idle_003.png",
                defaultPath+"deturret_idle_004.png",
                defaultPath+"deturret_idle_005.png",
                defaultPath+"deturret_idle_006.png",
                defaultPath+"deturret_idle_007.png",
                defaultPath+"deturret_idle_008.png",
                defaultPath+"deturret_idle_009.png",
                defaultPath+"deturret_idle_010.png",
            };

            AIBulletBank.Entry entry = StaticUndodgeableBulletEntries.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"), "deturretShell");

            MajorBreakable deturretLeft = BreakableAPIToolbox.GenerateMajorBreakable("deturretLeft", idlePaths, 7, idlePaths, 18, 15000, true, 16, 16, -4, 0, true, null, null, true, null);

			EnemyToolbox.GenerateShootPoint(deturretLeft.gameObject, new Vector2(0.5f, 0.5f), "attachy");
            AIBulletBank bulletBankLeft = deturretLeft.gameObject.AddComponent<AIBulletBank>();
			DeturretController deturretC = deturretLeft.gameObject.AddComponent<DeturretController>();
			deturretC.Speed = -60;
			bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
			bulletBankLeft.Bullets.Add(entry);


            StaticReferences.StoredRoomObjects.Add("deturretLeft", deturretLeft.gameObject);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:deturretLeft", deturretLeft.gameObject);


            MajorBreakable deturretRight = BreakableAPIToolbox.GenerateMajorBreakable("deturretRight", idlePaths.Reverse().ToArray(), 7, idlePaths, 18, 15000, true, 16, 16, -4, 0, true, null, null, true, null);
			EnemyToolbox.GenerateShootPoint(deturretRight.gameObject, new Vector2(0.5f, 0.5f), "attachy");
			AIBulletBank bulletBankRight = deturretRight.gameObject.AddComponent<AIBulletBank>();
			DeturretController deturretR = deturretRight.gameObject.AddComponent<DeturretController>();
			deturretR.Speed = 60;
			bulletBankRight.Bullets = new List<AIBulletBank.Entry>();
			bulletBankRight.Bullets.Add(entry);

			StaticReferences.StoredRoomObjects.Add("deturretRight", deturretRight.gameObject);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:deturretRight", deturretRight.gameObject);

        }
    }
}


namespace Planetside
{
	public class SpinBulletsControllerEnemyless : MonoBehaviour
	{

		public SpeculativeRigidbody getBody()
        {
			return this.GetComponent<SpeculativeRigidbody>();
        }

		public void Start()
		{
			m_regenTimer = RegenTimer;
			this.m_bulletBank = this.gameObject.GetComponent<AIBulletBank>();
			float num = float.MaxValue;
			if (this && getBody())
			{
				num = (getBody().GetUnitCenter(ColliderType.HitBox) - getBody().GetUnitCenter(ColliderType.HitBox)).magnitude;
			}
			for (int i = 0; i < this.NumBullets; i++)
			{
				float num2 = Mathf.Lerp(this.BulletMinRadius, this.BulletMaxRadius, (float)i / ((float)this.NumBullets - 1f));
				if (num2 * 2f > num)
				{
					for (int e = 0; e < AmountOFLines; e++)

					{
						this.m_projectiles.Add(new SpinBulletsControllerEnemyless.ProjectileContainer
						{
							projectile = null,
							angle = (360f / AmountOFLines) * e,
							distFromCenter = num2
						});
					}
				}
				else
				{
					for (int e = 0; e < AmountOFLines; e++)
					{
						GameObject gameObject = this.m_bulletBank.CreateProjectileFromBank(this.GetBulletPosition(0f, num2), (360f / AmountOFLines) * e, this.OverrideBulletName, null, false, true, false);
						Projectile component = gameObject.GetComponent<Projectile>();
						component.specRigidbody.Velocity = Vector2.zero;
						component.ManualControl = true;
						if (this.BulletsIgnoreTiles)
						{
							component.specRigidbody.CollideWithTileMap = false;
						}
						this.m_projectiles.Add(new SpinBulletsControllerEnemyless.ProjectileContainer
						{
							projectile = component,
							angle = (360f / AmountOFLines) * e,
							distFromCenter = num2
						});
					}
				}
			}
		}

	

		public void Upkeep()
		{
			//base.DecrementTimer(ref this.m_regenTimer, false);
		}

		public void Update()
		{

			RunContinuousUpdate();

		}

		public void RunContinuousUpdate()
		{
			m_regenTimer -= BraveTime.DeltaTime;
			IsEnabled = true;
			if (this)
			{
				
			}
			for (int j = 0; j < this.m_projectiles.Count; j++)
			{
				if (!this.m_projectiles[j].projectile || !this.m_projectiles[j].projectile.gameObject.activeSelf)
				{
					this.m_projectiles[j].projectile = null;
				}
			}
			for (int k = 0; k < this.m_projectiles.Count; k++)
			{
				float angle = this.m_projectiles[k].angle + BraveTime.DeltaTime * (float)this.BulletCircleSpeed;
				this.m_projectiles[k].angle = angle;
				Projectile projectile = this.m_projectiles[k].projectile;
				if (projectile)
				{


					Vector2 bulletPosition = this.GetBulletPosition(angle, this.m_projectiles[k].distFromCenter);
					projectile.specRigidbody.Velocity = (bulletPosition - (Vector2)projectile.transform.position) / BraveTime.DeltaTime;
					if (projectile.shouldRotate)
					{
						projectile.transform.rotation = Quaternion.Euler(0f, 0f, 180f + (Quaternion.Euler(0f, 0f, 90f) * (this.ShootPoint.transform.position.XY() - bulletPosition)).XY().ToAngle());
					}

					projectile.ResetDistance();
				}
				else if (this.m_regenTimer <= 0f)
				{
					Vector2 bulletPosition2 = this.GetBulletPosition(this.m_projectiles[k].angle, this.m_projectiles[k].distFromCenter);
					if (GameManager.Instance.Dungeon.CellExists(bulletPosition2) && !GameManager.Instance.Dungeon.data.isWall((int)bulletPosition2.x, (int)bulletPosition2.y))
					{
						GameObject gameObject = this.m_bulletBank.CreateProjectileFromBank(bulletPosition2, 0f, this.OverrideBulletName, null, false, true, false);
						projectile = gameObject.GetComponent<Projectile>();
						projectile.specRigidbody.Velocity = Vector2.zero;
						projectile.ManualControl = true;

                        SpawnManager.PoolManager.Remove(projectile.gameObject.transform);
                        projectile.BulletScriptSettings.preventPooling = true;

                        if (this.BulletsIgnoreTiles == true)
						{
							projectile.specRigidbody.CollideWithTileMap = false;
							projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyBlocker));
							projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle));
							projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.LowObstacle));
							projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.BeamBlocker));
							projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBreakable));
							projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.TileBlocker));
							projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker));
							projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker));
							projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker));
							//projectile.specRigidbody.AddCollisionLayerIgnoreOverride

						}
						this.m_projectiles[k].projectile = projectile;
						this.m_regenTimer = this.RegenTimer;
					}
				}
			}
			for (int l = 0; l < this.m_projectiles.Count; l++)
			{
				if (this.m_projectiles[l] != null && this.m_projectiles[l].projectile)
				{
					this.m_projectiles[l].projectile.collidesWithEnemies = false;
				}
			}
		}

		public void EndContinuousUpdate()
		{
			IsEnabled = false;
			this.DestroyProjectiles();
		}

		public void StartContinuousUpdate()
		{
			//this.m_updateEveryFrame = true;
		}


		public void OnDestroy()
		{
			IsEnabled = false;
		}

		private Vector2 GetBulletPosition(float angle, float distFromCenter)
		{
			return this.ShootPoint.transform.position.XY() + BraveMathCollege.DegreesToVector(angle, distFromCenter);
		}

		public void DestroyProjectiles()
		{
			for (int i = 0; i < this.m_projectiles.Count; i++)
			{
				Projectile projectile = this.m_projectiles[i].projectile;
				if (projectile != null)
				{
					projectile.DieInAir(false, true, true, false);
				}
			}
			this.m_projectiles.Clear();
		}

		public bool IsEnabled;

		public string OverrideBulletName;

		public GameObject ShootPoint;

		public int NumBullets;

		public float BulletMinRadius;

		public float BulletMaxRadius;

		public int BulletCircleSpeed;

		public bool BulletsIgnoreTiles;

		public float RegenTimer;

		public float AmountOFLines;

		private readonly List<SpinBulletsControllerEnemyless.ProjectileContainer> m_projectiles = new List<SpinBulletsControllerEnemyless.ProjectileContainer>();

		private AIBulletBank m_bulletBank;

		private float m_regenTimer;

		private class ProjectileContainer
		{
			public Projectile projectile;
			public float angle;
			public float distFromCenter;
		}
	}

}
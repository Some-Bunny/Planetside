using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

using UnityEngine.Serialization;

namespace Planetside
{
	// Token: 0x02000075 RID: 117
	public class BulletsOnBulletDestructionComponent : MonoBehaviour
	{
		//BROKEN AS fuck, DO FIX
		public BulletsOnBulletDestructionComponent()
		{
			this.SpawnAngleOffset = 0; //What ADDITIONAL Angular Offset the projectile will spawn at
			this.projectileIDToSpawn = null; //The ID of the projectile you're using
			this.AmountToSpawn = 1; //How many bullets are spawned
			this.RandomizedSpeeds = false; //Randomizes spawned bullets speeds between 75%-125% of their base speeds if enabled
			this.BulletsSplitEvenlyAllRound = false; //All projectiles will spawn evenly along a circle depending on how many bullets spawn. If 5 bullets are set to spawn it will spawn them at 72 degree angle intervals
			this.BulletsSplitTowardsInitialDirection = false; //If enabled, the bullets will spawn in a way that resembles the Mass Shotgun
			this.AngleOffsetForSpawnedBullets = 3f; //Only for BulletsSplitTowardsInitialDirection use, sets what spread the bullets spawn to
			this.UsesCustomSprite = false; //Will the bullets use a custom sprite? If so, yes
			this.CustomSprite = null; //The name of the custom sprite
			this.CustomSpriteX = 1; //The X value of the custom sprite
			this.CustomSpriteY = 1; //The Y value of the custom sprite
		}


		private void Awake()
		{
			this.m_projectile = base.GetComponent<Projectile>();
			this.speculativeRigidBoy = base.GetComponent<SpeculativeRigidbody>();
			this.m_projectile.OnDestruction += DoSpawnBullets;
		}

		private void DoSpawnBullets(Projectile projectile)
		{
			bool flag = this.m_projectile == null;
			if (flag)
			{
				this.m_projectile = base.GetComponent<Projectile>();
			}
			bool flag2 = this.speculativeRigidBoy == null;
			if (flag2)
			{
				this.speculativeRigidBoy = base.GetComponent<SpeculativeRigidbody>();
			}
			if (BulletsSplitEvenlyAllRound == true)
            {
				for (int i = 0; i < AmountToSpawn; i++)
				{
					this.SpawnProjectile(this.projectileIDToSpawn, this.m_projectile.sprite.WorldCenter, this.m_projectile.transform.eulerAngles.z + (((360/AmountToSpawn)*i)+this.SpawnAngleOffset), null);
				}
			}
			if (BulletsSplitTowardsInitialDirection == true)
            {
				int MathsNum;
				int MathsNum1;
				MathsNum = ((int)AmountToSpawn / 2)+1;
				MathsNum1 = MathsNum - (int)AmountToSpawn;
				for (int a = MathsNum1; a <= MathsNum; a++)
				{
					this.SpawnProjectile(this.projectileIDToSpawn, this.m_projectile.sprite.WorldCenter, this.m_projectile.transform.eulerAngles.z + (this.AngleOffsetForSpawnedBullets*a + this.SpawnAngleOffset), null);
				}
			}
		}

		// Token: 0x0600029A RID: 666 RVA: 0x000185BC File Offset: 0x000167BC
		private void SpawnProjectile(Projectile proj, Vector3 spawnPosition, float zRotation, SpeculativeRigidbody collidedRigidbody = null)
		{
			GameObject gameObject = SpawnManager.SpawnProjectile(proj.gameObject, spawnPosition, Quaternion.Euler(0f, 0f, zRotation), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			{
				bool flag = component;
				if (flag)
				{
					component.SpawnedFromOtherPlayerProjectile = true;
					BulletsOnBulletDestructionComponent Values = proj.gameObject.GetComponent<BulletsOnBulletDestructionComponent>();
					Destroy(Values);
					component.SpawnedFromOtherPlayerProjectile = true;
					PlayerController playerController = this.m_projectile.Owner as PlayerController;
					component.baseData.damage *= playerController.stats.GetStatValue(PlayerStats.StatType.Damage);
					if (RandomizedSpeeds == true)
					{
						component.baseData.speed *= UnityEngine.Random.Range(0.75f, 1.25f);
					}
					else
					{
						component.baseData.speed *= 1f;
					}
					playerController.DoPostProcessProjectile(component);
					component.AdditionalScaleMultiplier = 0.8f;
					component.baseData.range = 3f;
					if (UsesCustomSprite == true)
					{
						//component.SetProjectileSpriteRight(CustomSprite, CustomSpriteX, CustomSpriteY, true, tk2dBaseSprite.Anchor.MiddleCenter, new int?(CustomSpriteX - 2), new int?(CustomSpriteY - 2), null, null, null);
					}
				}
			}
			
		}


		// Token: 0x040000EB RID: 235
		private Projectile m_projectile;

		// Token: 0x040000EC RID: 236
		private SpeculativeRigidbody speculativeRigidBoy;

		public float SpawnAngleOffset;
		public Projectile projectileIDToSpawn;
		public float AmountToSpawn;
		public bool RandomizedSpeeds;
		public bool BulletsSplitEvenlyAllRound;
		public bool BulletsSplitTowardsInitialDirection;
		public float AngleOffsetForSpawnedBullets;
		public bool UsesCustomSprite;
		public string CustomSprite;
		public int CustomSpriteX;
		public int CustomSpriteY;
	}
}
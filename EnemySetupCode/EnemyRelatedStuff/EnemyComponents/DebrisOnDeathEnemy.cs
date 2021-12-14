using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planetside
{
	// Token: 0x0200008C RID: 140
	public class ExpandExplodeOnDeath : ExplodeOnDeath
	{
		public ExpandExplodeOnDeath()
		{
			this.immuneToIBombApp = false;
			this.deathType = OnDeathBehavior.DeathType.PreDeath;
			this.preDeathDelay = 0.1f;
			this.useDefaultExplosion = false;
			this.spawnItemsOnExplosion = false;
			this.isCorruptedObject = false;
			this.ExplosionNotGuranteed = false;
			this.isCorruptedNPC = false;
			this.spawnShardsOnDeath = false;
			this.numberOfDefaultItemsToSpawn = 1;
			this.ExplosionOdds = 0.3f;
			this.ExplosionDamage = 120f;
			this.breakStyle = MinorBreakable.BreakStyle.BURST;
			this.direction = Vector2.zero;
			this.minAngle = 0f;
			this.maxAngle = 0f;
			this.verticalSpeed = 0.4f;
			this.minMagnitude = 0.25f;
			this.maxMagnitude = 0.5f;
			this.heightOffGround = 0.1f;
			this.m_hasTriggered = false;
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x000A7124 File Offset: 0x000A5324
		public void ManuallyTrigger(Vector2 damageDirection)
		{
			this.OnTrigger(damageDirection);
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x000B2780 File Offset: 0x000B0980
		protected override void OnTrigger(Vector2 dirVec)
		{
			if (this.m_hasTriggered)
			{
				return;
			}
			this.m_hasTriggered = true;
			if (this.isCorruptedNPC)
			{
				this.EnableShopTheftAndCurse();
			}
			if (this.isCorruptedObject)
			{
				this.DoCorruptedDeathSFX();
			}
			if (this.spawnShardsOnDeath)
			{
				this.HandleShardSpawns(dirVec);
			}
			this.DoExplosion();
			if (this.spawnItemsOnExplosion)
			{
				this.DoItemSpawn();
			}
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x000B27DC File Offset: 0x000B09DC
		private void DoCorruptedDeathSFX()
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.position = base.gameObject.transform.position;
			if (gameObject.transform.position.GetAbsoluteRoom() != null)
			{
				gameObject.transform.parent = gameObject.transform.position.GetAbsoluteRoom().hierarchyParent;
				GameManager.Instance.StartCoroutine(this.DoDelayedCorruptionDeathSound(gameObject, 0.2f));
				return;
			}
			UnityEngine.Object.Destroy(gameObject);
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x000B285C File Offset: 0x000B0A5C
		private void EnableShopTheftAndCurse()
		{
			PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
			StatModifier item = new StatModifier
			{
				amount = 1f,
				modifyType = StatModifier.ModifyMethod.ADDITIVE,
				statToBoost = PlayerStats.StatType.Curse
			};
			bestActivePlayer.ownerlessStatModifiers.Add(item);
			bestActivePlayer.stats.RecalculateStats(bestActivePlayer, false, false);
			if (this.NPCShop != null)
			{
				this.NPCShop.SetCapableOfBeingStolenFrom(true, "DestroyedShopOwner", null);
			}
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x000B28D8 File Offset: 0x000B0AD8
		private void DoExplosion()
		{
			if (this.ExplosionNotGuranteed && UnityEngine.Random.value > this.ExplosionOdds)
			{
				return;
			}
			if (this.useDefaultExplosion)
			{
				Exploder.DoDefaultExplosion(base.specRigidbody.GetUnitCenter(ColliderType.HitBox), Vector2.zero, null, true, CoreDamageTypes.None, false);
				Exploder.DoRadialDamage(this.ExplosionDamage, base.specRigidbody.GetUnitCenter(ColliderType.HitBox), 3.5f, true, true, false, null);
				return;
			}
			Exploder.Explode(base.specRigidbody.GetUnitCenter(ColliderType.HitBox), this.ExpandExplosionData, Vector2.zero, null, true, CoreDamageTypes.None, false);
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x000B2970 File Offset: 0x000B0B70
		private void DoItemSpawn()
		{
			if (this.ItemList == null)
			{
				this.ItemList = new List<PickupObject>();
				if (this.numberOfDefaultItemsToSpawn == 1)
				{
					PickupObject.ItemQuality itemQuality = (UnityEngine.Random.value >= 0.2f) ? ((!BraveUtility.RandomBool()) ? PickupObject.ItemQuality.C : PickupObject.ItemQuality.D) : PickupObject.ItemQuality.B;
					GenericLootTable lootTable = (!BraveUtility.RandomBool()) ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable;
					PickupObject itemOfTypeAndQuality = LootEngine.GetItemOfTypeAndQuality<PickupObject>(itemQuality, lootTable, false);
					if (itemOfTypeAndQuality)
					{
						LootEngine.SpawnItem(itemOfTypeAndQuality.gameObject, base.specRigidbody.GetUnitCenter(ColliderType.HitBox), Vector2.zero, 0f, true, true, false);
					}
					return;
				}
				for (int i = 0; i < this.numberOfDefaultItemsToSpawn; i++)
				{
					PickupObject.ItemQuality itemQuality2 = (UnityEngine.Random.value >= 0.2f) ? ((!BraveUtility.RandomBool()) ? PickupObject.ItemQuality.C : PickupObject.ItemQuality.D) : PickupObject.ItemQuality.B;
					GenericLootTable lootTable2 = (!BraveUtility.RandomBool()) ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable;
					PickupObject itemOfTypeAndQuality2 = LootEngine.GetItemOfTypeAndQuality<PickupObject>(itemQuality2, lootTable2, false);
					if (itemOfTypeAndQuality2)
					{
						this.ItemList.Add(itemOfTypeAndQuality2);
					}
				}
			}
			if (this.ItemList.Count <= 0)
			{
				return;
			}
			if (this.ItemList.Count == 1)
			{
				LootEngine.SpawnItem(this.ItemList[0].gameObject, base.specRigidbody.GetUnitCenter(ColliderType.HitBox), Vector2.zero, 0f, true, true, false);
				return;
			}
			if (this.ItemList.Count > 1)
			{
				foreach (PickupObject pickupObject in this.ItemList)
				{
					LootEngine.SpawnItem(pickupObject.gameObject, base.specRigidbody.GetUnitCenter(ColliderType.HitBox), Vector2.zero, 0f, true, true, false);
				}
			}
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x000B2B54 File Offset: 0x000B0D54
		private IEnumerator DoDelayedCorruptionDeathSound(GameObject parentObject, float delay = 0.2f)
		{
			yield return new WaitForSeconds(delay);
			AkSoundEngine.PostEvent("Play_EX_CorruptedObjectDestroyed_01", parentObject);
			yield break;
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x000B2B6C File Offset: 0x000B0D6C
		public void HandleShardSpawns(Vector2 sourceVelocity)
		{
			MinorBreakable.BreakStyle breakStyle = this.breakStyle;
			if (sourceVelocity == Vector2.zero && this.breakStyle != MinorBreakable.BreakStyle.CUSTOM)
			{
				breakStyle = MinorBreakable.BreakStyle.BURST;
			}
			float num = 1.5f;
			switch (breakStyle)
			{
				case MinorBreakable.BreakStyle.CONE:
					this.SpawnShards(sourceVelocity, -45f, 45f, num, sourceVelocity.magnitude * 0.5f, sourceVelocity.magnitude * 1.5f);
					return;
				case MinorBreakable.BreakStyle.BURST:
					this.SpawnShards(Vector2.right, -180f, 180f, num, 1f, 2f);
					return;
				case MinorBreakable.BreakStyle.JET:
					this.SpawnShards(sourceVelocity, -15f, 15f, num, sourceVelocity.magnitude * 0.5f, sourceVelocity.magnitude * 1.5f);
					return;
				default:
					if (breakStyle == MinorBreakable.BreakStyle.CUSTOM)
					{
						this.SpawnShards(this.direction, this.minAngle, this.maxAngle, this.verticalSpeed, this.minMagnitude, this.maxMagnitude);
					}
					return;
			}
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x000B2C60 File Offset: 0x000B0E60
		public void SpawnShards(Vector2 direction, float minAngle, float maxAngle, float verticalSpeed, float minMagnitude, float maxMagnitude)
		{
			Vector3 position = base.specRigidbody.GetUnitCenter(ColliderType.HitBox);
			if (this.shardClusters != null && this.shardClusters.Length != 0)
			{
				int num = UnityEngine.Random.Range(0, 10);
				for (int i = 0; i < this.shardClusters.Length; i++)
				{
					ShardCluster shardCluster = this.shardClusters[i];
					int num2 = UnityEngine.Random.Range(shardCluster.minFromCluster, shardCluster.maxFromCluster + 1);
					int num3 = UnityEngine.Random.Range(0, shardCluster.clusterObjects.Length);
					for (int j = 0; j < num2; j++)
					{
						float lowDiscrepancyRandom = BraveMathCollege.GetLowDiscrepancyRandom(num);
						num++;
						float z = Mathf.Lerp(minAngle, maxAngle, lowDiscrepancyRandom);
						Vector3 vector = Quaternion.Euler(0f, 0f, z) * (direction.normalized * UnityEngine.Random.Range(minMagnitude, maxMagnitude)).ToVector3ZUp(verticalSpeed);
						int num4 = (num3 + j) % shardCluster.clusterObjects.Length;
						GameObject gameObject = SpawnManager.SpawnDebris(shardCluster.clusterObjects[num4].gameObject, position, Quaternion.identity);
						tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
						if (base.sprite.attachParent != null && component != null)
						{
							component.attachParent = base.sprite.attachParent;
							component.HeightOffGround = base.sprite.HeightOffGround;
						}
						DebrisObject component2 = gameObject.GetComponent<DebrisObject>();
						vector = Vector3.Scale(vector, shardCluster.forceAxialMultiplier) * shardCluster.forceMultiplier;
						component2.Trigger(vector, this.heightOffGround, shardCluster.rotationMultiplier);
					}
				}
			}
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x000B2DEC File Offset: 0x000B0FEC
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x04000677 RID: 1655
		public bool useDefaultExplosion;

		// Token: 0x04000678 RID: 1656
		public bool spawnItemsOnExplosion;

		// Token: 0x04000679 RID: 1657
		public bool isCorruptedObject;

		// Token: 0x0400067A RID: 1658
		public bool isCorruptedNPC;

		// Token: 0x0400067B RID: 1659
		public bool ExplosionNotGuranteed;

		// Token: 0x0400067C RID: 1660
		public bool spawnShardsOnDeath;

		// Token: 0x0400067D RID: 1661
		public int numberOfDefaultItemsToSpawn;

		// Token: 0x0400067E RID: 1662
		public float ExplosionOdds;

		// Token: 0x0400067F RID: 1663
		public float ExplosionDamage;

		// Token: 0x04000680 RID: 1664
		private bool m_hasTriggered;

		// Token: 0x04000681 RID: 1665
		public List<PickupObject> ItemList;

		// Token: 0x04000682 RID: 1666
		public ExplosionData ExpandExplosionData;

		// Token: 0x04000683 RID: 1667
		public BaseShopController NPCShop;

		// Token: 0x04000684 RID: 1668
		public MinorBreakable.BreakStyle breakStyle;

		// Token: 0x04000685 RID: 1669
		public Vector2 direction;

		// Token: 0x04000686 RID: 1670
		public float minAngle;

		// Token: 0x04000687 RID: 1671
		public float maxAngle;

		// Token: 0x04000688 RID: 1672
		public float verticalSpeed;

		// Token: 0x04000689 RID: 1673
		public float minMagnitude;

		// Token: 0x0400068A RID: 1674
		public float maxMagnitude;

		// Token: 0x0400068B RID: 1675
		public float heightOffGround;

		// Token: 0x0400068C RID: 1676
		public ShardCluster[] shardClusters;
	}
}

using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;

namespace Planetside
{
	public class BunnyBullet : AIActor
	{
		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("bunny_bullet", "Some Bunny", 16, 21,new List<int> { 44, 45, 46, 47 }, new List<int> { 48, 49, 50, 51, 52 }, null, new SkellScript(), 2f);
		}


		public class SkellScript : Script 
		{
			public override IEnumerator Top() 
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				DraGunController dragunController = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>();
				this.FireRocket(dragunController.skyRocket, player.sprite.WorldCenter);
				yield break;
			}
			private void FireRocket(GameObject skyRocket, Vector2 target)
			{
				SkyRocket component = SpawnManager.SpawnProjectile(skyRocket, base.Position, Quaternion.identity, true).GetComponent<SkyRocket>();
				component.TargetVector2 = target;
				tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
				component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
				component.ExplosionData.ignoreList.Add(base.BulletBank.specRigidbody);
			}
		}
	}
}









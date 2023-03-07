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
	public class ApacheBullet : AIActor
	{
		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("apache_bullet", "Apache Thunder", 16, 15, new List<int> { 9, 10, 11, 12 }, new List<int> { 13,14, 15, 16 }, null, new SkellScript());
		}

		public class SkellScript : Script 
		{
			public override IEnumerator Top() 
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
                RoomHandler currentRoom = player.CurrentRoom;
                AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_002");
                this.Mines_Cave_In = assetBundle.LoadAsset<GameObject>("Mines_Cave_In");
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Mines_Cave_In, player.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(-4, 4), UnityEngine.Random.Range(-4, 4)), Quaternion.identity);
                HangingObjectController RockSlideController = gameObject.GetComponent<HangingObjectController>();
                RockSlideController.triggerObjectPrefab = null;
                GameObject[] additionalDestroyObjects = new GameObject[]
                {
                RockSlideController.additionalDestroyObjects[1]
                };
                RockSlideController.additionalDestroyObjects = additionalDestroyObjects;
                UnityEngine.Object.Destroy(gameObject.transform.Find("Sign").gameObject);
                RockSlideController.ConfigureOnPlacement(currentRoom);
                yield return Wait(30);
                RockSlideController.Interact(player);
                yield break; 
			}
			private GameObject Mines_Cave_In;
		}
	}
}









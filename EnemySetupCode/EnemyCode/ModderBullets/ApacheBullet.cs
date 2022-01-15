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
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/apache/";
			string[] spritePaths = new string[]
			{
				TemplatePath+"apachebullet_idle_001.png",
				TemplatePath+"apachebullet_idle_002.png",
				TemplatePath+"apachebullet_idle_003.png",
				TemplatePath+"apachebullet_idle_004.png",

				TemplatePath+"apachebullet_die_001.png",
				TemplatePath+"apachebullet_die_002.png",
				TemplatePath+"apachebullet_die_003.png",
				TemplatePath+"apachebullet_die_004.png",
			};
			EnemyToolbox.CreateNewBulletBankerEnemy("apache_bullet", "Apache Thunder", 16, 15, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7 }, null, new SkellScript());
		}

		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				for (int j = 0; j < 2; j++)
                {
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
				}
				yield return new WaitForSeconds(3f);
				yield break; 
			}
			private GameObject Mines_Cave_In;
		}
	}
}









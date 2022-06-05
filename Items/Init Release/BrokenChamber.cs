using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Collections;
using SaveAPI;
using Brave.BulletScript;

namespace Planetside
{
	// Token: 0x02000035 RID: 53
	public class BrokenChamber : PassiveItem
	{
		public static void Init()
		{
			string name = "Broken Chamber";
			string resourcePath = "Planetside/Resources/brokenchamber.png";
			GameObject gameObject = new GameObject(name);
			BrokenChamber warVase = gameObject.AddComponent<BrokenChamber>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "A bad omen looms...";
			string longDesc = "A broken, forgotten chamber. Dark power seeps from it.\n\nYou feel unsafe, but feel like this is necessary.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog");
			warVase.quality = PickupObject.ItemQuality.EXCLUDED;
			BrokenChamber.BrokenChamberID = warVase.PickupObjectId;
		}
		public static int BrokenChamberID;
		public override void Pickup(PlayerController player)
		{
			BrokenChamberComponent Values = player.gameObject.AddComponent<BrokenChamberComponent>();
			Values.TimeBetweenRockFalls = 7.5f;
			Values.player = player;
			base.CanBeDropped = false;
			base.Pickup(player);
		}
		protected override void OnDestroy()
		{
			if (base.Owner != null)
            {
				PlayerController player = base.Owner;
				BrokenChamberComponent Values = player.gameObject.GetComponent<BrokenChamberComponent>();
				Destroy(Values);
			}
			base.OnDestroy();
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			return result;
		}
		protected override void Update()
		{
			base.Update();
			/*
			bool flag = base.Owner != null && this.OnCooldown;
			if (flag)
			{
				PlayerController player = base.Owner;
				this.OnCooldown = false;
			}
			*/
		}
		//private bool OnCooldown = true;
		public IEnumerator BasicBoolDown()
		{
			PlayerController player = base.Owner;
			yield return new WaitForSeconds(7f);
			AkSoundEngine.PostEvent("Play_BOSS_wall_slam_01", base.gameObject);
			GameObject dragunBoulder = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
			GameObject dragunRocket = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyRocket;
			IntVector2? vector = (player as PlayerController).CurrentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 2), CellTypes.FLOOR | CellTypes.PIT, false, null);
			Vector2 vector2 = vector.Value.ToVector2();
			this.FireRocket(dragunBoulder, player.sprite.WorldCenter);
			yield return new WaitForSeconds(0.5f);
			this.FireRocket(dragunBoulder, vector2);
			//this.OnCooldown = true;
			yield break;
		}

		private void FireRocket(GameObject skyRocket, Vector2 target)
		{
			PlayerController player = base.Owner;
			SkyRocket component = SpawnManager.SpawnProjectile(skyRocket, player.sprite.WorldCenter, Quaternion.identity, true).GetComponent<SkyRocket>();
			component.TargetVector2 = target;
			tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
			component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
			component.ExplosionData.ignoreList.Add(player.specRigidbody);
		}
		public int NumRockets = 1;

		public tk2dSprite CircleSprite;


	}
}

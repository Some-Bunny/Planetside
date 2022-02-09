using System;
using System.Collections.Generic;
using ItemAPI;
using UnityEngine;

namespace Planetside
{
	internal class LeSackPickup : PickupObject
	{
		public static void Init()
		{
			string name = "Sack Of Pickups";
			string resourcePath = "Planetside/Resources/Pickups/sackofstuff.png";
			GameObject gameObject = new GameObject(name);
			LeSackPickup pickup = gameObject.AddComponent<LeSackPickup>();
			SpeculativeRigidbody speculativeRigidbody = gameObject.AddComponent<SpeculativeRigidbody>();
			PixelCollider item = new PixelCollider
			{
				IsTrigger = true,
				ManualWidth = 14,
				ManualHeight = 16,
				ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
				CollisionLayer = CollisionLayer.PlayerBlocker,
				ManualOffsetX = 0,
				ManualOffsetY = 0
			};
			speculativeRigidbody.PixelColliders = new List<PixelCollider>
			{
				item
			};
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Goodie Bag";
			string longDesc = "A pre-packed sack filled with pickups. I wonder what's inside?";
			pickup.SetupItem(shortDesc, longDesc, "psog");
			pickup.quality = PickupObject.ItemQuality.COMMON;
			LeSackPickup.SaccID = pickup.PickupObjectId;
			pickup.CustomCost = 35;
			pickup.UsesCustomCost = true;

			WeightedGameObject weightedObject = new WeightedGameObject();
			weightedObject.SetGameObject(gameObject);
			weightedObject.weight = 0.33f;
			weightedObject.rawGameObject = gameObject;
			weightedObject.pickupId = pickup.PickupObjectId;
			weightedObject.forceDuplicatesPossible = true;
			weightedObject.additionalPrerequisites = new DungeonPrerequisite[0];
			

			GenericLootTable thanksbotluvya = ItemBuilder.LoadShopTable("Shop_Gungeon_Cheap_Items_01");
			thanksbotluvya.defaultItemDrops.elements.Add(weightedObject);
		}

		public override void Pickup(PlayerController player)
		{			
			this.random = UnityEngine.Random.Range(0.0f, 1.0f);
			if (random < 0.7 && random > 0)
			{
				float itemsToSpawn = UnityEngine.Random.Range(2, 4);
				float spewItemDir = 360 / itemsToSpawn;

				for (int i = 0; i < itemsToSpawn; i++)
				{
					int id = BraveUtility.RandomElement<int>(LeSackPickup.GenericPool);
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, player.sprite.WorldCenter, new Vector2((spewItemDir * (itemsToSpawn*i)) , spewItemDir * (itemsToSpawn * i)), 1.2f, false, true, false);
				}
			}
			else if (random > 0.7 && random < 0.95)
			{
				float itemsToSpawn = UnityEngine.Random.Range(2, 4);
				float spewItemDir = 360 / itemsToSpawn;

				for (int i = 0; i < itemsToSpawn; i++)
				{
					int id = BraveUtility.RandomElement<int>(LeSackPickup.HPPool);
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, player.sprite.WorldCenter, new Vector2((spewItemDir * (itemsToSpawn * i)), spewItemDir * (itemsToSpawn * i)), 1.2f, false, true, false);
				}
			}
			if (random > 0.95 && random < 1)
			{
				float itemsToSpawn = UnityEngine.Random.Range(3, 5);
				float spewItemDir = 360 / itemsToSpawn;
				for (int i = 0; i < itemsToSpawn; i++)
				{
					int id = BraveUtility.RandomElement<int>(LeSackPickup.Lootdrops);
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, player.sprite.WorldCenter, new Vector2((spewItemDir * (itemsToSpawn * i)), spewItemDir * (itemsToSpawn * i)), 1.2f, false, true, false);
				}
			}
			
			player.BloopItemAboveHead(base.sprite, "");
			AkSoundEngine.PostEvent("Play_ENM_critter_poof_01", base.gameObject);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		private float random;

		protected void Start()
		{
			try
			{
				this.storedBody = base.gameObject.GetComponent<SpeculativeRigidbody>();
				SpeculativeRigidbody speculativeRigidbody = this.storedBody;
				SpeculativeRigidbody speculativeRigidbody2 = speculativeRigidbody;
				speculativeRigidbody2.OnTriggerCollision = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Combine(speculativeRigidbody2.OnTriggerCollision, new SpeculativeRigidbody.OnTriggerDelegate(this.OnPreCollision));
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
			}
		}

		private void OnPreCollision(SpeculativeRigidbody otherRigidbody, SpeculativeRigidbody source, CollisionData collisionData)
		{
			bool hasBeenPickedUp = this.m_hasBeenPickedUp;
			if (!hasBeenPickedUp)
			{
				PlayerController component = otherRigidbody.GetComponent<PlayerController>();
				bool flag = component != null;
				if (flag)
				{
					this.m_hasBeenPickedUp = true;
					this.Pickup(component);
				}
			}
		}
		public static int SaccID;
		public SpeculativeRigidbody storedBody;
		private bool m_hasBeenPickedUp;
		public static List<int> GenericPool = new List<int>
		{
			67,//key
			224,//blank
			600,//partial-ammo
			78,//ammo
			565//glass guon stone
		};
		public static List<int> HPPool = new List<int>
		{
			73,//half-heart
			85,//full-heart
			120//armor

		};
		public static List<int> MoneyPool = new List<int>
		{
			68, //1 casing
		};

		public static List<int> Lootdrops = new List<int>
		{
			73,//half-heart
			85,//full-heart
			120,//armor
			67,//key
			224,//blank
			600,//partial-ammo
			78,//ammo
			565//glass guon stone
		};
	}
}

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

			Normal_Table = LootTableTools.CreateLootTable();
			Normal_Table.AddItemsToPool(new Dictionary<int, float>()
			{
				{67, 1},
				{224, 0.8f},
				{600, 0.5f},
				{78, 0.8f},
				{565, 0.5f},
				{73, 0.8f},
				{85, 0.5f},
				{120, 0.6f}
			});

			HP_Table = LootTableTools.CreateLootTable();
			HP_Table.AddItemsToPool(new Dictionary<int, float>() 
			{
				{73, 0.8f},
				{120, 1f},
				{85, 0.5f}
			});

			Wacky_Table = LootTableTools.CreateLootTable();
			Wacky_Table.AddItemsToPool(new Dictionary<int, float>()
			{
				{63, 0.3f},
				{74, 0.1f},
				{108, 0.3f },
				{67, 1},
				{224, 0.8f},
				{600, 0.5f},
				{78, 0.8f},
				{565, 0.5f},

				{73, 0.8f},
				{85, 0.5f},
				{120, 0.6f},
				{127, 0.5f},
				{148, 0.1f},
				{77, 0.4f},
				{79, 0.1f}
			});
		}
		public static GenericLootTable Normal_Table;
		public static GenericLootTable HP_Table;
		public static GenericLootTable Wacky_Table;
	


		public override void Pickup(PlayerController player)
		{
			int itemsToSpawn = UnityEngine.Random.Range(2, 5);
			float random = UnityEngine.Random.Range(0.00f, 1.00f);
			GenericLootTable Table = Normal_Table;
			if (random.IsBetweenRange(0.7f, 0.95f))
			{
				Table = HP_Table;
			}
			else if (random > 0.95 && random < 1)
			{
				Table = Wacky_Table;
			}

			for (int i = 0; i < itemsToSpawn; i++)
			{
				LootEngine.SpawnItem(Table.SelectByWeight(), player.sprite.WorldBottomCenter,MathToolbox.GetUnitOnCircle(MathToolbox.SubdivideArc(Vector2.left.ToAngle(), -180, itemsToSpawn, i), 1.5f), 1.2f, false, true, false);
			}

			player.BloopItemAboveHead(base.sprite, "");
			AkSoundEngine.PostEvent("Play_ENM_critter_poof_01", base.gameObject);
			UnityEngine.Object.Destroy(base.gameObject);
		}
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

	}
}

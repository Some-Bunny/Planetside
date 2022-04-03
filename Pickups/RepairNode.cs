using System;
using System.Collections.Generic;
using ItemAPI;
using UnityEngine;

namespace Planetside
{
	internal class RepairNode : PickupObject
	{
		public static void Init()
		{
			string name = "Hegemony Repair Node";
			string resourcePath = "Planetside/Resources/repairNode.png";
			GameObject gameObject = new GameObject(name);
			RepairNode pickup = gameObject.AddComponent<RepairNode>();
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
			string shortDesc = "I Will Put You Back Together";
			string longDesc = "Does nothing.\n\nUsed to repair the broken Hegemony Device.";
			pickup.SetupItem(shortDesc, longDesc, "psog");
			pickup.quality = PickupObject.ItemQuality.EXCLUDED;
			RepairNode.RepairNodeID = pickup.PickupObjectId;
			pickup.CustomCost = 25;
			pickup.UsesCustomCost = true;
		}

		public override void Pickup(PlayerController player)
		{						
			player.BloopItemAboveHead(base.sprite, "");
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
		public static int RepairNodeID;
		public SpeculativeRigidbody storedBody;
		private bool m_hasBeenPickedUp;
	}
}

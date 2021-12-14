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
using Pathfinding;


namespace Planetside
{
	public class CheatDeathComponent : BraveBehaviour
	{
		public void Start()
		{
			base.healthHaver.minimumHealth = this.CheatDeath;
		}
		public void Update()
		{
			if (base.healthHaver.GetCurrentHealth() <= this.CheatDeath)
			{
				base.StartCoroutine(this.ConvertToNearDeath());
			}
		}
		private IEnumerator ConvertToNearDeath()
		{
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			base.healthHaver.FullHeal();
			TextBoxManager.ShowTextBox(base.aiActor.transform.position + new Vector3(0.5f, 0.5f), base.aiActor.transform, 3, "poor aim, kid.", string.Empty, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
			this.teleporter = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>();
			UnityEngine.Object.Instantiate<GameObject>(this.teleporter.TelefragVFXPrefab, base.aiActor.sprite.WorldCenter, Quaternion.identity);
			CellArea area = base.aiActor.ParentRoom.area;
			Vector2 Center = area.UnitCenter;
			Vector2 b = base.aiActor.specRigidbody.UnitBottomCenter - base.aiActor.transform.position.XY();
			IntVector2? bestRewardLocation = player.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
			base.aiActor.transform.position = Pathfinder.GetClearanceOffset(bestRewardLocation.Value, base.aiActor.Clearance).WithY((float)bestRewardLocation.Value.y) - b;
			base.aiActor.specRigidbody.Reinitialize();
			yield break;
		}

		private TeleporterPrototypeItem teleporter;
		public float minimumHealth;
		public float CheatDeath = 2f;
	}
}
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
	public class FriendlyGhostEnemyComponent : BraveBehaviour
	{
		public void Start()
		{
			base.aiActor.SetIsFlying(true, "wing");
			base.aiActor.IgnoreForRoomClear = true;
			base.aiActor.PreventAutoKillOnBossDeath = true;

			base.aiActor.ManualKnockbackHandling = true; 
			base.aiActor.knockbackDoer.SetImmobile(true, "j"); 
			base.aiActor.PreventFallingInPitsEver = true;

			base.aiActor.RegisterOverrideColor(GhostColor, "spooky");
			base.healthHaver.minimumHealth = this.CheatDeath;
			base.aiActor.CanTargetEnemies = true;
			base.aiActor.CanTargetPlayers = false;

			base.aiActor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox,
				CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerBlocker, CollisionLayer.BeamBlocker));

			base.aiActor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.Trap));
			var text = new List<string>
			{
				"Am I... free?",
				"Where am I?"
			};
			float fard = UnityEngine.Random.Range(1, 4);
			if (fard == 1)
            {
				TextBoxManager.ShowTextBox(base.aiActor.transform.position + new Vector3(0.5f, 0.5f), base.aiActor.transform, 3, BraveUtility.RandomElement<string>(text), string.Empty, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
			}
			base.StartCoroutine(this.TimeOfDeath());
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

			yield break;
		}
		private IEnumerator TimeOfDeath()
		{
			yield return new WaitForSeconds(15f);
			TextBoxManager.ShowTextBox(base.aiActor.transform.position + new Vector3(0.5f, 0.5f), base.aiActor.transform, 3, "Time to depart again...", string.Empty, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
			yield return new WaitForSeconds(1f);
			this.teleporter = PickupObjectDatabase.GetById(573).GetComponent<ChestTeleporterItem>();
			UnityEngine.Object.Instantiate<GameObject>(this.teleporter.TeleportVFX, base.aiActor.sprite.WorldCenter, Quaternion.identity);
			Destroy(base.aiActor.gameObject);
			yield break;
		}
		public Color GhostColor = new Color(10f, 10f, 10f, 0.75f);
		private ChestTeleporterItem teleporter;
		public float minimumHealth;
		public float CheatDeath = 20f;
	}
}
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
using SaveAPI;

namespace Planetside
{
    public class TarnishedRounds : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Tarnished Rounds";
            string resourceName = "Planetside/Resources/tarnishedRounds.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<TarnishedRounds>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Old With Age";
            string longDesc = "These bullets have been subjected to the effects of aging, a rare occurance in the Gungeon." +
                "\n\nAny foe hit with these rounds will be tarnished just as they are.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.A;
			//item.SetupUnlockOnCustomFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED, true);

			TarnishedRounds.TarnishedRoundsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int TarnishedRoundsID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{

				float procChance = 0.25f;
				procChance *= effectChanceScalar;
				if (UnityEngine.Random.value <= procChance)
				{
					sourceProjectile.AdjustPlayerProjectileTint(new Color(0.6f, 0.7f ,0f), 2, 0f);
					sourceProjectile.AppliesPoison = true;
					sourceProjectile.PoisonApplyChance = 1f;
					sourceProjectile.healthEffect = DebuffLibrary.Corrosion;
				}
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
			}
		}

		private void PostProcessBeamTick(BeamController beam, SpeculativeRigidbody hitRigidBody, float tickrate)
		{
			float procChance = 0.25f; //Chance per second or some shit idk
			GameActor gameActor = hitRigidBody.gameActor;
			if (!gameActor)
			{
				return;
			}
			if (UnityEngine.Random.value < BraveMathCollege.SliceProbability(procChance, tickrate))
			{
				hitRigidBody.gameActor.ApplyEffect(DebuffLibrary.Corrosion);
			}
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeamTick += this.PostProcessBeamTick;
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeamTick -= this.PostProcessBeamTick;

			return result;
		}

		protected override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				base.Owner.PostProcessBeamTick -= this.PostProcessBeamTick;
			}
			base.OnDestroy();
		}
	}
}



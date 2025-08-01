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

	public class ConnoisseursRobes : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Connoisseurs Robes";
            //string resourceName = "Planetside/Resources/connosouiersRobes.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ConnoisseursRobes>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("connosouiersRobes"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "From A Grotto";
			string longDesc = "Despite looking like a robe, this is actually a pocket worn by a Glocktopus who *really* liked guns. However, its life was ended short after a small encounter with a gun-witch wielding a giga disc gun.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.S;

			ConnoisseursRobes.ConnoisseursRobesID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);
			item.SetupUnlockOnCustomStat(CustomTrackedStats.JAMMED_ARCHGUNJURERS_KILLED, 14, DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN);


		}
		public static int ConnoisseursRobesID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			float procChance = 0.2f;
			procChance *= effectChanceScalar;
			if (sourceProjectile.PossibleSourceGun != null)
			{
				if (sourceProjectile.PossibleSourceGun.Volley.projectiles != null)
				{
                    procChance /= Mathf.Sqrt(sourceProjectile.PossibleSourceGun.Volley.projectiles.Count);
                }
            }
			

			if (UnityEngine.Random.value <= procChance && sourceProjectile.gameObject.GetComponent<RecursionPreventer>() == null)
			{
				try
				{
					Gun gun = sourceProjectile.PossibleSourceGun ?? Owner.CurrentGun ?? PickupObjectDatabase.GetById(684) as Gun;
					sourceProjectile.baseData.speed *= 0.15f;
					sourceProjectile.UpdateSpeed();
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, base.Owner.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
					gameObject.transform.parent = sourceProjectile.transform;
					HoveringGunController hoverToDest = gameObject.GetComponent<HoveringGunController>();
					Destroy(hoverToDest);
					CustomHoveringGunController hover = gameObject.AddComponent<CustomHoveringGunController>();
					hover.attachObject = sourceProjectile.gameObject;
					hover.ConsumesTargetGunAmmo = false;
					hover.ChanceToConsumeTargetGunAmmo = 0f;
					hover.Position = CustomHoveringGunController.HoverPosition.ROTATE_AROUND_POSITION;
					hover.Aim = CustomHoveringGunController.AimType.NEAREST_ENEMY;
					hover.Trigger = CustomHoveringGunController.FireType.ON_COOLDOWN;


					hover.MagnitudePower = 1000f;
					hover.CooldownTime = 0.66f;
					hover.ShootDuration = 0.33f;

					hover.Radius = 0.5f;
					
					hover.OnlyOnEmptyReload = false;
					hover.Initialize(gun, base.Owner);
					hover.DamageMultiplier = 1f;
				}
				catch (Exception ex)
				{
					ETGModConsole.Log(ex.Message, false);
				}
			}
		}

		private void PostProcessBeamTick(BeamController beam, SpeculativeRigidbody hitRigidBody, float tickrate)
		{
			float procChance = 0.4f; //Chance per second or some shit idk
			GameActor gameActor = hitRigidBody.gameActor;
			if (!gameActor)
			{
				return;
			}
			SpinningDeathController cont = beam.projectile.gameObject.GetComponent<SpinningDeathController>();
			if (UnityEngine.Random.value < BraveMathCollege.SliceProbability(procChance, tickrate) && cont == null)
			{

				Gun gun = beam.projectile.PossibleSourceGun ?? Owner.CurrentGun ?? PickupObjectDatabase.GetById(684) as Gun;
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, base.Owner.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
				HoveringGunController hoverToDest = gameObject.GetComponent<HoveringGunController>();
				Destroy(hoverToDest);
				CustomHoveringGunController hover = gameObject.AddComponent<CustomHoveringGunController>();
				hover.ConsumesTargetGunAmmo = false;
				hover.ChanceToConsumeTargetGunAmmo = 0f;
				hover.Position = CustomHoveringGunController.HoverPosition.CIRCULATE;
				hover.Aim = CustomHoveringGunController.AimType.NEAREST_ENEMY;
				hover.Trigger = CustomHoveringGunController.FireType.ON_COOLDOWN;


				hover.MagnitudePower = 1000f;
				hover.CooldownTime = 0.66f;
				hover.ShootDuration = 0.33f;

				hover.Radius = 1f;

				hover.OnlyOnEmptyReload = false;
				hover.Initialize(gun, base.Owner);
				hover.DamageMultiplier = 1f;

				Destroy(gameObject, 3.5f);

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

		

		public override void OnDestroy()
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



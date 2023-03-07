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

//Garbage Code Incoming
namespace Planetside
{
    public class WrapaRounds : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Wrapa-Rounds";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<WrapaRounds>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("wraparounds"), data, obj);
            string shortDesc = "Walls Are A Construct";
            string longDesc = "These rounds have bent cosmic forces from better games to their will and will wrap around the room when they reach a wall.\n\nFavoured by heavily armored ghosts against little ships.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.B;
            item.AddPassiveStatModifier(PlayerStats.StatType.AdditionalShotPiercing, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.ProjectileSpeed, 0.8f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.Damage, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);

            WrapaRounds.WrapaRoundsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:wrapa-rounds",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "psog:immateria"
            };
            CustomSynergies.Add("Continuum", mandatoryConsoleIDs, optionalConsoleIDs, true);
        }

    
        public static string SFX = (PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

        public static int WrapaRoundsID;

	

		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
                WraparoundProjectile wraparound = sourceProjectile.gameObject.GetOrAddComponent<WraparoundProjectile>();
				wraparound.Cap += 3;
				wraparound.OnWrappedAround = (projectile, pos1, pos2) =>
				{
					if (projectile != null)
					{
                        GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, pos1, Quaternion.identity);
                        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, pos2, Quaternion.identity);
                        Destroy(gameObject1, 2);
                        Destroy(gameObject2, 2);
						projectile.baseData.damage *= 0.95f;
                        projectile.baseData.speed *= 0.95f;
						projectile.UpdateSpeed();
                        AkSoundEngine.PostEvent("Play_WPN_" + SFX + "_impact_01", projectile.gameObject);
                    }
                };
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
				hitRigidBody.gameActor.ApplyEffect(DebuffLibrary.Frailty);
			}
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			//player.PostProcessBeamTick += this.PostProcessBeamTick;
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			//player.PostProcessBeamTick -= this.PostProcessBeamTick;

			return result;
		}

		public override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				//base.Owner.PostProcessBeamTick -= this.PostProcessBeamTick;
			}
			base.OnDestroy();
		}
	}
}



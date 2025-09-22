using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Collections;
using SaveAPI;
using Brave.BulletScript;
using Alexandria.Misc;

namespace Planetside
{
	public class LostAdventurersShield : PassiveItem
	{
		public static void Init()
		{
			string name = "Stolen Shield";
			GameObject gameObject = new GameObject(name);
            LostAdventurersShield item = gameObject.AddComponent<LostAdventurersShield>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("lost_adventurer_shield_001"), data, gameObject); 
			string shortDesc = "Post-mortem Protection";
			string longDesc = "Block 3 hits before breaking.\n\nOnce it breaks, a piece of his legacy will die as well.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.SPECIAL;
			item.ArmorToGainOnInitialPickup = 1;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 2, StatModifier.ModifyMethod.ADDITIVE);
            ID = item.PickupObjectId;
            item.RemovePickupFromLootTables();
        }
        public static int ID;
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.healthHaver.ModifyDamage += Modify;

        }

		public void Modify(HealthHaver healthHaver, HealthHaver.ModifyDamageEventArgs modifyDamageEventArgs)
		{
			if (HitsLeft > 0)
			{
				healthHaver.invulnerabilityPeriod = 0.5f;

                Items.Crisis_Stone.ImpactVFX.SpawnAtPosition(Owner.sprite.WorldCenter);
                AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_01", Owner.gameObject);
                HitsLeft--;
				modifyDamageEventArgs.InitialDamage = 0;
				modifyDamageEventArgs.ModifiedDamage = 0;
				if (HitsLeft == 0)
				{
					Owner.DropPassiveItem(this);
				}
            }
		}

		public override void OnDestroy()
		{

			base.OnDestroy();
		}
		public int HitsLeft = 3;
		public override DebrisObject Drop(PlayerController player)
		{
            player.healthHaver.ModifyDamage -= Modify;

            DebrisObject result = base.Drop(player);
            if (HitsLeft == 0)
            {
				var r = result.transform.position.GetAbsoluteRoom();
				if (r != null)
				{
					r.DeregisterInteractable(this);
					this.StartCoroutine(DoBreak());
				}
            }
            return result;
		}

		private IEnumerator DoBreak()
		{
			yield return new WaitForSeconds(1f);
            AkSoundEngine.PostEvent("Play_obj_glass_break_02", this.gameObject);
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = this.sprite.WorldCenter,
                startSize = 4,
                rotation = 0,
                startLifetime = 0.333f,
                startColor = Color.white.WithAlpha(0.333f)
            });
            Destroy(this.gameObject);
			yield break;
		}

	}
}

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

namespace Planetside
{
	public class DerpyBullets : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Derpy Bullets";
			//string resourceName = "Planetside/Resources/derpybullets.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<DerpyBullets>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("derpybullets"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Zig Zaggy";
			string longDesc = "Damage up. These unusual bullets had an excess amount of rubber added to them to create incredibly bouncy bullets, however the end result ended up with the poor things unable to stand up straight and end up just flopping from left to right.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.3f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RangeMultiplier, 3f, StatModifier.ModifyMethod.MULTIPLICATIVE);

			item.quality = PickupObject.ItemQuality.C;

			DerpyBullets.DerpyBulletsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int DerpyBulletsID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			sourceProjectile.StartCoroutine(HandleDerpyMovement(sourceProjectile));
		}
		private void PostProcessBeam(BeamController obj)
		{
            obj.projectile.OverrideMotionModule = new DerpyMovementModule();
        }
        public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeam -= this.PostProcessBeam;
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeam += this.PostProcessBeam;
		}

		public override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				base.Owner.PostProcessBeam -= this.PostProcessBeam;
				base.OnDestroy();
			}
		}

		public IEnumerator HandleDerpyMovement(Projectile projectile)
		{
			float waitAmount = UnityEngine.Random.Range(0.025f, 0.075f);
			int RotationsBeforeFlip = 1;
			float Rotate = BraveUtility.RandomBool() ? 90 : -90;
			while (projectile)
			{
				if (waitAmount > 0)
				{
                    waitAmount -= BraveTime.DeltaTime;

                }
				else
				{
                    waitAmount = UnityEngine.Random.Range(0.025f, 0.075f);
                    projectile.SendInDirection(projectile.Direction.Rotate(Rotate), false);
					RotationsBeforeFlip--;
					if (RotationsBeforeFlip == 0)
					{
						RotationsBeforeFlip = 2;
                        Rotate *= -1;
                    }
                }

				yield return null;
			}
		}

	}
}
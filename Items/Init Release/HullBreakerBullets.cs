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

namespace Planetside
{
    public class HullBreakerBullets : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Hull-Breaker Bullets";
            string resourceName = "Planetside/Resources/hullbreaker.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<HullBreakerBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Once More Into The Breach";
            string longDesc = "Deal greater damage to yet undamaged enemies." +
                "\n\nThese bullets were initially forged to blast holes into the sides of spaceships, but in violation of the Guneva convention, are now used in the Gungeon.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.A;
			List<string> mandatoryConsoleIDs = new List<string>
			{
				"psog:hull-breaker_bullets",
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"explosive_rounds",
				"rocket_powered_bullets",
				"blast_helmet",
				"stout_bullets",
				"cobalt_hammer",
				"h4mmer"
			};
			CustomSynergies.Add("Shattering Justice", mandatoryConsoleIDs, optionalConsoleIDs, true);
			SynergyAPI.SynergyBuilder.AddItemToSynergy(item, CustomSynergyType.MASSIVE_EFFECT);
			SynergyAPI.SynergyBuilder.AddItemToSynergy(item, CustomSynergyType.MISSILE_BOW);

			item.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);

			HullBreakerBullets.HullBreakerBulletsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int HullBreakerBulletsID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
				sourceProjectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(sourceProjectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
			}
		}
		private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			
			bool flag = otherRigidbody && otherRigidbody.healthHaver;
			if (flag)
			{
				float maxHealth = otherRigidbody.healthHaver.GetMaxHealth();
				float num = maxHealth * 0.99f;
				float currentHealth = otherRigidbody.healthHaver.GetCurrentHealth();
				bool flag2 = currentHealth > num;
				if (flag2)
				{
					float damage = myRigidbody.projectile.baseData.damage;
					myRigidbody.projectile.baseData.damage *= 2.5f;
					GameManager.Instance.StartCoroutine(this.ChangeProjectileDamage(myRigidbody.projectile, damage));
					bool flagA = base.Owner.PlayerHasActiveSynergy("Shattering Justice");
					if (flagA)
					{
						if (otherRigidbody != null)
                        {
							AkSoundEngine.PostEvent("Play_obj_vent_break_01", base.gameObject);
							otherRigidbody.aiActor.ApplyEffect(DebuffLibrary.brokenArmor, 1f, null);
						}
					}
				}
			}
		}
		private IEnumerator ChangeProjectileDamage(Projectile bullet, float oldDamage)
		{
			yield return new WaitForSeconds(0.1f);
			bool flag = bullet != null;
			if (flag)
			{
				bullet.baseData.damage = oldDamage;
			}
			yield break;
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
		}
		protected override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			}
			base.OnDestroy();
		}
	}
}
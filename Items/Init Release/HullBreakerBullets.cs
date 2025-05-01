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
		public class HullBreakerTracker : MonoBehaviour { }


        public static void Init()
        {
            string itemName = "Hull-Breaker Bullets";
            //string resourceName = "Planetside/Resources/hullbreaker.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<HullBreakerBullets>();
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("hullbreaker"), data, obj);

            string shortDesc = "Once More Into The Breach";
            string longDesc = "First hit on an enemy deals greatly increased damage." +
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
			
			if (otherRigidbody != null && otherRigidbody.aiActor != null)
			{
				var tracker = otherRigidbody.aiActor.GetComponent<HullBreakerTracker>();
				if (tracker == null)
				{
                    if (myRigidbody && myRigidbody.projectile != null)
                    {
                        otherRigidbody.aiActor.AddComponent<HullBreakerTracker>();
                        float damage = myRigidbody.projectile.baseData.damage;

                        myRigidbody.projectile.baseData.damage *= 2.5f;
                        GameManager.Instance.StartCoroutine(this.ChangeProjectileDamage(myRigidbody.projectile, damage));
                        myRigidbody.projectile.OnHitEnemy += OHE;
                        AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_01", myRigidbody.gameObject);
                        AkSoundEngine.PostEvent("Play_obj_vent_break_01", myRigidbody.gameObject);

                        if (base.Owner.PlayerHasActiveSynergy("Shattering Justice"))
                        {
                            AkSoundEngine.PostEvent("Play_obj_vent_break_01", myRigidbody.gameObject);
                            otherRigidbody.aiActor.ApplyEffect(DebuffLibrary.brokenArmor, 1f, null);
                        }
                    }
				}
			}
		}

		public void OHE(Projectile p, SpeculativeRigidbody b, bool boo) 
		{
			if (b.aiActor) 
			{
				b.aiActor.PlayEffectOnActor(StaticVFXStorage.SeriousCannonImpact, new Vector2(0, 0));
			}
		}

        private IEnumerator ChangeProjectileDamage(Projectile bullet, float oldDamage)
		{
			yield return null;
			if (bullet != null)
			{
				bullet.baseData.damage = oldDamage;
                bullet.projectile.OnHitEnemy -= OHE;

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
		public override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			}
			base.OnDestroy();
		}
	}
}
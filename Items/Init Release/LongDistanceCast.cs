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
using UnityEngine.EventSystems;

namespace Planetside
{
	public class LDCBullets : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Teleporting Gunfire";
			//string resourceName = "Planetside/Resources/telecast.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<LDCBullets>();
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("telecast"), data, obj);

            string shortDesc = "I Cast Gun!";
			string longDesc = "Creates copies of themselves from beyond the Curtain." +
				"\n\nOriginally carried by an alchenmist seeking the gun, they met their quick end here due to accidental, and literal, backfire of several hundred explosives at once.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.S;
			item.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);

			LDCBullets.TeleportingGunfireID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);
		}
		public static int TeleportingGunfireID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
                float e = 0.5f;
                if (Owner.GetEffect("WarpBuff") != null)
                {
                    e = 2;
                }

				if (UnityEngine.Random.value < e * effectChanceScalar && sourceProjectile.gameObject.GetComponent<RecursionPreventer>() == null)
				{
                    if (base.Owner.CurrentRoom != null)
                    {
                        List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
                        if (activeEnemies != null && activeEnemies.Count > 0)
                        {
                            AIActor aiActor = BraveUtility.RandomElement<AIActor>(activeEnemies);
                            if (aiActor)
                            {
                                var castResult = DoRaycasts(aiActor, BraveUtility.RandomAngle());
                                SpawnProjectile(sourceProjectile, aiActor, castResult.Contact);
                                RaycastResult.Pool.Free(ref castResult);
                            }
                        }
                    }
                }
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
			}
		}

		public void SpawnProjectile(Projectile proj, AIActor enemy, Vector2 startPosition)
		{
			if (proj == null) { return; }
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), startPosition, Quaternion.identity);
            Destroy(gameObject, 1.5f);
            AkSoundEngine.PostEvent("Play_ENM_bullat_tackle_01", gameObject);



            GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(proj.PossibleSourceGun.DefaultModule.GetCurrentProjectile().gameObject, startPosition, Quaternion.Euler(0f, 0f, (enemy.sprite.WorldCenter - startPosition).ToAngle()), true);
            Projectile projectile = spawnedBulletOBJ.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.gameObject.AddComponent<RecursionPreventer>();
                projectile.Owner = base.Owner;
                projectile.Shooter = base.Owner.specRigidbody;
				projectile.baseData.UsesCustomAccelerationCurve = true;
				projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0.25f, 0, 1f, 1.25f);
				projectile.IgnoreTileCollisionsFor(3);
				projectile.baseData.range *= 2.5f;
				projectile.pierceMinorBreakables = true;
                if (Owner.GetEffect("WarpBuff") != null)
                {
                    projectile.baseData.speed *= 2f;
                    projectile.baseData.damage *= 1.1f;

                }
            }
        }


		public RaycastResult DoRaycasts(AIActor enemy, float angle)
		{
            int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.BulletBreakable);
            var cast = RaycastToolbox.ReturnRaycast(enemy.sprite.WorldCenter, MathToolbox.GetUnitOnCircle(angle, 0.5f), rayMask, 1000, enemy.specRigidbody);
            return cast;
        }

		private void PostProcessBeamTick(BeamController beam, SpeculativeRigidbody hitRigidbody, float tickRate)
		{
            try
            {
                if (UnityEngine.Random.value < 0.0166f  && beam.gameObject.GetComponent<RecursionPreventer>() == null && beam.projectile != null)
                {
                    Projectile currentProjectile = beam.projectile.PossibleSourceGun.DefaultModule.GetCurrentProjectile();
                    if (currentProjectile)
                    {
                        if (base.Owner.CurrentRoom != null)
                        {
                            List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
                            if (activeEnemies != null && activeEnemies.Count > 0)
                            {
                                AIActor aiActor = BraveUtility.RandomElement<AIActor>(activeEnemies);
                                if (aiActor)
                                {
                                    var castResult = DoRaycasts(aiActor, BraveUtility.RandomAngle());
                                    SpawnBeam(currentProjectile, aiActor, castResult.Contact);
                                    RaycastResult.Pool.Free(ref castResult);
                                }
                            }
                        }


                           
                    }                   
                }
            }
            catch (Exception ex)
            {
                ETGModConsole.Log(ex.Message, false);
            }
        }

        public void SpawnBeam(Projectile proj, AIActor enemy, Vector2 startPosition)
        {
            if (proj == null) { return; }
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), startPosition, Quaternion.identity);
            Destroy(gameObject, 1.5f);
            AkSoundEngine.PostEvent("Play_ENM_bullat_tackle_01", gameObject);
            bool b = BraveUtility.RandomBool();
            float f = (enemy.sprite.WorldCenter - startPosition).ToAngle() + (b == true ? 90 : -90);
            startPosition = startPosition.GetAbsoluteRoom() != null ? startPosition.GetAbsoluteRoom().GetNearestAvailableCell(startPosition).Value.ToCenterVector2() : startPosition;
            BeamController beamController3 = BeamToolbox.FreeFireBeamFromAnywhere(proj, base.Owner, null, startPosition, true, f, 4f + proj.GetComponent<BeamController>().chargeDelay, true, true, (b == true ? -22.5f : 22.5f), true);
            beamController3.gameObject.AddComponent<RecursionPreventer>();
        }

        public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
            player.PostProcessBeamTick -= this.PostProcessBeamTick;

            return result;
		}
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
            player.PostProcessBeamTick += this.PostProcessBeamTick;

        }

        public override void OnDestroy()
		{
			if (base.Owner != null)
            {
                base.Owner.PostProcessBeamTick -= this.PostProcessBeamTick;
                base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			}
			base.OnDestroy();
		}
	}
}
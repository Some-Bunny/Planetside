using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Collections;
using SaveAPI;
using Brave.BulletScript;
using Alexandria.cAPI;
using Alexandria.PrefabAPI;
using Planetside.Toolboxes;

namespace Planetside
{
	public class PileOfStardust : PassiveItem
	{
		public static void Init()
		{
			string name = "Pinch Of Stardust";
			GameObject gameObject = new GameObject(name);
            PileOfStardust item = gameObject.AddComponent<PileOfStardust>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("pileofstardust"), data, gameObject); 
			
			string shortDesc = "Shimmer And Sparkle";
			string longDesc = "Create multiple stars upon reloading. Stars attach to your projectiles and grant buffs.\n\nEach type of stardust has a different taste, and every type tastes absolutely horrible.";
			
			
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.B;
            PileOfStardust.ItemID = item.PickupObjectId;

            item.AddSynergy("Constellation", new List<PickupObject>() 
            {
                Guns.Crescent_Crossbow,
                Guns.Mr_Accretion_Jr,
            });

        }

        public static int ItemID;
		public override void Update()
		{
			base.Update();
		}


		public override void Pickup(PlayerController player)
		{
            player.OnReloadedGun += OnGunReloaded;
            base.Pickup(player);
        }
        public override void OnDestroy()
		{
			if (Owner) Owner.OnReloadedGun -= OnGunReloaded;
            base.OnDestroy();
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
            player.OnReloadedGun -= OnGunReloaded;
            return result;
		}

		public void OnGunReloaded(PlayerController playerController, Gun gun)
		{
            this.StartCoroutine(DoSparkles(gun));
		}
        public IEnumerator DoSparkles(Gun gun)
        {
            var player = base.Owner;

            bool Item = Owner.CurrentGun.PickupObjectId == Guns.Crescent_Crossbow.PickupObjectId || Owner.CurrentGun.PickupObjectId == Guns.Mr_Accretion_Jr.PickupObjectId;
            int amount = Item ? Mathf.Max(8, 3 + Mathf.CeilToInt(gun.reloadTime * 3)) : Mathf.Max(3, 1 + Mathf.CeilToInt(gun.reloadTime));

            for (int i = 0; i < amount; i++)
            {
                float angle = i == 0 ? player.CurrentGun.CurrentAngle : ProjSpawnHelper.GetAccuracyAngled(player.CurrentGun.CurrentAngle, 30, player);
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(WitherLance.AllStars[UnityEngine.Random.Range(0, WitherLance.AllStars.Count)].gameObject, player.gunAttachPoint.position, Quaternion.Euler(0f, 0f, angle), true);
                WitherLanceProjectile component = spawnedBulletOBJ.GetComponent<WitherLanceProjectile>();
                if (component != null)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    player.DoPostProcessProjectile(component);
                    AkSoundEngine.PostEvent("Play_WPN_star_impact_01", gun.gameObject);
                    for (int e = 0; e < 3; e++)
                    {
                        Vector2 push = MathToolbox.GetUnitOnCircle(gun.CurrentAngle + UnityEngine.Random.Range(-30, 30), UnityEngine.Random.value * 4.5f);
                        GlobalSparksDoer.DoSingleParticle(gun.transform.position, push, null, 1.5f + UnityEngine.Random.value, component.AllTrails[0].endColor * 2, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    }
                }

                yield return new WaitForSeconds(0.03333f);
            }
            yield break;
        }
    }
}

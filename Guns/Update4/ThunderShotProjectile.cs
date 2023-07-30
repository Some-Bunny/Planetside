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
	internal class ThunderShotProjectile : MonoBehaviour
	{
		public ThunderShotProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            this.Player = projectile.Owner as PlayerController;
            if (this.projectile != null)
            {
                if (Player != null && Player.PlayerHasActiveSynergy("KA-BLEWY!") == true) { this.projectile.baseData.damage *= 0.66f; }
                this.projectile.OnHitEnemy += OnHitEnemy;
            }
        }

        public void OnHitEnemy(Projectile self, SpeculativeRigidbody rigidBody, bool fatal)
        {
			if (rigidBody.aiActor && rigidBody.healthHaver)
			{
                (PickupObjectDatabase.GetById(58) as Gun).DefaultModule.projectiles[0].hitEffects.HandleEnemyImpact(rigidBody.aiActor.sprite.WorldCenter, 0, rigidBody.aiActor.transform, self.transform.position, self.LastVelocity, false);
                AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Shield_01", rigidBody.gameObject);

                float Dmg = 5;
                if (Player != null && Player.PlayerHasActiveSynergy("KA-BLEWY!") == true) { Dmg *= 4; }
                ExplodeOnDeath boomBoom = rigidBody.aiActor.gameObject.GetOrAddComponent<ExplodeOnDeath>();
                AIActor Grenade = EnemyDatabase.GetOrLoadByGuid("4d37ce3d666b4ddda8039929225b7ede");
                ExplosionData explosionData = Grenade.GetComponent<ExplodeOnDeath>().explosionData;
                explosionData.damage += Dmg; 
                explosionData.damageToPlayer = 0;
                boomBoom.explosionData = explosionData;
                boomBoom.immuneToIBombApp = true;
                boomBoom.deathType = OnDeathBehavior.DeathType.Death;
            }
		}
        private PlayerController Player;
        private Projectile projectile;
	}
}


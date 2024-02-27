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
	internal class WhistlerProjectile : MonoBehaviour
	{
		public WhistlerProjectile()
		{
		}



        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            this.Player = projectile.Owner as PlayerController;
        }
        public void Update()
        {
            if (this.projectile != null && this.Player != null)
            {
                List<AIActor> activeEnemies = Player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies == null || activeEnemies.Count <= 0)
                {
                    this.projectile.DieInAir(false);
                }
            }
            else
            {
                this.projectile.DieInAir(false);
            }
        }
        private PlayerController Player;
        private Projectile projectile;
	}
}


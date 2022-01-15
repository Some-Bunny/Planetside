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
            //GameObject original;
            this.projectile = base.GetComponent<Projectile>();
            this.Player = projectile.Owner as PlayerController;
            bool flag = this.projectile != null;
            bool flag2 = flag;
            if (flag2)
            {
                //AkSoundEngine.PostEvent("Play_Whistle", projectile.gameObject);
                Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat.mainTexture = this.projectile.sprite.renderer.material.mainTexture;
                mat.SetColor("_EmissiveColor", new Color32(153, 0, 255, 255));
                mat.SetFloat("_EmissiveColorPower", 1.55f);
                mat.SetFloat("_EmissivePower", 100);
                this.projectile.sprite.renderer.material = mat;
            }
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

        public void OnDestroy()
        {
            if (this.projectile != null)
            {
                //AkSoundEngine.PostEvent("Stop_Whistle", projectile.gameObject);
            }
        }
        private PlayerController Player;
        private Projectile projectile;
	}
}


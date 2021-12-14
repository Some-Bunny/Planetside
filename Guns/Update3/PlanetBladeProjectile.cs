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
	internal class PlanetBladeProjectile : MonoBehaviour
	{
		public PlanetBladeProjectile()
		{
		}



        public void Start()
        {
            //GameObject original;
            this.projectile = base.GetComponent<Projectile>();
            Projectile projectile = this.projectile;
            PlayerController playerController = projectile.Owner as PlayerController;
            Projectile component = base.gameObject.GetComponent<Projectile>();
            bool flag = component != null && playerController != null;
            bool flag2 = flag;
            if (flag2)
            {
                float Damage = playerController.stats.GetStatValue(PlayerStats.StatType.Damage);
                float Range = playerController.stats.GetStatValue(PlayerStats.StatType.RangeMultiplier);
                if (playerController == null)
                {
                    Damage = 1;
                    Range = 1;
                }
                

                Vector2 vector = playerController.unadjustedAimPoint.XY() - playerController.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 270;
                slasher.SlashRange = 0.5f * Range;
                slasher.playerKnockback = -10;
                slasher.SlashDamage *= Damage;
                slasher.soundToPlay = "Play_OBJ_katana_slash_01";
                slasher.SlashVFX = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
                playerController.knockbackDoer.ApplyKnockback(vector, 30f, true);
                slasher.InteractMode = SlashDoer.ProjInteractMode.REFLECT;
            }
        }
        public void Update()
        {
           
        }
        private Projectile projectile;
	}
}


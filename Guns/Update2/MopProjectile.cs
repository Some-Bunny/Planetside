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
	internal class MopProjectile : MonoBehaviour
	{
		public MopProjectile()
		{
		}

        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            Projectile projectile = this.projectile;

            PlayerController playerController = projectile.Owner as PlayerController;
            Projectile component = base.gameObject.GetComponent<Projectile>();
            bool flag = component != null;
            bool flag2 = flag;
            if (flag2)
            {
                component.HasDefaultTint = true;
                if (Mop.IsBlob)
               {
                    //ETGModConsole.Log("================================================");
                    //ETGModConsole.Log("Blob");
                    //component.DefaultTintColor = new Color32(213, 77, 77, 255);
                    component.AdjustPlayerProjectileTint(new Color32(213, 77, 77, 255), 0, 0f);
                    BulletSlowModifier wbesloew = component.gameObject.AddComponent<BulletSlowModifier>();
                    wbesloew.chanceToslow = 0.2f;
                    wbesloew.SpeedMultiplier = 0.2f;
                    wbesloew.slowLength = 5;
                    wbesloew.Color = new Color32(213, 77, 77, 255);
                    wbesloew.doTint = true;
                }
                if (Mop.IsCharm)
                {
                    //ETGModConsole.Log("================================================");
                    //ETGModConsole.Log("Charm");
                    component.AdjustPlayerProjectileTint(new Color32(252, 72, 241, 255), 0, 0f);
                    Gun charmbow = PickupObjectDatabase.GetById(200) as Gun;
                    component.hitEffects = charmbow.DefaultModule.chargeProjectiles[1].Projectile.hitEffects;
                    projectile.statusEffectsToApply.Add(DebuffStatics.charmingRoundsEffect);

                    //component.statusEffectsToApply.Add(status.CharmModifierEffect);

                }
                if (Mop.IsCheese)
                {
                    //ETGModConsole.Log("================================================");
                    //ETGModConsole.Log("Cheese");
                    component.AdjustPlayerProjectileTint(new Color32(254, 203, 0, 255), 0, 0f);
                    projectile.statusEffectsToApply.Add(DebuffStatics.cheeseeffect);
                }
                if (Mop.Isfire)
                {
                    //ETGModConsole.Log("================================================");
                    //ETGModConsole.Log("Fire");
                    component.AdjustPlayerProjectileTint(new Color32(255, 102, 0, 255), 0, 0f);
                    Gun fire = PickupObjectDatabase.GetById(384) as Gun;
                    component.hitEffects = fire.DefaultModule.projectiles[0].hitEffects;
                    component.AppliesFire = true;
                    component.FireApplyChance = 0.35f;
                    component.fireEffect = fire.DefaultModule.projectiles[0].fireEffect;


                }
                if (Mop.IsgreenFire)
                {
                    //ETGModConsole.Log("================================================");
                    //ETGModConsole.Log("Green fire");
                    component.AdjustPlayerProjectileTint(new Color32(211, 229, 73, 255), 0, 0f);
                    component.fireEffect.IsGreenFire = true;
                    projectile.statusEffectsToApply.Add(DebuffStatics.greenFireEffect);
                }
                if (Mop.IsOil)
                {
                    //ETGModConsole.Log("================================================");
                    //ETGModConsole.Log("Oil");
                    component.AdjustPlayerProjectileTint(new Color32(10, 6, 18, 255), 0, 0f);
                    component.gameObject.AddComponent<ApplyStep2>();
                }
                if (Mop.IsPoison)
                {
                   //ETGModConsole.Log("================================================");
                   //ETGModConsole.Log("Poison");
                    component.AdjustPlayerProjectileTint(new Color32(145, 227, 120, 255), 0, 0f);
                    projectile.statusEffectsToApply.Add(DebuffStatics.irradiatedLeadEffect);

                }
                if (Mop.IsWater)
                {
                    //ETGModConsole.Log("================================================");
                    //ETGModConsole.Log("Water");
                    component.baseData.damage *= 0.75f;
                }
                if (Mop.IsWeb)
                {
                    //ETGModConsole.Log("================================================");
                    //ETGModConsole.Log("Web");
                    component.AdjustPlayerProjectileTint(new Color32(184, 181, 147, 255), 0, 0f);
                    BulletSlowModifier wbesloew = component.gameObject.AddComponent<BulletSlowModifier>();
                    wbesloew.chanceToslow = 1;
                    wbesloew.SpeedMultiplier = 0.166f;
                    wbesloew.slowLength = 10;
                    //component.DefaultTintColor = new Color32(184, 181, 147, 255);
                }
                if (Mop.IsBlood)
                {
                    component.AdjustPlayerProjectileTint(new Color32(136, 8, 8, 255), 0, 0f);
                    component.gameObject.AddComponent<ApplyEnrage>();
                }
                if (Mop.IsPoop)
                {
                    component.AdjustPlayerProjectileTint(new Color32(123, 92, 0, 255), 0, 0f);
                    component.gameObject.AddComponent<ApplyFear>();
                }
                if (Mop.IsPossessive)
                {
                    component.AdjustPlayerProjectileTint(new Color32(255, 188, 76, 255), 0, 0f);
                    component.AppliesPoison = true;
                    component.PoisonApplyChance = 1f;
                    component.healthEffect = DebuffLibrary.Possessed;
                }
                if (Mop.IsFrail)
                {
                    component.AdjustPlayerProjectileTint(new Color32(136, 25, 149, 255), 0, 0f);
                    component.AppliesPoison = true;
                    component.PoisonApplyChance = 1f;
                    component.healthEffect = DebuffLibrary.Frailty;
                }
            }
        }
        private Projectile projectile;

    }
}


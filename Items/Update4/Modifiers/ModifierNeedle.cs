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
    public class ModifierCheck : MonoBehaviour
    {
        public ModifierCheck()
        {
            this.IsNeedle = false;
        }
        public bool IsNeedle;
    }


    public class ModifierNeedle : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Modifier 'NEEDLE'";
            string resourceName = "Planetside/Resources/Modifiers/modifierNeedle.png";
            GameObject obj = new GameObject(itemName);
            ModifierNeedle activeitem = obj.AddComponent<ModifierNeedle>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "";
            string longDesc = "Grants your current gun permanent additional piercing and shotspeed, but for reduced damage and accuracy.\n\nNot to be used for medical purposes.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.None, 0f);
            activeitem.consumable = true;
            activeitem.quality = PickupObject.ItemQuality.EXCLUDED;


            OffWorldMedicine.OffWorldMedicineID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

        }
        public static int OffWorldMedicineID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            if (!user.CurrentGun.InfiniteAmmo)
            {
                return true;
            }
            return base.CanBeUsed(user);
        }

        protected override void DoEffect(PlayerController user)
        {
            Gun heldGun = user.CurrentGun;
            foreach (ProjectileModule module in heldGun.Volley.projectiles)
            {
                List<ProjectileModule.ChargeProjectile> chargeProjlist =  module.chargeProjectiles.ToList();

                foreach (ProjectileModule.ChargeProjectile chargeProj in chargeProjlist)
                {
                    Projectile projectile = chargeProj.Projectile;
                    if (projectile.gameObject.GetComponent<ModifierCheck>() == null | projectile.gameObject.GetComponent<ModifierCheck>() != null && projectile.gameObject.GetComponent<ModifierCheck>().IsNeedle == false)
                    {
                        ModifierCheck mod = projectile.gameObject.AddComponent<ModifierCheck>();
                        mod.IsNeedle = transform;
                        projectile.baseData.speed *= 2.5f;
                        projectile.baseData.damage *= 0.75f;
                        PierceProjModifier pierce = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                        pierce.penetration = 5;
                        pierce.penetratesBreakables = true;
                    }
                }
                module.angleVariance = module.angleVariance * 1.2f;



                for (int i = 0; i < module.projectiles.Count; i++)
                {
                    Projectile projectile = module.projectiles[i];

                    if (projectile.gameObject.GetComponent<ModifierCheck>() == null | projectile.gameObject.GetComponent<ModifierCheck>() != null && projectile.gameObject.GetComponent<ModifierCheck>().IsNeedle == false)
                    {
                        ModifierCheck mod = projectile.gameObject.AddComponent<ModifierCheck>();
                        mod.IsNeedle = true;
                        projectile.baseData.speed *= 2.5f;
                        projectile.baseData.damage *= 0.75f;
                        PierceProjModifier pierce = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                        pierce.penetration = 5;
                        pierce.penetratesBreakables = true;
                    }
                }
            }
        }
        public static float ChangeSpiceWeight()
        {
            return 0f;
        }
    }
}




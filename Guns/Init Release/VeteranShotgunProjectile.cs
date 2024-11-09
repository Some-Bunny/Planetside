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
	internal class VeteranShotgunProjectile : MonoBehaviour
	{
		public VeteranShotgunProjectile()
		{
		}
        public float Chance_1 = 0.125f;
        public float Chance_2 = 0.0125f;
        public float Chance_3 = 0.00125f;

        public void Start()
        {
            if (UnityEngine.Random.value <= Chance_2)
            {
                List<int> ints = new List<int>();
                for (int i = 0; i < 3; i++)
                {
                    ints = RollBuffs(ints);
                }
            }
            else if (UnityEngine.Random.value <= Chance_1)
			{
                RollBuffs();
            }
            else if (UnityEngine.Random.value <= Chance_3)
            {
                ApplyFire();
                ApplyPoison();
                ApplyFrost();
                ApplySlow();
                ApplyCheese();
                ApplyTarnish();
                ApplyFrail();
                ApplyArmorBreak();
                ApplyStun();
                ApplyGreenFire();
                ApplyCharm();
                ApplySolarFire();
            }
        }

        private List<int> RollBuffs(List<int> bannedValues = null)
        {
            if (bannedValues == null) { bannedValues = new List<int>(); }
            int Debuffs = UnityEngine.Random.Range(0, 20);
            int failsafe = 100;
            while (bannedValues.Contains(Debuffs))
            {
                failsafe--;
                if (failsafe < 0) { break;}
                Debuffs = UnityEngine.Random.Range(0, 20);
            }

            switch (Debuffs)
            {
                //Fire
                case 0:
                    ApplyFire();
                    bannedValues.Add(0);
                    bannedValues.Add(1);
                    bannedValues.Add(2);
                    break;
                case 1:
                    ApplyFire();
                    bannedValues.Add(0);
                    bannedValues.Add(1);
                    bannedValues.Add(2);
                    break;
                case 2:
                    ApplyFire();
                    bannedValues.Add(0);
                    bannedValues.Add(1);
                    bannedValues.Add(2);
                    break;

                //Poison
                case 3:
                    ApplyPoison();
                    bannedValues.Add(3);
                    bannedValues.Add(4);
                    bannedValues.Add(5);
                    break;
                case 4:
                    ApplyPoison();
                    bannedValues.Add(3);
                    bannedValues.Add(4);
                    bannedValues.Add(5);
                    break;
                case 5:
                    ApplyPoison();
                    bannedValues.Add(3);
                    bannedValues.Add(4);
                    bannedValues.Add(5);
                    break;
                case 6:
                    ApplyFrost();
                    bannedValues.Add(6);
                    bannedValues.Add(19);
                    break;
                //Stun
                case 7:
                    ApplyStun();
                    bannedValues.Add(7);
                    break;
                //Cheese
                case 8:
                    ApplyCheese();
                    bannedValues.Add(8);
                    break;
                //Slow
                case 9:
                    ApplySlow();
                    bannedValues.Add(9);
                    bannedValues.Add(10);
                    bannedValues.Add(11);
                    bannedValues.Add(12);
                    break;
                case 10:
                    ApplySlow();
                    bannedValues.Add(9);
                    bannedValues.Add(10);
                    bannedValues.Add(11);
                    bannedValues.Add(12);
                    break;
                case 11:
                    ApplySlow();
                    bannedValues.Add(9);
                    bannedValues.Add(10);
                    bannedValues.Add(11);
                    bannedValues.Add(12);
                    break;
                case 12:
                    ApplySlow();
                    bannedValues.Add(9);
                    bannedValues.Add(10);
                    bannedValues.Add(11);
                    bannedValues.Add(12);
                    break;
                case 13:
                    ApplyFrail();
                    bannedValues.Add(13);

                    break;
                case 14:
                    ApplyTarnish();
                    bannedValues.Add(14);

                    break;
                case 15:
                    ApplyArmorBreak();
                    bannedValues.Add(15);
                    break;
                case 16:
                    ApplyGreenFire();
                    bannedValues.Add(16);
                    break;
                case 17:
                    ApplyCharm();
                    bannedValues.Add(17);
                    break;
                case 18:
                    ApplySolarFire();
                    bannedValues.Add(18);
                    break;
                case 19:
                    ApplyFrost();
                    bannedValues.Add(19);
                    bannedValues.Add(6);

                    break;

            }
            return bannedValues;
        }
        private void ApplyCharm()
        {
            projectile.AdjustPlayerProjectileTint(new Color(1, 0.6f, 0), 1);
            projectile.AppliesCharm = true;
            projectile.CharmApplyChance = 1;
            projectile.charmEffect = DebuffStatics.charmingRoundsEffect;
        }

        private void ApplyFire()
        {
            projectile.AdjustPlayerProjectileTint(new Color(1, 0.6f, 0), 1);
            projectile.AppliesFire = true;
            projectile.FireApplyChance = 1;
            projectile.fireEffect = DebuffStatics.hotLeadEffect;
        }
        private void ApplySolarFire()
        {
            projectile.AdjustPlayerProjectileTint(new Color(1, 0.7f, 0), 1);
            projectile.AppliesFire = true;
            projectile.FireApplyChance = 1;
            projectile.fireEffect = DebuffStatics.SolarFire;
        }
        private void ApplyPoison()
        {
            projectile.AdjustPlayerProjectileTint(new Color(0, 1f, 0.2f), 1);
            projectile.AppliesPoison = true;
            projectile.PoisonApplyChance = 1;
            projectile.healthEffect = DebuffStatics.irradiatedLeadEffect;
        }
        private void ApplyFrost()
        {
            projectile.AdjustPlayerProjectileTint(new Color(0, 0.1f, 1f), 1);
            projectile.AppliesFreeze = true;
            projectile.FreezeApplyChance = 1;
            projectile.freezeEffect = DebuffStatics.frostBulletsEffect;
        }
        private void ApplyStun()
        {
            projectile.AdjustPlayerProjectileTint(new Color(1, 1, 1f), 1);
            projectile.AppliesStun = true;
            projectile.StunApplyChance = 1;
            projectile.AppliedStunDuration = 1.5f;
        }
        private void ApplyCheese()
        {
            projectile.AdjustPlayerProjectileTint(new Color(1, 0.8f, 0f), 1);
            projectile.AppliesCheese = true;
            projectile.CheeseApplyChance = 1;
            projectile.cheeseEffect = DebuffStatics.cheeseeffect;
        }
        private void ApplySlow()
        {
            projectile.AdjustPlayerProjectileTint(new Color(1, 0, 0.3f), 1);
            projectile.AppliesSpeedModifier = true;
            projectile.SpeedApplyChance = 1;
            projectile.speedEffect = DebuffStatics.tripleCrossbowSlowEffect;
        }

        private void ApplyFrail()
        {
            projectile.AdjustPlayerProjectileTint(new Color(1, 0, 0.9f), 1);
            projectile.statusEffectsToApply = projectile.statusEffectsToApply == null ? new List<GameActorEffect>() : projectile.statusEffectsToApply;
            projectile.statusEffectsToApply.Add(DebuffLibrary.Frailty);
        }
        private void ApplyTarnish()
        {
            projectile.AdjustPlayerProjectileTint(new Color(1, 0.8f, 0.9f), 1);
            projectile.statusEffectsToApply = projectile.statusEffectsToApply == null ? new List<GameActorEffect>() : projectile.statusEffectsToApply;
            projectile.statusEffectsToApply.Add(DebuffLibrary.Corrosion);
        }

        private void ApplyArmorBreak()
        {
            projectile.AdjustPlayerProjectileTint(new Color(0.3f, 0.3f, 0.3f), 1);
            projectile.statusEffectsToApply = projectile.statusEffectsToApply == null ? new List<GameActorEffect>() : projectile.statusEffectsToApply;
            projectile.statusEffectsToApply.Add(DebuffLibrary.brokenArmor);
        }
        private void ApplyGreenFire()
        {
            projectile.AdjustPlayerProjectileTint(new Color(1, 0.6f, 0), 1);
            projectile.AppliesFire = true;
            projectile.FireApplyChance = 1;
            projectile.fireEffect = DebuffStatics.greenFireEffect;
        }
        public Projectile projectile;
	}
}


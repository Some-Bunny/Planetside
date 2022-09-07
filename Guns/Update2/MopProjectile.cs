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
using static Planetside.PrisonerSecondSubPhaseController;
using System.ComponentModel;

namespace Planetside
{

   

    internal class MopProjectile : MonoBehaviour
	{

        private Projectile projectile;


        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            Mop mopComp = this.projectile.PossibleSourceGun.GetComponent<Mop>();
            if (this.projectile != null && mopComp != null)
            {
                var effectContainer = mopComp.currentEffectContainer;
                if (effectContainer != null)
                {
                    //Color
                    if (effectContainer.projectileColor != null)
                    {
                        this.projectile.AdjustPlayerProjectileTint(effectContainer.projectileColor, 0, 0f);
                    }

                    //Debuffs
                    if (effectContainer.debuffs != null || effectContainer.debuffs.Count > 0)
                    {
                        for (int i = 0; i < effectContainer.debuffs.Count; i++)
                        {
                            if (effectContainer.debuffs[i] != null)
                            {
                                projectile.statusEffectsToApply.Add(effectContainer.debuffs[i]);
                            }
                        }
                    }
                    //Components
                    if (effectContainer.component != null)
                    {
                        projectile.gameObject.AddComponent(effectContainer.component);
                    }
                    projectile.baseData.damage *= effectContainer.DamageMultiplier;
                }

            }
        }
    }
}


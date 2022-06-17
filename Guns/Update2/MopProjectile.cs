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
            BulletSlowModifier OwnSlowModifier = new BulletSlowModifier();
            OwnSlowModifier.chanceToslow = 0.2f;
            OwnSlowModifier.SpeedMultiplier = 0.2f;
            OwnSlowModifier.slowLength = 5;
            OwnSlowModifier.Color = new Color32(213, 77, 77, 255);
            OwnSlowModifier.doTint = true;
            componentsToAddToProejctile.Add("blob", OwnSlowModifier);

            componentsToAddToProejctile.Add("oil", new ApplyStep2());

            BulletSlowModifier webslow = new BulletSlowModifier();
            webslow.chanceToslow = 1;
            webslow.SpeedMultiplier = 0.166f;
            webslow.slowLength = 10;
            componentsToAddToProejctile.Add("web", webslow);


            componentsToAddToProejctile.Add("blood", new ApplyEnrage());
            componentsToAddToProejctile.Add("poop", new ApplyFear());

        }

        private Projectile projectile;

        public Dictionary<string, Color32> debuffColorationKeys = new Dictionary<string, Color32>()
        {
            {"fire", new Color32(255, 102, 0, 255)},//DONE
            {"hellfire", new Color32(211, 229, 73, 255)},//DONE
            {"blob", new Color32(213, 77, 77, 255)},//DONE
            {"oil", new Color32(10, 6, 18, 255)},//DONE
            {"cheese", new Color32(255, 102, 0, 255)},
            {"charm", new Color32(252, 72, 241, 255)},//DONE
            {"poison", new Color32(145, 227, 120, 255)},//DONE
            {"web", new Color32(184, 181, 147, 255)},//DONE
            {"blood", new Color32(136, 8, 8, 255)},//DONE
            {"poop", new Color32(123, 92, 0, 255)},//DONE
            {"possessed", new Color32(255, 188, 76, 255)},//DONE
            {"frailty", new Color32(136, 25, 149, 255)},//DONE
            {"tarnish", new Color32(157, 147, 0, 255)},//DONE
        };

        public Dictionary<string, Component> componentsToAddToProejctile = new Dictionary<string, Component>()
        {

        };

        public Dictionary<string, GameActorEffect> effectsToInflict = new Dictionary<string, GameActorEffect>()
        {
            {"fire", DebuffStatics.hotLeadEffect},
            {"hellfire", DebuffStatics.greenFireEffect},
            {"cheese", DebuffStatics.cheeseeffect},
            {"charm", DebuffStatics.charmingRoundsEffect},
            {"poison", DebuffStatics.irradiatedLeadEffect},
            {"possessed", DebuffLibrary.Possessed},
            {"frailty", DebuffLibrary.Frailty},
            {"tarnish", DebuffLibrary.Corrosion},
        };



        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            Mop mopComp = this.projectile.PossibleSourceGun.GetComponent<Mop>();
            if (this.projectile != null && mopComp != null)
            {
                if (mopComp.CurrentGoopStringKey != null && mopComp.CurrentGoopStringKey != "none")
                {
                    if (mopComp.CurrentGoopStringKey == "Unknown")
                    {
                        GoopDefinition goopToBreakDown = mopComp.CurrentGoopKey;
                        this.projectile.AdjustPlayerProjectileTint(goopToBreakDown.baseColor32, 0, 0f);
                        List<GameActorEffect> effects = new List<GameActorEffect>();
                        if (goopToBreakDown.CharmModifierEffect != null) { effects.Add(goopToBreakDown.CharmModifierEffect); }
                        if (goopToBreakDown.fireEffect != null) { effects.Add(goopToBreakDown.fireEffect); }
                        if (goopToBreakDown.HealthModifierEffect != null) { effects.Add(goopToBreakDown.HealthModifierEffect); }
                        if (goopToBreakDown.CheeseModifierEffect != null) { effects.Add(goopToBreakDown.CheeseModifierEffect); }
                        if (goopToBreakDown.SpeedModifierEffect != null) { effects.Add(goopToBreakDown.SpeedModifierEffect); }
                        for (int i = 0; i < effects.Count; i++)
                        {
                            projectile.statusEffectsToApply.Add(effects[i]);
                        }
                    }
                    else
                    {
                        Color32 color = new Color32(0,0,0,0);
                        debuffColorationKeys.TryGetValue(mopComp.CurrentGoopStringKey, out color);
                        this.projectile.AdjustPlayerProjectileTint(color, 0, 0f);

                        GameActorEffect effect = null;
                        effectsToInflict.TryGetValue(mopComp.CurrentGoopStringKey, out effect);
                        if (effect != null) { projectile.statusEffectsToApply.Add(effect); }

                        Component component = null;
                        componentsToAddToProejctile.TryGetValue(mopComp.CurrentGoopStringKey, out component);
                        if (component != null) { projectile.gameObject.AddComponent(component); }

                        if (mopComp.CurrentGoopStringKey.ToLower() == "water")
                        {
                            projectile.baseData.damage *= 0.75f;
                        }
                    }
                }


            }
        }
    }
}


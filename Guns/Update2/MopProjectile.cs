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

        //private static BulletSlowModifier OwnSlowModifier;
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


                /*
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
                    //Gun fire = PickupObjectDatabase.GetById(384) as Gun;
                    //component.hitEffects = fire.DefaultModule.projectiles[0].hitEffects;
                    //component.AppliesFire = true;
                    //component.FireApplyChance = 0.35f;
                    //component.fireEffect = fire.DefaultModule.projectiles[0].fireEffect;
                    projectile.statusEffectsToApply.Add(DebuffStatics.hotLeadEffect);
                    ///PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>().FireModifierEffect



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
                */
            }
        }
    }
}


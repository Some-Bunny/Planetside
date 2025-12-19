using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;

using Brave.BulletScript;
using GungeonAPI;
using Alexandria.Misc;

namespace Planetside
{
	public class FrailtyHealthEffect : GameActorHealthEffect
	{
		public static string vfxNamefrailty = "FrailtyVFX";
		public static GameObject frailtyVFXObject;
		public Color TintColorFrailty = new Color(2f, 0f, 2f, 0.75f);
		public Color ClearUp = new Color(0f, 0f, 0f, 0f);

        public static GameObject BuildVFX()
        {
            /*
            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Frailty", debuffCollection.GetSpriteIdByName("frailtyeffecticon6"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);

            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            var clip = SpriteBuilder.AddAnimation(animator, debuffCollection, new List<int>()
            {
                debuffCollection.GetSpriteIdByName("frailtyeffecticon1"),
                debuffCollection.GetSpriteIdByName("frailtyeffecticon2"),
                debuffCollection.GetSpriteIdByName("frailtyeffecticon3"),
                debuffCollection.GetSpriteIdByName("frailtyeffecticon4"),
                debuffCollection.GetSpriteIdByName("frailtyeffecticon5"),
                debuffCollection.GetSpriteIdByName("frailtyeffecticon6"),

            }, "start", tk2dSpriteAnimationClip.WrapMode.Once, 7);

            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            */

            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Frailty VFX", debuffCollection.GetSpriteIdByName("frailtyeffecticon1"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            var sprite = BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.VFX_Animation_Data;
            animator.DefaultClipId = animator.GetClipIdByName("frailty_effect");
            animator.playAutomatically = true;
            sprite.usesOverrideMaterial = true;

            frailtyVFXObject = BrokenArmorVFXObject;
            return frailtyVFXObject;
        }

        public DamageTypeModifier FireMod;
        public DamageTypeModifier PoisonMod;
        public DamageTypeModifier IceMod;
        public DamageTypeModifier ElectricMod;
        public DamageTypeModifier VoidMod;
        public DamageTypeModifier WaterMod;
        public DamageTypeModifier NoneMod;



        public override void ApplyTint(GameActor actor)
        {
			actor.RegisterOverrideColor(TintColorFrailty, vfxNamefrailty);
            actor.EffectResistances = new ActorEffectResistance[]
            {

            };
            FireMod = new DamageTypeModifier() { damageMultiplier = 1, damageType = CoreDamageTypes.Fire };
            PoisonMod = new DamageTypeModifier() { damageMultiplier = 1, damageType = CoreDamageTypes.Poison };
            IceMod = new DamageTypeModifier() { damageMultiplier = 1, damageType = CoreDamageTypes.Ice };
            ElectricMod = new DamageTypeModifier() { damageMultiplier = 1, damageType = CoreDamageTypes.Electric };
            VoidMod = new DamageTypeModifier() { damageMultiplier = 1, damageType = CoreDamageTypes.Void };
            WaterMod = new DamageTypeModifier() { damageMultiplier = 1, damageType = CoreDamageTypes.Water };
            NoneMod = new DamageTypeModifier() { damageMultiplier = 1, damageType = CoreDamageTypes.None };

            actor.healthHaver.damageTypeModifiers.Add(FireMod);
            actor.healthHaver.damageTypeModifiers.Add(PoisonMod);
            actor.healthHaver.damageTypeModifiers.Add(IceMod);
            actor.healthHaver.damageTypeModifiers.Add(ElectricMod);
            actor.healthHaver.damageTypeModifiers.Add(VoidMod);
            actor.healthHaver.damageTypeModifiers.Add(WaterMod);
            actor.healthHaver.damageTypeModifiers.Add(NoneMod);

        }
        public float damageMult = 0;

        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{			
			if (this.AffectsEnemies && actor is AIActor)
			{

                if (actor.healthHaver.IsBoss)
                {
                    damageMult = 0.005f * BraveTime.DeltaTime;
                    actor.healthHaver.AllDamageMultiplier += 0.005f * BraveTime.DeltaTime;
                    FireMod.damageMultiplier += 0.125f * BraveTime.DeltaTime;
                    PoisonMod.damageMultiplier += 0.125f * BraveTime.DeltaTime;
                    IceMod.damageMultiplier += 0.125f * BraveTime.DeltaTime;
                    ElectricMod.damageMultiplier += 0.125f * BraveTime.DeltaTime;
                    VoidMod.damageMultiplier += 0.125f * BraveTime.DeltaTime;
                    WaterMod.damageMultiplier += 0.125f * BraveTime.DeltaTime;
                    NoneMod.damageMultiplier += 0.125f * BraveTime.DeltaTime;
                }
                else
                {
                    damageMult = 0.05f * BraveTime.DeltaTime;
                    actor.healthHaver.AllDamageMultiplier += 0.05f * BraveTime.DeltaTime;
                    FireMod.damageMultiplier += 0.5f * BraveTime.DeltaTime;
                    PoisonMod.damageMultiplier += 0.5f * BraveTime.DeltaTime;
                    IceMod.damageMultiplier += 0.5f * BraveTime.DeltaTime;
                    ElectricMod.damageMultiplier += 0.5f * BraveTime.DeltaTime;
                    VoidMod.damageMultiplier += 0.5f * BraveTime.DeltaTime;
                    WaterMod.damageMultiplier += 0.5f * BraveTime.DeltaTime;
                    NoneMod.damageMultiplier += 0.5f * BraveTime.DeltaTime;
                }
            }


            if (actor && actor.specRigidbody)
            {
                Vector2 unitDimensions = actor.specRigidbody.HitboxPixelCollider.UnitDimensions;
                Vector2 a = unitDimensions / 2f;
                int num2 = Mathf.RoundToInt((float)this.flameNumPerSquareUnit * 0.5f * Mathf.Min(30f, Mathf.Min(new float[]
                {
                unitDimensions.x * unitDimensions.y
                })));
                this.m_particleTimer += BraveTime.DeltaTime * (float)num2;
                if (this.m_particleTimer > 1f)
                {
                    int num3 = Mathf.FloorToInt(this.m_particleTimer);
                    Vector2 vector = actor.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
                    Vector2 vector2 = actor.specRigidbody.HitboxPixelCollider.UnitTopRight;
                    PixelCollider pixelCollider = actor.specRigidbody.GetPixelCollider(ColliderType.Ground);
                    if (pixelCollider != null && pixelCollider.ColliderGenerationMode == PixelCollider.PixelColliderGeneration.Manual)
                    {
                        vector = Vector2.Min(vector, pixelCollider.UnitBottomLeft);
                        vector2 = Vector2.Max(vector2, pixelCollider.UnitTopRight);
                    }
                    vector += Vector2.Min(a * 0.15f, new Vector2(0.25f, 0.25f));
                    vector2 -= Vector2.Min(a * 0.15f, new Vector2(0.25f, 0.25f));
                    vector2.y -= Mathf.Min(a.y * 0.1f, 0.1f);
                   
                    GlobalSparksDoer.DoRandomParticleBurst(num3, vector, vector2, Vector3.down, 0f, 0.5f, 0.3f, 1, Color.magenta, GlobalSparksDoer.SparksType.DARK_MAGICKS);
                    this.m_particleTimer -= Mathf.Floor(this.m_particleTimer);
                }
            }
        }
        public int flameNumPerSquareUnit = 10;
        private float m_particleTimer;


        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			actor.DeregisterOverrideColor(vfxNamefrailty);			
			base.OnEffectRemoved(actor, effectData);
            actor.healthHaver.AllDamageMultiplier -= damageMult;

            actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
            actor.healthHaver.damageTypeModifiers.Remove(FireMod);
            actor.healthHaver.damageTypeModifiers.Remove(PoisonMod);
            actor.healthHaver.damageTypeModifiers.Remove(IceMod);
            actor.healthHaver.damageTypeModifiers.Remove(ElectricMod);
            actor.healthHaver.damageTypeModifiers.Remove(VoidMod);
            actor.healthHaver.damageTypeModifiers.Remove(WaterMod);
            actor.healthHaver.damageTypeModifiers.Remove(NoneMod);
        }
    }
}



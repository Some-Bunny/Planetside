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
using tk2dRuntime.TileMap;

//Garbage Code Incoming
namespace Planetside
{
    public class AoEBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Aura Bullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<AoEBullets>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("aurabullets"), data, obj);
            
            string shortDesc = "Radiant";
            string longDesc = "Makes bullets deal damage to enemies near them." +
                "\n\nThese bullets contain a very rare and powerful radioactive isotope. Don't lick them!";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ProjectileSpeed, 0.7f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			item.quality = PickupObject.ItemQuality.B;
			List<string> mandatoryConsoleIDs = new List<string>
			{
				"psog:aura_bullets",
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"irradiated_lead",
				"uranium_ammolet",
				"monster_blood",
				"poison_vial",
				"big_boy",
				"shotgrub",
				"plunger",
				"plague_pistol"
			};
			CustomSynergies.Add("Khh..k k k k", mandatoryConsoleIDs, optionalConsoleIDs, true);
			List<string> optionalConsoleID1s = new List<string>
			{
				"hot_lead",
				"copper_ammolet",
				"napalm_strike",
				"ring_of_fire_resistance",
				"flame_hand",
				"phoenix",
				"pitchfork"
			};
			CustomSynergies.Add("Handle The Heat", mandatoryConsoleIDs, optionalConsoleID1s, true);
			List<string> optionalConsoleID2s = new List<string>
			{
				"frost_bullets",
				"frost_ammolet",
				"heart_of_ice",
				"ice_cube",
				"ice_bomb",
				"glacier"
			};
			CustomSynergies.Add("Below Zero", mandatoryConsoleIDs, optionalConsoleID2s, true);
			AoEBullets.AuraBulletsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int AuraBulletsID;

		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
				if (sourceProjectile.baseData.range >= 3)
				{
					PlayerController player = base.Owner;
					AoEDamageComponent Values = sourceProjectile.gameObject.AddComponent<AoEDamageComponent>();
					Values.DamageperDamageEvent = ((sourceProjectile.baseData.damage * 0.15f)+0.5f);
					Values.Radius = 2.75f;
					Values.TimeBetweenDamageEvents = 0.2f;
					Values.DealsDamage = true;
					Values.AreaIncreasesWithProjectileSizeStat = true;
					Values.DamageValuesAlsoScalesWithDamageStat = true;
					if (player.PlayerHasActiveSynergy("Khh..k k k k"))
					{
                        Values.debuffs.Add(DebuffStatics.irradiatedLeadEffect, 0.07f);
                    }
					if (player.PlayerHasActiveSynergy("Handle The Heat"))
					{
                        Values.debuffs.Add(DebuffStatics.hotLeadEffect, 0.07f);
                    }
                    if (player.PlayerHasActiveSynergy("Below Zero"))
					{
                        Values.debuffs.Add(DebuffStatics.frostBulletsEffect, 0.5f);
                    }
                    this.ShockRing(sourceProjectile, player.PlayerHasActiveSynergy("Handle The Heat") == true);
				}
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
			}
		}

		private void PostProcessBeam (BeamController beamC)
		{
            PlayerController player = base.Owner;
            AOEBeamTipController Values = beamC.gameObject.GetOrAddComponent<AOEBeamTipController>();
            Values.DamageperDamageEvent = ((beamC.projectile.baseData.damage * 0.15f) + 0.5f);
            Values.radiusValue = 2.75f;
            Values.TimeBetweenDamageEvents = 0.2f;
            Values.DealsDamage = true;
            Values.AreaIncreasesWithProjectileSizeStat = true;
            Values.DamageValuesAlsoScalesWithDamageStat = true;
            if (player.PlayerHasActiveSynergy("Khh..k k k k"))
            {
                Values.debuffs.Add(DebuffStatics.irradiatedLeadEffect, 0.1f);
            }
            if (player.PlayerHasActiveSynergy("Handle The Heat"))
            {
                Values.debuffs.Add(DebuffStatics.hotLeadEffect, 0.1f);
            }
            if (player.PlayerHasActiveSynergy("Below Zero"))
            {
                Values.debuffs.Add(DebuffStatics.frostBulletsEffect, 1f);
            }
        }

		private void EndRingEffect(Projectile projectile)
		{
			//this.m_radialIndicator.EndEffect();
		}
		private void ShockRing(Projectile projectile, bool f)
		{
			PlayerController player = projectile.Owner as PlayerController;
			float num = 0f;
			num = (player.stats.GetStatValue(PlayerStats.StatType.PlayerBulletScale));
            HeatIndicatorController m_radialIndicator = ((GameObject)UnityEngine.Object.Instantiate(StaticVFXStorage.RadialRing, projectile.sprite.WorldCenter, Quaternion.identity, projectile.transform)).GetComponent<HeatIndicatorController>();
			m_radialIndicator.CurrentColor = Color.white.WithAlpha(4f);
			m_radialIndicator.CurrentRadius = 1.75f* num;
			m_radialIndicator.IsFire = f;
			m_radialIndicator.gameObject.transform.parent = projectile.transform;
            var material = m_radialIndicator.GetComponent<MeshRenderer>().material;
            material.SetFloat("_PxWidth", 0.35f);
        }
        public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeam -= PostProcessBeam;

            return result;
		}



		public override void OnDestroy()
		{
			if (Owner != null)
			{
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
                base.Owner.PostProcessBeam -= PostProcessBeam;

            }
            base.OnDestroy();
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
            base.Owner.PostProcessBeam += PostProcessBeam;
        }


        public class AOEBeamTipController : MonoBehaviour
        {

            public float DamageperDamageEvent;
            public float TimeBetweenDamageEvents;
            public bool DealsDamage;
            public bool AreaIncreasesWithProjectileSizeStat;
            public bool DamageValuesAlsoScalesWithDamageStat;

            public Dictionary<GameActorEffect, float> debuffs = new Dictionary<GameActorEffect, float>();
            public Dictionary<GameActorEffect, Func<bool>> conditionalDebuffs = new Dictionary<GameActorEffect, Func<bool>>();

            private float elapsed;

            public AOEBeamTipController()
            {
                this.tickDelay = 0.05f;
                this.ignoreQueues = true;
                this.maxRadius = 5;
            }

            private void Start()
            {
                radialIndicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), base.transform.position, Quaternion.identity)).GetComponent<HeatIndicatorController>();
                radialIndicator.CurrentColor = Color.white.WithAlpha(4f);
                radialIndicator.IsFire = false;
                radialIndicator.CurrentRadius = 2.75f;
                var material = radialIndicator.GetComponent<MeshRenderer>().material;
                material.SetFloat("_PxWidth", 0.35f);

                radiusValue = radialIndicator.CurrentRadius;
                this.projectile = base.GetComponent<Projectile>();
                this.basicBeamController = base.GetComponent<BasicBeamController>();
                if (this.projectile.Owner is PlayerController)
                {
                    this.owner = (this.projectile.Owner as PlayerController);
                    if (owner)
                    {
                        MultiplierScale = AreaIncreasesWithProjectileSizeStat == true && owner ? owner.stats.GetStatValue(PlayerStats.StatType.PlayerBulletScale) : 1;
                        MultiplierDamage = DamageValuesAlsoScalesWithDamageStat == true && owner ? owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
                    }
                }
            }
            private float MultiplierScale = 1;
            private float MultiplierDamage = 1;
            private void Update()
            {
                this.DoTick();
                this.elapsed += BraveTime.DeltaTime;
                if (this.elapsed > TimeBetweenDamageEvents)
                {
                    elapsed = 0;
                    List<AIActor> activeEnemies = StoredBlastPosition.GetAbsoluteRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    Vector2 centerPosition = StoredBlastPosition;
                    if (activeEnemies != null)
                    {
                        for (int em = 0; em < activeEnemies.Count; em++)
                        {
                            AIActor aiactor = activeEnemies[em];
                            if (aiactor != null)
                            {
                                if (Vector2.Distance(aiactor.CenterPosition, centerPosition) < radiusValue * MultiplierScale)
                                {
                                    if (DealsDamage == true)
                                    {
                                        if (aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null)
                                        {
                                            aiactor.healthHaver.ApplyDamage(DamageperDamageEvent * MultiplierDamage, Vector2.zero, "Aura", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                                        }
                                    }
                                    foreach (var Entry in debuffs)
                                    {
                                        if (UnityEngine.Random.value < Entry.Value)
                                        {
                                            aiactor.ApplyEffect(Entry.Key, 1, null);
                                        }
                                    }
                                    foreach (var Entry in conditionalDebuffs)
                                    {
                                        if (Entry.Value != null)
                                        {
                                            if (Entry.Value() == true)
                                            {
                                                aiactor.ApplyEffect(Entry.Key, 1, null);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    this.elapsed = 0f;
                }
            }


            private void DoTick()
            {
                LinkedList<BasicBeamController.BeamBone> linkedList = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", this.basicBeamController);
                LinkedListNode<BasicBeamController.BeamBone> last = linkedList.Last;
                Vector2 bonePosition = this.basicBeamController.GetBonePosition(last.Value);
                StoredBlastPosition = bonePosition;
                if (maxRadius >= radiusValue) { radialIndicator.CurrentRadius = radiusValue; }
                radialIndicator.transform.position = bonePosition;
            }

            public void OnDestroy()
            {
                Destroy(radialIndicator.gameObject);
            }

            private Vector2 StoredBlastPosition;
            private HeatIndicatorController radialIndicator;

            public float maxRadius;
            public float radiusValue;


            public bool ignoreQueues;

            public float tickDelay;
            public ExplosionData explosionData;
            private Projectile projectile;

            private BasicBeamController basicBeamController;
            private PlayerController owner;
        }
    }
}



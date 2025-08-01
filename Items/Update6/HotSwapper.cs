using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Collections;
using SaveAPI;
using Brave.BulletScript;
using Alexandria.cAPI;
using Alexandria.PrefabAPI;
using Newtonsoft.Json.Linq;
using Alexandria.ItemAPI;
using static UnityEngine.UI.GridLayoutGroup;
using System.ComponentModel;
using static ETGMod;
using Planetside.DungeonPlaceables;

namespace Planetside
{
	public class HotSwapper : PassiveItem
	{
		public static void Init()
		{
			string name = "Hot Swapper";
			GameObject gameObject = new GameObject(name);
            HotSwapper item = gameObject.AddComponent<HotSwapper>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemAPI.ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("hotswapper"), data, gameObject); 
			
			string shortDesc = "Playing With Firepower";
			string longDesc = "Killing enemies grants boosts, as long as you swap weapons every kill.\n\nBullet Kin use these gloves to put out fires, consuming oxygen even faster and suffocating the fire.\n\nUnusually effective.";

            ItemAPI.ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.B;
            HotSwapper.ItemID = item.PickupObjectId;
            ItemAPI.ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalClipCapacityMultiplier, 1.2f, StatModifier.ModifyMethod.MULTIPLICATIVE);


            


        }
        public float GraceTime = 1.5f;
        public static int ItemID;
        public override void Update()
		{
			base.Update();
			if (Owner)
			{
				if (GraceTime > 0) { GraceTime -= Time.deltaTime; }
                if (CurrentKillStreak > 0)
                {
                    if (UnityEngine.Random.value < CurrentKillStreak * Time.deltaTime)
                    {
                        GlobalSparksDoer.DoRadialParticleBurst(1,
                            Owner.sprite.WorldBottomLeft,
                            Owner.sprite.WorldTopRight,
                            360,
                            CurrentKillStreak + 0.5f,
                            1f, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                    }
                }
			}
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
            player.GunChanged += Player_GunChanged;
            player.PostProcessProjectile += PPP;
            player.PostProcessBeam += PPB;
            Owner.PostProcessBeamTick += PPBT;

        }
        #region Post Processing
        private void PPP(Projectile arg1, float arg2)
        {
            arg1.baseData.damage *= 1 + 1 * (0.1f * Mathf.Sqrt(CurrentKillStreak + 0.5f));
            float AdditionalMath = (CurrentKillStreak * 0.02f);
            arg1.baseData.damage += AdditionalMath;
            arg1.baseData.speed *= 1 + AdditionalMath;
            arg1.baseData.force *= 1 + AdditionalMath;
            if (CurrentKillStreak >= 10)
            {
                if (UnityEngine.Random.value < (AdditionalMath - 0.1f) * arg2)
                {
                    arg1.AdjustPlayerProjectileTint(new Color(1, 0.3f, 0), 10);
                    projectile.FireApplyChance = 1;
                    arg1.AppliesFire = true;
                    arg1.fireEffect = DebuffStatics.hotLeadEffect;
                }
            }
            arg1.OnWillKillEnemy += (proj, body) =>
            {
                var p = (proj as Projectile);
                if (p == null) { return; }
                Gun gun = p.PossibleSourceGun;
                if (gun == null) { return; }
                var potentialEnemy = (body as SpeculativeRigidbody);
                if (potentialEnemy == null) { return; }
                if (potentialEnemy.gameActor != null && potentialEnemy.gameActor is AIActor enemy)
                {
                    if (!GuidBlackList.Contains(enemy.EnemyGuid))
                    {
                        if (GunsUsedForKill.Contains(gun))
                        {
                            ResetKillStreak();
                        }
                        else
                        {
                            IncrementKill(gun);
                        }
                    }
                }
            };
        }
        private void PPB(BeamController beam)
        {
            var arg1 = beam.projectile;

            arg1.baseData.damage *= 1 + 1 * (0.1f * Mathf.Sqrt(CurrentKillStreak + 0.5f));
            float AdditionalMath = (CurrentKillStreak * 0.02f);
            arg1.baseData.damage += AdditionalMath;
            arg1.baseData.speed *= 1 + AdditionalMath;
            arg1.baseData.force *= 1 + AdditionalMath;


            arg1.OnWillKillEnemy += (proj, body) =>
            {
                var p = (proj as Projectile);
                if (p == null) { return; }
                Gun gun = p.PossibleSourceGun;
                if (gun == null) { return; }
                var potentialEnemy = (body as SpeculativeRigidbody);
                if (potentialEnemy == null) { return; }
                if (potentialEnemy.gameActor != null && potentialEnemy.gameActor is AIActor enemy)
                {
                    if (!GuidBlackList.Contains(enemy.EnemyGuid))
                    {
                        if (GunsUsedForKill.Contains(gun))
                        {
                            ResetKillStreak();
                        }
                        else
                        {
                            IncrementKill(gun);
                        }
                    }
                }
            };
        }
        private void PPBT(BeamController beam, SpeculativeRigidbody speculativeRigidbody,  float arg2)
        {
            float AdditionalMath = (CurrentKillStreak * 0.005f);
            var arg1 = beam.projectile;
            if (CurrentKillStreak >= 10)
            {
                if (UnityEngine.Random.value < (AdditionalMath - 0.1f) * arg2)
                {
                    if (speculativeRigidbody.gameActor != null && speculativeRigidbody.gameActor is AIActor actor)
                    {
                        actor.ApplyEffect(DebuffStatics.hotLeadEffect);
                    }
                }
            }
        }
        #endregion

        public void Player_GunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            if (CurrentKillStreak == 0) { return; }
            if (Owner.inventory.AllGuns.Count == GunsUsedForKill.Count)
            {
                GunsUsedForKill.Clear();
                return;
            }
            if (GunsUsedForKill.Contains(newGun))
            {
                ResetKillStreak();
                //KILL    
            }
            else
            {
                List<AIActor> activeEnemies = Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                float Power = 2.5f + Mathf.Sqrt(CurrentKillStreak * 0.5f);
                if (activeEnemies != null)
                {
                    for (int i = 0; i < activeEnemies.Count; i++)
                    {
                        AIActor aiactor = activeEnemies[i];
                        if (aiactor.IsNormalEnemy)
                        {
                            float num = Vector2.Distance(Owner.CenterPosition, aiactor.CenterPosition);
                            if (num <= Power)
                            {
                                aiactor.ApplyEffect(DebuffStatics.hotLeadEffect);
                                aiactor.healthHaver.ApplyDamage(CurrentKillStreak, Vector2.zero, "Hot Tuah");
                            }
                        }
                    }
                }
                Exploder.DoRadialPush(Owner.gameObject.transform.PositionVector2(), 20, Power);
                Exploder.DoRadialKnockback(Owner.gameObject.transform.PositionVector2(), 20, Power);
                Exploder.DoRadialMinorBreakableBreak(Owner.gameObject.transform.PositionVector2(), Power);
                AkSoundEngine.PostEvent("Play_Immolate", Owner.gameObject);
                for (int i = 0; i < 24; i++)
                {
                    GlobalSparksDoer.DoRadialParticleBurst(1, 
                        Owner.sprite.WorldBottomLeft, 
                        Owner.sprite.WorldTopRight, 
                        360,
                        Power + 0.5f, 
                        1f, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                    
                    GlobalSparksDoer.DoRadialParticleBurst(1, 
                        Owner.sprite.WorldBottomLeft, 
                        Owner.sprite.WorldTopRight, 
                        360,
                        Power + 1f, 
                        0.5f, null, null, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
                }

            }
        }

      public List<Gun> GunsUsedForKill = new List<Gun>();

       public void ResetKillStreak()
       {
            if (GraceTime > 0) { return; }
            AkSoundEngine.PostEvent("Play_OBJ_dead_again_01", Owner.gameObject);
            AkSoundEngine.PostEvent("Play_BOSS_dragun_flap_01", Owner.gameObject);

            //m_BOSS_dragun_flap_01
            //m_OBJ_dead_again_01
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = Owner.sprite.WorldCenter,
                startSize = 2,
                rotation = 0,
                startLifetime = 0.5f,
                startColor = Color.white
            });
            CurrentKillStreak = 0;
            GunsUsedForKill.Clear();
            UpdateStats();
       }
        private StatModifier FireRate;
        public void IncrementKill(Gun gun)
        {
            if (Owner.inventory.AllGuns.Count < 2) { return; }
            GraceTime = 2;
            CurrentKillStreak++;
            if (CurrentKillStreak == 1)
            {
                FireRate = this.AddStat(PlayerStats.StatType.RateOfFire, CurrentKillStreak * 0.01f, StatModifier.ModifyMethod.ADDITIVE);
                Owner.stats.RecalculateStats(Owner, true, false);
            }
            GunsUsedForKill.Add(gun);
            UpdateStats();
            AkSoundEngine.PostEvent("Play_Immolate", Owner.gameObject);
            for (int i = 0; i < 18; i++)
            {

                GlobalSparksDoer.DoSingleParticle(
                    Owner.sprite.WorldCenter + MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 0.25f),
                    Vector2.up * UnityEngine.Random.Range(1.5f, 3.25f),
                    null,
                    0.5f,
                    Color.red,
                    GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
            }
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = Owner.sprite.WorldCenter,
                startSize = 2,
                rotation = 0,
                startLifetime = 0.5f,
                startColor = Color.red
            });
        }

        public void UpdateStats()
        {
            FireRate.amount = CurrentKillStreak * 0.025f;
            Owner.stats.RecalculateStats(Owner, true, false);
        }

        


        public override void OnDestroy()
		{
            if (Owner)
            {
                Owner.GunChanged -= Player_GunChanged;
                Owner.PostProcessProjectile -= PPP;
                Owner.PostProcessBeam -= PPB;
                Owner.PostProcessBeamTick -= PPBT;
            }
            base.OnDestroy();
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
            player.GunChanged -= Player_GunChanged;
            player.PostProcessProjectile -= PPP;
            player.PostProcessBeam -= PPB;
            Owner.PostProcessBeamTick -= PPBT;
            return result;
		}
        static List<string> GuidBlackList = new List<string>()
        {
            Alexandria.EnemyGUIDs.Blobulin_GUID,
            FodderEnemy.guid
        };
        private float CurrentKillStreak = 0;
    }
}

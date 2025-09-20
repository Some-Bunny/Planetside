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
using SynergyAPI;
using static Planetside.Wailer;

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

            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:hot_swapper",
                "phoenix"
            };
            Alexandria.ItemAPI.CustomSynergies.Add("Rebirth", mandatoryConsoleIDs, null, true);
            var phoenix = (PickupObjectDatabase.GetById(384) as Gun);
            var syn = phoenix.gameObject.AddComponent<PhoenixSynergy>();
            syn.GunSelf = phoenix;

            phoenix = (PickupObjectDatabase.GetById(736) as Gun);
            syn = phoenix.gameObject.AddComponent<PhoenixSynergy>();
            syn.GunSelf = phoenix;


            phoenixProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(35) as Gun).DefaultModule.projectiles[0]);
            phoenixProjectile.gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(phoenixProjectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(phoenixProjectile);

            phoenixProjectile.baseData.UsesCustomAccelerationCurve = true;
            phoenixProjectile.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1f, 0f, 1f);
            phoenixProjectile.baseData.CustomAccelerationCurveDuration = 0.2f;
            phoenixProjectile.baseData.speed = 35;
            phoenixProjectile.baseData.damage = 40;
            phoenixProjectile.shouldRotate = true;

            phoenixProjectile.fireEffect = DebuffStatics.hotLeadEffect;
            phoenixProjectile.FireApplyChance = 1;
            phoenixProjectile.AppliesFire = true;

            var spook = phoenixProjectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            spook.penetration = 5;

            Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(phoenixProjectile, "phoenixRebirth", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "phoenixRebirth",
            new List<IntVector2>() { new IntVector2(25, 31), new IntVector2(25, 31), new IntVector2(25, 31), new IntVector2(25, 31), new IntVector2(25, 31), new IntVector2(25, 31), new IntVector2(25, 31), new IntVector2(25, 31), new IntVector2(25, 31), },
            AnimateBullet.ConstructListOfSameValues(true, 9),
            AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 9),
            AnimateBullet.ConstructListOfSameValues(true, 9),
            AnimateBullet.ConstructListOfSameValues(false, 9),
            AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 9),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 9),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 9),
            AnimateBullet.ConstructListOfSameValues<Projectile>(null, 9));

            phoenixProjectile.AddTrail(new Vector2(-0.125f, 0.5f), 0.2f, 7, Color.white * 3, Color.yellow * 2);
            phoenixProjectile.AddTrail(new Vector2(-0.125f, -0.5f), 0.2f, 7, Color.white * 3, Color.yellow * 2);
            phoenixProjectile.AddTrail(new Vector2(-0f, 0.25f), 0.875f, 5.5f, Color.white * 3, Color.yellow * 2);
            phoenixProjectile.AddTrail(new Vector2(-0f, 0.25f), -0.875f, 5.5f, Color.white * 3, Color.yellow * 2);

            var material = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
            phoenixProjectile.sprite.usesOverrideMaterial = true;
            material.mainTexture = phoenixProjectile.sprite.renderer.material.mainTexture;
            phoenixProjectile.sprite.renderer.material = material;
            phoenixProjectile.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            phoenixProjectile.sprite.renderer.material.SetFloat("_EmissivePower", 35);
            phoenixProjectile.sprite.renderer.material.SetFloat("_EmissiveColorPower", 20);

        }
        public static Projectile phoenixProjectile;

        public class PhoenixSynergy : BraveBehaviour
        {
            public Gun GunSelf;
            public void Start()
            {
                GunSelf.OnPreFireProjectileModifier += OverrideForSynergy;
                GunSelf.OnPostFired += OPF;
            }

            public void OPF(PlayerController playerController, Gun gun)
            {
                if (ActivatePhoenix && Cooldown <= 0)
                {
                    Cooldown = 5f;
                    AkSoundEngine.PostEvent("Play_OBJ_bloodybullet_proc_01", gun.gameObject);
                    ActivatePhoenix = false;
                }
            }

            public Projectile OverrideForSynergy(Gun gun, Projectile projectile, ProjectileModule projectileModule)
            {
                if (ActivatePhoenix && Cooldown <= 0)
                {
                    projectile = phoenixProjectile;
                }
                return projectile;
            }
            private float Cooldown;
            private bool SynergyActive = false;
            public void Update()
            {
                if (GunSelf && GunSelf.CurrentOwner != null && GunSelf.CurrentOwner is PlayerController player)
                {    
                    if (SynergyActive)
                    {
                        if (Cooldown > 0) { Cooldown -= Time.deltaTime; }
                        else if (ActivatePhoenix)
                        {
                            Vector3 vector = GunSelf.sprite.WorldBottomLeft.ToVector3ZisY(0);
                            Vector3 vector2 = GunSelf.sprite.WorldTopRight.ToVector3ZisY(0);
                            float num = (vector2.y - vector.y) * (vector2.x - vector.x);
                            float num2 = 15f * num;
                            int num4 = Mathf.CeilToInt(Mathf.Max(1f, num2 * BraveTime.DeltaTime)); ;
                            Vector3 direction = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1);
                            float magnitudeVariance = 0.2f;
                            float? startLifetime = new float?(UnityEngine.Random.Range(0.1f, 0.4f));
                            GlobalSparksDoer.DoRandomParticleBurst(num4, vector, vector2, direction, 0, magnitudeVariance, 0.333f, startLifetime, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
                        }
                    }
                    if (player.PlayerHasActiveSynergy("Rebirth") != SynergyActive)
                    {
                        SynergyActive = player.PlayerHasActiveSynergy("Rebirth");
                        if (SynergyActive == true)
                        {
                            player.GunChanged += Player_GunChanged1;
                        }
                        else
                        {
                            player.GunChanged -= Player_GunChanged1;
                        }
                    }
                }
            }
            public bool ActivatePhoenix;
            private void Player_GunChanged1(Gun arg1, Gun arg2, bool arg3)
            {
               if (arg2 == this.GunSelf)
               {
                    ActivatePhoenix = true;
                    Cooldown -= Time.timeSinceLevelLoad - CurTime;
               }
               else
               {
                    CurTime = Time.timeSinceLevelLoad;
               }
            }
            private float CurTime;
        }



        public float GraceTime = 1.5f;
        public static int ItemID;
        public override void Update()
		{
			base.Update();
			if (Owner)
			{
				if (GraceTime > 0) { GraceTime -= Time.deltaTime; }
                if (GunsUsedForKill.Contains(Owner.CurrentGun))
                {
                    if (UnityEngine.Random.value < 12 * Time.deltaTime)
                    {
                        GlobalSparksDoer.DoRadialParticleBurst(1,
                            Owner.CurrentGun.sprite.WorldBottomLeft,
                            Owner.CurrentGun.sprite.WorldTopRight,
                            360,
                            CurrentKillStreak + 0.25f,
                            1f, null, null, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
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
                return;
                //ResetKillStreak();
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
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = Owner.sprite.WorldCenter,
                    startSize = .5f + Power,
                    rotation = 0,
                    startLifetime = 0.3f,
                    startColor = Color.red.WithAlpha(0.4f)
                });
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
                startSize = 6,
                rotation = 0,
                startLifetime = 0.25f,
                startColor = Color.white.WithAlpha(0.4f)
            });
            CurrentKillStreak = 0;
            GunsUsedForKill.Clear();
            UpdateStats();
       }
        private StatModifier FireRate;
        public void IncrementKill(Gun gun)
        {
            if (GunsUsedForKill.Contains(gun))
            {
                ResetKillStreak();
                return;
            }

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
                startSize = 3.5f,
                rotation = 0,
                startLifetime = 0.3f,
                startColor = Color.red.WithAlpha(0.4f)
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

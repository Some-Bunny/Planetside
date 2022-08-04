using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;



namespace Planetside
{
    public class HeatSink : GunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Heat Sink", "heat_sink");
            Game.Items.Rename("outdated_gun_mods:heat_sink", "psog:heat_sink");
            var behav = gun.gameObject.AddComponent<HeatSink>();
            
            gun.SetShortDescription("Hot N' Heavy");
            gun.SetLongDescription("This portable microwave can super-charge a localized space with super heated particles and cause them to go BOOM!\n\nBy all accounts, illegal everywhere within Hegemony-ruled space.");

            gun.SetupSprite(null, "heat_sink_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 32);
            gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 9);

            gun.isAudioLoop = true;
            for (int i = 0; i < 1; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);
            }
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {
                mod.ammoCost = 5;
                if (mod != gun.DefaultModule) { mod.ammoCost = 0; }
                mod.shootStyle = ProjectileModule.ShootStyle.Beam;
                mod.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                mod.cooldownTime = 0.001f;
                mod.numberOfShotsInClip = 25;
            List<string> BeamAnimPaths = new List<string>()
            {
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_001",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_002",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_003",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_004",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_005",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_006",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_007",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_008",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_009",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_010",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_011",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_012",
            };                
            List<string> ImpactAnimPaths = new List<string>()
            {
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_end_001",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_end_002",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_end_003",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_end_004",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_end_005",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_end_006",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_end_007",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_end_008",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_end_009",
                "Planetside/Resources/Beams/HeatSink/heatsinkbeam_end_010",
            };

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

                BasicBeamController beamComp = projectile.GenerateBeamPrefab(
                    "Planetside/Resources/Beams/HeatSink/heatsinkbeam_mid_001",
                    new Vector2(16, 10),
                    new Vector2(0, 3),
                    BeamAnimPaths,
                    12,
                    //Beam Impact
                    ImpactAnimPaths,
                    30,
                    new Vector2(16, 16),
                    new Vector2(0, 0),
                    //End of the Beam
                    ImpactAnimPaths,
                    30,
                    new Vector2(16, 16),
                    new Vector2(0, 0),
                    //Start of the Beam
                    null,
                    12,
                    new Vector2(10, 2),
                    new Vector2(0, 4)
                    );

                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 7f;
                projectile.baseData.force *= 0f;
                projectile.baseData.range = 6;
                projectile.baseData.speed *= 0.3f;
                projectile.specRigidbody.CollideWithOthers = false;
                HeatSinkController controller = projectile.gameObject.AddComponent<HeatSinkController>();

                EmmisiveBeams emiss = beamComp.gameObject.AddComponent<EmmisiveBeams>();
                emiss.EmissiveColorPower = 10f;
                emiss.EmissivePower = 100;

                beamComp.boneType = BasicBeamController.BeamBoneType.Projectile;

                beamComp.startAudioEvent = "Play_WPN_raidenlaser_shot_01";
                beamComp.projectile.baseData.damage = 7;
                beamComp.endAudioEvent = "Stop_WPN_raidenlaser_loop_01";
                beamComp.penetration = 10;
                beamComp.reflections = 0;
                beamComp.IsReflectedBeam = false;
                mod.projectiles[0] = projectile;

                gun.sprite.usesOverrideMaterial = true;

                Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
                mat.SetFloat("_EmissiveColorPower", 1.55f);
                mat.SetFloat("_EmissivePower", 50);
                mat.SetFloat("_EmissiveThresholdSensitivity", 0.2f);
                MeshRenderer component = gun.GetComponent<MeshRenderer>();
                if (!component)
                {
                    return;
                }
                Material[] sharedMaterials = component.sharedMaterials;
                for (int i = 0; i < sharedMaterials.Length; i++)
                {
                    if (sharedMaterials[i].shader == mat)
                    {
                        return;
                    }
                }
                Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
                Material material = new Material(mat);
                material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
                sharedMaterials[sharedMaterials.Length - 1] = material;
                component.sharedMaterials = sharedMaterials;

              
            }

            //GUN STATS
            gun.doesScreenShake = false;
            gun.reloadTime = 1.5f;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, 0.625f, 0f);
            gun.SetBaseMaxAmmo(500);
            gun.ammo = 500;
            gun.PreventNormalFireAudio = true;
            gun.gunClass = GunClass.BEAM;


            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(597) as Gun).gunSwitchGroup;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("HeatSink", "Planetside/Resources/GunClips/HeatSink/heatsinkfull", "Planetside/Resources/GunClips/HeatSink/heatsinkempty");
            gun.quality = PickupObject.ItemQuality.S;
            ETGMod.Databases.Items.Add(gun, false, "ANY");

            HeatSink.HeatSinkID = gun.PickupObjectId;
            ItemIDs.AddToList(gun.PickupObjectId);
        }
        public static int HeatSinkID;

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsFiring == true)
            {
                gun.CeaseAttack(false);
            }
            base.OnReloadPressed(player, gun, bSOMETHING);
        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
        }
    }
}

namespace Planetside
{
    public class HeatSinkController : MonoBehaviour
    {
        public HeatSinkController()
        {
            this.explosionData = StaticExplosionDatas.genericSmallExplosion;
            this.tickDelay = 0.05f;
            this.ignoreQueues = true;
            this.maxRadius = 5;
            radiusIncreasePerSecond = 1.33f;
        }

        private void Start()
        {
            radialIndicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), base.transform.position, Quaternion.identity)).GetComponent<HeatIndicatorController>();
            radialIndicator.CurrentColor = new Color(253, 110, 0).WithAlpha(5);
            radialIndicator.IsFire = true;
            radialIndicator.CurrentRadius = 1;
            radiusValue = radialIndicator.CurrentRadius;
            this.timer = this.tickDelay;
            this.projectile = base.GetComponent<Projectile>();
            this.basicBeamController = base.GetComponent<BasicBeamController>();
            bool flag = this.projectile.Owner is PlayerController;
            if (flag)
            {
                this.owner = (this.projectile.Owner as PlayerController);
            }
        }
        private void Update()
        {
            this.DoTick();
        }
        private void DoTick()
        {
            LinkedList<BasicBeamController.BeamBone> linkedList = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", this.basicBeamController);
            LinkedListNode<BasicBeamController.BeamBone> last = linkedList.Last;
            Vector2 bonePosition = this.basicBeamController.GetBonePosition(last.Value);
            StoredBlastPosition = bonePosition;
            radiusValue += (radiusIncreasePerSecond * BraveTime.DeltaTime);
            if (maxRadius >= radiusValue) { radialIndicator.CurrentRadius = radiusValue; }
            radialIndicator.transform.position = bonePosition;
        }

        private void Explode(Vector2 pos)
        {
            for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
            {
                PlayerController playerController = GameManager.Instance.AllPlayers[i];
                bool flag2 = playerController && playerController.specRigidbody;
                if (flag2)
                {
                    this.explosionData.ignoreList.Add(playerController.specRigidbody);
                }
            }
            float Mult = owner != null ? owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
            this.explosionData.damage = (26 * radiusValue) * Mult;
            this.explosionData.damageRadius = radiusValue;
            this.explosionData.useDefaultExplosion = false;
            Exploder.Explode(pos, this.explosionData, Vector2.zero, null, this.ignoreQueues, CoreDamageTypes.None, false);

            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
            objanimator.ignoreTimeScale = true;
            objanimator.AlwaysIgnoreTimeScale = true;
            objanimator.AnimateDuringBossIntros = true;
            objanimator.alwaysUpdateOffscreen = true;
            objanimator.playAutomatically = true;
            ParticleSystem objparticles = silencerVFX.GetComponentInChildren<ParticleSystem>();
            var main = objparticles.main;
            main.useUnscaledTime = true;
            GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, pos, Quaternion.identity);
            AkSoundEngine.PostEvent("Play_BOSS_RatMech_Bomb_01", silencerVFX.gameObject);
            Destroy(blankObj, 2f);
            blankObj.transform.localScale = Vector3.one * (radialIndicator.CurrentRadius / 4);
            Exploder.DoDistortionWave(pos, 10f, 0.4f, radialIndicator.CurrentRadius, 0.066f);
            Destroy(radialIndicator.gameObject);
        }
        public void OnDestroy()
        {
            this.Explode(StoredBlastPosition);
        }

        private Vector2 StoredBlastPosition;
        private HeatIndicatorController radialIndicator;

        public float maxRadius;
        public float radiusValue;


        public bool ignoreQueues;

        public float radiusIncreasePerSecond;
        public float tickDelay;
        public ExplosionData explosionData;
        private float timer;

        private Projectile projectile;

        private BasicBeamController basicBeamController;
        private PlayerController owner;
    }
}
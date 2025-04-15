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
    public class MeasuringTape : GunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Measuring Tape", "measuringtape");
            Game.Items.Rename("outdated_gun_mods:measuring_tape", "psog:measuring_tape");
            var behav = gun.gameObject.AddComponent<MeasuringTape>();            
            gun.SetShortDescription("The Long Way");
            gun.SetLongDescription("A hardy, basic measuring tape that's been left in a chest.\n\nIt's marked at the 2 inch mark.");
            gun.SetupSprite(null, "measuringtape_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);

            gun.isAudioLoop = true;

            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_WPN_blackhole_impact_01";
            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

            for (int i = 0; i < 1; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(180) as Gun, true, false);
            }
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {
                mod.ammoCost = 1;
                if (mod != gun.DefaultModule) { mod.ammoCost = 0; }
                mod.shootStyle = ProjectileModule.ShootStyle.Beam;
                mod.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                mod.cooldownTime = 0.01f;
                mod.numberOfShotsInClip = 100;
                mod.angleVariance = 0;

            List<string> BeamAnimPaths = new List<string>()
            { "Planetside/Resources/Beams/MeasuringTape/measuringtape_mid_001"};
            List<string> EndPaths = new List<string>()
            {"Planetside/Resources/Beams/MeasuringTape/measuringtape_end_001"};
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);
                BasicBeamController beamComp = projectile.GenerateBeamPrefab(
                    "Planetside/Resources/Beams/MeasuringTape/measuringtape_mid_001",
                    new Vector2(6, 2),
                    new Vector2(0, 2),
                    BeamAnimPaths,
                    1,
                    null,
                    -1,
                    null,
                    null,
                    EndPaths,
                    -1,
                    new Vector2(6, 2),
                    new Vector2(0, 2),
                    null,
                    -1,
                    null,
                    null
                    );

                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 30f;
                projectile.baseData.force = 2f;
                projectile.baseData.range *= 50;
                projectile.baseData.speed *= 0.66f;
                projectile.specRigidbody.CollideWithOthers = false;
                MeasuringTapeController controller = projectile.gameObject.AddComponent<MeasuringTapeController>();

                beamComp.boneType = BasicBeamController.BeamBoneType.Straight;

               beamComp.startAudioEvent = "Play_OBJ_hook_pull_01";
                beamComp.projectile.baseData.damage = 10;
                beamComp.penetration = 0;
                beamComp.reflections = 5;
                beamComp.IsReflectedBeam = false;

                mod.projectiles[0] = projectile;

            }

            //GUN STATS
            gun.doesScreenShake = false;
            gun.reloadTime = 1.2f;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.barrelOffset.transform.localPosition = new Vector3(0.5f, 0.25f, 0f);
            gun.SetBaseMaxAmmo(300);
            gun.PreventNormalFireAudio = true;
            gun.gunClass = GunClass.BEAM;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("MeasuringTape", "Planetside/Resources/GunClips/MeasuringTape/measurungTapefull", "Planetside/Resources/GunClips/MeasuringTape/measurungTapeempty");

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 0;

            gun.quality = PickupObject.ItemQuality.D;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            MeasuringTape.MeasuringTapeID = gun.PickupObjectId;

            List<string> yes = new List<string>
            {
                "psog:measuring_tape",
                "psog:rebar_puncher"
            };
            CustomSynergies.Add("Workplace Accident", yes, null, false);
            ItemIDs.AddToList(gun.PickupObjectId);
        }
        public static int MeasuringTapeID;

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsFiring == true)
            {
                gun.CeaseAttack(false);
            }
            if (gun.IsReloading && this.HasReloaded)
            {
                gun.CeaseAttack(false, null);
                gun.PreventNormalFireAudio = true;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;
                base.OnReloadPressed(player, gun, bSOMETHING);

            }
            
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
        }
        private bool HasReloaded;
        public bool HasFlipped;

        public override void Update()
        {
            gun.PreventNormalFireAudio = true;
        }
    }
    public class MeasuringTapeController : MonoBehaviour
    {
        private void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            this.basicBeamController = base.GetComponent<BasicBeamController>();
            /*
            if (this.projectile.Owner is PlayerController)
            {
                this.owner = (this.projectile.Owner as PlayerController);
                this.owner.PostProcessBeam += Owner_PostProcessBeam;
            }
            */
        }

        private void Owner_PostProcessBeam(BeamController obj)
        {
            if (obj is BasicBeamController)
            {
                BasicBeamController basicBeamController = obj as BasicBeamController;
                basicBeamController.DamageModifier = 1 + (basicBeamController.m_bones.Count());
            }
        }

        public void OnDestroy()
        {
            //if (owner)
            //{
                //this.owner.PostProcessBeam -= Owner_PostProcessBeam;
            //}
        }

        private Projectile projectile;
        private BasicBeamController basicBeamController;
        //private PlayerController owner;
    }
}

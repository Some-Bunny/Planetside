using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using Pathfinding;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using BreakAbleAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using SaveAPI;

namespace Planetside
{
	public class NemesisController : BraveBehaviour
    {

        public GunInventory GunInventorySelf;
        public AIShooter AIShooterSelf;
        public float GunSwitchTimer;
        private float elapsed;
        private float SpeedMultiplier;

        public void Start()
        {
            SpeedMultiplier = 1;
            IsPreDeath = false;
            elapsed = 0;
            GunSwitchTimer = UnityEngine.Random.Range(8, 10);
            AIShooterSelf = base.aiShooter;
            GunInventorySelf = AIShooterSelf.Inventory;
            if (GunInventorySelf != null)
            {
                GunInventorySelf.GunLocked.BaseValue = false;
            }
            HeldPrimaryPassive = PrimaryPassivesList[UnityEngine.Random.Range(0, PrimaryPassivesList.Count)];
            HeldSecondaryPassive = SecondaryPassivesList[UnityEngine.Random.Range(0, SecondaryPassivesList.Count)];
            PreferredWeapon = Weapons[UnityEngine.Random.Range(0, SecondaryPassivesList.Count)];
            HeldActive = ActiveList[UnityEngine.Random.Range(0, ActiveList.Count)];
            ProcessPrimaryFakeItems();
            ProcessSecondaryFakeItems();

            base.aiActor.healthHaver.OnPreDeath += OnPreDeathCleanup;
            PostProcessMovementOverride();
        }

        public bool HasBeenEngaged = false;

        public void DoEngagePostProcess()
        {
            if (HasBeenEngaged == true) { return; }
            HasBeenEngaged = true;
            switch (HeldSecondaryPassive)
            {
                case "bloodied_scarf":
                  
                    CustomScarfDoer scorf = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.ScarfObject.gameObject).AddComponent<CustomScarfDoer>();
                    scorf.AttachTarget = base.aiActor;
                    scorf.ScarfMaterial = StaticVFXStorage.ScarfObject.ScarfMaterial;
                    scorf.StartWidth = 0.0125f;
                    scorf.EndWidth = 0.175f;
                    scorf.AnimationSpeed = 30f;
                    scorf.ScarfLength = 0.625f;
                    scorf.AngleLerpSpeed = 60f;
                    scorf.BackwardZOffset = -0.2f;
                    scorf.CatchUpScale = 1.3f;
                    scorf.SinSpeed = 12f;
                    scorf.AmplitudeMod = 0.235f;
                    scorf.WavelengthMod = 1.3f;
                    scorf.ScarfMaterial.SetColor("_OverrideColor", new Color(1, 0, 0));
                    scorf.Initialize(base.aiActor);


                    return;
                case "guon":

                    GameObject transPos = base.aiActor.gameObject.transform.Find("SpecialGuonPos").gameObject;
                    if (transPos != null)
                    {
                        SpinBulletsController spinBulletsController = base.aiActor.gameObject.AddComponent<SpinBulletsController>();
                        spinBulletsController.ShootPoint = base.aiActor.transform.Find("SpecialGuonPos").gameObject;
                        spinBulletsController.OverrideBulletName = "NemesisGuon";
                        spinBulletsController.NumBullets = 2;
                        spinBulletsController.BulletMinRadius = 2.25f;
                        spinBulletsController.BulletMaxRadius = 2.5f;
                        spinBulletsController.BulletCircleSpeed = 60;
                        spinBulletsController.BulletsIgnoreTiles = true;
                        spinBulletsController.RegenTimer = 2;
                        spinBulletsController.AmountOFLines = 3;
                    }
                    return;                
            }
        }


        private void OnPreDeathCleanup(Vector2 obj)
        {
            IsPreDeath = true;
            for (int i = 0; i < activeLines.Count; i++)
            {
                if (activeLines[i].gameObject != null) { Destroy(activeLines[i].gameObject); }
            }
            GunInventorySelf.DestroyAllGuns();

        }
     

        private bool IsPreDeath;
        public void ProcessPrimaryFakeItems()
        {
            switch (HeldPrimaryPassive)
            {
                case "cloranthy":
                    foreach (OverrideBehaviorBase overrideBehavior in base.aiActor.behaviorSpeculator.OverrideBehaviors)
                    {
                        if (overrideBehavior is CustomDodgeRollBehavior dodge)
                        {
                            dodge.Cooldown *= 0.5f;
                        }
                        if (overrideBehavior is SansTeleportBehavior sans)
                        {
                            sans.Cooldown *= 0.5f;
                        }
                    }
                    return;
                case "bionic_leg":
                    base.aiActor.MovementSpeed *= 1.2f;
                    SpeedMultiplier *= 1.2f;
                    return;
                case "trigger_finger":
                    base.aiActor.behaviorSpeculator.CooldownScale *= 0.75f;
                    return;
                case "ice_cube":
                    return;
            }            
        }


        public void PostProcessMovementOverride()
        {
            bool Fuck = false;
            foreach (OverrideBehaviorBase overrideBehavior in base.aiActor.behaviorSpeculator.OverrideBehaviors)
            {
                if (overrideBehavior is ModifySpeedBehavior dodge)
                {
                    Fuck = true;
                    dodge.maxSpeed *= SpeedMultiplier;
                    dodge.minSpeed *= SpeedMultiplier;

                }
            }
            if (Fuck == true)
            {
                base.aiActor.behaviorSpeculator.OverrideBehaviors.Add(
                    new ModifySpeedBehavior()
                    {
                        minSpeed = (1.2f * SpeedMultiplier),
                        minSpeedDistance = 4,
                        maxSpeed = (3.2f * SpeedMultiplier),
                        maxSpeedDistance = 9
                    });
            }
        }
        public void ProcessSecondaryFakeItems()
        {
            switch (HeldSecondaryPassive)
            {
                case "bloodied_scarf":
                    var l = base.aiActor.behaviorSpeculator.OverrideBehaviors;
                    foreach (var b in l)
                    {
                        if (b is SansTeleportBehavior a) 
                        {a.Enabled = true; a.dodgeChance = 0.5f; }
                        if (b is CustomDodgeRollBehavior c)
                        { c.Enabled = false;}
                    }


                    return;
                case "guon":

                    base.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.NemesisGuon);
                    EnemyToolbox.GenerateShootPoint(base.aiActor.gameObject, base.aiActor.sprite.WorldCenter, "SpecialGuonPos");
                  
                    return;
                case "sweet":
                    base.aiActor.behaviorSpeculator.CooldownScale *= 0.85f;
                    base.aiActor.MovementSpeed *= 1.10f;
                    SpeedMultiplier *= 1.10f;

                    base.aiActor.healthHaver.SetHealthMaximum(base.aiActor.healthHaver.GetMaxHealth() * 1.15f);
                    base.aiActor.healthHaver.FullHeal();
                    return;
            }
        }

      


        public void Update()
        {
            elapsed += BraveTime.DeltaTime;
            if (elapsed > GunSwitchTimer)
            {
                if (BoolCanSwitch() == true)
                {
                    if (UnityEngine.Random.value < 0.3f && CurrentWeapon != PreferredWeapon)
                    {
                        switch (PreferredWeapon)
                        {
                            case "Railgun":
                                ProcessMovementBehavior(10, 8, 13, true);
                                GunInventorySelf.AddGunToInventory(PickupObjectDatabase.GetById(NemesisRailgun.NemesisRailgunID) as Gun, true);
                                ResetCachedGuneness(PickupObjectDatabase.GetById(NemesisRailgun.NemesisRailgunID) as Gun, "Railgun");
                                return;
                            case "Revolver":
                                ProcessMovementBehavior(6, 4, 9, false);
                                GunInventorySelf.AddGunToInventory(PickupObjectDatabase.GetById(NemesisGun.NemesisGunID) as Gun, true);
                                ResetCachedGuneness(PickupObjectDatabase.GetById(NemesisGun.NemesisGunID) as Gun, "Revolver");
                                return;
                            case "Shotgun":
                                ProcessMovementBehavior(3.5f, 2, 5, false);
                                GunInventorySelf.AddGunToInventory(PickupObjectDatabase.GetById(NemesisShotgun.NemesisGunID) as Gun, true);
                                ResetCachedGuneness(PickupObjectDatabase.GetById(NemesisShotgun.NemesisGunID) as Gun, "Shotgun");
                                return;
                            default:
                                ProcessMovementBehavior(6, 4, 9, false);
                                GunInventorySelf.AddGunToInventory(PickupObjectDatabase.GetById(NemesisGun.NemesisGunID) as Gun, true);
                                ResetCachedGuneness(PickupObjectDatabase.GetById(NemesisGun.NemesisGunID) as Gun, "Revolver");
                                return;
                        }
                    }
                    else
                    {
                        if (Vector2.Distance(base.aiActor.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.sprite.WorldCenter) < 6 && GunDesiredToSwitchToIsntCurrentGun(NemesisShotgun.NemesisGunID) == true)
                        {
                            ProcessMovementBehavior(3.5f, 2, 5, false);

                            GunInventorySelf.AddGunToInventory(PickupObjectDatabase.GetById(NemesisShotgun.NemesisGunID) as Gun, true);
                            ResetCachedGuneness(PickupObjectDatabase.GetById(NemesisShotgun.NemesisGunID) as Gun, "Shotgun");
                        }
                        else if (Vector2.Distance(base.aiActor.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.sprite.WorldCenter) > 9.5f && GunDesiredToSwitchToIsntCurrentGun(NemesisRailgun.NemesisRailgunID) == true)
                        {
                            ProcessMovementBehavior(10, 8, 13, true);
                            GunInventorySelf.AddGunToInventory(PickupObjectDatabase.GetById(NemesisRailgun.NemesisRailgunID) as Gun, true);
                            ResetCachedGuneness(PickupObjectDatabase.GetById(NemesisRailgun.NemesisRailgunID) as Gun, "Railgun");
                        }
                        else if (AIShooterSelf.equippedGunId != NemesisGun.NemesisGunID && GunDesiredToSwitchToIsntCurrentGun(NemesisGun.NemesisGunID) == true)
                        {
                            ProcessMovementBehavior(6, 4, 9, false);
                            GunInventorySelf.AddGunToInventory(PickupObjectDatabase.GetById(NemesisGun.NemesisGunID) as Gun, true);
                            ResetCachedGuneness(PickupObjectDatabase.GetById(NemesisGun.NemesisGunID) as Gun, "Revolver");
                        }
                    }

                    
                }     
            }
        }

        public void ProcessMovementBehavior(float customRange, float minActiveRange, float maxactiveRange, bool StopInRange)
        {
            foreach (MovementBehaviorBase moveBehav in base.aiActor.behaviorSpeculator.MovementBehaviors)
            {
                if (moveBehav is SeekTargetBehavior move)
                {
                    move.CustomRange = customRange;
                    move.MinActiveRange = minActiveRange;
                    move.MaxActiveRange = maxactiveRange;
                    move.StopWhenInRange = StopInRange;
                }
            }
        }

        public bool BoolCanSwitch()
        {
            if (IsPreDeath == true) { return false; }
            for (int e = 0; e < this.aiActor.behaviorSpeculator.AttackBehaviors.Count; e++)
            {
                if (this.aiActor.behaviorSpeculator.AttackCooldown <= 0.06f) { return false; }
            }
            return true;
        }
        public bool GunDesiredToSwitchToIsntCurrentGun(int desiredGunID)
        {
            bool can = AIShooterSelf.equippedGunId != desiredGunID;
            return can;
        }

        public void ResetCachedGuneness(Gun gun, string Name)
        {
            elapsed = 0;
            CurrentWeapon = Name;
            GunSwitchTimer = UnityEngine.Random.Range(5, 8);
            if (Name == PreferredWeapon) { elapsed -= 6; }
            for (int j = 0; j < base.aiActor.behaviorSpeculator.AttackBehaviors.Count; j++)
            {
                if (base.behaviorSpeculator.AttackBehaviors[j] is AttackBehaviorGroup && base.behaviorSpeculator.AttackBehaviors[j] != null)
                {
                    for (int i = 0; i < (base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup).AttackBehaviors.Count; i++)
                    {
                        AttackBehaviorGroup.AttackGroupItem attackGroupItem = (base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup).AttackBehaviors[i];
                        if ((base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup) != null && attackGroupItem.NickName.Contains(Name))
                        {
                            attackGroupItem.Probability = 1f;
                        }
                        else
                        {
                            attackGroupItem.Probability = 0f;
                        }
                    }
                }
            }


            AIShooterSelf.equippedGunId = gun.PickupObjectId;

            FieldInfo leEnabler = typeof(AIShooter).GetField("m_hasCachedGun", BindingFlags.Instance | BindingFlags.NonPublic);
            leEnabler.SetValue(base.aiShooter, false);

            GunInventorySelf.DestroyAllGuns();

            Gun equippedGun = GunInventorySelf.AddGunToInventory(gun, true);

            FieldInfo fieldInf = typeof(AIShooter).GetField("m_cachedGun", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInf.SetValue(base.aiShooter, equippedGun);


            FieldInfo m_currentGunField = typeof(GunInventory).GetField("m_currentGun", BindingFlags.Instance | BindingFlags.NonPublic);
            m_currentGunField.SetValue(GunInventorySelf, equippedGun);
            equippedGun.gameObject.SetActive(true);
            base.aiActor.StartCoroutine(PretendSwitch());
        }
        private IEnumerator PretendSwitch()
        {
            GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.BlueSynergyPoofVFX, false);
            gameObject.transform.position = base.aiActor.transform.Find("GunAttachPoint").gameObject.transform.position;
            gameObject.transform.localScale *= 1.5f;
            Destroy(gameObject, 2);

            GameObject gameObjectTwo = SpawnManager.SpawnVFX(StaticVFXStorage.MachoBraceDustupVFX, false);
            gameObjectTwo.transform.position = base.aiActor.transform.position - new Vector3(1.25f, 1.25f);
            Destroy(gameObjectTwo, 2);
            AkSoundEngine.PostEvent("Play_OBJ_weapon_pickup_01", base.aiActor.gameObject);
            elapsed = -0.5f;
            base.aiActor.behaviorSpeculator.Stun(0.5f, false);
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

        public List<GameObject> activeLines = new List<GameObject>();

        public string HeldPrimaryPassive;
        public string HeldSecondaryPassive;
        public string HeldActive;
        public string PreferredWeapon;
        public string CurrentWeapon = "Revolver";

        public List<string> Weapons = new List<string>()
        {
            "Shotgun",
            "Railgun",
            "Revolver",
        };

        public List<string> PrimaryPassivesList = new List<string>()
        {
            "cloranthy",
            "bionic_leg",
            "trigger_finger",
            "ice_cube"
        };

        public List<string> SecondaryPassivesList = new List<string>()
        {
            "bloodied_scarf",
            "guon",
            "sweet",
        };

        public List<string> ActiveList = new List<string>()
        {
            "gun_friendship",
            "fortunes_favor",
            "cluster",
            "blast_shower"
        };
    }
}

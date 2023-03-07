using System;
using System.Reflection;
using UnityEngine;

namespace ItemAPI
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    public class AdvancedGunBehaviourMultiActive : MonoBehaviour, IGunInheritable, ILevelLoadedListener
    {
        /// <summary>
        /// Update() is called every tick when the gun is the player's current gun or is dropped.
        /// </summary>
        public virtual void Update()
        {
            if (this.Player != null)
            {
                if (!this.everPickedUpByPlayer)
                {
                    this.everPickedUpByPlayer = true;
                }
            }
            if (this.Owner != null)
            {
                if (!this.everPickedUp)
                {
                    this.everPickedUp = true;
                }
            }
            if (this.lastOwner != this.Owner)
            {
                this.lastOwner = this.Owner;
            }
            if (this.Owner != null && !this.pickedUpLast)
            {
                this.OnPickup(this.Owner);
                this.pickedUpLast = true;
            }
            if (this.Owner == null && this.pickedUpLast)
            {
                if (this.lastOwner != null)
                {
                    this.OnPostDrop(this.lastOwner);
                    this.lastOwner = null;
                }
                this.pickedUpLast = false;
            }
            if (this.gun != null && !this.gun.IsReloading && !this.hasReloaded)
            {
                this.hasReloaded = true;
                if (this.Owner != null)
                {
                    this.OnReloadEnded(this.Owner as PlayerController, this.gun);
                }
            }
            this.gun.PreventNormalFireAudio = this.preventNormalFireAudio;
            this.gun.OverrideNormalFireAudioEvent = this.overrideNormalFireAudio;
        }

        /// <summary>
        /// Inherits data from another gun. Inherit the variables you want to be saved here!
        /// </summary>
        /// <param name="source">The source gun.</param>
        public virtual void InheritData(Gun source)
        {
            AdvancedGunBehaviourMultiActive component = source.GetComponent<AdvancedGunBehaviourMultiActive>();
            if (component != null)
            {
                this.preventNormalFireAudio = component.preventNormalFireAudio;
                this.preventNormalReloadAudio = component.preventNormalReloadAudio;
                this.overrideNormalReloadAudio = component.overrideNormalReloadAudio;
                this.overrideNormalFireAudio = component.overrideNormalFireAudio;
                this.everPickedUpByPlayer = component.everPickedUpByPlayer;
                this.everPickedUp = component.everPickedUp;
                this.usesOverrideHeroSwordCooldown = component.usesOverrideHeroSwordCooldown;
                this.overrideHeroSwordCooldown = component.overrideHeroSwordCooldown;
                this.overrideReloadSwitchGroup = component.overrideReloadSwitchGroup;
            }
        }

        /// <summary>
        /// Saves the data of the gun to a list. Save the variables you want to be saved here!
        /// </summary>
        /// <param name="data">The list.</param>
        /// <param name="dataIndex">DataIndex. You don't need to use this argument.</param>
        public virtual void MidGameSerialize(List<object> data, int dataIndex)
        {
            data.Add(this.preventNormalFireAudio);
            data.Add(this.preventNormalReloadAudio);
            data.Add(this.overrideNormalReloadAudio);
            data.Add(this.overrideNormalFireAudio);
            data.Add(this.everPickedUpByPlayer);
            data.Add(this.everPickedUp);
            data.Add(this.usesOverrideHeroSwordCooldown);
            data.Add(this.overrideHeroSwordCooldown);
            data.Add(this.overrideReloadSwitchGroup);
        }

        /// <summary>
        /// Sets the data of the gun to the contents of a list. Set the variables you want to be saved here!
        /// </summary>
        /// <param name="data">The list.</param>
        /// <param name="dataIndex">DataIndex. Add a number equal to the amount of your data to it.</param>
        public virtual void MidGameDeserialize(List<object> data, ref int dataIndex)
        {
            this.preventNormalFireAudio = (bool)data[dataIndex];
            this.preventNormalReloadAudio = (bool)data[dataIndex + 1];
            this.overrideNormalReloadAudio = (string)data[dataIndex + 2];
            this.overrideNormalFireAudio = (string)data[dataIndex + 3];
            this.everPickedUpByPlayer = (bool)data[dataIndex + 4];
            this.everPickedUp = (bool)data[dataIndex + 5];
            this.usesOverrideHeroSwordCooldown = (bool)data[dataIndex + 6];
            this.overrideHeroSwordCooldown = (float)data[dataIndex + 7];
            this.overrideReloadSwitchGroup = (string)data[dataIndex + 8];
            dataIndex += 9;
        }

        /// <summary>
        /// Start() is called when the gun is created. It's also called when the player picks up or drops the gun.
        /// </summary>
        public virtual void Start()
        {
            this.gun = base.GetComponent<Gun>();
            this.gun.OnInitializedWithOwner += this.OnInitializedWithOwner;
            if (this.gun.CurrentOwner != null)
            {
                this.OnInitializedWithOwner(this.gun.CurrentOwner);
            }
            this.gun.PostProcessProjectile += this.PostProcessProjectile;
            this.gun.PostProcessVolley += this.PostProcessVolley;
            this.gun.OnDropped += this.OnDropped;
            this.gun.OnAutoReload += this.OnAutoReload;
            this.gun.OnReloadPressed += this.OnReloadPressed;
            this.gun.OnFinishAttack += this.OnFinishAttack;
            this.gun.OnPostFired += this.OnPostFired;
            this.gun.OnAmmoChanged += this.OnAmmoChanged;
            this.gun.OnBurstContinued += this.OnBurstContinued;
            this.gun.OnPreFireProjectileModifier += this.OnPreFireProjectileModifier;
            base.StartCoroutine(this.UpdateCR());
        }

        public virtual void BraveOnLevelWasLoaded()
        {
        }

        private IEnumerator UpdateCR()
        {
            while (true)
            {
                this.NonCurrentGunUpdate();
                yield return null;
            }
        }

        /// <summary>
        /// NonCurrentGunUpdate() is called every tick EVEN IF THE GUN ISN'T ENABLED. That means it's able to run even if the player's current gun isn't this behaviour's gun.
        /// </summary>
        public virtual void NonCurrentGunUpdate()
        {
        }

        /// <summary>
        /// OnInitializedWithOwner() is called when a GunInventory creates a gun to add (for example when the player picks the gun up.) 
        /// </summary>
        /// <param name="actor">The gun's owner.</param>
        public virtual void OnInitializedWithOwner(GameActor actor)
        {
        }

        /// <summary>
        /// PostProcessProjectile() is called right after the gun shoots a projectile. If you want to change properties of a projectile in runtime, this is the place to do it.
        /// </summary>
        /// <param name="projectile">The target projectile.</param>
        public virtual void PostProcessProjectile(Projectile projectile)
        {
        }

        /// <summary>
        /// PostProcessVolley() is called when PlayerStats rebuilds a gun's volley. It's used by things like VolleyModificationSynergyProcessor to change the gun's volley if the player has a synergy.
        /// </summary>
        /// <param name="volley">The target volley.</param>
        public virtual void PostProcessVolley(ProjectileVolleyData volley)
        {
        }

        /// <summary>
        /// OnDropped() is called when an a player drops the gun. gun.CurrentOwner is set to null before this method is even called, so I wouldn't reccomend using it.
        /// </summary>
        public virtual void OnDropped()
        {
        }

        /// <summary>
        /// OnAutoReload() is called when a player reloads the gun with an empty clip.
        /// </summary>
        /// <param name="player">The player that reloaded the gun. Will be null if the gun's owner isn't a player.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnAutoReload(PlayerController player, Gun gun)
        {
            if (player != null)
            {
                this.OnAutoReloadSafe(player, gun);
            }
        }

        /// <summary>
        /// OnAutoReloadSafe() is called when a player reloads the gun with an empty clip and the gun's owner is a player.
        /// </summary>
        /// <param name="player">The player that reloaded the gun. Can't be null.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnAutoReloadSafe(PlayerController player, Gun gun)
        {
        }

        /// <summary>
        /// OnReloadPressed() is called when the owner reloads the gun or the player presses the reload key.
        /// </summary>
        /// <param name="player">The player that reloaded the gun/pressed the reload key. Will be null if the gun's owner isn't a player.</param>
        /// <param name="gun">The gun.</param>
        /// <param name="manualReload">True if the owner reloaded the gun by pressing the reload key. False if the owner reloaded the gun by firing with an empty clip.</param>
        public virtual void OnReloadPressed(PlayerController player, Gun gun, bool manualReload)
        {
            if (this.hasReloaded && gun.IsReloading)
            {
                this.OnReload(player, gun);
                this.hasReloaded = false;
            }
            if (player != null)
            {
                this.OnReloadPressedSafe(player, gun, manualReload);
            }
        }

        /// <summary>
        /// OnGunsChanged() is called when the player changes the current gun.
        /// </summary>
        /// <param name="previous">The previous current gun.</param>
        /// <param name="current">The new current gun.</param>
        /// <param name="newGun">True if the gun was changed because player picked up a new gun.</param>
        public virtual void OnGunsChanged(Gun previous, Gun current, bool newGun)
        {
            if (previous != this.gun && current == this.gun)
            {
                this.OnSwitchedToThisGun();
            }
            if (previous == this.gun && current != this.gun)
            {
                this.OnSwitchedAwayFromThisGun();
            }
        }

        /// <summary>
        /// OnSwitchedToThisGun() when the player switches to this behaviour's affected gun.
        /// </summary>
        public virtual void OnSwitchedToThisGun()
        {
        }

        /// <summary>
        /// OnSwitchedToThisGun() when the player switches away from this behaviour's affected gun.
        /// </summary>
        public virtual void OnSwitchedAwayFromThisGun()
        {
        }

        /// <summary>
        /// OnReloadPressedSafe() is called when the owner reloads the gun or the player presses the reload key and the gun's owner is a player.
        /// </summary>
        /// <param name="player">The player that reloaded the gun/pressed the reload key. Can't be null.</param>
        /// <param name="gun">The gun.</param>
        /// <param name="manualReload">True if the owner reloaded the gun by pressing the reload key. False if the owner reloaded the gun by firing with an empty clip.</param>
        public virtual void OnReloadPressedSafe(PlayerController player, Gun gun, bool manualReload)
        {
            if (this.hasReloaded && gun.IsReloading)
            {
                this.OnReloadSafe(player, gun);
                this.hasReloaded = false;
            }
        }

        /// <summary>
        /// OnReload() is called when the gun is reloaded.
        /// </summary>
        /// <param name="player">The player that reloaded the gun. Will be null if the gun's owner isn't a player.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnReload(PlayerController player, Gun gun)
        {
            if (this.preventNormalReloadAudio)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                if (!string.IsNullOrEmpty(this.overrideNormalReloadAudio))
                {
                    AkSoundEngine.PostEvent(this.overrideNormalReloadAudio, base.gameObject);
                }
            }
        }

        /// <summary>
        /// OnReloadEnded() is called at the end of reload.
        /// </summary>
        /// <param name="player">The player that reloaded the gun. Will be null if the gun's owner isn't a player.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnReloadEnded(PlayerController player, Gun gun)
        {
            if (player != null)
            {
                this.OnReloadEndedSafe(player, gun);
            }
        }

        /// <summary>
        /// OnReloadEndedSafe() is called at the end of reload and if the gun's owner is a player.
        /// </summary>
        /// <param name="player">The player that reloaded the gun. Can't be null.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnReloadEndedSafe(PlayerController player, Gun gun)
        {

        }

        /// <summary>
        /// OnReloadSafe() is called when the gun is reloaded and the gun's owner is a player.
        /// </summary>
        /// <param name="player">The player that reloaded the gun. Can't be null.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnReloadSafe(PlayerController player, Gun gun)
        {
        }

        /// <summary>
        /// OnFinishAttack() is called when the gun finishes firing, for example when the player releases the Shoot key or the gun's clip empties and if the owner is a player.
        /// </summary>
        /// <param name="player">The player. Can't be null.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnFinishAttack(PlayerController player, Gun gun)
        {
        }

        /// <summary>
        /// OnPostFired() is called after the gun fired and if the owner is a player.
        /// </summary>
        /// <param name="player">The player. Can't be null.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnPostFired(PlayerController player, Gun gun)
        {
            if (gun.IsHeroSword)
            {
                if (this.HeroSwordCooldown == 0.5f)
                {
                    this.OnHeroSwordCooldownStarted(player, gun);
                }
            }
        }

        /// <summary>
        /// OnHeroSwordCooldownStarted() when the gun's Sword Slash started and if the gun is a HeroSword (if gun.IsHeroSword = true).
        /// </summary>
        /// <param name="player"></param>
        /// <param name="gun"></param>
        public virtual void OnHeroSwordCooldownStarted(PlayerController player, Gun gun)
        {
            if (this.usesOverrideHeroSwordCooldown)
            {
                this.HeroSwordCooldown = this.overrideHeroSwordCooldown;
            }
        }

        /// <summary>
        /// OnAmmoChanged() is called when the gun's ammo amount increases/decreases.
        /// </summary>
        /// <param name="player">The player. Will be null if the gun's owner isn't a player.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnAmmoChanged(PlayerController player, Gun gun)
        {
            if (player != null)
            {
                this.OnAmmoChangedSafe(player, gun);
            }
        }

        /// <summary>
        /// OnAmmoChangedSafe() is called when the gun's ammo amount increases/decreases and if the gun's owner is a player.
        /// </summary>
        /// <param name="player">The player. Can't be null.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnAmmoChangedSafe(PlayerController player, Gun gun)
        {
        }

        /// <summary>
        /// OnBurstContinued() is called when the gun continues a burst (attacks while bursting).
        /// </summary>
        /// <param name="player">The player. Will be null if the gun's owner isn't a player.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnBurstContinued(PlayerController player, Gun gun)
        {
            if (player != null)
            {
                this.OnBurstContinuedSafe(player, gun);
            }
        }

        /// <summary>
        /// OnBurstContinuedSafe() is called when the gun continues a burst (attacks while bursting) and if the gun's owner is a player.
        /// </summary>
        /// <param name="player">The player. Can't be null.</param>
        /// <param name="gun">The gun.</param>
        public virtual void OnBurstContinuedSafe(PlayerController player, Gun gun)
        {
        }

        /// <summary>
        /// OnPreFireProjectileModifier() is called before the gun shoots a projectile. If the method returns something that's not the projectile argument, the projectile the gun will shoot will be replaced with the returned projectile.
        /// </summary>
        /// <param name="gun">The gun.</param>
        /// <param name="projectile">Original projectile.</param>
        /// <param name="mod">Target ProjectileModule.</param>
        /// <returns>The replacement projectile.</returns>
        public virtual Projectile OnPreFireProjectileModifier(Gun gun, Projectile projectile, ProjectileModule mod)
        {
            return projectile;
        }

        public AdvancedGunBehaviourMultiActive()
        {
        }

        /// <summary>
        /// OnPickup() is called when an actor picks the gun up.
        /// </summary>
        /// <param name="owner">The actor that picked up the gun.</param>
        public virtual void OnPickup(GameActor owner)
        {
            if (owner is PlayerController)
            {
                this.OnPickedUpByPlayer(owner as PlayerController);
            }
            if (owner is AIActor)
            {
                this.OnPickedUpByEnemy(owner as AIActor);
            }
        }

        /// <summary>
        /// OnPostDrop() is called AFTER the owner drops the gun.
        /// </summary>
        /// <param name="owner">The actor that dropped the gun.</param>
        public virtual void OnPostDrop(GameActor owner)
        {
            if (owner is PlayerController)
            {
                this.OnPostDroppedByPlayer(owner as PlayerController);
            }
            if (owner is AIActor)
            {
                this.OnPostDroppedByEnemy(owner as AIActor);
            }
        }

        /// <summary>
        /// OnPickup() is called when a player picks the gun up.
        /// </summary>
        /// <param name="player">The player that picked up the gun.</param>
        public virtual void OnPickedUpByPlayer(PlayerController player)
        {
            player.GunChanged += this.OnGunsChanged;
        }

        /// <summary>
        /// OnPostDrop() is called AFTER the player drops the gun. If you modify player's stats here, don't forget to call player.stats.RecalculateStats()!
        /// </summary>
        /// <param name="player">The player that dropped the gun.</param>
        public virtual void OnPostDroppedByPlayer(PlayerController player)
        {
        }

        /// <summary>
        /// OnPickup() is called when an enemy picks the gun up.
        /// </summary>
        /// <param name="enemy">The enemy that picked up the gun.</param>
        public virtual void OnPickedUpByEnemy(AIActor enemy)
        {
        }

        /// <summary>
        /// OnPostDrop() is called AFTER the enemy drops the gun.
        /// </summary>
        /// <param name="enemy">The enemy that dropped the gun.</param>
        public virtual void OnPostDroppedByEnemy(AIActor enemy)
        {
        }

        /// <summary>
        /// Returns true if the gun's current owner isn't null.
        /// </summary>
        public bool PickedUp
        {
            get
            {
                return this.gun.CurrentOwner != null;
            }
        }

        /// <summary>
        /// If the gun's owner is a player, returns the gun's current owner as a player.
        /// </summary>
        public PlayerController Player
        {
            get
            {
                if (this.gun.CurrentOwner is PlayerController)
                {
                    return this.gun.CurrentOwner as PlayerController;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the HeroSwordCooldown of the gun if it isn't null. If it's null, returns -1.
        /// </summary>
        public float HeroSwordCooldown
        {
            get
            {
                if (this.gun != null)
                {
                    return (float)heroSwordCooldown.GetValue(this.gun);
                }
                return -1f;
            }
            set
            {
                if (this.gun != null)
                {
                    heroSwordCooldown.SetValue(this.gun, value);
                }
            }
        }

        /// <summary>
        /// Returns the gun's current owner.
        /// </summary>
        public GameActor Owner
        {
            get
            {
                return this.gun.CurrentOwner;
            }
        }

        /// <summary>
        /// Returns true if the gun's owner isn't null and is a player.
        /// </summary>
        public bool PickedUpByPlayer
        {
            get
            {
                return this.Player != null;
            }
        }

        private bool pickedUpLast = false;
        private GameActor lastOwner = null;
        /// <summary>
        /// Returns true if the gun was ever picked up by a player.
        /// </summary>
        public bool everPickedUpByPlayer = false;
        /// <summary>
        /// Returns true if the gun was ever picked up.
        /// </summary>
        public bool everPickedUp = false;
        /// <summary>
        /// Returns the gun this behaviour is applied to.
        /// </summary>
        public Gun gun;
        private bool hasReloaded = true;
        /// <summary>
        /// If true, prevents the gun's normal fire audio.
        /// </summary>
        public bool preventNormalFireAudio;
        /// <summary>
        /// If true, prevents the gun's normal reload audio.
        /// </summary>
        public bool preventNormalReloadAudio;
        /// <summary>
        /// The gun's override fire audio. Only works if preventNormalFireAudio is true.
        /// </summary>
        public string overrideNormalFireAudio;
        public string overrideReloadSwitchGroup;
        /// <summary>
        /// The gun's override reload audio. Only works if preventNormalReloadAudio is true.
        /// </summary>
        public string overrideNormalReloadAudio;
        public bool usesOverrideHeroSwordCooldown;
        public float overrideHeroSwordCooldown;
        private static FieldInfo heroSwordCooldown = typeof(Gun).GetField("HeroSwordCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    //I NEED BOTH WTFF????

    public class AdvancedGunBehavior : MonoBehaviour
    {
        public virtual void Update()
        {
            bool flag = this.Player != null;
            if (flag)
            {
                this.lastPlayer = this.Player;
                bool flag2 = !this.everPickedUpByPlayer;
                if (flag2)
                {
                    this.everPickedUpByPlayer = true;
                }
            }
            bool flag3 = this.Player != null && !this.pickedUpLast;
            if (flag3)
            {
                this.OnPickup(this.Player);
                this.pickedUpLast = true;
            }
            bool flag4 = this.Player == null && this.pickedUpLast;
            if (flag4)
            {
                bool flag5 = this.lastPlayer != null;
                if (flag5)
                {
                    this.OnPostDrop(this.lastPlayer);
                    this.lastPlayer = null;
                }
                this.pickedUpLast = false;
            }
            bool flag6 = this.gun != null && !this.gun.IsReloading && !this.hasReloaded;
            if (flag6)
            {
                this.hasReloaded = true;
            }
            this.gun.PreventNormalFireAudio = this.preventNormalFireAudio;
            this.gun.OverrideNormalFireAudioEvent = this.overrrideNormalFireAudio;
        }

        // Token: 0x060000B5 RID: 181 RVA: 0x000083DC File Offset: 0x000065DC
        public virtual void Start()
        {
            this.gun = base.GetComponent<Gun>();
            Gun gun = this.gun;
            gun.OnInitializedWithOwner = (Action<GameActor>)Delegate.Combine(gun.OnInitializedWithOwner, new Action<GameActor>(this.OnInitializedWithOwner));
            Gun gun2 = this.gun;
            gun2.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(gun2.PostProcessProjectile, new Action<Projectile>(this.PostProcessProjectile));
            Gun gun3 = this.gun;
            gun3.PostProcessVolley = (Action<ProjectileVolleyData>)Delegate.Combine(gun3.PostProcessVolley, new Action<ProjectileVolleyData>(this.PostProcessVolley));
            Gun gun4 = this.gun;
            gun4.OnDropped = (Action)Delegate.Combine(gun4.OnDropped, new Action(this.OnDropped));
            Gun gun5 = this.gun;
            gun5.OnAutoReload = (Action<PlayerController, Gun>)Delegate.Combine(gun5.OnAutoReload, new Action<PlayerController, Gun>(this.OnAutoReload));
            Gun gun6 = this.gun;
            gun6.OnReloadPressed = (Action<PlayerController, Gun, bool>)Delegate.Combine(gun6.OnReloadPressed, new Action<PlayerController, Gun, bool>(this.OnReloadPressed));
            Gun gun7 = this.gun;
            gun7.OnFinishAttack = (Action<PlayerController, Gun>)Delegate.Combine(gun7.OnFinishAttack, new Action<PlayerController, Gun>(this.OnFinishAttack));
            Gun gun8 = this.gun;
            gun8.OnPostFired = (Action<PlayerController, Gun>)Delegate.Combine(gun8.OnPostFired, new Action<PlayerController, Gun>(this.OnPostFired));
            Gun gun9 = this.gun;
            gun9.OnAmmoChanged = (Action<PlayerController, Gun>)Delegate.Combine(gun9.OnAmmoChanged, new Action<PlayerController, Gun>(this.OnAmmoChanged));
            Gun gun10 = this.gun;
            gun10.OnBurstContinued = (Action<PlayerController, Gun>)Delegate.Combine(gun10.OnBurstContinued, new Action<PlayerController, Gun>(this.OnBurstContinued));
            Gun gun11 = this.gun;
            gun11.OnPreFireProjectileModifier = (Func<Gun, Projectile, ProjectileModule, Projectile>)Delegate.Combine(gun11.OnPreFireProjectileModifier, new Func<Gun, Projectile, ProjectileModule, Projectile>(this.OnPreFireProjectileModifier));
            Gun gun12 = this.gun;
        }

        // Token: 0x060000B6 RID: 182 RVA: 0x000085AE File Offset: 0x000067AE
        public virtual void OnInitializedWithOwner(GameActor actor)
        {
        }

        // Token: 0x060000B7 RID: 183 RVA: 0x000085B1 File Offset: 0x000067B1
        public virtual void PostProcessProjectile(Projectile projectile)
        {
        }
        // Token: 0x060000B8 RID: 184 RVA: 0x000085B4 File Offset: 0x000067B4
        public virtual void PostProcessVolley(ProjectileVolleyData volley)
        {
        }

        // Token: 0x060000B9 RID: 185 RVA: 0x000085B7 File Offset: 0x000067B7
        public virtual void OnDropped()
        {
        }

        // Token: 0x060000BA RID: 186 RVA: 0x000085BA File Offset: 0x000067BA
        public virtual void OnAutoReload(PlayerController player, Gun gun)
        {
        }

        // Token: 0x060000BB RID: 187 RVA: 0x000085C0 File Offset: 0x000067C0
        public virtual void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            bool flag = this.hasReloaded;
            if (flag)
            {
                this.OnReload(player, gun);
                this.hasReloaded = false;
            }
        }

        // Token: 0x060000BC RID: 188 RVA: 0x000085EC File Offset: 0x000067EC
        public virtual void OnReload(PlayerController player, Gun gun)
        {
            bool flag = this.preventNormalReloadAudio;
            if (flag)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                bool flag2 = !string.IsNullOrEmpty(this.overrideNormalReloadAudio);
                if (flag2)
                {
                    AkSoundEngine.PostEvent(this.overrideNormalReloadAudio, base.gameObject);
                }
            }
        }

        // Token: 0x060000BD RID: 189 RVA: 0x0000863D File Offset: 0x0000683D
        public virtual void OnFinishAttack(PlayerController player, Gun gun)
        {
        }

        // Token: 0x060000BE RID: 190 RVA: 0x00008640 File Offset: 0x00006840
        public virtual void OnPostFired(PlayerController player, Gun gun)
        {
            bool isHeroSword = gun.IsHeroSword;
            if (isHeroSword)
            {
                bool flag = (float)AdvancedGunBehavior.heroSwordCooldown.GetValue(gun) == 0.5f;
                if (flag)
                {
                    this.OnHeroSwordCooldownStarted(player, gun);
                }
            }
        }

        // Token: 0x060000BF RID: 191 RVA: 0x00008680 File Offset: 0x00006880
        public virtual void OnHeroSwordCooldownStarted(PlayerController player, Gun gun)
        {
        }

        // Token: 0x060000C0 RID: 192 RVA: 0x00008683 File Offset: 0x00006883
        public virtual void OnAmmoChanged(PlayerController player, Gun gun)
        {
        }

        // Token: 0x060000C1 RID: 193 RVA: 0x00008686 File Offset: 0x00006886
        public virtual void OnBurstContinued(PlayerController player, Gun gun)
        {
        }

        // Token: 0x060000C2 RID: 194 RVA: 0x0000868C File Offset: 0x0000688C
        public virtual Projectile OnPreFireProjectileModifier(Gun gun, Projectile projectile, ProjectileModule mod)
        {
            return projectile;
        }

        // Token: 0x060000C4 RID: 196 RVA: 0x000086C5 File Offset: 0x000068C5
        public virtual void OnPickup(PlayerController player)
        {
        }

        // Token: 0x060000C5 RID: 197 RVA: 0x000086C8 File Offset: 0x000068C8
        public virtual void OnPostDrop(PlayerController player)
        {
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x060000C6 RID: 198 RVA: 0x000086CC File Offset: 0x000068CC
        public bool PickedUp
        {
            get
            {
                return this.gun.CurrentOwner != null;
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x060000C7 RID: 199 RVA: 0x000086F0 File Offset: 0x000068F0
        public PlayerController Player
        {
            get
            {
                bool flag = this.gun.CurrentOwner is PlayerController;
                PlayerController result;
                if (flag)
                {
                    result = (this.gun.CurrentOwner as PlayerController);
                }
                else
                {
                    result = null;
                }
                return result;
            }
        }

        public float HeroSwordCooldown
        {
            get
            {
                bool flag = this.gun != null;
                float result;
                if (flag)
                {
                    result = (float)AdvancedGunBehavior.heroSwordCooldown.GetValue(this.gun);
                }
                else
                {
                    result = -1f;
                }
                return result;
            }
            set
            {
                bool flag = this.gun != null;
                if (flag)
                {
                    AdvancedGunBehavior.heroSwordCooldown.SetValue(this.gun, value);
                }
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x060000CA RID: 202 RVA: 0x000087A8 File Offset: 0x000069A8
        public GameActor Owner
        {
            get
            {
                return this.gun.CurrentOwner;
            }
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x060000CB RID: 203 RVA: 0x000087C8 File Offset: 0x000069C8
        public bool OwnerIsPlayer
        {
            get
            {
                return this.Player != null;
            }
        }

        // Token: 0x04000063 RID: 99
        private bool pickedUpLast = false;

        // Token: 0x04000064 RID: 100
        private PlayerController lastPlayer = null;

        // Token: 0x04000065 RID: 101
        public bool everPickedUpByPlayer = false;

        // Token: 0x04000066 RID: 102
        public Gun gun;

        // Token: 0x04000067 RID: 103
        private bool hasReloaded = true;

        // Token: 0x04000068 RID: 104
        public bool preventNormalFireAudio;

        public bool preventNormalReloadAudio;

        public string overrrideNormalFireAudio;

        public string overrideNormalReloadAudio;

        private static FieldInfo heroSwordCooldown = typeof(Gun).GetField("HeroSwordCooldown", BindingFlags.Instance | BindingFlags.NonPublic);
    }
}

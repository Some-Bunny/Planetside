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
using System.ComponentModel;

namespace Planetside
{
    public class CoinTosser  : LabelablePlayerItem
    {

        public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);

        public static void Init()
        {
            string itemName = "Coin Tosser";
            string resourceName = "Planetside/Resources/cointosser.png";
            GameObject obj = new GameObject(itemName);
            CoinTosser activeitem = obj.AddComponent<CoinTosser>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Version One";
            string longDesc = "Toss a coin out of your own pocket. Thrown coins can be interracted with in many ways (such as shooting them) to greatly increase damage output!\n\nOriginally carried by a robot seeking to find Bullet Hell to fuel itself for all of eternity.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 0.333f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.S;

            updateLabelHook = new Hook(typeof(GameUIItemController).GetMethod("UpdateItem", BindingFlags.Instance | BindingFlags.Public),typeof(CoinTosser).GetMethod("UpdateCustomLabel"));

            activeitem.currentLabel = "0";
            
            Gun gun4 = PickupObjectDatabase.GetById(43) as Gun;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun4.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 5f;
            projectile.baseData.speed *= 0.75f;
            projectile.AdditionalScaleMultiplier = 1f;
            projectile.shouldRotate = true;
            projectile.pierceMinorBreakables = true;
            projectile.baseData.range = 1000f;
            
            BounceProjModifier bouncy = projectile.gameObject.AddComponent<BounceProjModifier>();
            bouncy.numberOfBounces = 1;

            CoinComponent coin = projectile.gameObject.AddComponent<CoinComponent>();
            coin.DeadTime = 0.2f;
            projectile.AnimateProjectile(new List<string> {
                "coinflip_001",
                "coinflip_002",
                "coinflip_003",
                "coinflip_004",
                "coinflip_005",
                "coinflip_006",
                "coinflip_007",
                "coinflip_008"

            }, 20, true, new List<IntVector2> {
                new IntVector2(8, 8), 
                new IntVector2(8, 8),         
                new IntVector2(8, 8), 
                new IntVector2(8, 8),
                new IntVector2(8, 8),
                new IntVector2(8, 8),
                new IntVector2(8, 8),
                new IntVector2(8, 8),

            }, AnimateBullet.ConstructListOfSameValues(false, 8), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 8), AnimateBullet.ConstructListOfSameValues(true, 8), AnimateBullet.ConstructListOfSameValues(false, 8),
            AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 8));
            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.2f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0.9f , 0.5f, 0.3f, 1f);

            CoinProjectile = projectile;

            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:coin_tosser",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "scouter",
                "bionic_leg",
                "gunboots",
                "cog_of_battle",
                "robots_left_hand"
            };
            CustomSynergies.Add("You Call Punching A Coin An Art?", mandatoryConsoleIDs, optionalConsoleIDs, true);
            List<string> optionalConsoleIDs2 = new List<string>
            {
                "old_goldie",
                "zilla_shotgun",
                "blunderbuss",
                "void_shotgun",
                "the_membrane"
            };
            CustomSynergies.Add("HYPERDEATH", mandatoryConsoleIDs, optionalConsoleIDs2, true);
            List<string> optionalConsoleIDs3 = new List<string>
            {
                "coin_crown",
                "gilded_bullets",
                "psog:gilded_pot",
                "gilded_hydra",

            };
            CustomSynergies.Add("C-C-C-C-C-C-C-Combo!", mandatoryConsoleIDs, optionalConsoleIDs3, true);
            List<string> optionalConsoleIDs4 = new List<string>
            {
                "railgun",
                "prototype_railgun",
                "cobalt_hammer",
                "glass_cannon",
                "awp",
                "sniper_rifle",
                "trident",
                "m1",
                "hexagun",
                "laser_rifle",
                "strafe_gun",
                "gunbow",
                "eye_of_the_beholster",
                "shock_rifle"
            };

            CustomSynergies.Add("Malicious", mandatoryConsoleIDs, optionalConsoleIDs4, true);
            CoinTosser.CoinTosserID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);
        }
        private static Hook BlankHook = new Hook(typeof(SilencerInstance).GetMethod("TriggerSilencer", BindingFlags.Instance | BindingFlags.Public), typeof(CoinTosser).GetMethod("DoCoinBlankBoost", BindingFlags.Instance | BindingFlags.Public), typeof(SilencerInstance));


        public void DoCoinBlankBoost(Action<SilencerInstance, Vector2, Single,Single ,GameObject, Single, Single, Single, Single, Single, Single, Single, PlayerController, bool, bool> orig, SilencerInstance self, Vector2 centerPoint, float expandSpeed, float maxRadius, GameObject silencerVFX, float distIntensity, float distRadius, float pushForce, float pushRadius, float knockbackForce, float knockbackRadius, float additionalTimeAtMaxRadius, PlayerController user, bool breaksWalls, bool skipBreakables)
        {
            orig(self ,centerPoint, expandSpeed, maxRadius, silencerVFX, distIntensity, distRadius, pushForce, pushRadius, knockbackForce, knockbackRadius, additionalTimeAtMaxRadius, user, breaksWalls, skipBreakables);
            try
            {
                if (user != null)
                {
                    foreach (GameObject coin in AllActiveCoins)
                    {
                        var proj = coin.GetComponent<Projectile>();
                        if (coin != null && proj != null)
                        {
                            proj.baseData.damage *= 2;
                            proj.baseData.speed *= 2;
                            proj.UpdateSpeed();
                            proj.baseData.range += 50;
                            BounceProjModifier bouncy = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
                            bouncy.numberOfBounces += 2;
                            PierceProjModifier spook = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
                            spook.penetration += 1;
                            CoinComponent coincmp = proj.gameObject.GetComponent<CoinComponent>();
                            coincmp.AmountOfBlanksUsedWhileAlive += 1;
                        }
                    }
                }           
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }

        public static int CoinTosserID;
        public static Projectile CoinProjectile;
        public static Hook updateLabelHook;
        public static List<GameObject> AllActiveCoins = new List<GameObject>();
        public static void UpdateCustomLabel(Action<GameUIItemController, PlayerItem, List<PlayerItem>> orig, GameUIItemController self, PlayerItem current, List<PlayerItem> items)
        {
            orig(self, current, items);
            if (current && current is LabelablePlayerItem)
            {
                LabelablePlayerItem labelable = current as LabelablePlayerItem;
                if (!string.IsNullOrEmpty(labelable.currentLabel))
                {
                    self.ItemCountLabel.IsVisible = true;
                    self.ItemCountLabel.Text = labelable.currentLabel;
                }
            }
        }
        public override bool CanBeUsed(PlayerController user)
        {
            if (user && user.carriedConsumables.Currency >= 0 && user.carriedConsumables.Currency != 0)
            {
                return true;
            }
            return false;
        }
        public override void Update()
        {
            if (base.LastOwner != null)
            {
                if (int.Parse(this.currentLabel) != CalculatePrice())
                {
                    this.currentLabel = CalculatePrice().ToString();
                }
                base.Update();
            }
            base.Update();
        }

        private int CalculatePrice()
        {
            return Mathf.RoundToInt(base.LastOwner.carriedConsumables.Currency);
        }
        public static List<int> HitscanGunList = new List<int>()
        {
            370,
            358,
            390,
            49,
            5,
            385,
            444,
            25,
            153,
            90,
            542,
            540,
            54,
            210
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        protected override void DoEffect(PlayerController user)
        {
            user.carriedConsumables.Currency -= 1;
            float finaldir = ProjSpawnHelper.GetAccuracyAngled(user.CurrentGun.CurrentAngle, 3, user);
            GameObject prefab = CoinProjectile.gameObject;
            GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(prefab, user.sprite.WorldCenter, Quaternion.Euler(0f, 0f, finaldir), true);
            Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
            if (component != null)
            {
                component.Owner = user;
                component.Shooter = user.specRigidbody; 
            }
        }
    }
}



namespace Planetside
{
    public class CoinRicoshotComponent : MonoBehaviour 
    {
        public Action<Projectile> OnReflected;
        public Action OnDestroyed;

        public void OnDestroy()
        {
            if (OnDestroyed != null)
            {
                this.OnDestroyed();
            }
        }

        public GameObject obj;
    }
    public class CoinArbitraryDamageMultiplier : MonoBehaviour
    {
        public float Multiplier = 1;
        public float CustomMultiplierChangeValue = -1;
        public bool CanChangeMultiplier = false;
        public void ChangeMultiplier(float newMult)
        {
            if (CanChangeMultiplier == false) { return; }
            float m = CustomMultiplierChangeValue != -1 ? CustomMultiplierChangeValue : newMult;
            Multiplier = m;
        }
    }



    public class CoinComponent : MonoBehaviour
	{
		public CoinComponent()
		{
            this.DeadTime = 0.1f;
        }
        private void Start()
        {

            this.gameObject.AddComponent<CoinInterractManager>();
      
            AkSoundEngine.PostEvent("Play_CoinFlip", this.gameObject);
            HasPerformedRicochet = false;
            HasBeenBeamBoosted = false;
            AmountOfBlanksUsedWhileAlive = 0;
            if (base.gameObject != null)
            {
                CoinTosser.AllActiveCoins.Add(base.gameObject);
            }

            if (this.m_projectile == null)
            {
                this.m_projectile = base.GetComponent<Projectile>();
            }
            PlayerController player = this.m_projectile.Owner as PlayerController;
            if (player.PlayerHasActiveSynergy("You Call Punching A Coin An Art?"))
            {
                this.m_projectile.baseData.speed *= 1.2f;
                BounceProjModifier bouncy = this.m_projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
                bouncy.numberOfBounces += 1;
                PierceProjModifier spook = this.m_projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                spook.penetration = 2;
                spook.penetratesBreakables = true;
            }
            SpriteOutlineManager.AddOutlineToSprite(this.m_projectile.sprite, Color.black);

        }
      
        public bool MaxSpeedReached;
        private void Update()
        {
            bool flag = this.m_projectile == null;
            if (flag)
            {
                this.m_projectile = base.GetComponent<Projectile>();
            }
            if (m_projectile != null && m_projectile.baseData.speed >= 75)
            {
                if (MaxSpeedReached != true)
                {
                    MaxSpeedReached = true;
                    LootEngine.DoDefaultSynergyPoof(base.gameObject.transform.position, true);
                    AkSoundEngine.PostEvent("Play_CHR_weapon_charged_01", base.gameObject);
                }
                m_projectile.baseData.speed = 75;
                m_projectile.UpdateSpeed();
            }

            float AmountPunched = this.m_projectile.GetComponent<CoinInterractManager>() ? (this.m_projectile.GetComponent<CoinInterractManager>().AmountOfCoinBounces/2.5f)+1 : 1;
            
            this.elapsed += BraveTime.DeltaTime;
            Vector3 vector = m_projectile.sprite.WorldBottomLeft.ToVector3ZisY(0f);
            Vector3 vector2 = m_projectile.sprite.WorldTopRight.ToVector3ZisY(0f);
            float num = (vector2.y - vector.y) * (vector2.x - vector.x);
            int num2 = Mathf.CeilToInt(40f * num);
            int num3 = num2;
            float flame = UnityEngine.Random.Range(0.00f, 1.00f);
            if (flame <= (m_projectile.baseData.damage / 3600) +0.0001f && m_projectile != null)
            {
                GlobalSparksDoer.DoRandomParticleBurst(num3, vector, vector2, Vector3.zero, UnityEngine.Random.Range(-180, 180), 0.3f, 0.33f, 2, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
            }
            Vector2 centerPosition = m_projectile.sprite.WorldCenter;
            foreach (Projectile proj in StaticReferenceManager.AllProjectiles)
            {
                PlayerController player = proj.Owner as PlayerController;                
                bool isBem = proj.GetComponent<BasicBeamController>() != null;
                if (isBem == true && BeamToolbox.PosIsNearAnyBoneOnBeam(proj.GetComponent<BasicBeamController>(), centerPosition, 1.1f) && proj.Owner != null && proj.Owner == player && elapsed >= DeadTime && !CoinTosser.AllActiveCoins.Contains(proj.gameObject))
                {
                    HasBeenBeamBoosted = true;
                    float DMGMult = ((proj.baseData.damage / 3000)) + 1;
                    float SPDMult = (0.4f / 60) + 1;
                    float SZEMult = (0.15f / 60) + 1;
                    float SpeedCapPenalty = MaxSpeedReached ? 0.6f : 1;

                    float DamageCalc = (m_projectile.baseData.damage * (DMGMult * SpeedCapPenalty)) + 0.05f;
                    m_projectile.baseData.damage = DamageCalc;
                    m_projectile.baseData.speed *= SPDMult;
                    m_projectile.UpdateSpeed();
                    m_projectile.baseData.range += 5;
                    m_projectile.AdditionalScaleMultiplier *= SZEMult;
                    float random = UnityEngine.Random.Range(0.00f, 1.00f);
                    if (random <= 0.066f)
                    {
                        BounceProjModifier bouncy = m_projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
                        bouncy.numberOfBounces += 1 + (int)AmountOfBlanksUsedWhileAlive;
                    }
                }
                else
                {

                    if (Vector2.Distance(proj.sprite.WorldCenter, centerPosition) < 0.666f && elapsed >= DeadTime && HasPerformedRicochet == false && !CoinTosser.AllActiveCoins.Contains(proj.gameObject) && proj.gameObject.GetComponent<CoinRicoshotComponent>() != null)
                    {
                        var CrC = proj.gameObject.GetComponent<CoinRicoshotComponent>();
                        if (CrC.OnReflected != null)
                        {
                            CrC.OnReflected(proj);
                            HasPerformedRicochet = true;
                            LootEngine.DoDefaultItemPoof(base.gameObject.transform.position, false, true);
                            if (HasBeenBeamBoosted != true)
                            {
                                AkSoundEngine.PostEvent("Play_perfectshot", base.gameObject);
                                Destroy(base.gameObject);
                            }
                            else
                            {
                                CoinTosser.AllActiveCoins.Remove(base.gameObject);
                                proj.baseData.range += 30;
                                m_projectile.baseData.speed *= 3;
                                m_projectile.UpdateSpeed();
                                AkSoundEngine.PostEvent("Play_ENM_rubber_blast_01", base.gameObject);
                                m_projectile.ModifyVelocity = (Func<Vector2, Vector2>)Delegate.Combine(m_projectile.ModifyVelocity, new Func<Vector2, Vector2>(this.ModifyVelocity));
                                m_projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(m_projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));
                                m_projectile.gameObject.AddComponent<PierceDeadActors>();
                                PierceProjModifier spook = m_projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                                spook.penetration += 2;
                                Exploder.DoDistortionWave(m_projectile.sprite.WorldCenter, 4, 0.07f, 4, 0.3f);
                            }
                        }
                    }
                    else if (Vector2.Distance(proj.sprite.WorldCenter, centerPosition) < 0.666f && proj.Owner != null && proj.Owner == player && elapsed >= DeadTime && HasPerformedRicochet == false && !CoinTosser.AllActiveCoins.Contains(proj.gameObject))
                    {
                        float blankdamage = (AmountOfBlanksUsedWhileAlive * 0.333f) + 1;
                        float DmgMult = 1.35f;
                        float Dmgplus = 5;
                        if (player != null && player.PlayerHasActiveSynergy("HYPERDEATH"))
                        {
                            DmgMult = 1.65f;
                            Dmgplus += 1;
                        }
                        if (player != null && player.PlayerHasActiveSynergy("C-C-C-C-C-C-C-Combo!"))
                        {
                            DmgMult -= 0.25f;
                            Dmgplus -= 1;
                        }
                      
                        HasPerformedRicochet = true;
                        proj.baseData.speed *= 1.2f;
                        proj.UpdateSpeed();
                        float DamageCalc = ((proj.baseData.damage * DmgMult) + Dmgplus) * blankdamage;

                        var arbitraryMultiplier = proj.gameObject.GetComponent<CoinArbitraryDamageMultiplier>();
                        if (arbitraryMultiplier != null)
                        {
                            DamageCalc *= arbitraryMultiplier.Multiplier;
                            arbitraryMultiplier.ChangeMultiplier(1);
                        }

                        proj.baseData.damage = DamageCalc * AmountPunched;
                        proj.pierceMinorBreakables = true;
                        proj.baseData.range += 10;
                        RicochetOffCoinComponent rico = proj.gameObject.GetOrAddComponent<RicochetOffCoinComponent>();
                        rico.NumberOfRicochets += 1;
                        if (base.gameObject.GetComponent<Projectile>() != null && player != null && player.PlayerHasActiveSynergy("Malicious"))
                        {
                            Projectile projectile = base.gameObject.GetComponent<Projectile>();
                            if (CoinTosser.HitscanGunList.Contains(projectile.PossibleSourceGun.PickupObjectId))
                            {
                                ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
                                this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
                                this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
                                this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
                                this.smallPlayerSafeExplosion.damage = proj.baseData.damage *= ((0.5f * (AmountOfBlanksUsedWhileAlive + 1)) * AmountPunched);
                                Exploder.Explode(base.gameObject.transform.PositionVector2(), this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);
                            }
                        }
                        LootEngine.DoDefaultItemPoof(base.gameObject.transform.position, false, true);
                        if (HasBeenBeamBoosted != true)
                        {
                            AkSoundEngine.PostEvent("Play_perfectshot", base.gameObject);
                            Destroy(base.gameObject);
                        }
                        else
                        {
                            CoinTosser.AllActiveCoins.Remove(base.gameObject);
                            proj.baseData.range += 30;
                            m_projectile.baseData.speed *= 3;
                            m_projectile.UpdateSpeed();
                            AkSoundEngine.PostEvent("Play_ENM_rubber_blast_01", base.gameObject);
                            m_projectile.ModifyVelocity = (Func<Vector2, Vector2>)Delegate.Combine(m_projectile.ModifyVelocity, new Func<Vector2, Vector2>(this.ModifyVelocity));
                            m_projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(m_projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));
                            m_projectile.gameObject.AddComponent<PierceDeadActors>();
                            PierceProjModifier spook = m_projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                            spook.penetration += 2;
                            Exploder.DoDistortionWave(m_projectile.sprite.WorldCenter, 4, 0.07f, 4, 0.3f);
                        }
                        float random = UnityEngine.Random.Range(0.00f, 1.00f);
                        if (random <= 0.2f)
                        {
                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, base.gameObject.transform.PositionVector2(), Vector2.zero, 1f, false, false, false);
                        }
                    }
                }
            }
        }
        private Vector2 ModifyVelocity(Vector2 inVel)
        {
            Vector2 vector = inVel;
            RoomHandler absoluteRoomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.m_projectile.LastPosition.IntXY(VectorConversions.Floor));
            List<AIActor> activeEnemies = absoluteRoomFromPosition.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (activeEnemies == null || activeEnemies.Count == 0)
            {
                return inVel;
            }
            float num = float.MaxValue;
            Vector2 vector2 = Vector2.zero;
            AIActor x = null;
            Vector2 b = (!m_projectile.sprite) ? base.transform.position.XY() : m_projectile.sprite.WorldCenter;
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                AIActor aiactor = activeEnemies[i];
                if (aiactor && aiactor.IsWorthShootingAt && !aiactor.IsGone)
                {
                    Vector2 vector3 = aiactor.CenterPosition - b;
                    float sqrMagnitude = vector3.sqrMagnitude;
                    if (sqrMagnitude < num)
                    {
                        vector2 = vector3;
                        num = sqrMagnitude;
                        x = aiactor;
                    }
                }
            }
            num = Mathf.Sqrt(num);
            if (num < this.HomingRadius && x != null)
            {
                float num2 = 1f - num / this.HomingRadius;
                float target = vector2.ToAngle();
                float num3 = inVel.ToAngle();
                float maxDelta = this.AngularVelocity * num2 * this.m_projectile.LocalDeltaTime;
                float num4 = Mathf.MoveTowardsAngle(num3, target, maxDelta);
                if (this.m_projectile is HelixProjectile)
                {
                    float angleDiff = num4 - num3;
                    (this.m_projectile as HelixProjectile).AdjustRightVector(angleDiff);
                }
                else
                {
                    if (this.m_projectile.shouldRotate)
                    {
                        base.transform.rotation = Quaternion.Euler(0f, 0f, num4);
                    }
                    vector = BraveMathCollege.DegreesToVector(num4, inVel.magnitude);
                }
                if (this.m_projectile.OverrideMotionModule != null)
                {
                    this.m_projectile.OverrideMotionModule.AdjustRightVector(num4 - num3);
                }
            }
            if (vector == Vector2.zero || float.IsNaN(vector.x) || float.IsNaN(vector.y))
            {
                return inVel;
            }
            return vector;
        }
        public float HomingRadius = 100;
        public float AngularVelocity = 10000;
        private void HandleHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor != null)
            {
                HasBeenBeamBoosted = false;
                HasPerformedRicochet = false;
                m_projectile.baseData.damage = CoinTosser.CoinProjectile.baseData.speed * (AmountOfBlanksUsedWhileAlive+1);
                m_projectile.baseData.speed = CoinTosser.CoinProjectile.baseData.speed * (AmountOfBlanksUsedWhileAlive + 1);
                m_projectile.UpdateSpeed();
                float sizedown = m_projectile.AdditionalScaleMultiplier / 1;
                m_projectile.AdditionalScaleMultiplier *= sizedown;
                arg1.ModifyVelocity = (Func<Vector2, Vector2>)Delegate.Remove(arg1.ModifyVelocity, new Func<Vector2, Vector2>(this.ModifyVelocity));
                BounceProjModifier bouncy = m_projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
                bouncy.numberOfBounces += 2;
                CoinTosser.AllActiveCoins.Add(base.gameObject);
            }
        }


        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 3f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 50f,
            doExplosionRing = true,
            doDestroyProjectiles = false,
            doForce = true,
            debrisForce = 200f,
            preventPlayerForce = true,
            explosionDelay = 0f,
            usesComprehensiveDelay = false,
            doScreenShake = true,
            playDefaultSFX = true,
        };

        private void OnDestroy()
        {
            if (CoinTosser.AllActiveCoins.Contains(base.gameObject))
            {
                CoinTosser.AllActiveCoins.Remove(base.gameObject);
            }
        }
        private Projectile m_projectile;
        private bool HasPerformedRicochet;
        public float AmountOfBlanksUsedWhileAlive;
        public float DeadTime;
        public bool HasBeenBeamBoosted;
		private float elapsed;
	}
}
namespace Planetside
{
    public class RicochetOffCoinComponent : MonoBehaviour
    {
        public int NumberOfRicochets;
        public RicochetOffCoinComponent()
        {
            this.player = GameManager.Instance.PrimaryPlayer;
            this.RicochetSoftCap = 5;
            this.BlankBonus = 0;
        }
        private void Start()
        {
            hasRetargetedToEnemy = false;
            bool flag = this.m_projectile == null;
            if (flag)
            {
                this.m_projectile = base.GetComponent<Projectile>();
            }
            ImprovedAfterImage yes = m_projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.2f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0.9f, 0.5f, 0.3f, 1f);
            m_projectile.ModifyVelocity = (Func<Vector2, Vector2>)Delegate.Combine(m_projectile.ModifyVelocity, new Func<Vector2, Vector2>(this.ModifyVelocity));
            m_projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(m_projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));
            RicochetSoftCap += BlankBonus * 3;
            if (player.PlayerHasActiveSynergy("C-C-C-C-C-C-C-Combo!"))
            {
                RicochetSoftCap += 50;
            }
        }
        private void HandleHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor != null)
            {
                if (NumberOfRicochets >= 5)
                {
                    TeleporterPrototypeItem teleporter = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>();
                    UnityEngine.Object.Instantiate<GameObject>(teleporter.TelefragVFXPrefab, arg2.sprite.WorldCenter, Quaternion.identity);
                }
                if (hasRetargetedToEnemy == true && arg1 != null)
                {
                    arg1.ModifyVelocity = (Func<Vector2, Vector2>)Delegate.Remove(arg1.ModifyVelocity, new Func<Vector2, Vector2>(this.ModifyVelocity));
                }
            }
        }
        private Vector2 ModifyVelocity(Vector2 inVel)
        {
            Vector2 vector = inVel;
            RoomHandler absoluteRoomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.m_projectile.LastPosition.IntXY(VectorConversions.Floor));
            List<AIActor> activeEnemies = absoluteRoomFromPosition.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            List<GameObject> activecoins = CoinTosser.AllActiveCoins;
            if (activecoins == null || activecoins.Count == 0 | hasRetargetedToEnemy == true | NumberOfRicochets >= RicochetSoftCap)
            {
                hasRetargetedToEnemy = true;
                if (activeEnemies == null || activeEnemies.Count == 0)
                {
                    return inVel;
                }
                float num = float.MaxValue;
                Vector2 vector2 = Vector2.zero;
                AIActor x = null;
                Vector2 b = (!m_projectile.sprite) ? base.transform.position.XY() : m_projectile.sprite.WorldCenter;
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    AIActor aiactor = activeEnemies[i];
                    if (aiactor && aiactor.IsWorthShootingAt && !aiactor.IsGone)
                    {
                        Vector2 vector3 = aiactor.CenterPosition - b;
                        float sqrMagnitude = vector3.sqrMagnitude;
                        if (sqrMagnitude < num)
                        {
                            vector2 = vector3;
                            num = sqrMagnitude;
                            x = aiactor;
                        }
                    }
                }
                num = Mathf.Sqrt(num);
                if (num < this.HomingRadius && x != null)
                {
                    float num2 = 1f - num / this.HomingRadius;
                    float target = vector2.ToAngle();
                    float num3 = inVel.ToAngle();
                    float maxDelta = this.AngularVelocity * num2 * this.m_projectile.LocalDeltaTime;
                    float num4 = Mathf.MoveTowardsAngle(num3, target, maxDelta);
                    if (this.m_projectile is HelixProjectile)
                    {
                        float angleDiff = num4 - num3;
                        (this.m_projectile as HelixProjectile).AdjustRightVector(angleDiff);
                    }
                    else
                    {
                        if (this.m_projectile.shouldRotate)
                        {
                            base.transform.rotation = Quaternion.Euler(0f, 0f, num4);
                        }
                        vector = BraveMathCollege.DegreesToVector(num4, inVel.magnitude);
                    }
                    if (this.m_projectile.OverrideMotionModule != null)
                    {
                        this.m_projectile.OverrideMotionModule.AdjustRightVector(num4 - num3);
                    }
                }
                if (vector == Vector2.zero || float.IsNaN(vector.x) || float.IsNaN(vector.y))
                {
                    return inVel;
                }
            }
            if (hasRetargetedToEnemy == false)
            {
                float num = float.MaxValue;
                Vector2 vector2 = Vector2.zero;
                GameObject x = null;
                Vector2 b = (!m_projectile.sprite) ? m_projectile.transform.position.XY() : m_projectile.sprite.WorldCenter;
                for (int i = 0; i < activecoins.Count; i++)
                {
                    GameObject aiactor = activecoins[i];
                    if (aiactor)
                    {
                        Vector2 vector3 = aiactor.transform.PositionVector2() - b;
                        float sqrMagnitude = vector3.sqrMagnitude;
                        if (sqrMagnitude < num)
                        {
                            vector2 = vector3;
                            num = sqrMagnitude;
                            x = aiactor.gameObject;
                        }
                    }
                }
                num = Mathf.Sqrt(num);
                if (num < this.HomingRadius && x != null)
                {
                    float num2 = 1f - num / this.HomingRadius;
                    float target = vector2.ToAngle();
                    float num3 = inVel.ToAngle();
                    float maxDelta = this.AngularVelocity * num2 * this.m_projectile.LocalDeltaTime;
                    float num4 = Mathf.MoveTowardsAngle(num3, target, maxDelta);
                    if (this.m_projectile is HelixProjectile)
                    {
                        float angleDiff = num4 - num3;
                        (this.m_projectile as HelixProjectile).AdjustRightVector(angleDiff);
                    }
                    else
                    {
                        if (this.m_projectile.shouldRotate)
                        {
                            base.transform.rotation = Quaternion.Euler(0f, 0f, num4);
                        }
                        vector = BraveMathCollege.DegreesToVector(num4, inVel.magnitude);
                    }
                    if (this.m_projectile.OverrideMotionModule != null)
                    {
                        this.m_projectile.OverrideMotionModule.AdjustRightVector(num4 - num3);
                    }
                }
                if (vector == Vector2.zero || float.IsNaN(vector.x) || float.IsNaN(vector.y))
                {
                    return inVel;
                }
            }
            return vector;
        }
        public int BlankBonus;
        private int RicochetSoftCap;
        private bool hasRetargetedToEnemy;
        public float HomingRadius = 100;
        public float AngularVelocity = 10000;
        private Projectile m_projectile;
        public PlayerController player;
    }
}

namespace Planetside
{
    public class CoinInterractManager : BraveBehaviour, IPlayerInteractable
    {
        private void Start()
        {
            IsPunched = false;
            AmountOfCoinBounces = 0;
            TimeElapsed = 1f;
            if (base.GetComponent<Projectile>() != null)
            {
                ProjectileSelf = base.GetComponent<Projectile>();
            }
            //Some boilerplate code that associates and register the interactable with its current room
            this.m_room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Round));
            this.m_room.RegisterInteractable(this);
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite) return float.MaxValue;
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2));
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            //A method that runs whenever the player enters the interaction range of the interactable. This is what outlines it in white to show that it can be interacted with
            if (TimeElapsed >= 1 | IsPunched == true)
            {
                SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
            }
        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
        }
        
        


        public void Interact(PlayerController interactor)
        {
            //This method is what runs when the player actually interacts with the interactable. I put an if statement around the code to ensure that the projectile still exists
            if (ProjectileSelf != null && ProjectileSelf.gameObject.GetComponent<CoinComponent>().DeadTime >= 0.166f && TimeElapsed >= 1)
            {
                IsPunched = true;
                AmountOfCoinBounces += 1;
                int Mult = 1;
                if (ProjectileSelf.GetComponent<CoinComponent>()!=null&& ProjectileSelf.GetComponent<CoinComponent>().HasBeenBeamBoosted == true)
                {
                    Mult *= 2;
                }
                float TimeReduction = (Speed / 150)*Mult;
                TimeElapsed = 0.375f - TimeReduction;
                PunchDir = interactor.CurrentGun.CurrentAngle;
                GameManager.Instance.StartCoroutine(this.PunchCoin(ProjectileSelf, TimeReduction));

                Exploder.DoDistortionWave(ProjectileSelf.sprite.WorldCenter, Speed/10, 0.01f*(Speed/10), 1, 0.25f);

                LootEngine.DoDefaultItemPoof(base.gameObject.transform.position, false, true);
                AkSoundEngine.PostEvent("Play_BOSS_Punchout_Punch_Hit_01", ProjectileSelf.gameObject);
                BounceProjModifier bouncy = ProjectileSelf.gameObject.GetOrAddComponent<BounceProjModifier>();
                bouncy.numberOfBounces += 2+ (AmountOfCoinBounces * 2);
                PierceProjModifier spook = ProjectileSelf.gameObject.GetOrAddComponent<PierceProjModifier>();
                spook.penetration += 2+(AmountOfCoinBounces);
            }
            else if (IsPunched == true)
            {
                Exploder.DoDistortionWave(ProjectileSelf.sprite.WorldCenter, Speed / 60, 0.01f * (Speed / 10), 1, 0.25f);
                AkSoundEngine.PostEvent("Play_BOSS_Punchout_Swing_Right_01", ProjectileSelf.gameObject);
                PunchDir = interactor.CurrentGun.CurrentAngle;
            }
        }
        private bool IsPunched;
        private float PunchDir;
        private IEnumerator PunchCoin(Projectile proj,  float TimeReduction)
        {
            float BaseSpeed = 1;
            if (proj)
            {
                BaseSpeed = proj.baseData.speed;
                float SpeedCapPenalty = proj.GetComponent<CoinComponent>().MaxSpeedReached ? 0.7f : 1;
                proj.baseData.damage *= ((AmountOfCoinBounces/3)*SpeedCapPenalty) +1;
                Vector2 Point = MathToolbox.GetUnitOnCircle(PunchDir, 1);
                proj.SendInDirection(Point, false, true);
            }
            float ela = 0f;
            float dura = 0.5f + TimeReduction;
            while (ela < dura)
            {
                ela += BraveTime.DeltaTime;
                if (proj != null)
                {
                    proj.baseData.speed = 0;
                    proj.UpdateSpeed();
                }
                yield return null;
            }
            if (proj != null)
            {
                this.m_room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Round));
                if (!this.m_room.GetRoomInteractables().Contains(this))
                {
                    this.m_room.RegisterInteractable(this);
                }
                proj.baseData.speed = BaseSpeed*1.33f;
                proj.UpdateSpeed();
                IsPunched = false;
            }
            yield break;
        }
        public void Update()
        {
            Speed = ProjectileSelf ? ProjectileSelf.baseData.speed : 10;
            if (TimeElapsed <= 1)
            {
                this.TimeElapsed += BraveTime.DeltaTime;
            }
        }
        private float Speed;
        private float TimeElapsed;
        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            //Some boilerplate code for determining if the interactable should be flipped
            shouldBeFlipped = false;
            return string.Empty;
        }

        public float GetOverrideMaxDistance()
        {
            if (IsPunched == true)
            {
                return 1.2f;
            }
            if (1+ (AmountOfCoinBounces / 5f) >= 2f)
            {
                return 2f;
            }
            return 1 + (AmountOfCoinBounces/5f);
        }

        protected override void OnDestroy()
        {
            if (this.m_room.GetRoomInteractables().Contains(this))
            {
                this.m_room.DeregisterInteractable(this);
            }
            base.OnDestroy();
        }
        public int AmountOfCoinBounces;
        private Projectile ProjectileSelf;
        private RoomHandler m_room;
    }
}
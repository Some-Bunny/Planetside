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
using SaveAPI;

namespace Planetside
{
    public class KnuckleBlaster : LabelablePlayerItem
    {

        public static void Init()
        {
            string itemName = "Knuckle-Backer";
            string resourceName = "Planetside/Resources/VFX/KnuckleBlaster/fistboth.png";
            GameObject obj = new GameObject(itemName);
            KnuckleBlaster activeitem = obj.AddComponent<KnuckleBlaster>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Ska-Doosh!";
            string longDesc = "Grants the player 2 hydraulic fists that they can alternate between. (X for Keyboard, Reload on full clip for Controller).\n\nThe Feedbacker is faster, weaker, uses 1 Charge and can directly parry *any* projectile, while the Knuckle-Blaster is stronger, slower, uses 3 Charges and releases a powerful shockwave on use.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 0f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.C;
            activeitem.AddToSubShop(ItemBuilder.ShopType.OldRed, 1f);
            activeitem.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);

            activeitem.SetupUnlockOnCustomFlag(CustomDungeonFlags.HAS_COMPLETED_SOMETHING_WICKED, true);

            KnuckleBlaster.ItemspriteIDs = new int[KnuckleBlaster.itemspritepaths.Length];
            KnuckleBlaster.ItemspriteIDs[0] = SpriteBuilder.AddSpriteToCollection(KnuckleBlaster.itemspritepaths[0], activeitem.sprite.Collection);
            KnuckleBlaster.ItemspriteIDs[1] = SpriteBuilder.AddSpriteToCollection(KnuckleBlaster.itemspritepaths[1], activeitem.sprite.Collection);
            KnuckleBlaster.ItemspriteIDs[2] = SpriteBuilder.AddSpriteToCollection(KnuckleBlaster.itemspritepaths[2], activeitem.sprite.Collection);


            GameObject feedbackertether = OtherTools.MakeLine("Planetside/Resources/VFX/KnuckleBlaster/feedbackerchain", new Vector2(10, 10), new Vector2(0, 0), new List<string> { "Planetside/Resources/VFX/KnuckleBlaster/feedbackerchain", "Planetside/Resources/VFX/KnuckleBlaster/feedbackerchain" });
            feedbackertether.SetActive(false);
            FakePrefab.MarkAsFakePrefab(feedbackertether);
            UnityEngine.Object.DontDestroyOnLoad(feedbackertether);
            KnuckleBlaster.FeedbackerChainVFX = feedbackertether;


            GameObject knuckleblastertether = OtherTools.MakeLine("Planetside/Resources/VFX/KnuckleBlaster/knuckleblasterchain", new Vector2(10, 10), new Vector2(0, 0), new List<string> { "Planetside/Resources/VFX/KnuckleBlaster/knuckleblasterchain", "Planetside/Resources/VFX/KnuckleBlaster/knuckleblasterchain" });
            knuckleblastertether.SetActive(false);
            FakePrefab.MarkAsFakePrefab(knuckleblastertether);
            UnityEngine.Object.DontDestroyOnLoad(knuckleblastertether);
            KnuckleBlaster.KnuckleBlasterChainVFX = knuckleblastertether;



            GameObject iconObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/KnuckleBlaster/feedbackerenabled", null, true);
            iconObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(iconObject);
            UnityEngine.Object.DontDestroyOnLoad(iconObject);
            GameObject iconObject2 = new GameObject("Icon");
            tk2dSprite icontk2dSprite = iconObject2.AddComponent<tk2dSprite>();
            icontk2dSprite.SetSprite(iconObject.GetComponent<tk2dBaseSprite>().Collection, iconObject.GetComponent<tk2dBaseSprite>().spriteId);

            KnuckleBlaster.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/KnuckleBlaster/feedbackerenabled", icontk2dSprite.Collection));
            KnuckleBlaster.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/KnuckleBlaster/knuckleblasterenabled", icontk2dSprite.Collection));

            
            icontk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
            KnuckleBlaster.spriteIds.Add(icontk2dSprite.spriteId);
            iconObject2.SetActive(false);

            icontk2dSprite.SetSprite(KnuckleBlaster.spriteIds[0]); //Non Synergy
            icontk2dSprite.SetSprite(KnuckleBlaster.spriteIds[1]); //Synergy

            FakePrefab.MarkAsFakePrefab(iconObject2);
            UnityEngine.Object.DontDestroyOnLoad(iconObject2);
            KnuckleBlaster.IconPrefab = iconObject2;



            GameObject FistObject2 = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/KnuckleBlaster/feedbackerfist", null, true);
            tk2dSprite fisttk2dSprite = FistObject2.AddComponent<tk2dSprite>();
            fisttk2dSprite.SetSprite(FistObject2.GetComponent<tk2dBaseSprite>().Collection, FistObject2.GetComponent<tk2dBaseSprite>().spriteId);

            KnuckleBlaster.FistspriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/KnuckleBlaster/feedbackerfist", fisttk2dSprite.Collection));
            KnuckleBlaster.FistspriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/KnuckleBlaster/knuckleblasterfist", fisttk2dSprite.Collection));


            fisttk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
            KnuckleBlaster.spriteIds.Add(fisttk2dSprite.spriteId);
            FistObject2.SetActive(false);

            fisttk2dSprite.SetSprite(KnuckleBlaster.FistspriteIds[0]); //Non Synergy
            fisttk2dSprite.SetSprite(KnuckleBlaster.FistspriteIds[1]); //Synergy



            FakePrefab.MarkAsFakePrefab(FistObject2);
            UnityEngine.Object.DontDestroyOnLoad(FistObject2);

            
            KnuckleBlaster.FistPrefab = FistObject2;
            KnuckleBlaster.FuckYouFistPrefab = FistObject2;

        }
        public static GameObject KnuckleBlasterChainVFX;
        public static GameObject FeedbackerChainVFX;
        public static GameObject CopiedTetherObject;

        public static GameObject IconPrefab;
        public static List<int> spriteIds = new List<int>();

        public static GameObject FistPrefab;
        public static GameObject FuckYouFistPrefab;

        public static List<int> FistspriteIds = new List<int>();

        public static int KnucleBlasterID;
        public override void Pickup(PlayerController player)
        {
            player.OnReloadPressed += reloadPressed;
            CanSwitch = true;
            ISFeedBacker = true;
            base.Pickup(player);
        }
        public static bool ISFeedBacker;

        public void reloadPressed(PlayerController player, Gun gun)
        {
            bool controller = (BraveInput.GetInstanceForPlayer(base.LastOwner.PlayerIDX).IsKeyboardAndMouse(false));
            if (gun.ClipShotsRemaining == gun.ClipCapacity && controller != true)
            {
                SwitchHand();
            }
        }
        protected override void OnPreDrop(PlayerController player)
        {
            player.OnReloadPressed -= reloadPressed;
            base.OnPreDrop(player);
        }

        private static readonly string[] itemspritepaths = new string[]
        {
            "Planetside/Resources/VFX/KnuckleBlaster/fistbluechosen.png",
            "Planetside/Resources/VFX/KnuckleBlaster/fistredchosen.png",
            "Planetside/Resources/VFX/KnuckleBlaster/fistboth.png",

        };
        private static int[] ItemspriteIDs;


        public void SpawnIcon(int ID)
        {
            GameObject original= GameObject.Instantiate(KnuckleBlaster.IconPrefab, base.LastOwner.CenterPosition, Quaternion.identity, base.LastOwner.transform);
            tk2dSprite component = original.GetComponent<tk2dSprite>();
            component.transform.position.WithZ(transform.position.z + 99999);
            component.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.LastOwner.CenterPosition, tk2dBaseSprite.Anchor.LowerCenter);
            base.LastOwner.sprite.AttachRenderer(component.GetComponent<tk2dBaseSprite>());
            component.PlaceAtPositionByAnchor(base.LastOwner.CenterPosition + new Vector2(0,1), tk2dBaseSprite.Anchor.LowerCenter);
            component.scale = Vector3.one;
            component.GetComponent<tk2dBaseSprite>().SetSprite(KnuckleBlaster.spriteIds[ID]);
            ShrikeADoodle size = original.AddComponent<ShrikeADoodle>();
            size.self = component;
            size.TransPos = base.LastOwner.transform;
        }


        public override void Update()
        {
            if (base.sprite.spriteId != KnuckleBlaster.ItemspriteIDs[2] && base.LastOwner == null)
            {
                base.sprite.SetSprite(KnuckleBlaster.ItemspriteIDs[2]);
            }
            if (base.LastOwner != null)
            {
                if (ISFeedBacker == true && base.sprite.spriteId != KnuckleBlaster.ItemspriteIDs[0])
                {
                    base.sprite.SetSprite(KnuckleBlaster.ItemspriteIDs[0]);
                }
                if (ISFeedBacker != true && base.sprite.spriteId != KnuckleBlaster.ItemspriteIDs[1])
                {
                    base.sprite.SetSprite(KnuckleBlaster.ItemspriteIDs[1]);
                }

                bool controller = (BraveInput.GetInstanceForPlayer(base.LastOwner.PlayerIDX).IsKeyboardAndMouse(false));
                if (controller == true)
                {
                    if (Input.GetKeyDown(KeyCode.X) && CanSwitch == true)
                    {
                        SwitchHand();
                    }
                }
                if (this.Charge <= 3)
                {
                    this.Charge += BraveTime.DeltaTime/1.5f;
                    this.currentLabel = (Mathf.Round(this.Charge * 10f) * 0.1f).ToString();
                }
            }
            base.Update();
        }
        public void SwitchHand()
        {
            if (CanSwitch != false)
            {
                AkSoundEngine.PostEvent("Play_OBJ_metacoin_collect_01", base.LastOwner.gameObject);
                CanSwitch = false;
                GameManager.Instance.StartCoroutine(this.Cooldown());
                if (ISFeedBacker != true)
                {
                    ISFeedBacker = true;
                    SpawnIcon(0);
                }
                else if (ISFeedBacker == true)
                {
                    SpawnIcon(1);
                    ISFeedBacker = false;
                }
            }
        }
        public IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(0.25f);
            CanSwitch = true;
            yield break;
        }
        private bool CanSwitch;
        private float Charge;

        public override bool CanBeUsed(PlayerController user)
        {
            return user && ISFeedBacker ? Charge >= 1 : Charge >= 3 | Charge == 3;
        }
        
        protected override void DoEffect(PlayerController user)
        {
            if (ISFeedBacker != true)
            {
                Charge = 0;
                SpawnFist(1, user);
            }
            else if (ISFeedBacker == true)
            {
                Charge -= 1;
                SpawnFist(0, user);
            }
        }

        public void SpawnFist(int ID, PlayerController player)
        {
            try 
            {
                AkSoundEngine.PostEvent("Play_ENM_gunnut_swing_01", base.gameObject);
                GameObject original = GameObject.Instantiate(FistPrefab.gameObject, base.LastOwner.sprite.WorldCenter, Quaternion.identity, base.LastOwner.transform);
                original.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.LastOwner.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                original.GetComponent<tk2dBaseSprite>().sprite.AttachRenderer(original.GetComponent<tk2dBaseSprite>());
                original.GetComponent<tk2dBaseSprite>().SetSprite(KnuckleBlaster.FistspriteIds[ID]);
                FistComponent fist = original.GetOrAddComponent<FistComponent>();
                fist.Angle = player.CurrentGun.CurrentAngle;
                fist.FlightTime = ISFeedBacker ? 0.1f : 0.175f;
                fist.StopTime = ISFeedBacker ? 0.25f : 0.4f;
                fist.WithdrawTime = ISFeedBacker ? 0.2f : 0.35f;
                fist.Player = player;
                fist.Range = ISFeedBacker ? 2.125f : 1.875f;
                fist.ISFeedbacker = KnuckleBlaster.ISFeedBacker;
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
            }

        }
    }
}
//Code that controls the fist completely
namespace Planetside
{
    public class FistComponent : MonoBehaviour
    {
        public FistComponent()
        {
            this.Angle = 0;
            this.Range = 3;
            this.FlightTime = 0.25f;
            this.WithdrawTime = 0.5f;
            this.StopTime = 0.25f;
            this.Player = GameManager.Instance.PrimaryPlayer;
            this.ISFeedbacker = true;
        }
        public void Start()
        {
            FinishedFlight = false;
            FinishedStop = false;
        }
        private GameObject CopiedLinkVFXPrefab;
        public GameObject CopiedTetherObject;
        public bool ISFeedbacker;

        private float elapsed;

        private bool FinishedFlight;
        private bool FinishedStop;

        public void Update()
        {
            elapsed += BraveTime.DeltaTime;
            float dmg = Player != null ? Player.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
            float knockback = Player != null ? Player.stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier) : 1;
            float range = Player != null ? Player.stats.GetStatValue(PlayerStats.StatType.RangeMultiplier) : 1;
            if (elapsed <= FlightTime && FinishedFlight != true)
            {
                float t = elapsed / FlightTime * (elapsed / FlightTime);
                if (base.gameObject != null)
                {
                    Vector2 Point1 = MathToolbox.GetUnitOnCircle(Angle, Range * t);
                    base.gameObject.transform.position = Player.sprite != null ? Player.sprite.WorldCenter + Point1 : Player.transform.PositionVector2() + Point1;
                    base.gameObject.transform.rotation = Quaternion.Euler(0, 0, Angle);

                    float DamageperTick = ISFeedbacker ? 2 * dmg : 8 * dmg;
                    Exploder.DoRadialDamage(DamageperTick, base.gameObject.transform.PositionVector2(), 0.5f, false, true, true, null);
                    Exploder.DoRadialPush(base.gameObject.transform.PositionVector2(), 30 * knockback, 0.5f * range);

                    foreach (Projectile proj in StaticReferenceManager.AllProjectiles)
                    {
                        if (proj != null)
                        {
                            AIActor enemy = proj.Owner as AIActor;
                            PlayerController player = proj.Owner as PlayerController;
                            bool isBem = proj.GetComponent<BasicBeamController>() != null;
                            if (isBem != true)
                            {
                                float RangeFist = ISFeedbacker ? 0.875f : 0.75f;
                                bool ae = Vector2.Distance(proj.sprite ? proj.sprite.WorldCenter : proj.transform.PositionVector2(), base.gameObject.transform.PositionVector2()) < RangeFist && proj.Owner != null && proj.Owner == enemy && proj.gameObject.GetComponent<MarkedProjectile>() == null && proj.HasDiedInAir != true;
                                if (ae)
                                {
                                    proj.gameObject.AddComponent<MarkedProjectile>();
                                    if (ISFeedbacker)
                                    {
                                        AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_01", base.gameObject);
                                        AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_01", base.gameObject);
                                        LootEngine.DoDefaultSynergyPoof(proj.transform.position, true);
                                        FistReflectBullet(proj, Player.gameActor, proj.baseData.speed *= 2, Angle, 1f, proj.IsBlackBullet ? 16 * dmg : 8 * dmg, 0f);
                                    }
                                    else
                                    {
                                        LootEngine.DoDefaultItemPoof(proj.transform.position, false, true);
                                        AkSoundEngine.PostEvent("Play_OBJ_rock_break_02", base.gameObject);
                                        AkSoundEngine.PostEvent("Play_OBJ_rock_break_02", base.gameObject);

                                        FistReflectBullet(proj, Player.gameActor, proj.baseData.speed *= 2, UnityEngine.Random.insideUnitCircle.normalized.ToAngle(), 1f, proj.IsBlackBullet?14*dmg : 7 * dmg, 0f);
                                    }
                                }

                                bool dsadads = Vector2.Distance(proj.sprite.WorldCenter, base.gameObject.transform.PositionVector2()) < 0.75f && proj.Owner != null && proj.Owner == player && proj.gameObject.GetComponent<MarkedProjectile>() == null;
                                if (dsadads)
                                {
                                    LootEngine.DoDefaultItemPoof(proj.transform.position, false, true);
                                    proj.gameObject.AddComponent<MarkedProjectile>();
                                    AkSoundEngine.PostEvent("Play_OBJ_brazier_flip_01", base.gameObject);
                                    Vector2 angle = MathToolbox.GetUnitOnCircle(Angle, 1);
                                    proj.SendInDirection(angle, true);
                                    proj.baseData.damage *= ISFeedbacker ? 1.5f : 2.25f;
                                    proj.baseData.speed *= ISFeedbacker ? 1.4f : 1.8f;
                                    Exploder.DoDistortionWave(base.gameObject.transform.PositionVector2(), 2, 0.15f, 2, 0.2f);
                                    ImprovedAfterImage yes = proj.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                                    yes.spawnShadows = true;
                                    yes.shadowLifetime += 0.3f;
                                    yes.shadowTimeDelay = 0.01f;
                                    yes.dashColor = new Color(0.5f, 0.08f, 0.06f, 0.01f);
                                }

                            }
                        }
                    }

                }
            }
            if (elapsed <= StopTime && FinishedFlight == true && FinishedStop != true)
            {
                elapsed += BraveTime.DeltaTime;
                Vector2 Point1 = MathToolbox.GetUnitOnCircle(Angle, Range);
                base.gameObject.transform.position = Player.sprite != null ? Player.sprite.WorldCenter + Point1 : Player.transform.PositionVector2() + Point1;
                base.gameObject.transform.rotation = Quaternion.Euler(0, 0, Angle);
            }
            if (elapsed <= WithdrawTime && FinishedFlight == true && FinishedStop == true)
            {
                elapsed += BraveTime.DeltaTime;
                float t = elapsed / WithdrawTime * (elapsed / WithdrawTime);
                if (base.gameObject != null)
                {
                    Vector2 Point1 = MathToolbox.GetUnitOnCircle(Angle, Range - (Range * t));
                    base.gameObject.transform.position = Player.sprite != null ? Player.sprite.WorldCenter + Point1 : Player.transform.PositionVector2() + Point1;
                    base.gameObject.transform.rotation = Quaternion.Euler(0, 0, Angle);
                }
            }
            if (elapsed >= FlightTime && FinishedFlight != true)
            {
                AkSoundEngine.PostEvent("Play_OBJ_chain_switch_01", base.gameObject);
                elapsed = 0;
                FinishedFlight = true;
            }
            if (elapsed >= StopTime && FinishedStop != true)
            {
                elapsed = 0;
                FinishedStop = true;
                if (ISFeedbacker != true)
                {
                    if (base.gameObject != null)
                    {
                        AkSoundEngine.PostEvent("Play_ENM_cannonball_blast_01", base.gameObject);
                        Exploder.DoRadialPush(base.gameObject.transform.PositionVector2(), 150 * knockback, 2.5f * range);
                        Exploder.DoRadialKnockback(base.gameObject.transform.PositionVector2(), 150 * knockback, 2.5f * range);
                        Exploder.DoRadialMinorBreakableBreak(base.gameObject.transform.PositionVector2(), 2.5f * range);
                        Exploder.DoRadialDamage(12 * dmg, base.gameObject.transform.PositionVector2(), 2.5f * range, false, true, false, null);
                        Exploder.DoDistortionWave(base.gameObject.transform.PositionVector2(), 10, 0.0625f, 2.5f * range, 0.1f);
                        foreach (Projectile proj in StaticReferenceManager.AllProjectiles)
                        {
                            if (proj != null)
                            {
                                AIActor enemy = proj.Owner as AIActor;
                                //PlayerController player = proj.Owner as PlayerController;
                                bool isBem = proj.GetComponent<BasicBeamController>() != null;
                                if (isBem != true)
                                {
                                    bool ae = Vector2.Distance(proj.sprite ? proj.sprite.WorldCenter : proj.transform.PositionVector2(), base.gameObject.transform.PositionVector2()) < 3f && proj.Owner != null && proj.Owner == enemy && proj.gameObject.GetComponent<MarkedProjectile>() == null;
                                    if (ae)
                                    {
                                        proj.gameObject.AddComponent<MarkedProjectile>();
                                        FistReflectBullet(proj, Player.gameActor, proj.baseData.speed *= 2, UnityEngine.Random.insideUnitCircle.normalized.ToAngle(), 1f, proj.IsBlackBullet ? 14 * dmg : 7 * dmg, 0f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (elapsed >= WithdrawTime && FinishedStop == true && FinishedFlight == true)
            {
                if (FistTether != null)
                {
                    Destroy(FistTether);
                }
                if (base.gameObject != null)
                {
                    Destroy(base.gameObject);
                }
            }

            if (this.FistTether == null)
            { 
                this.CopiedLinkVFXPrefab = GameObject.Instantiate(ISFeedbacker ? KnuckleBlaster.FeedbackerChainVFX.gameObject : KnuckleBlaster.KnuckleBlasterChainVFX.gameObject, base.gameObject.transform.PositionVector2(), Quaternion.identity, base.gameObject.transform);
                tk2dTiledSprite component3 = CopiedLinkVFXPrefab.GetComponent<tk2dTiledSprite>();
                component3.renderer.enabled = false;
                FistTether = component3;   
            }
            else if (Player != null && this.FistTether != null)
            {
                UpdateLink(this.FistTether);
            }
            else if (FistTether != null && base.gameObject == null)
            {
                Destroy(FistTether.gameObject);
            }
        }


        private void UpdateLink(tk2dTiledSprite m_extantLink)
        {
            if (base.gameObject == null)
            {
                if (FistTether != null)
                {
                    Destroy(FistTether.gameObject);
                }
            }
            else if (FistTether != null)
            {
                Vector2 unitCenter = base.gameObject.GetComponent<tk2dBaseSprite>().sprite != null ?base.gameObject.GetComponent<tk2dBaseSprite>().sprite.WorldCenter : base.gameObject.transform.PositionVector2();
                Vector2 unitCenter2 = Player.sprite != null ? Player.sprite.WorldCenter : Player.transform.PositionVector2();// + Point1;
                m_extantLink.transform.position = unitCenter;
                Vector2 vector = unitCenter2 - unitCenter;
                float num = BraveMathCollege.Atan2Degrees(vector.normalized);
                int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
                m_extantLink.dimensions = new Vector2((float)num2, 6);
                m_extantLink.transform.rotation = Quaternion.Euler(0f, 0f, num);
                m_extantLink.UpdateZDepth();
                m_extantLink.anchor = tk2dAnimatedSprite.Anchor.MiddleLeft;
                m_extantLink.HeightOffGround = base.gameObject.GetComponent<tk2dBaseSprite>().HeightOffGround - 10;
                m_extantLink.renderer.enabled = true;

            }

        }

        public float WithdrawTime;
        public float StopTime;
        public float FlightTime;

        public float Angle;
        public float Range;
        public tk2dTiledSprite FistTether;
        public PlayerController Player;

        public static void FistReflectBullet(Projectile p, GameActor newOwner, float minReflectedBulletSpeed, float ReflectAngle, float scaleModifier = 1f, float damageModifier = 10f, float spread = 0f)
        {
            p.RemoveBulletScriptControl();
            Vector2 Point1 = MathToolbox.GetUnitOnCircle(ReflectAngle, 1);
            p.Direction = Point1;

            if (spread != 0f)
            {
                p.Direction = p.Direction.Rotate(UnityEngine.Random.Range(-spread, spread));
            }
            if (p.Owner && p.Owner.specRigidbody)
            {
                p.specRigidbody.DeregisterSpecificCollisionException(p.Owner.specRigidbody);
            }
            p.Owner = newOwner;
            p.SetNewShooter(newOwner.specRigidbody);
            p.allowSelfShooting = false;
            if (newOwner is AIActor)
            {
                p.collidesWithPlayer = true;
                p.collidesWithEnemies = false;
            }
            else
            {
                p.collidesWithPlayer = false;
                p.collidesWithEnemies = true;
            }
            if (scaleModifier != 1f)
            {
                SpawnManager.PoolManager.Remove(p.transform);
                p.RuntimeUpdateScale(scaleModifier);
            }
            if (p.Speed < minReflectedBulletSpeed)
            {
                p.Speed = minReflectedBulletSpeed;
            }
            if (p.baseData.damage < ProjectileData.FixedFallbackDamageToEnemies)
            {
                p.baseData.damage = ProjectileData.FixedFallbackDamageToEnemies;
            }
            p.baseData.damage = damageModifier;
            p.UpdateCollisionMask();
            p.ResetDistance();
            p.Reflected();
        }
    }
    public class MarkedProjectile : MonoBehaviour
    {
        public void OnDestroy()
        {
            if (base.gameObject != null)
            {
                Destroy(this);
            }
        }
    }
}


namespace Planetside
{
    public class ShrikeADoodle : MonoBehaviour
    {
        public ShrikeADoodle()
        {
            this.self = KnuckleBlaster.IconPrefab.GetComponent<tk2dBaseSprite>();
        }
        public void Update()
        {
            if (elapsed <= 0.05f)
            {
                this.elapsed += BraveTime.DeltaTime/5;
            }
            else
            {
                this.elapsed += BraveTime.DeltaTime;
            }

            Transform copySprite = self.transform;
            copySprite.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, elapsed * 1.5f);

            copySprite.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(TransPos.PositionVector2()+ new Vector2((elapsed*5)+0.75f, 1.625f), tk2dBaseSprite.Anchor.LowerCenter);
            if (copySprite.localScale == Vector3.zero)
            {
                Destroy(base.gameObject);
            }
        }
        public Transform TransPos;
        private float elapsed;
        public tk2dBaseSprite self;
    }
}

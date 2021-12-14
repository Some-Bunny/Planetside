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

namespace Planetside
{
    internal class ElectrostaticGuonStone : IounStoneOrbitalItem
    {
        public static void Init()
        {
            string name = "Electro-Static Guon Stone";
            string resourcePath = "Planetside/Resources/Guons/ElectroGuon/electrostaticguonitem.png";
            GameObject gameObject = new GameObject();
            ElectrostaticGuonStone item = gameObject.AddComponent<ElectrostaticGuonStone>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Batteries Included";
            string longDesc = "This enchanted battery releases bursts of power on collision." + "\n\nOriginally stuffed into an old battery-powered device, a bored gunjurer made it float for the kicks.";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");

            item.quality = PickupObject.ItemQuality.C;
            ElectrostaticGuonStone.BuildPrefab();
            item.OrbitalPrefab = ElectrostaticGuonStone.orbitalPrefab;
            item.Identifier = IounStoneOrbitalItem.IounStoneIdentifier.GENERIC;
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:electro-static_guon_stone",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "polaris",
                "iron_coin",
                "backup_gun",
                "roll_bomb",
                "portable_table_device"
            };
            CustomSynergies.Add("Flip-Side", mandatoryConsoleIDs, optionalConsoleIDs, true);

            List<string> optionalConsoleIsDs = new List<string>
            {
                "the_emperor",
                "shock_rifle",
                "shock_rounds"
            };
            CustomSynergies.Add("Shocker", mandatoryConsoleIDs, optionalConsoleIsDs, true);
            ElectrostaticGuonStone.ElectrostaticGuonStoneID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);

        }
        public static int ElectrostaticGuonStoneID;

        public static void BuildPrefab()
        {
            string value = "AWESOME";
            string.IsNullOrEmpty(value);
            bool flag = ElectrostaticGuonStone.orbitalPrefab != null;
            if (!flag)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/Guons/ElectroGuon/electrostaticguonfloaty.png");
                gameObject.name = "Electro Guon Stone Orbital";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(7, 13));
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                ElectrostaticGuonStone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                ElectrostaticGuonStone.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
                ElectrostaticGuonStone.orbitalPrefab.shouldRotate = false;
                ElectrostaticGuonStone.orbitalPrefab.orbitRadius = 6f;
                ElectrostaticGuonStone.orbitalPrefab.SetOrbitalTier(0);
                ElectrostaticGuonStone.orbitalPrefab.orbitDegreesPerSecond = 45f;
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public override void Pickup(PlayerController player)
        {
            ElectrostaticGuonStone.guonHook = new Hook(typeof(PlayerOrbital).GetMethod("Initialize"), typeof(ElectrostaticGuonStone).GetMethod("GuonInit"));
            player.gameObject.AddComponent<ElectrostaticGuonStone.ElectricGuonbehavior>();
            GameManager.Instance.OnNewLevelFullyLoaded += this.FixGuon;
            base.Pickup(player);
        }

        private void FixGuon()
        {
            bool flag = base.Owner && base.Owner.GetComponent<ElectrostaticGuonStone.ElectricGuonbehavior>() != null;
            bool flag2 = flag;
            bool flag3 = flag2;
            if (flag3)
            {
                base.Owner.GetComponent<ElectrostaticGuonStone.ElectricGuonbehavior>().Destroy();
            }
            PlayerController owner = base.Owner;
            owner.gameObject.AddComponent<ElectrostaticGuonStone.ElectricGuonbehavior>();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            SpawnManager.Despawn(extantLink.gameObject);
            player.GetComponent<ElectrostaticGuonStone.ElectricGuonbehavior>().Destroy();
            ElectrostaticGuonStone.guonHook.Dispose();
            GameManager.Instance.OnNewLevelFullyLoaded -= this.FixGuon;
            return base.Drop(player);
        }

        protected override void OnDestroy()
        {
            if (base.Owner != null)
            {
                if (extantLink.gameObject != null)
                {
                    SpawnManager.Despawn(extantLink.gameObject);
                }
                ElectrostaticGuonStone.guonHook.Dispose();
                /*
                bool flag = base.Owner && base.Owner.GetComponent<ElectrostaticGuonStone.ElectricGuonbehavior>() != null;
                bool flag2 = flag;
                bool flag3 = flag2;
                if (flag3)
                {
                    base.Owner.GetComponent<ElectrostaticGuonStone.ElectricGuonbehavior>().Destroy();
                }
                */
                GameManager.Instance.OnNewLevelFullyLoaded -= this.FixGuon;
                base.OnDestroy();
            }
        }

        private GameObject LinkVFXPrefab;
        private tk2dTiledSprite extantLink;
        protected override void Update()
        {
            base.Update();
            bool flag = this.m_extantOrbital != null;
            if (flag)
            {
                if (this.LinkVFXPrefab == null)
                {
                    this.LinkVFXPrefab = FakePrefab.Clone(Game.Items["shock_rounds"].GetComponent<ComplexProjectileModifier>().ChainLightningVFX);
                }
                if (base.Owner && this.extantLink == null)
                {
                    tk2dTiledSprite component = SpawnManager.SpawnVFX(this.LinkVFXPrefab, false).GetComponent<tk2dTiledSprite>();
                    this.extantLink = component;
                }
                else if (base.Owner && this.extantLink != null)
                {
                    UpdateLink(base.Owner, this.extantLink);
                }
                else if (extantLink != null)
                {
                    SpawnManager.Despawn(extantLink.gameObject);
                    extantLink = null;
                }
            }
                
        }
        private void UpdateLink(PlayerController target, tk2dTiledSprite m_extantLink)
        {

            SpeculativeRigidbody specRigidbody = base.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody;
            SpeculativeRigidbody speculativeRigidbody = specRigidbody;

            Vector2 unitCenter = speculativeRigidbody.UnitCenter;
            Vector2 unitCenter2 = target.specRigidbody.HitboxPixelCollider.UnitCenter;
            m_extantLink.transform.position = unitCenter;
            Vector2 vector = unitCenter2 - unitCenter;
            float num = BraveMathCollege.Atan2Degrees(vector.normalized);
            int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f); 
                m_extantLink.dimensions = new Vector2((float)num2, m_extantLink.dimensions.y);
            m_extantLink.transform.rotation = Quaternion.Euler(0f, 0f, num);
            m_extantLink.UpdateZDepth();
            this.ApplyLinearDamage(unitCenter, unitCenter2);

        }
        private void ApplyLinearDamage(Vector2 p1, Vector2 p2)
        {
            float num = 5;
            bool flagA = base.Owner.PlayerHasActiveSynergy("Shocker");
            if (flagA)
            {
                num *= 3;
            }
            for (int i = 0; i < StaticReferenceManager.AllEnemies.Count; i++)
            {
                AIActor aiactor = StaticReferenceManager.AllEnemies[i];
                if (!this.m_damagedEnemies.Contains(aiactor))
                {
                    if (aiactor && aiactor.HasBeenEngaged && aiactor.IsNormalEnemy && aiactor.specRigidbody)
                    {
                        Vector2 zero = Vector2.zero;
                        if (BraveUtility.LineIntersectsAABB(p1, p2, aiactor.specRigidbody.HitboxPixelCollider.UnitBottomLeft, aiactor.specRigidbody.HitboxPixelCollider.UnitDimensions, out zero))
                        {
                            aiactor.healthHaver.ApplyDamage(num, Vector2.zero, "Chain Lightning", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                            GameManager.Instance.StartCoroutine(this.HandleDamageCooldown(aiactor));
                        }
                    }
                }
            }
        }
        private IEnumerator HandleDamageCooldown(AIActor damagedTarget)
        {
            this.m_damagedEnemies.Add(damagedTarget);
            yield return new WaitForSeconds(0.25f);
            this.m_damagedEnemies.Remove(damagedTarget);
            yield break;
        }
        private HashSet<AIActor> m_damagedEnemies = new HashSet<AIActor>();



        public static void GuonInit(Action<PlayerOrbital, PlayerController> orig, PlayerOrbital self, PlayerController player)
        {
            orig(self, player);
        }
        public static Hook guonHook;
        public static PlayerOrbital orbitalPrefab;
        private class ElectricGuonbehavior : BraveBehaviour
        {
            private void Start()
            {
                this.owner = base.GetComponent<PlayerController>();
                foreach (IPlayerOrbital playerOrbital in this.owner.orbitals)
                {
                    PlayerOrbital playerOrbital2 = (PlayerOrbital)playerOrbital;
                    SpeculativeRigidbody specRigidbody = playerOrbital2.specRigidbody;
                    specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision));
                }
            }
          
            
            private PlayerController owner;

            private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody other, PixelCollider otherCollider)
            {
                Projectile component = other.GetComponent<Projectile>();
                bool flag = component != null && !(component.Owner is PlayerController);
                if (flag)
                {
                    bool iei = !ElectricGuonbehavior.onCooldown;
                    if (iei)
                    {
                        float Lines = 2;
                        bool flagA = owner.PlayerHasActiveSynergy("Flip-Side");
                        if (flagA)
                        {
                            Lines = 4;
                        }
                        ElectricGuonbehavior.onCooldown = true;
                        GameManager.Instance.StartCoroutine(ElectricGuonbehavior.StartCooldown());
                        for (int counter = 0; counter < Lines; counter++)
                        {
                            Vector3 position = myRigidbody.sprite.WorldCenter;
                            GameObject gameObject = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(153) as Gun).DefaultModule.projectiles[0].gameObject, position, Quaternion.Euler(0f, 0f, BraveMathCollege.Atan2Degrees(owner.sprite.WorldCenter - myRigidbody.sprite.WorldCenter) + (counter * (360/Lines)+90)), true);
                            Projectile eatfarts = gameObject.GetComponent<Projectile>();
                            bool flag12 = eatfarts != null;
                            bool flag2 = flag12;
                            if (flag2)
                            {
                                PierceProjModifier spook = eatfarts.gameObject.AddComponent<PierceProjModifier>();
                                spook.penetration = 10;
                                eatfarts.AdjustPlayerProjectileTint(Color.blue.WithAlpha(Color.blue.a / 50f), 50, 0f);
                                eatfarts.SpawnedFromOtherPlayerProjectile = true;
                                eatfarts.Shooter = myRigidbody.specRigidbody;
                                eatfarts.Owner = owner;
                                eatfarts.baseData.damage = 5f;
                                eatfarts.AdditionalScaleMultiplier = 0.66f;
                                eatfarts.baseData.range = 5f;
                                eatfarts.SetOwnerSafe(owner, "Player");
                                eatfarts.ignoreDamageCaps = true;
                                eatfarts.PenetratesInternalWalls = true;

                            }
                            // base.StartCoroutine(this.Speed(component));
                        }
                    }
                    
                }
            }
            public void Destroy()
            {
                UnityEngine.Object.Destroy(this);
            }
            private static IEnumerator StartCooldown()
            {
                yield return new WaitForSeconds(0.2f);
                ElectricGuonbehavior.onCooldown = false;
                yield break;
            }
            private static bool onCooldown;
        }
    }
}
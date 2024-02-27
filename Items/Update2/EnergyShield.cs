using Alexandria.PrefabAPI;
using ItemAPI;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planetside
{
    internal class EnergyShield : PassiveItem
    {
        public static void Init()
        {
            string name = "Energy-Plated Shield";
            GameObject gameObject = new GameObject();
            EnergyShield item = gameObject.AddComponent<EnergyShield>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("energyshield"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Thunk";
            string longDesc = "Grants 4 protective shields.\n\nCreated as a reusable defense for spaceships, the designers forgot the simple fact that enemies usually don't just fire once.";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");

            item.quality = PickupObject.ItemQuality.B;
            EnergyShield.BuildPrefab();

            item.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);

            item.gameObject.AddComponent<IronsideItemPool>();

            EnergyShield.EnergyPlatedShieldID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);

        }
        public static int EnergyPlatedShieldID;

        public static void BuildPrefab()
        {
            if (EnergyShield.orbitalPrefab == null)
            {

                GameObject gameObject = PrefabBuilder.BuildObject("EnergyShield");

                tk2dSprite sprite = gameObject.AddComponent<tk2dSprite>();
                sprite.collection = StaticSpriteDefinitions.Guon_Sheet_Data;
                sprite.SetSprite(StaticSpriteDefinitions.Guon_Sheet_Data.GetSpriteIdByName("energyshiledguon_001"));

                tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
                animator.library = StaticSpriteDefinitions.Guon_Animation_Data;
                animator.defaultClipId = StaticSpriteDefinitions.Guon_Animation_Data.GetClipIdByName("energyShield_idle");
                animator.playAutomatically = true;

                sprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;


                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                EnergyShield.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                EnergyShield.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
                EnergyShield.orbitalPrefab.shouldRotate = false;
                EnergyShield.orbitalPrefab.orbitRadius = 3.2f;
                EnergyShield.orbitalPrefab.SetOrbitalTier(0);
                EnergyShield.orbitalPrefab.orbitDegreesPerSecond = 0;
                EnergyShield.orbitalPrefab.perfectOrbitalFactor = 0.5f;
                var orb = EnergyShield.orbitalPrefab.gameObject.AddComponent<EnergyGuonGlowManager>();

                sprite.usesOverrideMaterial = true;

                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
                Material mat = sprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat.mainTexture = sprite.renderer.material.mainTexture;
                mat.SetColor("_EmissiveColor", new Color32(0, 242, 255, 255));
                mat.SetFloat("_EmissiveColorPower", 1.55f);
                mat.SetFloat("_EmissivePower", 70);
                sprite.renderer.material = mat;
                orb.GlowMat = mat;


                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
                EnergyGuon = gameObject;

            }
        }
        private static GameObject EnergyGuon;
        public List<GameObject> EnergyOrbitals = new List<GameObject>();
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            for (int i = 0; i < 4; i++)
            {
                GameObject guon = PlayerOrbitalItem.CreateOrbital(base.Owner, EnergyShield.EnergyGuon, false);
                this.EnergyOrbitals.Add(guon);
            }
            GameManager.Instance.OnNewLevelFullyLoaded += this.FixGuon;
        }

        private void FixGuon()
        {
            this.EnergyOrbitals.Clear();
            for (int i = 0; i < 4; i++)
            {
                GameObject guon = PlayerOrbitalItem.CreateOrbital(base.Owner, EnergyShield.EnergyGuon, false);
                this.EnergyOrbitals.Add(guon);
            }           
        }

        public override DebrisObject Drop(PlayerController player)
        {
            for (int i = EnergyOrbitals.Count - 1; i > -1; i--)
            {
                if (EnergyOrbitals[i] != null)
                {
                    Destroy(EnergyOrbitals[i].gameObject);
                }
            }
            this.EnergyOrbitals.Clear();
            GameManager.Instance.OnNewLevelFullyLoaded -= this.FixGuon;
            return base.Drop(player);
        }

        public override void OnDestroy()
        {
            if (base.Owner != null)
            {
                for (int i = EnergyOrbitals.Count - 1; i > -1; i--)
                {
                    if (EnergyOrbitals[i] != null)
                    {
                        Destroy(EnergyOrbitals[i].gameObject);
                    }
                }
                EnergyOrbitals.Clear();
                GameManager.Instance.OnNewLevelFullyLoaded -= this.FixGuon;
                base.OnDestroy();
            }
        }

        public static PlayerOrbital orbitalPrefab;
       
    }
}

namespace Planetside
{
    public class EnergyGuonGlowManager : MonoBehaviour
    {
        private tk2dSpriteAnimator dSpriteAnimator;
        public void Start()
        {
            LootEngine.DoDefaultSynergyPoof(this.transform.PositionVector2());
            this.Orbital = base.GetComponent<PlayerOrbital>();
            dSpriteAnimator = base.GetComponent<tk2dSpriteAnimator>();
            SpeculativeRigidbody specRigidbody = Orbital.specRigidbody;
            specRigidbody.OnPreRigidbodyCollision += OnPreCollision;
        }

        public void OnDestroy()
        {
            if (this)
            {
                SpeculativeRigidbody specRigidbody = Orbital.specRigidbody;
                specRigidbody.OnPreRigidbodyCollision -= OnPreCollision;
                LootEngine.DoDefaultSynergyPoof(this.transform.PositionVector2());
            }
        }

        public Material GlowMat;
        private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody other, PixelCollider otherCollider)
        {
            if (Cooldown == false)
            {
                Hits++;
                Cooldown = true;
                this.Invoke("C", 0.25f);
            }

            if (other.projectile != null && !(other.projectile.Owner is PlayerController) && Hits > 3 && HasBeenHit == false)
            {
                GameManager.Instance.StartCoroutine(this.PhaseOut());   
            }
            else if (other.projectile != null && !(other.projectile.Owner is PlayerController) && HasBeenHit == true)
            {
                PhysicsEngine.SkipCollision = true;
            }
        }

        private bool Cooldown = false;
        public void C()
        {
            Cooldown = false;
        }


        private IEnumerator PhaseOut()
        {
            if (dSpriteAnimator)
            {
                dSpriteAnimator.Paused = true;
            }
            HasBeenHit = true;
            Material material1 = Orbital.sprite.renderer.material;
            material1.shader = ShaderCache.Acquire("Brave/Internal/HologramShader");
            material1.SetFloat("_IsGreen", 0f);
            Orbital.sprite.renderer.material = material1;
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", Orbital.gameObject);
            SpeculativeRigidbody specRigidbody = Orbital.specRigidbody;
            specRigidbody.CollideWithTileMap = false;
            specRigidbody.CollideWithOthers = true;
            
            yield return new WaitForSeconds(5);
            if (dSpriteAnimator)
            {
                dSpriteAnimator.Paused = false;
            }
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", Orbital.gameObject);
            Orbital.sprite.renderer.material = GlowMat;
            Hits = 0;
            specRigidbody.CollideWithTileMap = false;
            specRigidbody.CollideWithOthers = true;
            HasBeenHit = false;

            yield break;
        }
        private bool HasBeenHit;
        private int Hits;
        private PlayerOrbital Orbital;
    }
}
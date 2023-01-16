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
            //string resourcePath = "Planetside/Resources/energyshield.png";
            GameObject gameObject = new GameObject();
            EnergyShield item = gameObject.AddComponent<EnergyShield>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("energyshield"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Thunk";
            string longDesc = "4 Plates of protective shields.\n\nOriginally banned by the Guneva Convention for its use in torture methods, it was disguised as a defensive item when being smuggled around the Hegemony. In a hilarious twist, it proved to function better as a defensive shield than a torture device.";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");

            item.quality = PickupObject.ItemQuality.B;
            EnergyShield.BuildPrefab();
            //item.OrbitalPrefab = EnergyShield.orbitalPrefab;
            //item.Identifier = IounStoneOrbitalItem.IounStoneIdentifier.GENERIC;
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
                GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/Guons/EnergyPlatedGuon/energyshiledguon.png");
                gameObject.name = "Energy Shield Orbital";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                EnergyShield.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                EnergyShield.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
                EnergyShield.orbitalPrefab.shouldRotate = false;
                EnergyShield.orbitalPrefab.orbitRadius = 4f;
                EnergyShield.orbitalPrefab.SetOrbitalTier(0);
                EnergyShield.orbitalPrefab.orbitDegreesPerSecond = 0;
                EnergyShield.orbitalPrefab.perfectOrbitalFactor = 1000f;
                EnergyShield.orbitalPrefab.gameObject.AddComponent<EnergyGuonGlowManager>();
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
                EnergyGuon = gameObject;

            }
        }
        private static GameObject EnergyGuon;
        public static List<GameObject> EnergyOrbitals = new List<GameObject>();
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            for (int i = 0; i < 4; i++)
            {
                GameObject guon = PlayerOrbitalItem.CreateOrbital(base.Owner, EnergyShield.EnergyGuon, false);
                EnergyOrbitals.Add(guon);
            }
            GameManager.Instance.OnNewLevelFullyLoaded += this.FixGuon;
        }

        protected override void Update()
        {
           
        }



        private void FixGuon()
        {
            EnergyOrbitals.Clear();
            for (int i = 0; i < 4; i++)
            {
                GameObject guon = PlayerOrbitalItem.CreateOrbital(base.Owner, EnergyShield.EnergyGuon, false);
                EnergyOrbitals.Add(guon);
            }
           
            PlayerController owner = base.Owner;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            foreach (GameObject guon in EnergyOrbitals)
            {
                if (guon != null)
                {
                    Destroy(guon.gameObject);
                }
            }
            EnergyOrbitals.Clear();
            GameManager.Instance.OnNewLevelFullyLoaded -= this.FixGuon;
            return base.Drop(player);
        }

        protected override void OnDestroy()
        {
            if (base.Owner != null)
            {
                foreach (GameObject guon in EnergyOrbitals)
                {
                    if (guon != null)
                    {
                        Destroy(guon.gameObject);
                    }
                }
                EnergyOrbitals.Clear();
                GameManager.Instance.OnNewLevelFullyLoaded -= this.FixGuon;
                base.OnDestroy();
            }
        }




        public static void GuonInit(Action<PlayerOrbital, PlayerController> orig, PlayerOrbital self, PlayerController player)
        {
            orig(self, player);
        }
        public static PlayerOrbital orbitalPrefab;
       
    }
}

namespace Planetside
{
    public class EnergyGuonGlowManager : MonoBehaviour
    {
        public EnergyGuonGlowManager()
        {
            this.player = GameManager.Instance.PrimaryPlayer;
        }

        public void Awake()
        {
            this.actor = base.GetComponent<PlayerOrbital>();
        }

        public void Start()
        {

            PlayerOrbital playerOrbital2 = (PlayerOrbital)actor;
            SpeculativeRigidbody specRigidbody = playerOrbital2.specRigidbody;
            specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision));
            playerOrbital2.sprite.usesOverrideMaterial = true;

            playerOrbital2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
            Material mat = playerOrbital2.sprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = playerOrbital2.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 242, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 70);
            playerOrbital2.sprite.renderer.material = mat;
            GlowMat = mat;
        }
        private Material GlowMat;
        private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody other, PixelCollider otherCollider)
        {
            Hits++;
            Projectile component = other.GetComponent<Projectile>();
            bool flag = component != null && !(component.Owner is PlayerController) && Hits >= 1 && HasBeenHit == false;
            if (flag)
            {
                GameManager.Instance.StartCoroutine(this.PhaseOut(actor));   
            }
            else if (component != null && !(component.Owner is PlayerController) && HasBeenHit == true)
            {
                PhysicsEngine.SkipCollision = true;
            }
        }


        private IEnumerator PhaseOut(PlayerOrbital playerorbital)
        {
            HasBeenHit = true;
            Material material1 = playerorbital.sprite.renderer.material;
            material1.shader = ShaderCache.Acquire("Brave/Internal/HologramShader");
            material1.SetFloat("_IsGreen", 0f);
            playerorbital.sprite.renderer.material = material1;
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", playerorbital.gameObject);
            SpeculativeRigidbody specRigidbody = playerorbital.specRigidbody;
            specRigidbody.CollideWithTileMap = false;
            specRigidbody.CollideWithOthers = true;
            yield return new WaitForSeconds(3);
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", playerorbital.gameObject);
            playerorbital.sprite.renderer.material = GlowMat;
            Hits = 0;
            specRigidbody.CollideWithTileMap = false;
            specRigidbody.CollideWithOthers = true;
            HasBeenHit = false;

            yield break;
        }
        private bool HasBeenHit;
        private int Hits;
        private PlayerOrbital actor;
        public PlayerController player;
    }
}
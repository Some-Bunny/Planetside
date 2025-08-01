using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using GungeonAPI;
using Alexandria;
using Planetside.Static_Storage;

namespace Planetside
{
    public class StaticInformation 
    {
        public static void Init()
        {
            StaticBulletEntries.Init();
            StaticEnemyShadows.Init();
            StaticTextures.Init();
            ModderBulletGUIDs = new List<string>();
        }
        public static List<string> ModderBulletGUIDs;
    }
    public class StaticEnemyShadows
    {
        public static void Init()
        {
            massiveShadow = (EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").ShadowPrefab);
            if (massiveShadow == null) { ETGModConsole.Log("massiveShadow IS NULL"); }

            largeShadow = (EnemyDatabase.GetOrLoadByGuid("eed5addcc15148179f300cc0d9ee7f94").ShadowPrefab);
            if (largeShadow == null) { ETGModConsole.Log("largeShadow IS NULL"); }

            highPriestShadow = (EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowPrefab);
            if (highPriestShadow == null) { ETGModConsole.Log("highPriestShadow IS NULL"); }

            defaultShadow = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("DefaultShadowSprite"));
            if (defaultShadow == null) { ETGModConsole.Log("defaultShadow IS NULL"); }
        }
        public static GameObject highPriestShadow;
        public static GameObject massiveShadow;
        public static GameObject largeShadow;
        public static GameObject defaultShadow;

    }
    public class StaticTextures
    {
        public static void Init()
        {
            NebulaTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\nebula_reducednoise.png");
            VoidTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\voidTex.png");
            AdvancedParticleBlue = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources2\\ParticleTextures\\advancedparticles.png");

            Keep_1_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("keep_1");
            Keep_2_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("keep_2");

            Sewer_1_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("sewer_1");
            Sewer_2_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("sewer_2");

            Proper_1_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("proper_1");
            Proper_2_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("proper_2");

            Abbey_1_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("abbey_1");
            Abbey_2_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("abbey_2");

            Mines_1_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("mines_1");
            Mines_2_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("mines_2");

            Hollow_1_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("hollow_1");
            Hollow_2_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("hollow_2");

            Forge_1_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("forge_1");
            Forge_2_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("forge_2");

            Hell_1_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("hell_1");
            Hell_2_Texture = PlanetsideModule.ModAssets.LoadAsset<Texture2D>("hell_2");

            Gradient_Texture_Trail = EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing").BulletObject.GetComponent<Projectile>().gameObject.GetComponentInChildren<CustomTrailRenderer>().material.mainTexture;

            Hell_Drag_Zone_Texture = StaticVFXStorage.hellDragController.HoleObject.GetComponent<MeshRenderer>().material.GetTexture("_PortalTex");

            Ouroborous_Medal_Main = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("ouroborosmedal");
            Ouroborous_Medal_Left = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("ouroborosMedal2Left");
            Ouroborous_Medal_Right = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("ouroborosMedal2Right");
            Curse_Ring = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("blessed_pot_circle_001");
            Gradient_Circle = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("Gradient_Round_White");

        }
        public static Texture AdvancedParticleBlue;
        public static Texture VoidTexture;
        public static Texture NebulaTexture;

        public static Texture Keep_1_Texture;
        public static Texture Keep_2_Texture;

        public static Texture Sewer_1_Texture;
        public static Texture Sewer_2_Texture;

        public static Texture Proper_1_Texture;
        public static Texture Proper_2_Texture;

        public static Texture Abbey_1_Texture;
        public static Texture Abbey_2_Texture;

        public static Texture Mines_1_Texture;
        public static Texture Mines_2_Texture;

        public static Texture Hollow_1_Texture;
        public static Texture Hollow_2_Texture;

        public static Texture Forge_1_Texture;
        public static Texture Forge_2_Texture;

        public static Texture Hell_1_Texture;
        public static Texture Hell_2_Texture;

        public static Texture Gradient_Texture_Trail;

        public static Texture Hell_Drag_Zone_Texture;

        public static Texture2D Ouroborous_Medal_Main;
        public static Texture2D Ouroborous_Medal_Left;
        public static Texture2D Ouroborous_Medal_Right;
        public static Texture2D Curse_Ring;
        public static Texture2D Gradient_Circle;

    }


}

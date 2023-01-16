using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{
    public static class StaticSpriteDefinitions 
    {
        public static tk2dSpriteCollectionData Modder_Bullet_Sheet_Data;
        public static tk2dSpriteCollectionData Passive_Item_Sheet_Data;
        public static tk2dSpriteCollectionData Active_Item_Sheet_Data;
        public static tk2dSpriteCollectionData Debuff_Sheet_Data;
        public static tk2dSpriteCollectionData Oddments_Sheet_Data;
        public static tk2dSpriteCollectionData Pickup_Sheet_Data;



        public static void Init()
        {
            Modder_Bullet_Sheet_Data = DoFastSetup("ModderBulletCollection", "modderbullet material.mat");
            if (Modder_Bullet_Sheet_Data == null) { ETGModConsole.Log("Modder_Bullet_Sheet_Data is NULL"); }
            Passive_Item_Sheet_Data = DoFastSetup("Item_Collection", "item_collection material.mat");
            if (Passive_Item_Sheet_Data == null) { ETGModConsole.Log("Passive_Item_Sheet_Data is NULL"); }
            Active_Item_Sheet_Data = DoFastSetup("Active_Collection", "active_item material.mat");
            if (Active_Item_Sheet_Data == null) { ETGModConsole.Log("Active_Item_Sheet_Data is NULL"); }
            Debuff_Sheet_Data = DoFastSetup("DebuffCollection", "debuff material.mat");
            if (Debuff_Sheet_Data == null) { ETGModConsole.Log("Debuff_Sheet_Data is NULL"); }
            Oddments_Sheet_Data = DoFastSetup("OddmentsCollection", "oddments material.mat");
            if (Oddments_Sheet_Data == null) { ETGModConsole.Log("Oddments_Sheet_Data is NULL"); }
            Pickup_Sheet_Data = DoFastSetup("PickupCollection", "pickup material.mat");
            if (Pickup_Sheet_Data == null) { ETGModConsole.Log("Pickup_Sheet_Data is NULL"); }
        }

        public static tk2dSpriteCollectionData DoFastSetup(string CollectionName, string MaterialName)
        {
            tk2dSpriteCollectionData Colection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>(CollectionName).GetComponent<tk2dSpriteCollectionData>();
            Material material = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>(MaterialName);
            FastAssetBundleSpriteSetup(Colection, material);
             return Colection;
        }
        public static void FastAssetBundleSpriteSetup(tk2dSpriteCollectionData bundleData, Material mat)
        {
            Texture texture = mat.GetTexture("_MainTex");
            texture.filterMode = FilterMode.Point;
            mat.SetTexture("_MainTex", texture);
            bundleData.material = mat;

            bundleData.materials = new Material[]
            {
                mat,
            };
            bundleData.materialInsts = new Material[]
            {
                mat,
            };
            foreach (var c in bundleData.spriteDefinitions)
            {
                c.material = bundleData.materials[0];
                c.materialInst = bundleData.materials[0];
                c.materialId = 0;
            }    
        }
    }
}

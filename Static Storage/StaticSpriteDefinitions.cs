using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static tk2dSpriteCollectionData Gun_Sheet_Data;
        public static tk2dSpriteCollectionData Projectile_Sheet_Data;
        public static tk2dSpriteCollectionData Guon_Sheet_Data;

        public static tk2dSpriteAnimation Debuff_Animation_Data;


        public static tk2dSpriteAnimation Gun_Animation_Data;
        public static tk2dSpriteAnimation Projectile_Animation_Data;
        public static tk2dSpriteAnimation Guon_Animation_Data;


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

            Gun_Sheet_Data = DoFastSetup("PlanetsideGunCollection", "psoggun material.mat");
            if (Gun_Sheet_Data == null) { ETGModConsole.Log("Gun_Sheet_Data is NULL"); }

            Projectile_Sheet_Data = DoFastSetup("PlanetsideProjectileCollection", "projectile material.mat");
            if (Projectile_Sheet_Data == null) { ETGModConsole.Log("Projectile_Sheet_Data is NULL"); }

            Guon_Sheet_Data = DoFastSetup("PSOGGuonCollection", "guonpsog material.mat");
            if (Guon_Sheet_Data == null) { ETGModConsole.Log("Guon_Sheet_Data is NULL"); }

            Debuff_Animation_Data = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("DebuffAnimation").GetComponent<tk2dSpriteAnimation>();

            Gun_Animation_Data = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("PlanetsideGunAnimation").GetComponent<tk2dSpriteAnimation>();

            Projectile_Animation_Data = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("PlanetsideProjectileAnimation").GetComponent<tk2dSpriteAnimation>();

            Guon_Animation_Data = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("GuonAnimation").GetComponent<tk2dSpriteAnimation>();

        }

        public static void SetupSpritesFromAssembly(Assembly asmb, string path)
        {
            if (asmb != null)
            {
                path = path.Replace("/", ".").Replace("\\", ".");
                if (!path.EndsWith("."))
                {
                    path += ".";
                }

                tk2dSpriteCollectionData tk2dSpriteCollectionData = Gun_Sheet_Data;
                List<string> list5 = new List<string>();
                string[] manifestResourceNames = asmb.GetManifestResourceNames();

                foreach (string text in manifestResourceNames)
                {
                    if (text.StartsWith(path) && text.Length > path.Length)
                    {

                        string[] array2 = text.Substring(path.LastIndexOf(".") + 1).Split(new char[]
                        {
                            '.'
                        });

                        if (array2.Length == 3)
                        {
                            string text2 = array2[2];
                            if (text2.ToLowerInvariant() == "json" || text2.ToLowerInvariant() == "jtk2d")
                            {
                                list5.Add(text);
                            }
                        }         
                    }
                }
                foreach (string text5 in list5)
                {
                    string[] array5 = text5.Substring(path.LastIndexOf(".") + 1).Split(new char[]
                    {
                        '.'
                    });
                    string collection = array5[0];
                    string text6 = array5[1];

                    if (((tk2dSpriteCollectionData != null) ? tk2dSpriteCollectionData.spriteDefinitions : null) != null && tk2dSpriteCollectionData.Count > 0)
                    {
                        int spriteIdByName = tk2dSpriteCollectionData.GetSpriteIdByName(text6, -1);

                        if (spriteIdByName > -1)
                        {
                            using (Stream manifestResourceStream2 = asmb.GetManifestResourceStream(text5))
                            {
                                AssetSpriteData assetSpriteData = default(AssetSpriteData);
                                try
                                {
                                    assetSpriteData = JSONHelper.ReadJSON<AssetSpriteData>(manifestResourceStream2);
                                }
                                catch
                                {
                                    ETGModConsole.Log("Error: invalid json at project path " + text5, false);
                                    continue;
                                }
                                tk2dSpriteCollectionData.SetAttachPoints(spriteIdByName, assetSpriteData.attachPoints);
                                //tk2dSpriteCollectionData.inst.SetAttachPoints(spriteIdByName, assetSpriteData.attachPoints);
                            }
                        }
                    }            
                }                
            }
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

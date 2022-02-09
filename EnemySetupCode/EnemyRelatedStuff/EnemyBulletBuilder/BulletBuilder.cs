using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using System.Collections;

namespace EnemyBulletBuilder
{
    static class BulletBuilder
    {
        /// <summary>
        /// initializes all the necessaary fake prefab hooks and lists
        /// </summary>
        public static void Init()
        {
            BulletBuilderFakePrefabHooks.Init();
            bulletEntries = new Dictionary<string, AIBulletBank.Entry>();
        }
        public static void SetProjectileSpriteRight(this Projectile proj, int pixelWidth, int pixelHeight, int? offsetX = null, int? offsetY = null)
        {
            try
            {
                float w = (float)pixelWidth / 16f;
                float h = (float)pixelHeight / 16f;
                float x = ((offsetX == null) ? 0 : offsetX.Value / 16f);
                float y = (-h / 2f) + ((offsetY == null) ? 0 : offsetY.Value / 16f);
                tk2dBaseSprite sprite = proj.sprite;

                tk2dSpriteDefinition tk2dSpriteDefinition = sprite.GetCurrentSpriteDef();
                tk2dSpriteDefinition.position0 = new Vector3(x, y, 0);
                tk2dSpriteDefinition.position1 = new Vector3(x + w, y, 0f);
                tk2dSpriteDefinition.position2 = new Vector3(x, y + h, 0f);
                tk2dSpriteDefinition.position3 = new Vector3(x + w, y + h, 0f);
                tk2dSpriteDefinition.colliderVertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(w / 2f, h / 2f, 0) };

            }
            catch (Exception ex)
            {
                ETGModConsole.Log("Ooops! Seems like something got very, Very, VERY wrong. Here's the exception:", false);
                ETGModConsole.Log(ex.ToString(), false);
            }
        }

        /// <summary>
        /// creates a fake prefab of an enemy bullet and adds it to the list of bullet entries. returns the fake prefab object    
        /// </summary>
        /// <param name="spritePath">the path for the sprite.</param>
        /// <param name="bulletEntryName">the name of the bullet in the bullet bank, and also the name by which you will instantiate the entry.</param>
        /// <param name="shouldRotate">makes the projectile able to rotate when shot, like lead maidens projectiles.</param>
        /// <param name="colliderGenerationMode">this is experimental, and should always be set to manual.</param>
        /// <param name="manualWidth">width of collider.</param>
        /// <param name="manualHeight">height of collider.</param>
        /// <param name="manualOffsetX">x offset of collider.</param>
        /// <param name="manualOffsetY">y offset of collider.</param>
        public static GameObject CreateBulletPrefab(string spritePath, string bulletEntryName, bool shouldRotate = false, bool UsesGlow = true
            , int? manualWidth = null, int? manualHeight = null, int? manualOffsetX = null, int? manualOffsetY = null)
        {
            if (spritePath == null || bulletEntryName == null)
            {
                ETGModConsole.Log("BULLET BUILDER : sprite path or bullet name are null");
                return null;
            }
            if (bulletEntries.ContainsKey(bulletEntryName))
            {
                ETGModConsole.Log("BULLET BUILDER : " + bulletEntryName + " already exists in the database.");
                return null;
            }

            int spriteID = SpriteBuilder.AddSpriteToCollection(spritePath, ETGMod.Databases.Items.ProjectileCollection);
            Texture2D texture = ResourceExtractor.GetTextureFromResource(spritePath);
            int width = texture.width;
            int height = texture.height;
            UnityEngine.Object.Destroy(texture);
            if (manualWidth != null)
            {
                width = manualWidth.Value;
            }
            if (manualHeight != null)
            {
                height = manualHeight.Value;
            }

            GameObject bulletObject = null;
            if (shouldRotate)
            {
                bulletObject = UnityEngine.Object.Instantiate(EnemyDatabase.GetOrLoadByGuid("05891b158cd542b1a5f3df30fb67a7ff").bulletBank.GetBullet().BulletObject);
            }
            else
            {
                bulletObject = UnityEngine.Object.Instantiate(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet().BulletObject);
            }

            bulletObject.SetActive(false);
            BulletBuilderFakePrefab.MarkAsFakePrefab(bulletObject);
            BulletBuilderFakePrefab.DontDestroyOnLoad(bulletObject);

            Projectile bulletProjectile = bulletObject.GetComponent<Projectile>();

            bulletProjectile.sprite.SetSprite(ETGMod.Databases.Items.ProjectileCollection, spriteID);
            bulletProjectile.SetProjectileSpriteRight(width, height, manualOffsetX, manualOffsetY);


            bulletProjectile.BulletScriptSettings.preventPooling = true;

            bulletProjectile.collidesWithEnemies = false;

            SpeculativeRigidbody body = bulletProjectile.specRigidbody;


            body.PrimaryPixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Tk2dPolygon;

            body.UpdateCollidersOnRotation = shouldRotate;

            if (UsesGlow)
            {

                Material sharedMaterial = body.sprite.renderer.sharedMaterial;
                body.sprite.usesOverrideMaterial = true;
                Material material = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
                material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
                BulletBuilder.LerpMaterialGlow(material, 0f, 22f, 0.4f);
                material.SetFloat("_EmissiveColorPower", 8f);
                material.SetColor("_EmissiveColor", Color.red);
                body.sprite.renderer.material = material;

            }

            AIBulletBank.Entry bulletEntry = new AIBulletBank.Entry();
            bulletEntry.BulletObject = bulletObject;
            bulletEntry.Name = bulletEntryName;
            bulletEntry.ProjectileData = bulletProjectile.baseData;
            VFXPool muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            bulletEntry.MuzzleFlashEffects = muzzleFlashEffects;
            bulletEntries.Add(bulletEntryName, bulletEntry);

            return bulletObject;
        }

        public static GameObject CreateModifiedBulletPrefab(AIBulletBank.Entry entry, string name)
        {
            GameObject bulletObject = UnityEngine.Object.Instantiate(entry.BulletObject);


            bulletObject.SetActive(false);
            BulletBuilderFakePrefab.MarkAsFakePrefab(bulletObject);
            BulletBuilderFakePrefab.DontDestroyOnLoad(bulletObject);

            Projectile bulletProjectile = bulletObject.GetComponent<Projectile>();

            bulletProjectile.BulletScriptSettings.preventPooling = true;

            bulletProjectile.collidesWithEnemies = false;

            SpeculativeRigidbody body = bulletProjectile.specRigidbody;


            body.PrimaryPixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Tk2dPolygon;

          
            AIBulletBank.Entry bulletEntry = new AIBulletBank.Entry();
            bulletEntry.BulletObject = bulletObject;
            bulletEntry.Name = name;
            bulletEntry.ProjectileData = bulletProjectile.baseData;
            VFXPool muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            bulletEntry.MuzzleFlashEffects = muzzleFlashEffects;
            bulletEntries.Add(name, bulletEntry);

            return bulletObject;
        }

        public static void LerpMaterialGlow(Material targetMaterial, float startGlow, float targetGlow, float duration)
        {
            targetMaterial.SetFloat("_EmissivePower", Mathf.Lerp(startGlow, targetGlow, duration));
        }
        public static AIBulletBank.Entry InitializeBulletEntryByName(string BulletEntryName)
        {
            if (bulletEntries.ContainsKey(BulletEntryName))
            {
                AIBulletBank.Entry entry = new AIBulletBank.Entry();
                entry.Name = bulletEntries[BulletEntryName].Name;
                entry.BulletObject = UnityEngine.Object.Instantiate(bulletEntries[BulletEntryName].BulletObject);
                BulletBuilderFakePrefab.MarkAsFakePrefab(entry.BulletObject);
                UnityEngine.Object.DontDestroyOnLoad(entry.BulletObject);
                entry.ProjectileData = entry.BulletObject.GetComponent<Projectile>().baseData;
                return entry;
            }
            ETGModConsole.Log("BULLET BUILDER : custom bullet entry not found (wrong name?)");
            return null;
        }
        public static AIBulletBank.Entry GetBulletEntryByName(string BulletEntryName)
        {
            if (bulletEntries.ContainsKey(BulletEntryName))
            {
                return bulletEntries[BulletEntryName];
            }
            ETGModConsole.Log("BULLET BUILDER : custom bullet entry not found (wrong name?)");
            return null;
        }
        private static Dictionary<string, AIBulletBank.Entry> bulletEntries = null;
    }
}

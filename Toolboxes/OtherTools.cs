using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using tk2dRuntime.TileMap;
using UnityEngine;
using ItemAPI;
using FullInspector;

using Gungeon;

//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;

using Brave.BulletScript;
using GungeonAPI;

using System.Text;
using System.IO;
using System.Reflection;
using SaveAPI;

using MonoMod.RuntimeDetour;

namespace Planetside
{
    static class PlanetsideReflectionHelper
    {
        public static T GetTypedValue<T>(this FieldInfo This, object instance) { return (T)This.GetValue(instance); }
        public static T ReflectGetField<T>(Type classType, string fieldName, object o = null)
        {
            if (classType == null) { ETGModConsole.Log("classType IS NULL"); }
            if (classType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | ((o != null) ? BindingFlags.Instance : BindingFlags.Static)) == null) { ETGModConsole.Log("field IS NULL"); }
            if (o == null) { ETGModConsole.Log("object IS NULL"); }


            FieldInfo field = classType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | ((o != null) ? BindingFlags.Instance : BindingFlags.Static));
            return (T)field.GetValue(o);
        }
        public static T InvokeMethod<T>(Type type, string methodName, object typeInstance = null, object[] methodParams = null)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | ((typeInstance == null) ? BindingFlags.Static : BindingFlags.Instance);
            return (T)((object)type.GetMethod(methodName, bindingAttr).Invoke(typeInstance, methodParams));
        }

        public static void InvokeMethod(Type type, string methodName, object typeInstance = null, object[] methodParams = null)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | ((typeInstance == null) ? BindingFlags.Static : BindingFlags.Instance);
            type.GetMethod(methodName, bindingAttr).Invoke(typeInstance, methodParams);
        }
    }
}


namespace Planetside
{

    public static class OtherTools
    {
        public static GameObject MakeLine(string spritePath, Vector2 colliderDimensions, Vector2 colliderOffsets, List<string> beamAnimationPaths = null, int beamFPS = -1)
        {
            try
            {

                GameObject line = new GameObject("line");

                float convertedColliderX = colliderDimensions.x / 16f;
                float convertedColliderY = colliderDimensions.y / 16f;
                float convertedOffsetX = colliderOffsets.x / 16f;
                float convertedOffsetY = colliderOffsets.y / 16f;

                int spriteID = SpriteBuilder.AddSpriteToCollection(spritePath, ETGMod.Databases.Items.ProjectileCollection);
                tk2dTiledSprite tiledSprite = line.GetOrAddComponent<tk2dTiledSprite>();

                tiledSprite.SetSprite(ETGMod.Databases.Items.ProjectileCollection, spriteID);
                tk2dSpriteDefinition def = tiledSprite.GetCurrentSpriteDef();
                def.colliderVertices = new Vector3[]{
                    new Vector3(convertedOffsetX, convertedOffsetY, 0f),
                    new Vector3(convertedColliderX, convertedColliderY, 0f)
                };

                def.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft);
                /*
                tk2dSpriteAnimator animator = line.GetOrAddComponent<tk2dSpriteAnimator>();
                tk2dSpriteAnimation animation = line.GetOrAddComponent<tk2dSpriteAnimation>();
                animation.clips = new tk2dSpriteAnimationClip[0];
                animator.Library = animation;
                animator.sprite.SetSprite(spriteID); 

                if (beamAnimationPaths != null)
                {
                    tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip() { name = "line_idle_thing", frames = new tk2dSpriteAnimationFrame[0], fps = beamFPS };
                    List<string> spritePaths = beamAnimationPaths;

                    List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
                    foreach (string path in spritePaths)
                    {
                        tk2dSpriteCollectionData collection = ETGMod.Databases.Items.ProjectileCollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection(path, collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft);
                        frameDef.colliderVertices = def.colliderVertices;
                        frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }
                    clip.frames = frames.ToArray();
                    animation.clips = animation.clips.Concat(new tk2dSpriteAnimationClip[] { clip }).ToArray();
                }
                */
                return line;
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
                return null;
            }
        }


        public static T CopyFields<T>(Projectile sample2) where T : Projectile
        {
            T sample = sample2.gameObject.AddComponent<T>();
            sample.PossibleSourceGun = sample2.PossibleSourceGun;
            sample.SpawnedFromOtherPlayerProjectile = sample2.SpawnedFromOtherPlayerProjectile;
            sample.PlayerProjectileSourceGameTimeslice = sample2.PlayerProjectileSourceGameTimeslice;
            sample.BulletScriptSettings = sample2.BulletScriptSettings;
            sample.damageTypes = sample2.damageTypes;
            sample.allowSelfShooting = sample2.allowSelfShooting;
            sample.collidesWithPlayer = sample2.collidesWithPlayer;
            sample.collidesWithProjectiles = sample2.collidesWithProjectiles;
            sample.collidesOnlyWithPlayerProjectiles = sample2.collidesOnlyWithPlayerProjectiles;
            sample.projectileHitHealth = sample2.projectileHitHealth;
            sample.collidesWithEnemies = sample2.collidesWithEnemies;
            sample.shouldRotate = sample2.shouldRotate;
            sample.shouldFlipVertically = sample2.shouldFlipVertically;
            sample.shouldFlipHorizontally = sample2.shouldFlipHorizontally;
            sample.ignoreDamageCaps = sample2.ignoreDamageCaps;
            sample.baseData = sample2.baseData;
            sample.AppliesPoison = sample2.AppliesPoison;
            sample.PoisonApplyChance = sample2.PoisonApplyChance;
            sample.healthEffect = sample2.healthEffect;
            sample.AppliesSpeedModifier = sample2.AppliesSpeedModifier;
            sample.SpeedApplyChance = sample2.SpeedApplyChance;
            sample.speedEffect = sample2.speedEffect;
            sample.AppliesCharm = sample2.AppliesCharm;
            sample.CharmApplyChance = sample2.CharmApplyChance;
            sample.charmEffect = sample2.charmEffect;
            sample.AppliesFreeze = sample2.AppliesFreeze;
            sample.FreezeApplyChance = sample2.FreezeApplyChance;
            sample.freezeEffect = (sample2.freezeEffect);
            sample.AppliesFire = sample2.AppliesFire;
            sample.FireApplyChance = sample2.FireApplyChance;
            sample.fireEffect = (sample2.fireEffect);
            sample.AppliesStun = sample2.AppliesStun;
            sample.StunApplyChance = sample2.StunApplyChance;
            sample.AppliedStunDuration = sample2.AppliedStunDuration;
            sample.AppliesBleed = sample2.AppliesBleed;
            sample.bleedEffect = (sample2.bleedEffect);
            sample.AppliesCheese = sample2.AppliesCheese;
            sample.CheeseApplyChance = sample2.CheeseApplyChance;
            sample.cheeseEffect = (sample2.cheeseEffect);
            sample.BleedApplyChance = sample2.BleedApplyChance;
            sample.CanTransmogrify = sample2.CanTransmogrify;
            sample.ChanceToTransmogrify = sample2.ChanceToTransmogrify;
            sample.TransmogrifyTargetGuids = sample2.TransmogrifyTargetGuids;
            sample.BossDamageMultiplier = sample2.BossDamageMultiplier;
            sample.SpawnedFromNonChallengeItem = sample2.SpawnedFromNonChallengeItem;
            sample.TreatedAsNonProjectileForChallenge = sample2.TreatedAsNonProjectileForChallenge;
            sample.hitEffects = sample2.hitEffects;
            sample.CenterTilemapHitEffectsByProjectileVelocity = sample2.CenterTilemapHitEffectsByProjectileVelocity;
            sample.wallDecals = sample2.wallDecals;
            sample.persistTime = sample2.persistTime;
            sample.angularVelocity = sample2.angularVelocity;
            sample.angularVelocityVariance = sample2.angularVelocityVariance;
            sample.spawnEnemyGuidOnDeath = sample2.spawnEnemyGuidOnDeath;
            sample.HasFixedKnockbackDirection = sample2.HasFixedKnockbackDirection;
            sample.FixedKnockbackDirection = sample2.FixedKnockbackDirection;
            sample.pierceMinorBreakables = sample2.pierceMinorBreakables;
            sample.objectImpactEventName = sample2.objectImpactEventName;
            sample.enemyImpactEventName = sample2.enemyImpactEventName;
            sample.onDestroyEventName = sample2.onDestroyEventName;
            sample.additionalStartEventName = sample2.additionalStartEventName;
            sample.IsRadialBurstLimited = sample2.IsRadialBurstLimited;
            sample.MaxRadialBurstLimit = sample2.MaxRadialBurstLimit;
            sample.AdditionalBurstLimits = sample2.AdditionalBurstLimits;
            sample.AppliesKnockbackToPlayer = sample2.AppliesKnockbackToPlayer;
            sample.PlayerKnockbackForce = sample2.PlayerKnockbackForce;
            sample.HasDefaultTint = sample2.HasDefaultTint;
            sample.DefaultTintColor = sample2.DefaultTintColor;
            sample.IsCritical = sample2.IsCritical;
            sample.BlackPhantomDamageMultiplier = sample2.BlackPhantomDamageMultiplier;
            sample.PenetratesInternalWalls = sample2.PenetratesInternalWalls;
            sample.neverMaskThis = sample2.neverMaskThis;
            sample.isFakeBullet = sample2.isFakeBullet;
            sample.CanBecomeBlackBullet = sample2.CanBecomeBlackBullet;
            sample.TrailRenderer = sample2.TrailRenderer;
            sample.CustomTrailRenderer = sample2.CustomTrailRenderer;
            sample.ParticleTrail = sample2.ParticleTrail;
            sample.DelayedDamageToExploders = sample2.DelayedDamageToExploders;
            sample.OnHitEnemy = sample2.OnHitEnemy;
            sample.OnWillKillEnemy = sample2.OnWillKillEnemy;
            sample.OnBecameDebris = sample2.OnBecameDebris;
            sample.OnBecameDebrisGrounded = sample2.OnBecameDebrisGrounded;
            sample.IsBlackBullet = sample2.IsBlackBullet;
            sample.statusEffectsToApply = sample2.statusEffectsToApply;
            sample.AdditionalScaleMultiplier = sample2.AdditionalScaleMultiplier;
            sample.ModifyVelocity = sample2.ModifyVelocity;
            sample.CurseSparks = sample2.CurseSparks;
            sample.PreMoveModifiers = sample2.PreMoveModifiers;
            sample.OverrideMotionModule = sample2.OverrideMotionModule;
            sample.Shooter = sample2.Shooter;
            sample.Owner = sample2.Owner;
            sample.Speed = sample2.Speed;
            sample.Direction = sample2.Direction;
            sample.DestroyMode = sample2.DestroyMode;
            sample.Inverted = sample2.Inverted;
            sample.LastVelocity = sample2.LastVelocity;
            sample.ManualControl = sample2.ManualControl;
            sample.ForceBlackBullet = sample2.ForceBlackBullet;
            sample.IsBulletScript = sample2.IsBulletScript;
            sample.OverrideTrailPoint = sample2.OverrideTrailPoint;
            sample.SkipDistanceElapsedCheck = sample2.SkipDistanceElapsedCheck;
            sample.ImmuneToBlanks = sample2.ImmuneToBlanks;
            sample.ImmuneToSustainedBlanks = sample2.ImmuneToSustainedBlanks;
            sample.ForcePlayerBlankable = sample2.ForcePlayerBlankable;
            sample.IsReflectedBySword = sample2.IsReflectedBySword;
            sample.LastReflectedSlashId = sample2.LastReflectedSlashId;
            sample.TrailRendererController = sample2.TrailRendererController;
            sample.braveBulletScript = sample2.braveBulletScript;
            sample.TrapOwner = sample2.TrapOwner;
            sample.SuppressHitEffects = sample2.SuppressHitEffects;
            UnityEngine.Object.Destroy(sample2);
            return sample;
        }
        public static void DumpCollection(tk2dSpriteCollectionData collection)
        {
            string collectionName = string.IsNullOrEmpty(collection.name) ? collection.gameObject.name + "_Collection" : collection.name;

            tk2dSpriteDefinition def;
            string defName;
            Material material;
            Texture2D texture, output;
            int width, height, minX, minY, maxX, maxY, w, h;
            Vector2[] uvs;
            Color[] pixels;
            for (int i = 0; i < collection.spriteDefinitions.Length; i++)
            {
                def = collection.spriteDefinitions[i];
                if (def == null) continue;


                defName = string.IsNullOrEmpty(def.name) ? collectionName + "_" + i : def.name;
                material = def.material == null ? def.materialInst : def.material;
                if (material == null || material.mainTexture == null)
                {
                    GungeonAPI.ShrineTools.PrintError($"Failed to dump {defName} in {collectionName}: No valid material");
                    continue;
                }

                texture = (Texture2D)material.mainTexture.GetReadable();
                width = texture.width;
                height = texture.height;

                uvs = def.uvs;
                if (def.uvs == null || def.uvs.Length < 4)
                {
                    GungeonAPI.ShrineTools.PrintError($"Failed to dump {defName} in {collectionName}: Invalid UV's");
                    continue;
                }

                minX = Mathf.RoundToInt(uvs[0].x * width);
                minY = Mathf.RoundToInt(uvs[0].y * height);
                maxX = Mathf.RoundToInt(uvs[3].x * width);
                maxY = Mathf.RoundToInt(uvs[3].y * height);

                w = maxX - minX;
                h = maxY - minY;
                if (w <= 0 || h <= 0)
                {
                    GungeonAPI.ShrineTools.ExportTexture(new Texture2D(1, 1) { name = defName });
                    continue;
                };

                pixels = texture.GetPixels(minX, minY, w, h);

                output = new Texture2D(w, h);
                output.SetPixels(pixels);
                output.Apply();
                if (def.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
                {
                    output = output.Rotated().Flipped();
                }
                output.name = def.name;
                PlanetsideModule.Log(output.name, PlanetsideModule.TEXT_COLOR);
                GungeonAPI.ShrineTools.ExportTexture(output, "SpriteDump/" + collectionName);



            }
        }
        public static Texture2D Rotated(this Texture2D texture, bool clockwise = false)
        {
            Color32[] original = texture.GetPixels32();
            Color32[] rotated = new Color32[original.Length];
            int w = texture.width;
            int h = texture.height;

            int iRotated, iOriginal;

            for (int j = 0; j < h; ++j)
            {
                for (int i = 0; i < w; ++i)
                {
                    iRotated = (i + 1) * h - j - 1;
                    iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                    rotated[iRotated] = original[iOriginal];
                }
            }

            Texture2D rotatedTexture = new Texture2D(h, w);
            rotatedTexture.SetPixels32(rotated);
            rotatedTexture.Apply();
            return rotatedTexture;
        }

        public static Texture2D Flipped(this Texture2D texture, bool horizontal = true)
        {
            int w = texture.width;
            int h = texture.height;

            Texture2D output = new Texture2D(w, h);
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    output.SetPixel(i, j, texture.GetPixel(w - i - 1, j));
                }
            }
            output.Apply();
            return output;
        }
        public static Texture GetReadable(this Texture texture)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(
                    texture.width,
                    texture.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit(texture, tmp);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;

            Texture2D output = new Texture2D(texture.width, texture.height);
            output.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            output.Apply();
            RenderTexture.active = previous;

            return output;
        }


        public static VFXPool CreateMuzzleflash(string name, List<string> spriteNames, int fps, List<IntVector2> spriteSizes, List<tk2dBaseSprite.Anchor> anchors, List<Vector2> manualOffsets, bool orphaned, bool attached, bool persistsOnDeath,
            bool usesZHeight, float zHeight, VFXAlignment alignment, bool destructible, List<float> emissivePowers, List<Color> emissiveColors)
        {
            VFXPool pool = new VFXPool();
            pool.type = VFXPoolType.All;
            VFXComplex complex = new VFXComplex();
            VFXObject vfObj = new VFXObject();
            GameObject obj = new GameObject(name);
            obj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            tk2dSprite sprite = obj.AddComponent<tk2dSprite>();
            tk2dSpriteAnimator animator = obj.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            clip.fps = fps;
            clip.frames = new tk2dSpriteAnimationFrame[0];
            for (int i = 0; i < spriteNames.Count; i++)
            {
                string spriteName = spriteNames[i];
                IntVector2 spriteSize = spriteSizes[i];
                tk2dBaseSprite.Anchor anchor = anchors[i];
                Vector2 manualOffset = manualOffsets[i];
                float emissivePower = emissivePowers[i];
                Color emissiveColor = emissiveColors[i];
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteId = OtherTools.VFXCollection.GetSpriteIdByName(spriteName);
                tk2dSpriteDefinition def = OtherTools.SetupDefinitionForShellSprite(spriteName, frame.spriteId, spriteSize.x, spriteSize.y);
                def.ConstructOffsetsFromAnchor(anchor, def.position3);
                def.MakeOffset(manualOffset);
                def.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                def.material.SetFloat("_EmissiveColorPower", emissivePower);
                def.material.SetColor("_EmissiveColor", emissiveColor);
                def.materialInst.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                def.materialInst.SetFloat("_EmissiveColorPower", emissivePower);
                def.materialInst.SetColor("_EmissiveColor", emissiveColor);
                frame.spriteCollection = OtherTools.VFXCollection;
                clip.frames = clip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            sprite.renderer.material.SetFloat("_EmissiveColorPower", emissivePowers[0]);
            sprite.renderer.material.SetColor("_EmissiveColor", emissiveColors[0]);
            clip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            clip.name = "start";
            animator.spriteAnimator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animator.spriteAnimator.Library.clips = new tk2dSpriteAnimationClip[] { clip };
            animator.spriteAnimator.Library.enabled = true;
            SpriteAnimatorKiller kill = animator.gameObject.AddComponent<SpriteAnimatorKiller>();
            kill.fadeTime = -1f;
            kill.animator = animator;
            kill.delayDestructionTime = -1f;
            vfObj.orphaned = orphaned;
            vfObj.attached = attached;
            vfObj.persistsOnDeath = persistsOnDeath;
            vfObj.usesZHeight = usesZHeight;
            vfObj.zHeight = zHeight;
            vfObj.alignment = alignment;
            vfObj.destructible = destructible;
            vfObj.effect = obj;
            complex.effects = new VFXObject[] { vfObj };
            pool.effects = new VFXComplex[] { complex };
            animator.playAutomatically = true;
            animator.DefaultClipId = animator.GetClipIdByName("start");
            return pool;
        }
        public static tk2dSpriteCollectionData VFXCollection
        {
            get
            {
                return (PickupObjectDatabase.GetById(95) as Gun).clipObject.GetComponent<tk2dBaseSprite>().Collection;
            }
        }
        public static tk2dSpriteDefinition SetupDefinitionForShellSprite(string name, int id, int pixelWidth, int pixelHeight, tk2dSpriteDefinition overrideToCopyFrom = null)
        {
            float thing = 14;
            float trueWidth = (float)pixelWidth / thing;
            float trueHeight = (float)pixelHeight / thing;
            tk2dSpriteDefinition def = overrideToCopyFrom ?? OtherTools.VFXCollection.inst.spriteDefinitions[(PickupObjectDatabase.GetById(202) as Gun).shellCasing.GetComponent<tk2dBaseSprite>().spriteId].CopyDefinitionFrom();
            def.boundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
            def.boundsDataExtents = new Vector3(trueWidth, trueHeight, 0f);
            def.untrimmedBoundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
            def.untrimmedBoundsDataExtents = new Vector3(trueWidth, trueHeight, 0f);
            def.position0 = new Vector3(0f, 0f, 0f);
            def.position1 = new Vector3(0f + trueWidth, 0f, 0f);
            def.position2 = new Vector3(0f, 0f + trueHeight, 0f);
            def.position3 = new Vector3(0f + trueWidth, 0f + trueHeight, 0f);
            def.name = name;
            OtherTools.VFXCollection.spriteDefinitions[id] = def;
            return def;
        }
        //==========================================================================================================================================================
        //==========================================================================================================================================================
        //==========================================================================================================================================================
        public static FieldInfo ProjectileHealthHaverHitCountInfo = typeof(Projectile).GetField("m_healthHaverHitCount", AnyBindingFlags);
        public static MethodInfo ProjectileHandleDelayedDamageInfo = typeof(Projectile).GetMethod("HandleDelayedDamage", AnyBindingFlags);

        public static BindingFlags AnyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        //==========================================================================================================================================================
        //==========================================================================================================================================================
        //==========================================================================================================================================================
        public static bool verbose = true;
        private static string defaultLog = Path.Combine(ETGMod.ResourcesDirectory, "PSOG.txt");
        public static string modID = "PSOG";


        public static List<string> PastNames = new List<string>
        {
            "fs_convict",
            "fs_pilot",
            "fs_bullet",
            "fs_soldier",
            "fs_guide",
            "fs_robot"
        };

        public static List<string> BossBlackList = new List<string>
        {
            "39de9bd6a863451a97906d949c103538",
            "fa6a7ac20a0e4083a4c2ee0d86f8bbf7",
            "47bdfec22e8e4568a619130a267eab5b",
            "ea40fcc863d34b0088f490f4e57f8913",
            "c00390483f394a849c36143eb878998f",
            "ec6b674e0acd4553b47ee94493d66422",
            "ffca09398635467da3b1f4a54bcfda80",
            "1b5810fafbec445d89921a4efb4e42b7",
            "4b992de5b4274168a8878ef9bf7ea36b",
            "c367f00240a64d5d9f3c26484dc35833",
            "da797878d215453abba824ff902e21b4",
            "5729c8b5ffa7415bb3d01205663a33ef",
            "fa76c8cfdf1c4a88b55173666b4bc7fb",
            "8b0dd96e2fe74ec7bebc1bc689c0008a",
            "5e0af7f7d9de4755a68d2fd3bbc15df4",
            "9189f46c47564ed588b9108965f975c9",
            "6868795625bd46f3ae3e4377adce288b",
            "4d164ba3f62648809a4a82c90fc22cae",
            "6c43fddfd401456c916089fdd1c99b1c",
            "3f11bbbc439c4086a180eb0fb9990cb4",
            "f3b04a067a65492f8b279130323b41f0",
            "41ee1c8538e8474a82a74c4aff99c712",
            "465da2bb086a4a88a803f79fe3a27677",
            "05b8afe0b6cc4fffa9dc6036fa24c8ec",
            "cd88c3ce60c442e9aa5b3904d31652bc",
            "68a238ed6a82467ea85474c595c49c6e",
            "7c5d5f09911e49b78ae644d2b50ff3bf",
            "76bc43539fc24648bff4568c75c686d1",
            "0ff278534abb4fbaaa65d3f638003648",
            "6ad1cafc268f4214a101dca7af61bc91",
            "14ea47ff46b54bb4a98f91ffcffb656d",
            "shellrax",
            "Bullet_Banker",
            "Fungannon",
            "Ophanaim",
            "annihichamber"

        };
        public static void Init()
        {
            if (File.Exists(defaultLog)) File.Delete(defaultLog);
        }
        public static bool OwnerHasSynergy(this Gun gun, string synergyName)
        {
            return gun.CurrentOwner is PlayerController && (gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy(synergyName);
        }

        public static void PrintNoID<T>(T obj, string color = "FFFFFF", bool force = false)
        {
            if (verbose || force)
            {
                string[] lines = obj.ToString().Split('\n');
                foreach (var line in lines)
                    LogToConsole($"<color=#{color}> {line}</color>");
            }

            Log(obj.ToString());
        }
        public static void LogToConsole(string message)
        {
            message.Replace("\t", "    ");
            ETGModConsole.Log(message);
        }
        public static void Log<T>(T obj)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(ETGMod.ResourcesDirectory, defaultLog), true))
            {
                writer.WriteLine(obj.ToString());
            }
        }

        public static void Notify(string header, string text, string spriteID, UINotificationController.NotificationColor color = UINotificationController.NotificationColor.SILVER)
        {
            tk2dSpriteCollectionData encounterIconCollection = AmmonomiconController.Instance.EncounterIconCollection;
            int spriteIdByName = encounterIconCollection.GetSpriteIdByName(spriteID);
            GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, encounterIconCollection, spriteIdByName, color, false, false);
        }
        public static void ApplyStat(PlayerController player, PlayerStats.StatType statType, float amountToApply, StatModifier.ModifyMethod modifyMethod)
        {
            player.stats.RecalculateStats(player, false, false);
            StatModifier statModifier = new StatModifier()
            {
                statToBoost = statType,
                amount = amountToApply,
                modifyType = modifyMethod
            };
            player.ownerlessStatModifiers.Add(statModifier);
            player.stats.RecalculateStats(player, false, false);
        }
        public static bool Randomizer(float value)
        {
            return UnityEngine.Random.value > value;
        }


        public static void AnimateProjectile(this Projectile proj, List<string> names, int fps, bool loops, List<IntVector2> pixelSizes, List<bool> lighteneds, List<tk2dBaseSprite.Anchor> anchors, List<bool> anchorsChangeColliders,
            List<bool> fixesScales, List<Vector3?> manualOffsets, List<IntVector2?> overrideColliderPixelSizes, List<IntVector2?> overrideColliderOffsets, List<Projectile> overrideProjectilesToCopyFrom, string clipName = "idle")
        {
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            clip.name = clipName;
            clip.fps = fps;
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
            for (int i = 0; i < names.Count; i++)
            {
                string name = names[i];
                IntVector2 pixelSize = pixelSizes[i];
                IntVector2? overrideColliderPixelSize = overrideColliderPixelSizes[i];
                IntVector2? overrideColliderOffset = overrideColliderOffsets[i];
                Vector3? manualOffset = manualOffsets[i];
                bool anchorChangesCollider = anchorsChangeColliders[i];
                bool fixesScale = fixesScales[i];
                if (!manualOffset.HasValue)
                {
                    manualOffset = new Vector2?(Vector2.zero);
                }
                tk2dBaseSprite.Anchor anchor = anchors[i];
                bool lightened = lighteneds[i];
                Projectile overrideProjectileToCopyFrom = overrideProjectilesToCopyFrom[i];
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteId = ETGMod.Databases.Items.ProjectileCollection.inst.GetSpriteIdByName(name);
                frame.spriteCollection = ETGMod.Databases.Items.ProjectileCollection;
                frames.Add(frame);
                int? overrideColliderPixelWidth = null;
                int? overrideColliderPixelHeight = null;
                if (overrideColliderPixelSize.HasValue)
                {
                    overrideColliderPixelWidth = overrideColliderPixelSize.Value.x;
                    overrideColliderPixelHeight = overrideColliderPixelSize.Value.y;
                }
                int? overrideColliderOffsetX = null;
                int? overrideColliderOffsetY = null;
                if (overrideColliderOffset.HasValue)
                {
                    overrideColliderOffsetX = overrideColliderOffset.Value.x;
                    overrideColliderOffsetY = overrideColliderOffset.Value.y;
                }
                tk2dSpriteDefinition def = GunTools.SetupDefinitionForProjectileSprite(name, frame.spriteId, pixelSize.x, pixelSize.y, lightened, overrideColliderPixelWidth, overrideColliderPixelHeight, overrideColliderOffsetX, overrideColliderOffsetY,
                    overrideProjectileToCopyFrom);
                def.ConstructOffsetsFromAnchor(anchor, def.position3, fixesScale, anchorChangesCollider);
                def.position0 += manualOffset.Value;
                def.position1 += manualOffset.Value;
                def.position2 += manualOffset.Value;
                def.position3 += manualOffset.Value;
                if (i == 0)
                {
                    proj.GetAnySprite().SetSprite(frame.spriteCollection, frame.spriteId);
                }
            }
            clip.wrapMode = loops ? tk2dSpriteAnimationClip.WrapMode.Loop : tk2dSpriteAnimationClip.WrapMode.Once;
            clip.frames = frames.ToArray();
            if (proj.sprite.spriteAnimator == null)
            {
                proj.sprite.spriteAnimator = proj.sprite.gameObject.AddComponent<tk2dSpriteAnimator>();
            }
            proj.sprite.spriteAnimator.playAutomatically = true;
            bool flag = proj.sprite.spriteAnimator.Library == null;
            if (flag)
            {
                proj.sprite.spriteAnimator.Library = proj.sprite.spriteAnimator.gameObject.AddComponent<tk2dSpriteAnimation>();
                proj.sprite.spriteAnimator.Library.clips = new tk2dSpriteAnimationClip[0];
                proj.sprite.spriteAnimator.Library.enabled = true;
            }
            proj.sprite.spriteAnimator.Library.clips = proj.sprite.spriteAnimator.Library.clips.Concat(new tk2dSpriteAnimationClip[] { clip }).ToArray();
            proj.sprite.spriteAnimator.DefaultClipId = proj.sprite.spriteAnimator.Library.GetClipIdByName(clipName);
            proj.sprite.spriteAnimator.deferNextStartClip = false;
        }
        public static AIActor SetupAIActorDummy(string name, IntVector2 colliderOffset, IntVector2 colliderDimensions)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            SpeculativeRigidbody speculativeRigidbody = gameObject.AddComponent<tk2dSprite>().SetUpSpeculativeRigidbody(colliderOffset, colliderDimensions);
            PixelCollider pixelCollider = new PixelCollider();
            pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            pixelCollider.CollisionLayer = CollisionLayer.EnemyCollider;
            pixelCollider.ManualWidth = colliderDimensions.x;
            pixelCollider.ManualHeight = colliderDimensions.y;
            pixelCollider.ManualOffsetX = colliderOffset.x;
            pixelCollider.ManualOffsetY = colliderOffset.y;
            speculativeRigidbody.PixelColliders.Add(pixelCollider);
            speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyCollider;
            AIActor result = gameObject.AddComponent<AIActor>();
            HealthHaver healthHaver = gameObject.AddComponent<HealthHaver>();
            healthHaver.SetHealthMaximum(10f, null, false);
            healthHaver.ForceSetCurrentHealth(10f);
            BehaviorSpeculator behaviorSpeculator = gameObject.AddComponent<BehaviorSpeculator>();
            ((ISerializedObject)behaviorSpeculator).SerializedObjectReferences = new List<UnityEngine.Object>(0);
            ((ISerializedObject)behaviorSpeculator).SerializedStateKeys = new List<string>
            {
                "OverrideBehaviors",
                "OtherBehaviors",
                "TargetBehaviors",
                "AttackBehaviors",
                "MovementBehaviors"
            };
            ((ISerializedObject)behaviorSpeculator).SerializedStateValues = new List<string>
            {
                "",
                "",
                "",
                "",
                ""
            };
            return result;
        }

        public static void DisableSuperTinting(AIActor actor)
        {
            Material mat = actor.sprite.renderer.material;
            mat.mainTexture = actor.sprite.renderer.material.mainTexture;
            mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            mat.DisableKeyword("BRIGHTNESS_CLAMP_OFF");
        }
        public static void AddPassiveStatModifier(this Gun gun, PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod modifyMethod)
        {
            gun.passiveStatModifiers = gun.passiveStatModifiers.Concat(new StatModifier[]
            {
                new StatModifier
                {
                    statToBoost = statType,
                    amount = amount,
                    modifyType = modifyMethod
                }
            }).ToArray<StatModifier>();
        }
        public class CompanionisedEnemyBulletModifiers : BraveBehaviour //----------------------------------------------------------------------------------------------
        {
            public CompanionisedEnemyBulletModifiers()
            {
                this.scaleDamage = false;
                this.scaleSize = false;
                this.scaleSpeed = false;
                this.doPostProcess = false;
                this.baseBulletDamage = 10f;
                this.TintBullets = false;
                this.TintColor = Color.grey;
                this.jammedDamageMultiplier = 2f;
            }
            public void Start()
            {
                enemy = base.aiActor;
                AIBulletBank bulletBank2 = enemy.bulletBank;
                foreach (AIBulletBank.Entry bullet in bulletBank2.Bullets)
                {
                    SpawnManager.PoolManager.Remove(bullet.BulletObject.transform);
                    bullet.BulletObject.GetComponent<Projectile>().BulletScriptSettings.preventPooling = true;
                }
                if (enemy.aiShooter != null)
                {
                    AIShooter aiShooter = enemy.aiShooter;
                    aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(this.PostProcessSpawnedEnemyProjectiles));
                }

                if (enemy.bulletBank != null)
                {
                    AIBulletBank bulletBank = enemy.bulletBank;
                    bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.PostProcessSpawnedEnemyProjectiles));
                }
            }

            private void PostProcessSpawnedEnemyProjectiles(Projectile proj)
            {
                if (TintBullets) { proj.AdjustPlayerProjectileTint(this.TintColor, 1); }
                if (enemy != null)
                {
                    if (enemy.aiActor != null)
                    {
                        proj.baseData.damage = baseBulletDamage;
                        if (enemyOwner != null)
                        {
                            //ETGModConsole.Log("Companionise: enemyOwner is not null");
                            if (scaleDamage) proj.baseData.damage *= enemyOwner.stats.GetStatValue(PlayerStats.StatType.Damage);
                            if (scaleSize)
                            {
                                proj.RuntimeUpdateScale(enemyOwner.stats.GetStatValue(PlayerStats.StatType.PlayerBulletScale));
                            }
                            if (scaleSpeed)
                            {
                                proj.baseData.speed *= enemyOwner.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                                proj.UpdateSpeed();
                            }
                            //ETGModConsole.Log("Damage: " + proj.baseData.damage);
                            if (doPostProcess) enemyOwner.DoPostProcessProjectile(proj);
                        }
                        if (enemy.aiActor.IsBlackPhantom) { proj.baseData.damage = baseBulletDamage * jammedDamageMultiplier; }
                    }
                }
                else { ETGModConsole.Log("Shooter is NULL"); }
            }

            private AIActor enemy;
            public PlayerController enemyOwner;
            public bool scaleDamage;
            public bool scaleSize;
            public bool scaleSpeed;
            public bool doPostProcess;
            public float baseBulletDamage;
            public float jammedDamageMultiplier;
            public bool TintBullets;
            public Color TintColor;

        }
        public static AIBulletBank CopyAIBulletBank(AIBulletBank bank)
        {
            UnityEngine.GameObject obj = new UnityEngine.GameObject();
            AIBulletBank newBank = obj.GetOrAddComponent<AIBulletBank>();
            newBank.Bullets = bank.Bullets;
            newBank.FixedPlayerPosition = bank.FixedPlayerPosition;
            newBank.OnProjectileCreated = bank.OnProjectileCreated;
            newBank.OverrideGun = bank.OverrideGun;
            newBank.rampTime = bank.rampTime;
            newBank.OnProjectileCreatedWithSource = bank.OnProjectileCreatedWithSource;
            newBank.rampBullets = bank.rampBullets;
            newBank.transforms = bank.transforms;
            newBank.useDefaultBulletIfMissing = bank.useDefaultBulletIfMissing;
            newBank.rampStartHeight = bank.rampStartHeight;
            newBank.SpecificRigidbodyException = bank.SpecificRigidbodyException;
            newBank.PlayShells = bank.PlayShells;
            newBank.PlayAudio = bank.PlayAudio;
            newBank.PlayVfx = bank.PlayVfx;
            newBank.CollidesWithEnemies = bank.CollidesWithEnemies;
            newBank.FixedPlayerRigidbodyLastPosition = bank.FixedPlayerRigidbodyLastPosition;
            newBank.ActorName = bank.ActorName;
            newBank.TimeScale = bank.TimeScale;
            newBank.SuppressPlayerVelocityAveraging = bank.SuppressPlayerVelocityAveraging;
            newBank.FixedPlayerRigidbody = bank.FixedPlayerRigidbody;
            return newBank;
        }
        public class EasyTrailComponent: BraveBehaviour //----------------------------------------------------------------------------------------------
        {
            public EasyTrailComponent()
            {
                //=====
                this.TrailPos = new Vector3(0, 0, 0);
                //======
                this.BaseColor = Color.red;
                this.StartColor = Color.red;
                this.EndColor = Color.white;
                //======
                this.LifeTime = 1f;
                //======
                this.StartWidth = 1;
                this.EndWidth = 0;

            }
            /// <summary>
            /// Lets you add a trail to your projectile.    
            /// </summary>
            /// <param name="TrailPos">Where the trail attaches its center-point to. You can input a custom Vector3 but its best to use the base preset. (Namely"projectile.transform.position;").</param>
            /// <param name="BaseColor">The Base Color of your trail.</param>
            /// <param name="StartColor">The Starting color of your trail.</param>
            /// <param name="EndColor">The End color of your trail. Having it different to the StartColor will make it transition from the Starting/Base Color to its End Color during its lifetime.</param>
            /// <param name="LifeTime">How long your trail lives for.</param>
            /// <param name="StartWidth">The Starting Width of your Trail.</param>
            /// <param name="EndWidth">The Ending Width of your Trail. Not sure why youd want it to be something other than 0, but the options there.</param>
            public void Start()
            {
                obj = base.gameObject;
                {
                    TrailRenderer tr;
                    var tro = obj.AddChild("trail object");
                    tro.transform.position = obj.transform.position;
                    tro.transform.localPosition = TrailPos;

                    tr = tro.AddComponent<TrailRenderer>();
                    tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    tr.receiveShadows = false;
                    var mat = new Material(Shader.Find("Sprites/Default"));
                    mat.mainTexture = _gradTexture;
                    tr.material = mat;
                    tr.minVertexDistance = 0.1f;
                    //======
                    mat.SetColor("_Color", BaseColor);
                    tr.startColor = StartColor;
                    tr.endColor = EndColor;
                    //======
                    tr.time = LifeTime;
                    //======
                    tr.startWidth = StartWidth;
                    tr.endWidth = EndWidth;
                    tr.autodestruct = false;
                    //tr.
                }
                
            }

            public Texture _gradTexture;
            private GameObject obj;

            public Vector2 TrailPos;
            public Color BaseColor;
            public Color StartColor;
            public Color EndColor;
            public float LifeTime;
            public float StartWidth;
            public float EndWidth;

        }


        public class EasyTrailOnEnemy : BraveBehaviour //----------------------------------------------------------------------------------------------
        {
            public EasyTrailOnEnemy()
            {
                //=====
                this.TrailPos = new Vector3(0, 0, 0);
                //======
                this.BaseColor = Color.red;
                this.StartColor = Color.red;
                this.EndColor = Color.white;
                //======
                this.LifeTime = 1f;
                //======
                this.StartWidth = 1;
                this.EndWidth = 0;
                this.castingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            }


            public void SetMode(UnityEngine.Rendering.ShadowCastingMode mode)
            { 
                this.castingMode = mode;    
            }
                /// <summary>
                /// Lets you add a trail to your projectile.    
                /// </summary>
                /// <param name="TrailPos">Where the trail attaches its center-point to. You can input a custom Vector3 but its best to use the base preset. (Namely"projectile.transform.position;").</param>
                /// <param name="BaseColor">The Base Color of your trail.</param>
                /// <param name="StartColor">The Starting color of your trail.</param>
                /// <param name="EndColor">The End color of your trail. Having it different to the StartColor will make it transition from the Starting/Base Color to its End Color during its lifetime.</param>
                /// <param name="LifeTime">How long your trail lives for.</param>
                /// <param name="StartWidth">The Starting Width of your Trail.</param>
                /// <param name="EndWidth">The Ending Width of your Trail. Not sure why youd want it to be something other than 0, but the options there.</param>
            public void Start()
            {
                AI = base.aiActor;
                {
                    TrailRenderer tr;
                    var tro = base.aiActor.gameObject.AddChild("trail object");
                    tro.transform.position = base.aiActor.transform.position;
                    tro.transform.localPosition = TrailPos;

                    tr = tro.AddComponent<TrailRenderer>();
                    tr.shadowCastingMode = castingMode;
                    tr.receiveShadows = false;
                    var mat = new Material(Shader.Find("Sprites/Default"));
                    mat.mainTexture = _gradTexture;
                    tr.material = mat;
                    tr.minVertexDistance = 0.1f;
                    //======
                    mat.SetColor("_Color", BaseColor);
                    tr.startColor = StartColor;
                    tr.endColor = EndColor;
                    //======
                    tr.time = LifeTime;
                    //======
                    tr.startWidth = StartWidth;
                    tr.endWidth = EndWidth;
                    tr.enabled = true;
                }

            }

            public Texture _gradTexture;
            private AIActor AI;

            public Vector2 TrailPos;
            public Color BaseColor;
            public Color StartColor;
            public Color EndColor;
            public float LifeTime;
            public float StartWidth;
            public float EndWidth;
            public UnityEngine.Rendering.ShadowCastingMode castingMode;

        }

        public class UmbraBulletComponent : BraveBehaviour //----------------------------------------------------------------------------------------------
        {
            public UmbraBulletComponent()
            {
                this.doPostProcess = false;
                this.TintBullets = true;
                this.TintColor = Color.grey;
            }
            public void Start()
            {
                enemy = base.aiActor;
                AIBulletBank bulletBank2 = enemy.bulletBank;
                foreach (AIBulletBank.Entry bullet in bulletBank2.Bullets)
                {
                    bullet.BulletObject.GetComponent<Projectile>().BulletScriptSettings.preventPooling = true;
                }
                if (enemy.aiShooter != null)
                {
                    AIShooter aiShooter = enemy.aiShooter;
                    aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(this.PostProcessSpawnedEnemyProjectiles));
                }

                if (enemy.bulletBank != null)
                {
                    AIBulletBank bulletBank = enemy.bulletBank;
                    bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.PostProcessSpawnedEnemyProjectiles));
                }
            }

            private void PostProcessSpawnedEnemyProjectiles(Projectile proj)
            {
               // if (TintBullets) { proj.Ad(this.TintColor, 1); }
                if (enemy != null)
                {
                    if (enemy.aiActor != null)
                    {
                        ImprovedAfterImage yeah = proj.gameObject.AddComponent<ImprovedAfterImage>();
                        yeah.dashColor = Color.black;
                        yeah.spawnShadows = true;
                        yeah.shadowTimeDelay = 0.01f;
                        yeah.shadowLifetime = 0.75f;
                    }
                }
                else { ETGModConsole.Log("Shooter is NULL"); }
            }

            private AIActor enemy;
            public bool doPostProcess;
            public bool TintBullets;
            public Color TintColor;

        }
    }
    

    public static class AnimateBullet//----------------------------------------------------------------------------------------------
    {
        public static List<T> ConstructListOfSameValues<T>(T value, int length)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < length; i++)
            {
                list.Add(value);
            }
            return list;
        }
    }

}


namespace Planetside
{
    public class fuck
    {

        //GameManager.Instance.StartCoroutine(fuck.DoDistortionWaveLocal(obj.transform.position, 1.8f, 0.2f, 10f, 0.4f));

        public static IEnumerator DoReverseDistortionWaveLocal(Vector2 center, float distortionIntensity, float distortionRadius, float maxRadius, float duration)
        {
            Material distMaterial = new Material(ShaderCache.Acquire("Brave/Internal/DistortionWave"));
            Vector4 distortionSettings = fuck.GetCenterPointInScreenUV(center, distortionIntensity, distortionRadius);
            distMaterial.SetVector("_WaveCenter", distortionSettings);
            Pixelator.Instance.RegisterAdditionalRenderPass(distMaterial);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                float t = elapsed / duration;
                t = BraveMathCollege.LinearToSmoothStepInterpolate(0f, 1f, t);
                distortionSettings = fuck.GetCenterPointInScreenUV(center, distortionIntensity, distortionRadius);
                distortionSettings.w = Mathf.Lerp(distortionSettings.w, 0f, t);
                distMaterial.SetVector("_WaveCenter", distortionSettings);
                float currentRadius = Mathf.Lerp(maxRadius, 0f, t);
                distMaterial.SetFloat("_DistortProgress", (currentRadius / (maxRadius * 5)));
                yield return null;
            }
            Pixelator.Instance.DeregisterAdditionalRenderPass(distMaterial);
            UnityEngine.Object.Destroy(distMaterial);

        }
        private static Vector4 GetCenterPointInScreenUV(Vector2 centerPoint, float dIntensity, float dRadius)
        {
            Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
            return new Vector4(vector.x, vector.y, dRadius, dIntensity);
        }
        public AIActor GetNearestEnemy(List<AIActor> activeEnemies, Vector2 position, out float nearestDistance, string[] filter)
        {
            AIActor aiactor = null;
            nearestDistance = float.MaxValue;
            bool flag = activeEnemies == null;
            bool flag2 = flag;
            bool flag3 = flag2;
            bool flag4 = flag3;
            bool flag5 = flag4;
            AIActor result;
            if (flag5)
            {
                result = null;
            }
            else
            {
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    AIActor aiactor2 = activeEnemies[i];
                    bool flag6 = !aiactor2.healthHaver.IsDead && aiactor2.healthHaver.IsVulnerable;
                    bool flag7 = flag6;
                    if (flag7)
                    {
                        bool flag8 = filter == null || !filter.Contains(aiactor2.EnemyGuid);
                        bool flag9 = flag8;
                        if (flag9)
                        {
                            float num = Vector2.Distance(position, aiactor2.CenterPosition);
                            bool flag10 = num < nearestDistance;
                            bool flag11 = flag10;
                            if (flag11)
                            {
                                nearestDistance = num;
                                aiactor = aiactor2;
                            }
                        }
                    }
                }
                result = aiactor;
            }
            return result;
        }
    }
}
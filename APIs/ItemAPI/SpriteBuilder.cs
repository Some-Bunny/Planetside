﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

using ItemAPI;
using Planetside;

namespace ItemAPI
{
    public static class SpriteBuilder
    {
        public static tk2dSpriteCollectionData itemCollection = PickupObjectDatabase.GetById(155).sprite.Collection;
        public static tk2dSpriteCollectionData ammonomiconCollection = AmmonomiconController.ForceInstance.EncounterIconCollection;
        private static tk2dSprite baseSprite = PickupObjectDatabase.GetById(155).GetComponent<tk2dSprite>();
        /// <summary>
        /// Returns an object with a tk2dSprite component with the 
        /// texture of a file in the sprites folder
        /// </summary>
        public static GameObject SpriteFromFile(string spriteName, GameObject obj = null)
        {
            string filename = spriteName.Replace(".png", "");

            var texture = ResourceExtractor.GetTextureFromFile(filename);
            if (texture == null) return null;

            return SpriteFromTexture(texture, spriteName, obj);
        }

        /// <summary>
        /// Returns an object with a tk2dSprite component with the 
        /// texture of an embedded resource
        /// </summary>
        public static GameObject SpriteFromResource(string spriteName, GameObject obj = null, bool v = false)
        {
            string extension = !spriteName.EndsWith(".png") ? ".png" : "";
            string resourcePath = spriteName + extension;

            var texture = ResourceExtractor.GetTextureFromResource(resourcePath);
            if (texture == null) return null;

            return SpriteFromTexture(texture, resourcePath, obj);
        }



        /// <summary>
        /// Returns an object with a tk2dSprite component with the texture provided
        /// </summary>
        public static GameObject SpriteFromTexture(Texture2D texture, string spriteName, GameObject obj = null)
        {
            if (obj == null)
            {
                obj = new GameObject();
            }
            tk2dSprite sprite;
            sprite = obj.AddComponent<tk2dSprite>();

            int id = AddSpriteToCollection(spriteName, itemCollection);
            sprite.SetSprite(itemCollection, id);
            sprite.SortingOrder = 0;
            sprite.IsPerpendicular = true;
           

            obj.GetComponent<BraveBehaviour>().sprite = sprite;

            return obj;
        }

        /// <summary>
        /// Adds a sprite (from a resource) to a collection
        /// </summary>
        /// <returns>The spriteID of the defintion in the collection</returns>
        public static int AddSpriteToCollection(string resourcePath, tk2dSpriteCollectionData collection)
        {
            string extension = !resourcePath.EndsWith(".png") ? ".png" : "";
            resourcePath += extension;
            var texture = ResourceExtractor.GetTextureFromResource(resourcePath); //Get Texture

            var definition = ConstructDefinition(texture); //Generate definition
            definition.name = texture.name; //naming the definition is actually extremely important 

            return AddSpriteToCollection(definition, collection);
        }





        public static int AddSpriteToCollection(tk2dSpriteDefinition spriteDefinition, tk2dSpriteCollectionData collection)
        {
            //Add definition to collection
            var defs = collection.spriteDefinitions;
            var newDefs = defs.Concat(new tk2dSpriteDefinition[] { spriteDefinition }).ToArray();
            collection.spriteDefinitions = newDefs;

            //Reset lookup dictionary
            FieldInfo f = typeof(tk2dSpriteCollectionData).GetField("spriteNameLookupDict", BindingFlags.Instance | BindingFlags.NonPublic);
            f.SetValue(collection, null);  //Set dictionary to null
            collection.InitDictionary(); //InitDictionary only runs if the dictionary is null
            return newDefs.Length - 1;
        }


        /// <summary>
        /// Adds a sprite from a definition to a collection
        /// </summary>
        /// <returns>The spriteID of the defintion in the collection</returns>
        public static int AddSpriteToCollectionFromOtherCollection(tk2dSpriteDefinition spriteDefinition, tk2dSpriteCollectionData collectionToTakeFrom , tk2dSpriteCollectionData collection)
        {
            //Add definition to collection
            var defs = collectionToTakeFrom.spriteDefinitions;
            var newDefs = defs.Concat(new tk2dSpriteDefinition[] { spriteDefinition }).ToArray();
            collection.spriteDefinitions = newDefs;

            //Reset lookup dictionary
            FieldInfo f = typeof(tk2dSpriteCollectionData).GetField("spriteNameLookupDict", BindingFlags.Instance | BindingFlags.NonPublic);
            f.SetValue(collection, null);  //Set dictionary to null
            collection.InitDictionary(); //InitDictionary only runs if the dictionary is null
            return newDefs.Length - 1;
        }

        /// <summary>
        /// Adds a sprite definition to the Ammonomicon sprite collection
        /// </summary>
        /// <returns>The spriteID of the defintion in the ammonomicon collection</returns>
        public static int AddToAmmonomicon(tk2dSpriteDefinition spriteDefinition)
        {
            return AddSpriteToCollection(spriteDefinition, ammonomiconCollection);
        }

        public static tk2dSpriteAnimationClip AddAnimation(tk2dSpriteAnimator animator, tk2dSpriteCollectionData collection, List<int> spriteIDs,
            string clipName, tk2dSpriteAnimationClip.WrapMode wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection)//, bool Debug = false)
        {
            if (animator.Library == null)
            {
                animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
                animator.Library.clips = new tk2dSpriteAnimationClip[0];
                animator.Library.enabled = true;
               
            }

            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
            for (int i = 0; i < spriteIDs.Count; i++)
            {
                tk2dSpriteDefinition sprite = collection.spriteDefinitions[spriteIDs[i]];
                if (sprite.Valid)
                {
                    frames.Add(new tk2dSpriteAnimationFrame()
                    {
                        spriteCollection = collection,
                        spriteId = spriteIDs[i],
                        
                    });
                }        
            }

            var clip = new tk2dSpriteAnimationClip();
            clip.name = clipName;
            clip.fps = 15;
            clip.wrapMode = wrapMode;
            Array.Resize(ref animator.Library.clips, animator.Library.clips.Length + 1);
            animator.Library.clips[animator.Library.clips.Length - 1] = clip;

            clip.frames = frames.ToArray();
            return clip;
        }

        public static tk2dSpriteAnimationClip AddAnimationDebug(tk2dSpriteAnimator animator, tk2dSpriteCollectionData collection, List<int> spriteIDs,
            string clipName, tk2dSpriteAnimationClip.WrapMode wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection)
        {

            ETGModConsole.Log("Starting null checks");
            if (animator == null)
            {
                ETGModConsole.Log("animator is NULL.");
            }
            if (collection == null)
            {
                ETGModConsole.Log("collection is NULL.");
            }
            if (spriteIDs == null)
            {
                ETGModConsole.Log("spriteIDs List is NULL.");
            }
            if (clipName == null)
            {
                ETGModConsole.Log("clipName is NULL.");
            }


            ETGModConsole.Log("Starting AddAnimation of clip name: "+clipName);
            if (animator.Library == null)
            {
                ETGModConsole.Log("Library is NULL, constructing new one.");

                animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
                animator.Library.clips = new tk2dSpriteAnimationClip[0];
                animator.Library.enabled = true;
                ETGModConsole.Log("Library created.");
            }

            ETGModConsole.Log("Starting to add frames.");
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
            ETGModConsole.Log("Created new List<tk2dSpriteAnimationFrame>");

            for (int i = 0; i < spriteIDs.Count; i++)
            {
                tk2dSpriteDefinition sprite = collection.spriteDefinitions[spriteIDs[i]];

                if (collection.spriteDefinitions == null)
                {
                    ETGModConsole.Log("collection.spriteDefinitions is NULL.");
                }


                if (sprite == null) 
                {
                    ETGModConsole.Log("Frame " + i + "is NULL.");
                }

                

                if (sprite.Valid)
                {
                    if (collection == null)
                    {
                        ETGModConsole.Log("collection is NULL.");
                    }
                    frames.Add(new tk2dSpriteAnimationFrame()
                    {
                        spriteCollection = collection,
                        spriteId = spriteIDs[i],
                        
                    });
                }
                ETGModConsole.Log("Frame "+i+"of animation added");
            }
            ETGModConsole.Log("Creating new clip");

            var clip = new tk2dSpriteAnimationClip();
            clip.name = clipName;
            clip.fps = 15;
            clip.wrapMode = wrapMode;
            Array.Resize(ref animator.Library.clips, animator.Library.clips.Length + 1);
            animator.Library.clips[animator.Library.clips.Length - 1] = clip;
            ETGModConsole.Log("Adding frames");
            clip.frames = frames.ToArray();
            ETGModConsole.Log("Finsihed adding frames, returning CLIP");

            return clip;
        }



        public static tk2dSpriteAnimationClip AddAnimation(tk2dSpriteAnimator targetAnimator, tk2dSpriteCollectionData collection, List<string> spriteNameList, string clipName, tk2dSpriteAnimationClip.WrapMode wrapMode = tk2dSpriteAnimationClip.WrapMode.Once, int frameRate = 15, int loopStart = 0, float minFidgetDuration = 0.5f, float maxFidgetDuration = 1)
        {
            if (!targetAnimator.Library)
            {
                targetAnimator.Library = targetAnimator.gameObject.AddComponent<tk2dSpriteAnimation>();
                targetAnimator.Library.clips = new tk2dSpriteAnimationClip[0];
            }
            List<tk2dSpriteAnimationFrame> animationList = new List<tk2dSpriteAnimationFrame>();
            for (int i = 0; i < spriteNameList.Count; i++)
            {
                tk2dSpriteDefinition spriteDefinition = collection.GetSpriteDefinition(spriteNameList[i]);
                if (spriteDefinition != null && spriteDefinition.Valid)
                {
                    animationList.Add(
                        new tk2dSpriteAnimationFrame
                        {
                            spriteCollection = collection,
                            spriteId = collection.GetSpriteIdByName(spriteNameList[i]),
                            invulnerableFrame = false,
                            groundedFrame = true,
                            requiresOffscreenUpdate = false,
                            eventAudio = string.Empty,
                            eventVfx = string.Empty,
                            eventStopVfx = string.Empty,
                            eventLerpEmissive = false,
                            eventLerpEmissiveTime = 0.5f,
                            eventLerpEmissivePower = 30,
                            forceMaterialUpdate = false,
                            finishedSpawning = false,
                            triggerEvent = false,
                            eventInfo = string.Empty,
                            eventInt = 0,
                            eventFloat = 0,
                            eventOutline = tk2dSpriteAnimationFrame.OutlineModifier.Unspecified,
                        }
                    );
                }
            }

            if (animationList.Count <= 0)
            {
                ETGModConsole.Log("[ExpandTheGungeon] AddAnimation: ERROR! Animation list is empty! No valid sprites found in specified list!");
                return null;
            }
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip()
            {
                name = clipName,
                frames = animationList.ToArray(),
                fps = frameRate,
                wrapMode = wrapMode,
                loopStart = loopStart,
                minFidgetDuration = minFidgetDuration,
                maxFidgetDuration = maxFidgetDuration,
            };
            Array.Resize(ref targetAnimator.Library.clips, targetAnimator.Library.clips.Length + 1);
            targetAnimator.Library.clips[targetAnimator.Library.clips.Length - 1] = animationClip;
            return animationClip;
        }


        public static tk2dSpriteAnimationClip AddAnimationDebug(tk2dSpriteAnimator animator, tk2dSpriteCollectionData collection, List<int> spriteIDs,
          string clipName, tk2dSpriteAnimationClip.WrapMode wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection, bool Debug = false)
        {
            if (animator.Library == null)
            {
                animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
                animator.Library.clips = new tk2dSpriteAnimationClip[0];
                animator.Library.enabled = true;
            }

            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
            for (int i = 0; i < spriteIDs.Count; i++)
            {
                //if (Debug == true) { ETGModConsole.Log("1"); }
                tk2dSpriteDefinition sprite = collection.spriteDefinitions[spriteIDs[i]];
                if (sprite.Valid)
                {
                    //if (Debug == true) { ETGModConsole.Log("2"); }
                    frames.Add(new tk2dSpriteAnimationFrame()
                    {
                        spriteCollection = collection,
                        spriteId = spriteIDs[i]
                    });
                    //if (Debug == true) { ETGModConsole.Log("3"); }
                }

            }

            var clip = new tk2dSpriteAnimationClip();
            clip.name = clipName;
            clip.fps = 15;
            clip.wrapMode = wrapMode;
            Array.Resize(ref animator.Library.clips, animator.Library.clips.Length + 1);
            animator.Library.clips[animator.Library.clips.Length - 1] = clip;

            clip.frames = frames.ToArray();
            return clip;
        }

        public static SpeculativeRigidbody SetUpSpeculativeRigidbody(this tk2dSprite sprite, IntVector2 offset, IntVector2 dimensions)
        {
            var body = sprite.gameObject.GetOrAddComponent<SpeculativeRigidbody>();
            PixelCollider collider = new PixelCollider();
            collider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            collider.CollisionLayer = CollisionLayer.EnemyCollider;

            collider.ManualWidth = dimensions.x;
            collider.ManualHeight = dimensions.y;
            collider.ManualOffsetX = offset.x;
            collider.ManualOffsetY = offset.y;

            body.PixelColliders = new List<PixelCollider>() { collider };

            return body;
        }
        public static SpeculativeRigidbody SetUpSpeculativeRigidbody(this tk2dBaseSprite sprite, IntVector2 offset, IntVector2 dimensions)
        {
            var body = sprite.gameObject.GetOrAddComponent<SpeculativeRigidbody>();
            PixelCollider collider = new PixelCollider();
            collider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            collider.CollisionLayer = CollisionLayer.EnemyCollider;

            collider.ManualWidth = dimensions.x;
            collider.ManualHeight = dimensions.y;
            collider.ManualOffsetX = offset.x;
            collider.ManualOffsetY = offset.y;

            body.PixelColliders = new List<PixelCollider>() { collider };

            return body;
        }
        /// <summary>
        /// Constructs a new tk2dSpriteDefinition with the given texture
        /// </summary>
        /// <returns>A new sprite definition with the given texture</returns>
        public static tk2dSpriteDefinition ConstructDefinition(Texture2D texture)
        {
            RuntimeAtlasSegment ras = ETGMod.Assets.Packer.Pack(texture); //pack your resources beforehand or the outlines will turn out weird

            Material material = new Material(ShaderCache.Acquire(PlayerController.DefaultShaderName));
            material.mainTexture = ras.texture;
            //material.mainTexture = texture;

            var width = texture.width;
            var height = texture.height;

            var x = 0f;
            var y = 0f;

            var w = width / 16f;
            var h = height / 16f;

            var def = new tk2dSpriteDefinition
            {
                normals = new Vector3[] {
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
            },
                tangents = new Vector4[] {
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            },
                texelSize = new Vector2(1 / 16f, 1 / 16f),
                extractRegion = false,
                regionX = 0,
                regionY = 0,
                regionW = 0,
                regionH = 0,
                flipped = tk2dSpriteDefinition.FlipMode.None,
                complexGeometry = false,
                physicsEngine = tk2dSpriteDefinition.PhysicsEngine.Physics3D,
                colliderType = tk2dSpriteDefinition.ColliderType.None,
                collisionLayer = CollisionLayer.HighObstacle,
                position0 = new Vector3(x, y, 0f),
                position1 = new Vector3(x + w, y, 0f),
                position2 = new Vector3(x, y + h, 0f),
                position3 = new Vector3(x + w, y + h, 0f),
                material = material,
                materialInst = material,
                materialId = 0,
                //uvs = ETGMod.Assets.GenerateUVs(texture, 0, 0, width, height), //uv machine broke
                uvs = ras.uvs,
                boundsDataCenter = new Vector3(w / 2f, h / 2f, 0f),
                boundsDataExtents = new Vector3(w, h, 0f),
                untrimmedBoundsDataCenter = new Vector3(w / 2f, h / 2f, 0f),
                untrimmedBoundsDataExtents = new Vector3(w, h, 0f),
            };

            def.name = texture.name;
            return def;
        }

        public static tk2dSpriteCollectionData ConstructCollection(GameObject obj, string name)
        {
            var collection = obj.AddComponent<tk2dSpriteCollectionData>();
            UnityEngine.Object.DontDestroyOnLoad(collection);

            collection.assetName = name;
            collection.spriteCollectionGUID = name;
            collection.spriteCollectionName = name;
            collection.spriteDefinitions = new tk2dSpriteDefinition[0];
            return collection;
        }

        public static T CopyFrom<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            PropertyInfo[] pinfos = type.GetProperties();
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)//i LOVE HARDCODING i LOVE HARDCODING
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { }
                }
                else
                {
                }
            }

            FieldInfo[] finfos = type.GetFields();
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }

        public static void SetColor(this tk2dSprite sprite, Color color)
        {
            sprite.renderer.material.SetColor("_OverrideColor", color);
        }

        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().CopyFrom(toAdd) as T;
        }

        public static tk2dSpriteAnimationClip AddAnimation(tk2dSpriteAnimator animator, tk2dSpriteCollectionData collection, List<int> spriteIDs,
            string clipName, tk2dSpriteAnimationClip.WrapMode wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop, float fps = 15)
        {
            if (animator.Library == null)
            {
                animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
                animator.Library.clips = new tk2dSpriteAnimationClip[0];
                animator.Library.enabled = true;

            }

            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
            for (int i = 0; i < spriteIDs.Count; i++)
            {
                tk2dSpriteDefinition sprite = collection.spriteDefinitions[spriteIDs[i]];
                if (sprite.Valid)
                {
                    frames.Add(new tk2dSpriteAnimationFrame()
                    {
                        spriteCollection = collection,
                        spriteId = spriteIDs[i]
                    });
                }
            }

            var clip = new tk2dSpriteAnimationClip()
            {
                name = clipName,
                fps = fps,
                wrapMode = wrapMode,
            };
            Array.Resize(ref animator.Library.clips, animator.Library.clips.Length + 1);
            animator.Library.clips[animator.Library.clips.Length - 1] = clip;

            clip.frames = frames.ToArray();
            return clip;
        }

    }
}
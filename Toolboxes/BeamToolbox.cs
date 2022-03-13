using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;



namespace Planetside
{
    static class BeamToolbox
    {
        public static GameObject InstantiateAndFakeprefab(this GameObject target)
        {

            GameObject instantiatedTarget = UnityEngine.Object.Instantiate<GameObject>(target);
            instantiatedTarget.SetActive(false);
            FakePrefab.MarkAsFakePrefab(instantiatedTarget);
            UnityEngine.Object.DontDestroyOnLoad(instantiatedTarget);
            return instantiatedTarget;
        }
        public static void AddTrailToProjectile(this Projectile target, string spritePath, Vector2 colliderDimensions, Vector2 colliderOffsets, List<string> animPaths = null, int animFPS = -1, List<string> startAnimPaths = null, int startAnimFPS = -1, float timeTillAnimStart = -1, float cascadeTimer = -1, float softMaxLength = -1, bool destroyOnEmpty = false)
        {
            try
            {
                GameObject newTrailObject = new GameObject();
                newTrailObject.InstantiateAndFakeprefab();
                newTrailObject.transform.parent = target.transform;

                float convertedColliderX = colliderDimensions.x / 16f;
                float convertedColliderY = colliderDimensions.y / 16f;
                float convertedOffsetX = colliderOffsets.x / 16f;
                float convertedOffsetY = colliderOffsets.y / 16f;

                int spriteID = SpriteBuilder.AddSpriteToCollection(spritePath, ETGMod.Databases.Items.ProjectileCollection);
                tk2dTiledSprite tiledSprite = newTrailObject.GetOrAddComponent<tk2dTiledSprite>();

                tiledSprite.SetSprite(ETGMod.Databases.Items.ProjectileCollection, spriteID);
                tk2dSpriteDefinition def = tiledSprite.GetCurrentSpriteDef();
                def.colliderVertices = new Vector3[]{
                    new Vector3(convertedOffsetX, convertedOffsetY, 0f),
                    new Vector3(convertedColliderX, convertedColliderY, 0f)
                };

                def.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft);

                tk2dSpriteAnimator animator = newTrailObject.GetOrAddComponent<tk2dSpriteAnimator>();
                tk2dSpriteAnimation animation = newTrailObject.GetOrAddComponent<tk2dSpriteAnimation>();
                animation.clips = new tk2dSpriteAnimationClip[0];
                animator.Library = animation;

                TrailController trail = newTrailObject.AddComponent<TrailController>();

                //---------------- Sets up the animation for the main part of the trail
                if (animPaths != null)
                {
                    BeamToolbox.SetupBeamPart(animation, animPaths, "trail_mid", animFPS, null, null, def.colliderVertices);
                    trail.animation = "trail_mid";
                    trail.usesAnimation = true;
                }
                else
                {
                    trail.usesAnimation = false;
                }

                if (startAnimPaths != null)
                {
                    BeamToolbox.SetupBeamPart(animation, startAnimPaths, "trail_start", startAnimFPS, null, null, def.colliderVertices);
                    trail.startAnimation = "trail_start";
                    trail.usesStartAnimation = true;
                }
                else
                {
                    trail.usesStartAnimation = false;
                }

                //Trail Variables
                if (softMaxLength > 0) { trail.usesSoftMaxLength = true; trail.softMaxLength = softMaxLength; }
                if (cascadeTimer > 0) { trail.usesCascadeTimer = true; trail.cascadeTimer = cascadeTimer; }
                if (timeTillAnimStart > 0) { trail.usesGlobalTimer = true; trail.globalTimer = timeTillAnimStart; }
                trail.destroyOnEmpty = destroyOnEmpty;
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
            }
        }

        public static bool PosIsNearAnyBoneOnBeam(this BasicBeamController beam, Vector2 positionToCheck, float distance)
        {
            LinkedList<BasicBeamController.BeamBone> bones;
            bones = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam);
            foreach (BasicBeamController.BeamBone bone in bones)
            {
                Vector2 bonepos = beam.GetBonePosition(bone);
                if (Vector2.Distance(positionToCheck, bonepos) < distance) return true;
            }
            return false;
        }

        public static int GetBoneCount (this BasicBeamController beam)
        {
            if (!beam.UsesBones)
            {
                return 1;
            }
            else
            {
                LinkedList<BasicBeamController.BeamBone> bones;
                bones = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam);
                return bones.Count();
            }
        }
        public static float GetFinalBoneDirection(this BasicBeamController beam)
        {
            if (!beam.UsesBones)
            {
                return beam.Direction.ToAngle();
            }
            else
            {
                LinkedList<BasicBeamController.BeamBone> bones;
                bones = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam);
                LinkedListNode<BasicBeamController.BeamBone> linkedListNode = bones.Last;
                return linkedListNode.Value.RotationAngle;
            }
        }

        public static BasicBeamController.BeamBone GetIndexedBone(this BasicBeamController beam, int boneIndex)
        {
            LinkedList<BasicBeamController.BeamBone> bones;
            bones = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam);
            if (bones == null) return null;
            if (bones.ElementAt(boneIndex) == null) { Debug.LogError("Attempted to fetch a beam bone at an invalid index"); return null; }
            return bones.ElementAt(boneIndex);
        }
        public static Vector2 GetIndexedBonePosition(this BasicBeamController beam, int boneIndex)
        {
            LinkedList<BasicBeamController.BeamBone> bones;
            bones = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam);

            if (bones.ElementAt(boneIndex) == null) { Debug.LogError("Attempted to fetch the position of a beam bone at an invalid index"); return Vector2.zero; }
            if (!beam.UsesBones)
            {
                return beam.Origin + BraveMathCollege.DegreesToVector(beam.Direction.ToAngle(), bones.ElementAt(boneIndex).PosX);
            }
            if (beam.ProjectileAndBeamMotionModule != null)
            {
                return bones.ElementAt(boneIndex).Position + beam.ProjectileAndBeamMotionModule.GetBoneOffset(bones.ElementAt(boneIndex), beam, beam.projectile.Inverted);
            }
            return bones.ElementAt(boneIndex).Position;
        }
        public static Vector2 GetBonePosition(this BasicBeamController beam, BasicBeamController.BeamBone bone)
        {
            if (!beam.UsesBones)
            {
                return beam.Origin + BraveMathCollege.DegreesToVector(beam.Direction.ToAngle(), bone.PosX);
            }
            if (beam.ProjectileAndBeamMotionModule != null)
            {
                return bone.Position + beam.ProjectileAndBeamMotionModule.GetBoneOffset(bone, beam, beam.projectile.Inverted);
            }
            return bone.Position;
        }

        public static BasicBeamController GenerateBeamPrefab(this Projectile projectile, string spritePath, Vector2 colliderDimensions, Vector2 colliderOffsets, List<string> beamAnimationPaths = null, int beamFPS = -1, List<string> impactVFXAnimationPaths = null, int beamImpactFPS = -1, Vector2? impactVFXColliderDimensions = null, Vector2? impactVFXColliderOffsets = null, List<string> endVFXAnimationPaths = null, int beamEndFPS = -1, Vector2? endVFXColliderDimensions = null, Vector2? endVFXColliderOffsets = null, List<string> muzzleVFXAnimationPaths = null, int beamMuzzleFPS = -1, Vector2? muzzleVFXColliderDimensions = null, Vector2? muzzleVFXColliderOffsets = null, bool glows = false)
        {
            try
            {
                projectile.specRigidbody.CollideWithOthers = false;


                float convertedColliderX = colliderDimensions.x / 16f;
                float convertedColliderY = colliderDimensions.y / 16f;
                float convertedOffsetX = colliderOffsets.x / 16f;
                float convertedOffsetY = colliderOffsets.y / 16f;

                int spriteID = SpriteBuilder.AddSpriteToCollection(spritePath, ETGMod.Databases.Items.ProjectileCollection);
                tk2dTiledSprite tiledSprite = projectile.gameObject.GetOrAddComponent<tk2dTiledSprite>();


                tiledSprite.SetSprite(ETGMod.Databases.Items.ProjectileCollection, spriteID);
                tk2dSpriteDefinition def = tiledSprite.GetCurrentSpriteDef();
                def.colliderVertices = new Vector3[]{
                    new Vector3(convertedOffsetX, convertedOffsetY, 0f),
                    new Vector3(convertedColliderX, convertedColliderY, 0f)
                };

                def.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft);

                //tiledSprite.anchor = tk2dBaseSprite.Anchor.MiddleCenter;
                tk2dSpriteAnimator animator = projectile.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
                tk2dSpriteAnimation animation = projectile.gameObject.GetOrAddComponent<tk2dSpriteAnimation>();
                animation.clips = new tk2dSpriteAnimationClip[0];
                animator.Library = animation;
                UnityEngine.Object.Destroy(projectile.GetComponentInChildren<tk2dSprite>());
                projectile.sprite = tiledSprite;

                BasicBeamController beamController = projectile.gameObject.GetOrAddComponent<BasicBeamController>();

                //---------------- Sets up the animation for the main part of the beam
                if (beamAnimationPaths != null)
                {
                    tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip() { name = "beam_idle", frames = new tk2dSpriteAnimationFrame[0], fps = beamFPS };
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
                    beamController.beamAnimation = "beam_idle";
                }

                //------------- Sets up the animation for the part of the beam that touches the wall
                if (endVFXAnimationPaths != null && endVFXColliderDimensions != null && endVFXColliderOffsets != null)
                {
                    SetupBeamPart(animation, endVFXAnimationPaths, "beam_end", beamEndFPS, (Vector2)endVFXColliderDimensions, (Vector2)endVFXColliderOffsets);
                    beamController.beamEndAnimation = "beam_end";
                }
                else
                {
                    SetupBeamPart(animation, beamAnimationPaths, "beam_end", beamFPS, null, null, def.colliderVertices);
                    beamController.beamEndAnimation = "beam_end";
                }

                //---------------Sets up the animaton for the VFX that plays over top of the end of the beam where it hits stuff
                if (impactVFXAnimationPaths != null && impactVFXColliderDimensions != null && impactVFXColliderOffsets != null)
                {
                    SetupBeamPart(animation, impactVFXAnimationPaths, "beam_impact", beamImpactFPS, (Vector2)impactVFXColliderDimensions, (Vector2)impactVFXColliderOffsets);
                    beamController.impactAnimation = "beam_impact";
                }

                //--------------Sets up the animation for the very start of the beam
                if (muzzleVFXAnimationPaths != null && muzzleVFXColliderDimensions != null && muzzleVFXColliderOffsets != null)
                {
                    SetupBeamPart(animation, muzzleVFXAnimationPaths, "beam_start", beamMuzzleFPS, (Vector2)muzzleVFXColliderDimensions, (Vector2)muzzleVFXColliderOffsets);
                    beamController.beamStartAnimation = "beam_start";
                }
                else
                {
                    SetupBeamPart(animation, beamAnimationPaths, "beam_start", beamFPS, null, null, def.colliderVertices);
                    beamController.beamStartAnimation = "beam_start";
                }

                if (glows)
                {
                    EmmisiveBeams emission = projectile.gameObject.GetOrAddComponent<EmmisiveBeams>();
                    //emission

                }
                return beamController;
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
                return null;
            }
        }
        private static void SetupBeamPart(tk2dSpriteAnimation beamAnimation, List<string> animSpritePaths, string animationName, int fps, Vector2? colliderDimensions = null, Vector2? colliderOffsets = null, Vector3[] overrideVertices = null)
        {
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip() { name = animationName, frames = new tk2dSpriteAnimationFrame[0], fps = fps };
            List<string> spritePaths = animSpritePaths;

            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
            foreach (string path in spritePaths)
            {
                tk2dSpriteCollectionData collection = ETGMod.Databases.Items.ProjectileCollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection(path, collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter);
                if (overrideVertices != null)
                {
                    frameDef.colliderVertices = overrideVertices;
                }
                else
                {
                    if (colliderDimensions == null || colliderOffsets == null)
                    {
                        ETGModConsole.Log("<size=100><color=#ff0000ff>BEAM ERROR: colliderDimensions or colliderOffsets was null with no override vertices!</color></size>", false);
                    }
                    else
                    {
                        Vector2 actualDimensions = (Vector2)colliderDimensions;
                        Vector2 actualOffsets = (Vector2)colliderDimensions;
                        frameDef.colliderVertices = new Vector3[]{
                            new Vector3(actualOffsets.x / 16, actualOffsets.y / 16, 0f),
                            new Vector3(actualDimensions.x / 16, actualDimensions.y / 16, 0f)
                        };
                    }
                }
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            clip.frames = frames.ToArray();
            beamAnimation.clips = beamAnimation.clips.Concat(new tk2dSpriteAnimationClip[] { clip }).ToArray();
        }
        public static BeamController FreeFireBeamFromAnywhere(Projectile projectileToSpawn, PlayerController owner, GameObject otherShooter, Vector2 fixedPosition, bool usesFixedPosition, float targetAngle, float duration, bool skipChargeTime = false, bool CanRotate = false, float RotationSpeedperSecond = 60, bool DoPostProcess = false)
        {
            Vector2 sourcePos = Vector2.zero;
            SpeculativeRigidbody rigidBod = null;
            if (usesFixedPosition) { sourcePos = fixedPosition; }
            else
            {
                if (otherShooter.GetComponent<SpeculativeRigidbody>()) rigidBod = otherShooter.GetComponent<SpeculativeRigidbody>();
                else if (otherShooter.GetComponentInChildren<SpeculativeRigidbody>()) rigidBod = otherShooter.GetComponentInChildren<SpeculativeRigidbody>();

                if (rigidBod) { sourcePos = rigidBod.UnitCenter; }
                

                if (otherShooter == null) { ETGModConsole.Log("projectileToSpawn.gameObject is NULL"); }
                if (rigidBod == null) { ETGModConsole.Log("rigidBod is NULL"); }

            }

            if (otherShooter.gameObject != null && sourcePos == Vector2.zero)
            {
                sourcePos = otherShooter.transform.PositionVector2();
            }

            if (sourcePos != Vector2.zero)
            {

                BasicBeamController basicBeam = projectileToSpawn.gameObject.GetComponent<BasicBeamController>();
                if (basicBeam) { basicBeam.SkipPostProcessing = DoPostProcess; }

                GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, sourcePos, Quaternion.identity, true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.Owner = owner;
                BeamController component2 = gameObject.GetComponent<BeamController>();
                if (skipChargeTime)
                {
                    component2.chargeDelay = 0f;
                    component2.usesChargeDelay = false;
                }
                component2.Owner = owner;
                component2.HitsPlayers = false;
                component2.HitsEnemies = true;
                Vector3 vector = BraveMathCollege.DegreesToVector(targetAngle, 1f);
                component2.Direction = vector;
                component2.Origin = sourcePos;
                component.StartCoroutine(BeamToolbox.HandleFreeFiringBeam(component2, rigidBod, fixedPosition, usesFixedPosition, targetAngle, duration, CanRotate, RotationSpeedperSecond));

              


                return component2;
            }
            else
            {

                ETGModConsole.Log("ERROR IN BEAM FREEFIRE CODE. SOURCEPOS WAS NULL, EITHER DUE TO INVALID FIXEDPOS OR SOURCE GAMEOBJECT.");
                return null;
            }
        }
        private static IEnumerator HandleFreeFiringBeam(BeamController beam, SpeculativeRigidbody otherShooter, Vector2 fixedPosition, bool usesFixedPosition, float targetAngle, float duration, bool CanRotate = false, float RotationSpeedPerSecond = 60)
        {
            float elapsed = 0f;
            yield return null;
            while (elapsed < duration)
            {
                Vector2 sourcePos;
                if (otherShooter == null) {  break; }
                if (beam == null) { break; }
                if (usesFixedPosition) sourcePos = fixedPosition;
                else sourcePos = otherShooter.UnitCenter;

                elapsed += BraveTime.DeltaTime;
                if (sourcePos != null)
                {
                    if (CanRotate == true)
                    {
                        Vector3 vector = BraveMathCollege.DegreesToVector(beam.Direction.ToAngle() + RotationSpeedPerSecond * BraveTime.DeltaTime, 1f);
                        beam.Direction = vector; 
                    }
                    beam.Origin = sourcePos;
                    beam.LateUpdatePosition(sourcePos);


                }
                else { ETGModConsole.Log("SOURCEPOS WAS NULL IN BEAM FIRING HANDLER"); }
                yield return null;
            }
            beam.CeaseAttack();

            yield break;
        }



        public static BeamController FreeFireBeamFromPosition(Projectile projectileToSpawn, PlayerController owner, Vector2 fixedPosition, float targetAngle, float duration, bool skipChargeTime = false, bool CanRotate = false, float RotationSpeedperSecond = 60, bool DoPostProcess = false)
        {
            if (fixedPosition != Vector2.zero)
            {
                BasicBeamController basicBeam = projectileToSpawn.gameObject.GetComponent<BasicBeamController>();
                if (basicBeam) { basicBeam.SkipPostProcessing = DoPostProcess; }


                GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, fixedPosition, Quaternion.identity, true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.Owner = owner;
                BeamController component2 = gameObject.GetComponent<BeamController>();
                if (skipChargeTime)
                {
                    component2.chargeDelay = 0f;
                    component2.usesChargeDelay = false;
                }
                component2.Owner = owner;
                component2.HitsPlayers = false;
                component2.HitsEnemies = true;
                Vector3 vector = BraveMathCollege.DegreesToVector(targetAngle, 1f);
                component2.Direction = vector;
                component2.Origin = fixedPosition;
                GameManager.Instance.Dungeon.StartCoroutine(BeamToolbox.HandleFreeFiringBeamFromPosition(component2, fixedPosition, targetAngle, duration, CanRotate, RotationSpeedperSecond));
                return component2;
            }
            else
            {

                ETGModConsole.Log("ERROR IN BEAM FREEFIRE CODE. SOURCEPOS WAS NULL, EITHER DUE TO INVALID FIXEDPOS OR SOURCE GAMEOBJECT.");
                return null;
            }
        }

        private static IEnumerator HandleFreeFiringBeamFromPosition(BeamController beam, Vector2 fixedPosition, float targetAngle, float duration, bool CanRotate = false, float RotationSpeedPerSecond = 60)
        {
            float elapsed = 0f;
            yield return null;
            while (elapsed < duration)
            {
                Vector2 sourcePos;
                if (beam == null) { break; }
                sourcePos = fixedPosition;
                elapsed += BraveTime.DeltaTime;
                if (sourcePos != null)
                {
                    if (CanRotate == true)
                    {
                        Vector3 vector = BraveMathCollege.DegreesToVector(beam.Direction.ToAngle() + RotationSpeedPerSecond * BraveTime.DeltaTime, 1f);
                        beam.Direction = vector;
                    }
                    beam.Origin = sourcePos;
                    beam.LateUpdatePosition(sourcePos);


                }
                else { ETGModConsole.Log("SOURCEPOS WAS NULL IN BEAM FIRING HANDLER"); }
                yield return null;
            }
            if (beam != null)
            {
                beam.CeaseAttack();
            }
            yield break;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{
    public static class SpriteTrimmer
    {
        public static Texture2D DesheetTexture(this tk2dSpriteDefinition definition)
        {
            if (definition?.material?.mainTexture != null && definition.material.mainTexture is Texture2D tex)
            {
                var sheet = tex.GetRW();
                var sheetWidth = sheet.width;
                var sheetHeight = sheet.height;
                var uv = definition.uvs;
                if (uv.Length >= 4)
                {
                    var x = Mathf.RoundToInt(uv[0].x * sheetWidth);
                    var y = Mathf.RoundToInt(uv[0].y * sheetHeight);
                    var width = Mathf.RoundToInt((uv[3].x - uv[0].x) * sheetWidth);
                    var height = Mathf.RoundToInt((uv[3].y - uv[0].y) * sheetHeight);
                    var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                    texture.SetPixels(sheet.GetPixels(x, y, width, height));
                    texture.Apply(false, false);
                    texture.name = definition.name;
                    return texture;
                }
            }
            return null;
        }


        public static void TrimGunSpritesForSpecificAnims(this Gun gun, params string[] anims)
        {
            List<KeyValuePair<tk2dSpriteCollectionData, int>> ids = new List<KeyValuePair<tk2dSpriteCollectionData, int>>();

            anims.ToList().ForEach(x => gun.TryTrimGunAnimation(x, ids));

            var defaultId = gun.sprite.spriteId;
            var defaultDefinition = gun.sprite.Collection.spriteDefinitions[defaultId];
            var globalOffset = new Vector2(-defaultDefinition.position0.x, -defaultDefinition.position0.y);
            foreach (var x in ids)
            {
                x.Key?.spriteDefinitions[x.Value]?.AddOffset(globalOffset);
                var attach = x.Key?.GetAttachPoints(x.Value);
                if (attach == null)
                {
                    continue;
                }
                foreach (var attachPoint in attach)
                {
                    attachPoint.position += globalOffset.ToVector3ZUp(0f);
                }
            };
        }

        public static void TrimGunSprites(this Gun gun)
        {
            List<KeyValuePair<tk2dSpriteCollectionData, int>> ids = new List<KeyValuePair<tk2dSpriteCollectionData, int>>();
            gun.TryTrimGunAnimation(gun.shootAnimation, ids);
            gun.TryTrimGunAnimation(gun.reloadAnimation, ids);
            gun.TryTrimGunAnimation(gun.emptyReloadAnimation, ids);
            gun.TryTrimGunAnimation(gun.idleAnimation, ids);
            gun.TryTrimGunAnimation(gun.chargeAnimation, ids);
            gun.TryTrimGunAnimation(gun.dischargeAnimation, ids);
            gun.TryTrimGunAnimation(gun.emptyAnimation, ids);
            gun.TryTrimGunAnimation(gun.introAnimation, ids);
            gun.TryTrimGunAnimation(gun.finalShootAnimation, ids);
            gun.TryTrimGunAnimation(gun.enemyPreFireAnimation, ids);
            gun.TryTrimGunAnimation(gun.outOfAmmoAnimation, ids);
            gun.TryTrimGunAnimation(gun.criticalFireAnimation, ids);
            gun.TryTrimGunAnimation(gun.dodgeAnimation, ids);
            gun.TryTrimGunAnimation(gun.alternateIdleAnimation, ids);
            gun.TryTrimGunAnimation(gun.alternateReloadAnimation, ids);
            gun.TryTrimGunAnimation(gun.alternateShootAnimation, ids);
            var defaultId = gun.sprite.spriteId;
            var defaultDefinition = gun.sprite.Collection.spriteDefinitions[defaultId];
            var globalOffset = new Vector2(-defaultDefinition.position0.x, -defaultDefinition.position0.y);
            foreach (var x in ids)
            {
                x.Key?.spriteDefinitions[x.Value]?.AddOffset(globalOffset);
                var attach = x.Key?.GetAttachPoints(x.Value);
                if (attach == null)
                {
                    continue;
                }
                foreach (var attachPoint in attach)
                {
                    attachPoint.position += globalOffset.ToVector3ZUp(0f);
                }
            };
            gun.barrelOffset.localPosition += globalOffset.ToVector3ZUp(0f);
        }

        public static void TryTrimGunAnimation(this Gun gun, string animation, List<KeyValuePair<tk2dSpriteCollectionData, int>> ids)
        {
            if (!string.IsNullOrEmpty(animation) && gun.spriteAnimator != null)
            {
                var clip = gun.spriteAnimator.GetClipByName(animation);
                if (clip != null)
                {
                    foreach (var frame in clip.frames)
                    {
                        if (frame.spriteCollection?.spriteDefinitions != null && frame.spriteId >= 0 && frame.spriteId < frame.spriteCollection.spriteDefinitions.Length)
                        {
                            var definition = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                            ETGMod.Assets.TextureMap.TryGetValue("sprites/" + frame.spriteCollection.name + "/" + definition.name, out var texture);
                            if (texture != null && definition != null)
                            {
                                var pixelOffset = texture.TrimTexture();
                                RuntimeAtlasSegment ras = ETGMod.Assets.Packer.Pack(texture); //pack your resources beforehand or the outlines will turn out weird

                                Material material = new Material(definition.material);
                                material.mainTexture = ras.texture;
                                definition.uvs = ras.uvs;
                                definition.material = material;
                                if (definition.materialInst != null)
                                {
                                    definition.materialInst = new Material(material);
                                }
                                float num = texture.width * 0.0625f;
                                float num2 = texture.height * 0.0625f;
                                definition.position0 = new Vector3(0f, 0f, 0f);
                                definition.position1 = new Vector3(num, 0f, 0f);
                                definition.position2 = new Vector3(0f, num2, 0f);
                                definition.position3 = new Vector3(num, num2, 0f);
                                definition.boundsDataCenter = definition.untrimmedBoundsDataCenter = new Vector3(num / 2f, num2 / 2f, 0f);
                                definition.boundsDataExtents = definition.untrimmedBoundsDataExtents = new Vector3(num, num2, 0f);
                                definition.AddOffset(pixelOffset.ToVector2() / 16f);
                                ids.Add(new KeyValuePair<tk2dSpriteCollectionData, int>(frame.spriteCollection, frame.spriteId));
                            }
                        }
                    }
                }
            }
        }

        public static void AddOffset(this tk2dSpriteDefinition def, Vector2 offset, bool changesCollider = false)
        {
            float xOffset = offset.x;
            float yOffset = offset.y;
            def.position0 += new Vector3(xOffset, yOffset, 0);
            def.position1 += new Vector3(xOffset, yOffset, 0);
            def.position2 += new Vector3(xOffset, yOffset, 0);
            def.position3 += new Vector3(xOffset, yOffset, 0);
            def.boundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.boundsDataExtents += new Vector3(xOffset, yOffset, 0);
            def.untrimmedBoundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.untrimmedBoundsDataExtents += new Vector3(xOffset, yOffset, 0);
            if (def.colliderVertices != null && def.colliderVertices.Length > 0 && changesCollider)
            {
                def.colliderVertices[0] += new Vector3(xOffset, yOffset, 0);
            }
        }

        // totally not stolen from ccm code :)
        public static IntVector2 TrimTexture(this Texture2D orig)
        {
            RectInt bounds = orig.GetTrimmedBounds();
            Color[][] pixels = new Color[bounds.width][];

            for (int x = bounds.x; x < bounds.x + bounds.width; x++)
            {
                for (int y = bounds.y; y < bounds.y + bounds.height; y++)
                {
                    if (pixels[x - bounds.x] == null)
                    {
                        pixels[x - bounds.x] = new Color[bounds.height];
                    }
                    pixels[x - bounds.x][y - bounds.y] = orig.GetPixel(x, y);
                }
            }

            orig.Resize(bounds.width, bounds.height);

            for (int x = 0; x < bounds.width; x++)
            {
                for (int y = 0; y < bounds.height; y++)
                {
                    orig.SetPixel(x, y, pixels[x][y]);
                }
            }
            orig.Apply(false, false);
            return new IntVector2(bounds.x, bounds.y);
        }

        public static RectInt GetTrimmedBounds(this Texture2D t)
        {

            int xMin = t.width;
            int yMin = t.height;
            int xMax = 0;
            int yMax = 0;

            for (int x = 0; x < t.width; x++)
            {
                for (int y = 0; y < t.height; y++)
                {
                    if (t.GetPixel(x, y).a > 0)
                    {
                        if (x < xMin) xMin = x;
                        if (y < yMin) yMin = y;
                        if (x > xMax) xMax = x;
                        if (y > yMax) yMax = y;
                    }
                }
            }

            return new RectInt(xMin, yMin, xMax - xMin + 1, yMax - yMin + 1);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ItemAPI;
using System.Collections;
using UnityEngine;

namespace Planetside
{
    static class ObjectMakers
    {
        public static GameObject CreateSpriteObject(string spritepath, string Name = "TemplateName")
        {
            GameObject obj = SpriteBuilder.SpriteFromResource(spritepath, new GameObject(Name));
            obj.SetActive(false);
            tk2dBaseSprite vfxSprite = obj.GetComponent<tk2dBaseSprite>();
            vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            return obj;
        }

        public static VFXPool MakeObjectIntoVFX(GameObject obj)
        {
            VFXPool pool = new VFXPool();
            pool.type = VFXPoolType.All;
            VFXComplex complex = new VFXComplex();
            VFXObject vfObj = new VFXObject();
            vfObj.effect = obj;
            complex.effects = new VFXObject[] { vfObj };
            pool.effects = new VFXComplex[] { complex };
            return pool;
        }

        public static SpeculativeRigidbody CreateFastBody(this GameObject gameObject, IntVector2 colliderX_Y, IntVector2 OffsetX_Y, CollisionLayer collisionLayer, bool overrideAddNewSpecualativeBody = false, bool isTrigger = false)
        {
            SpeculativeRigidbody specBody = overrideAddNewSpecualativeBody ? gameObject.AddComponent<SpeculativeRigidbody>() : gameObject.GetOrAddComponent<SpeculativeRigidbody>();
            specBody.CollideWithTileMap = false;
            if (specBody.PixelColliders == null) { specBody.PixelColliders = new List<PixelCollider>(); }
            specBody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = collisionLayer,
                IsTrigger = isTrigger,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = OffsetX_Y.x,
                ManualOffsetY = OffsetX_Y.y,
                ManualWidth = colliderX_Y.x,
                ManualHeight = colliderX_Y.y,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            return specBody;
        }

    }
}

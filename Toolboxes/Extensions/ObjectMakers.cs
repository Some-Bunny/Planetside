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
    }
}

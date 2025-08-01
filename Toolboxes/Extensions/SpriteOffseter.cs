using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{
    public static class SpriteOffseter
    {
        internal static void MakeOffset(this tk2dSpriteCollectionData tk2DSpriteCollectionData, tk2dSpriteDefinition def, Vector3 offset, string[] ApplyToAttachPoints = null, bool changesCollider = false)
        {
            def.position0 += offset;
            def.position1 += offset;
            def.position2 += offset;
            def.position3 += offset;
            def.boundsDataCenter += offset;
            def.untrimmedBoundsDataCenter += offset;
            if (changesCollider && def.colliderVertices != null && def.colliderVertices.Length > 0)
                def.colliderVertices[0] += offset;

            if (ApplyToAttachPoints != null)
            {
                var attach = def.GetAttachPoints(tk2DSpriteCollectionData, tk2DSpriteCollectionData.GetSpriteIdByName(def.name));
                if (attach != null)
                {
                    foreach (var entry in attach)
                    {
                        foreach (var Spr in ApplyToAttachPoints)
                        {
                            if (entry.name == Spr)
                            {
                                entry.position += offset;
                                break;
                            }
                        }                   
                    }
                }
            }

        }
    }
}

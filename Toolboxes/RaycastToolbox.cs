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
using DaikonForge;


namespace Planetside
{

    public static class RaycastToolbox
    {
        public static RaycastResult ReturnRaycast(Vector2 startPosition, Vector2 angle, int rayCastMask, float overrideDistance = 1000)
        {
            //            int rayMask2 = CollisionMask.LayerToMask(collisionLayersToAccount[0], collisionLayersToAccount[1], collisionLayersToAccount[2], collisionLayersToAccount[3]);


            Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
            RaycastResult raycastResult2;
            PhysicsEngine.Instance.Raycast(startPosition, angle, overrideDistance, out raycastResult2, true, true, rayCastMask, null, false, rigidbodyExcluder, null);
            return raycastResult2;
        }
        
    }
}




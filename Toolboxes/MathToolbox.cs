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

    public static class MathToolbox
    {

        public static float SubdivideRange(float startValue, float endValue, int numDivisions, int i, bool offset = false)
        {
            return Mathf.Lerp(startValue, endValue, ((float)i + ((!offset) ? 0f : 0.5f)) / (float)(numDivisions - 1));
        }
        public static float SubdivideArc(float startAngle, float sweepAngle, int numBullets, int i, bool offset = false)
        {
            return startAngle + Mathf.Lerp(0f, sweepAngle, ((float)i + ((!offset) ? 0f : 0.5f)) / (float)(numBullets - 1));
        }

        public static float SubdivideCircle(float startAngle, int numBullets, int i, float direction = 1f, bool offset = false)
        {
            return startAngle + direction * Mathf.Lerp(0f, 360f, ((float)i + ((!offset) ? 0f : 0.5f)) / (float)numBullets);
        }

        public static Vector2 GetUnitOnCircle(float angleDegrees, float radius)
        {

            // initialize calculation variables
            float _x = 0;
            float _y = 0;
            float angleRadians = 0;
            Vector2 _returnVector;

            // convert degrees to radians
            angleRadians = angleDegrees * Mathf.PI / 180.0f;

            // get the 2D dimensional coordinates
            _x = radius * Mathf.Cos(angleRadians);
            _y = radius * Mathf.Sin(angleRadians);

            // derive the 2D vector
            _returnVector = new Vector2(_x, _y);

            // return the vector info
            return _returnVector;
        }

        public static float SinLerpTValue(float t)
        {
            return  Mathf.Sin(t * (Mathf.PI / 2));
        }
    }
}




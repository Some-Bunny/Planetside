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
        public static float EaseIn(float t)
        {
            return t * t;
        }
        public static float EaseInInverse(float t)
        {
            return 1 - (t * t);
        }

        public static float Flip(float t)
        {
            return 1 - t;
        }
        public static float EaseOut(float t)
        {
            return Flip(EaseIn(Flip(t)));
        }
        public static float EaseInOut(float t)
        {
            return Mathf.Lerp(EaseIn(t), EaseOut(t), t);
        }

        public static float EaseInAndBack(float t)
        {
            return Mathf.Lerp(EaseIn(t), EaseInInverse(t), t);
        }

        public static float SinLerpTValueFull(float t)
        {
            return Mathf.Sin(t * (Mathf.PI));
        }
        public static float ToAngle(Vector2 v)
        {
            return Mathf.Atan2(v.y, v.x) * 57.29578f;
        }
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

        public static bool IsCloserThan(Vector2 pos1, Vector2 pos2, float Value_Thats_Supposed_To_Be_Larger_Than_What_You_Want_To_Return_True)
        {
            return (pos1 - pos2).sqrMagnitude < Value_Thats_Supposed_To_Be_Larger_Than_What_You_Want_To_Return_True * Value_Thats_Supposed_To_Be_Larger_Than_What_You_Want_To_Return_True;
        }
        public static bool IsFurtherThan(Vector2 pos1, Vector2 pos2, float Value_Thats_Supposed_To_Be_Larger_Than_What_You_Want_To_Return_True)
        {
            return (pos1 - pos2).sqrMagnitude > Value_Thats_Supposed_To_Be_Larger_Than_What_You_Want_To_Return_True * Value_Thats_Supposed_To_Be_Larger_Than_What_You_Want_To_Return_True;
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
        public static Vector3 GetUnitOnCircle3(float angleDegrees, float radius)
        {

            // initialize calculation variables
            float _x = 0;
            float _y = 0;
            float angleRadians = 0;
            Vector3 _returnVector;

            // convert degrees to radians
            angleRadians = angleDegrees * Mathf.PI / 180.0f;

            // get the 2D dimensional coordinates
            _x = radius * Mathf.Cos(angleRadians);
            _y = radius * Mathf.Sin(angleRadians);

            // derive the 2D vector
            _returnVector = new Vector3(_x, _y);

            // return the vector info
            return _returnVector;
        }

        public static float SinLerpTValue(float t)
        {
            return  Mathf.Sin(t * (Mathf.PI / 2));
        }
    }
}




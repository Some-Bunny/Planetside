using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{
    public class Easing
    {
        public static float DoLerpT(float t, Easing.Ease ease = Easing.Ease.IN_OUT)
        {
            switch (ease)
            {
                case Easing.Ease.IN:
                    return Easing.EaseIn(t);
                case Easing.Ease.OUT:
                    return Easing.EaseOut(t);
                case Easing.Ease.IN_OUT:
                    return Easing.EaseInOut(t);
                case Easing.Ease.NONE:
                    return t;
                case Easing.Ease.EASE_IN_AND_BACK:
                    return Easing.EaseInAndBack(t);
                default:
                    return t;
            }
        }
        public enum Ease
        {
            IN,
            OUT,
            IN_OUT,
            NONE,
            EASE_IN_AND_BACK
        }
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
    }
}

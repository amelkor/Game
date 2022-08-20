using System.Runtime.CompilerServices;
using UnityEngine;

namespace Bsr.CharacterController
{
    public static class MathTool
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Map(float x, float inMin, float inMax, float outMin, float outMax)
        {
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Map0(float x, float inMax, float outMax)
        {
            return x * outMax / inMax;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ScaleInRange(Vector3 value, Vector2 xRange, Vector2 yRange, Vector2 zRange)
        {
            Vector3 result;

            result.x = value.x > 0
                ? Map0(value.x, 1, xRange.y)
                : Map0(value.x, 1, xRange.x);

            result.y = value.y > 0
                ? Map0(value.y, 1, yRange.y)
                : Map0(value.y, 1, yRange.x);

            result.z = value.z > 0
                ? Map0(value.z, 1, zRange.y)
                : Map0(value.z, 1, zRange.x);

            return result;
        }
    }
}
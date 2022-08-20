using UnityEngine;

namespace Bsr.CharacterController
{
    internal static class VectorExtensions
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2XZ(this Vector3 v) => new(v.x, v.z);
    }
}
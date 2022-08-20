using UnityEditor;
using UnityEngine;

namespace Bsr.CharacterController
{
    public static class GizmosExtensions
    {
        public static void DrawCapsuleGizmos(this CapsuleCollider capsule)
        {
            DrawCapsuleGizmos(capsule.transform.position + Vector3.up * capsule.center.y, Quaternion.identity, capsule.height, capsule.radius, Color.yellow);
        }


        private static void DrawCapsuleGizmos(Vector3 pos, Quaternion rot, float height, float radius, Color color)
        {
            var angleMatrix = Matrix4x4.TRS(pos, rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(color, angleMatrix))
            {
                Gizmos.color = color;
                var pointOffset = (height - (radius * 2)) / 2;

                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
                Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);

                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);

                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);
            }
        }
    }
}
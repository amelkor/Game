using System;
using UnityEngine;

namespace Bsr.CharacterController
{
    [Unity.VisualScripting.TypeOptionsAdd]
    [SelectionBase]
    public class Player : MonoBehaviour
    {
        [Serializable]
        private class Components
        {
            public CharacterInputHandler input;
            public MotionProcessor motion;
            public GroundDetection ground;
            public CapsuleCollider bodyCollider;
        }

        public Color c;
        [SerializeField] private Components components;

        private void OnValidate()
        {
            if (!components.motion) this.TryGetComponentInChildren(out components.motion);
            if (!components.input) this.TryGetComponentInChildren(out components.input);
            if (!components.ground) this.TryGetComponentInChildren(out components.ground);
            if (!components.bodyCollider) gameObject.TryGetComponentInChildrenWithName("body_collider", out components.bodyCollider);
        }

        private void OnDrawGizmos()
        {
            if (components.bodyCollider)
            {
                components.bodyCollider.DrawCapsuleGizmos();
            }
        }
    }
}
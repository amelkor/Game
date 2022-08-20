using UnityEngine;
using UnityEngine.Events;

namespace Bsr.CharacterController
{
    [Unity.VisualScripting.TypeOptionsAdd]
    public class GroundDetection : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider bodyCollider;
        [SerializeField] private LayerMask layerMask = 1;
        [SerializeField] private float checkDistance = 0.1f;
        [SerializeField] private float steepSlopeAngle = 48f;

        private Transform _cachedTransform;
        private bool _isGrounded;
        private bool _isOnSteepSlope;
        private float _slopeAngle;
        private readonly RaycastHit[] _bottomHit = new RaycastHit[1];
        private readonly RaycastHit[] _topHit = new RaycastHit[1];

        public UnityEvent<bool> onGrounded;
        public UnityEvent<float> onSlopeAngleChaged;
        public UnityEvent<bool> onSteepSlope;

        public bool IsGrounded => _isGrounded;
        public float SlopeAngle => _slopeAngle;
        public RaycastHit Hit => _bottomHit[0];
        public bool IsOnSteepSlope
        {
            get => _isOnSteepSlope;
            private set
            {
                var isChanged = _isOnSteepSlope != value;
                _isOnSteepSlope = value;
                if(isChanged)
                    onSteepSlope.Invoke(_isOnSteepSlope);
            }
        }

        private void Awake()
        {
            _cachedTransform = bodyCollider.transform;
        }

        private void FixedUpdate()
        {
            UpdateGround();
            UpdateSlopeAngle();
        }
        
        private void UpdateGround()
        {
            var position = _cachedTransform.position;
            
            Physics.RaycastNonAlloc(position, Vector3.down, _bottomHit, checkDistance, layerMask, QueryTriggerInteraction.Ignore);
            var isGrounded = Physics.CheckSphere(position, checkDistance, layerMask, QueryTriggerInteraction.Ignore);
            
            if (_isGrounded != isGrounded)
            {
                _isGrounded = isGrounded;
                onGrounded.Invoke(_isGrounded);
            }
        }

        private void UpdateSlopeAngle()
        {
            var previousAngle = _slopeAngle;
            _slopeAngle = _isGrounded ? Vector3.Angle(Vector3.up, _bottomHit[0].normal) : 0f;
            if (!Mathf.Approximately(_slopeAngle, previousAngle))
            {
                onSlopeAngleChaged.Invoke(_slopeAngle);
            }

            IsOnSteepSlope = _slopeAngle > steepSlopeAngle;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var hit = _bottomHit[0];
            if (_isGrounded)
            {
                
                UnityEditor.Handles.color = Color.green;
                UnityEditor.Handles.ArrowHandleCap(0, hit.point, hit.normal != Vector3.zero ? Quaternion.LookRotation(hit.normal) : Quaternion.identity, 0.3f, EventType.Repaint);
            }
        }
#endif
    }
}
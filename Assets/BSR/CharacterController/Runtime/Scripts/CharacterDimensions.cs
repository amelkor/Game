using System;
using UnityEngine;

namespace Bsr.CharacterController
{
    [Unity.VisualScripting.TypeOptionsAdd]
    public class CharacterDimensions : MonoBehaviour
    {
        public enum State
        {
            Normal = 0,
            Shrinking = 1,
            Expanding = 2,
            Crouch = 4
        }

        [SerializeField] private CapsuleCollider bodyCollider;
        [SerializeField] private CapsuleCollider obstacleCollider;
        [SerializeField] private CapsuleCollider bodyDamageCollider;
        [SerializeField] private Transform aimer;

        [Header("Obstacle above detection")]
        [SerializeField] private float checkDistance = 0.1f;
        [SerializeField] private LayerMask layerMask = 1;

        private Transform _cachedBodyColliderTransform;
        private Vector3 _aimerInitialPosition;
        private State _state;
        private float _initialHeight;
        private float _targetHeight;
        private float _currentHeight;

        private const float DEFAULT_SPEED = 10f;
        private float _speed = DEFAULT_SPEED;

        private float _obstacleColliderHeightOffset;
        private float _bodyDamageColliderHeightOffset;

        private readonly RaycastHit[] _hits = new RaycastHit[1];

        private void Awake()
        {
            var height = bodyCollider.height;

            _cachedBodyColliderTransform = bodyCollider.transform;
            _aimerInitialPosition = aimer.localPosition;
            _initialHeight = height;
            _currentHeight = _initialHeight;
            _targetHeight = _initialHeight;

            _obstacleColliderHeightOffset = obstacleCollider.height - _initialHeight;
            _bodyDamageColliderHeightOffset = bodyDamageCollider.height - _initialHeight;
        }

        public CapsuleCollider BodyCollider => bodyCollider;
        public State HeightState => _state;
        public bool IsExpanding => _state is State.Expanding;
        
        public void ResetHeight(float speed = DEFAULT_SPEED) => SetHeight(_initialHeight, speed);

        public void SetHeight(float height, float speed = DEFAULT_SPEED)
        {
            _speed = speed;
            _targetHeight = height;
        }
        
        public bool CanExpand()
        {
            var radius = bodyCollider.radius;
            return !Physics.CheckSphere(_cachedBodyColliderTransform.position + (_initialHeight - radius + checkDistance) * Vector3.up, radius, layerMask);
        }

        private void FixedUpdate()
        {
            const float tolerance = 0.01f;
            if (Math.Abs(_currentHeight - _targetHeight) < tolerance)
            {
                _state = _currentHeight + 0.1f < _initialHeight ? State.Crouch : State.Normal;
                return;
            }

            _state = _targetHeight > _currentHeight ? State.Expanding : State.Shrinking;

            if (_state == State.Expanding && !CanExpand())
            {
                return;
            }

            _currentHeight = Mathf.Lerp(_currentHeight, _targetHeight, Time.fixedDeltaTime * _speed);

            const float half = 0.5f;

            bodyCollider.height = _currentHeight;
            bodyCollider.center = _currentHeight * half * Vector3.up;
            obstacleCollider.height = _currentHeight + _obstacleColliderHeightOffset;
            obstacleCollider.center = (_currentHeight + _obstacleColliderHeightOffset) * half * Vector3.up;
            bodyDamageCollider.height = _currentHeight + _bodyDamageColliderHeightOffset;
            bodyDamageCollider.center = (_currentHeight + _bodyDamageColliderHeightOffset) * half * Vector3.up;

            if (_state is State.Shrinking or State.Expanding)
            {
                // move aimer by the same percentage as the height difference is
                var i = _aimerInitialPosition;
                var p = aimer.localPosition;
                var y = Mathf.Lerp(p.y, i.y * (_targetHeight / _initialHeight), Time.fixedDeltaTime * _speed);
                aimer.localPosition = new Vector3(i.x, y, i.z);
            }
        }
    }
}
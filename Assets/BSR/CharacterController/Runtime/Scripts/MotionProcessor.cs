using System;
using Bsr.CharacterController.Parameters;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Bsr.CharacterController
{
    [Unity.VisualScripting.TypeOptionsAdd]
    public class MotionProcessor : MonoBehaviour
    {
        [Serializable]
        private class ParametersNames
        {
            private const string TOOLTIP = "String literal for the parameter same as it appears in the motion data";
            [SerializeField, Tooltip(TOOLTIP)] public string forwardSpeed = "forward_speed";
            [SerializeField, Tooltip(TOOLTIP)] public string strafeSpeed = "strafe_speed";
            [SerializeField, Tooltip(TOOLTIP)] public string backwardsSpeed = "backwards_speed";
            [SerializeField, Tooltip(TOOLTIP)] public string movementAxis = "movement_axis";
            [SerializeField, Tooltip(TOOLTIP)] public string drag = "drag";
            [SerializeField, Tooltip(TOOLTIP)] public string canJump = "can_jump";
            [SerializeField, Tooltip(TOOLTIP)] public string canSprint = "can_sprint";
            [SerializeField, Tooltip(TOOLTIP)] public string canCrouch = "can_crouch";
            [SerializeField, Tooltip(TOOLTIP)] public string jumpForce = "jump_force";
            [SerializeField, Tooltip(TOOLTIP)] public string directionTransform = "direction_transform";
            [SerializeField, Tooltip(TOOLTIP)] public string lockMoveInput = "lock_movement_input";
        }

        [SerializeField] private ParametersNames motionParameters;

        [Header("References")]
        [SerializeField] private ParametersData parametersData;
        [SerializeField] private CharacterDimensions dimensions;
        [SerializeField] private Transform directionTransform;
        [SerializeField] private Variables variables;

        [Header("Events")]
        [Tooltip("Triggered when character horizontal input changed")] public UnityEvent<Vector2> horizontalInputChanged;
        [Tooltip("Triggered when character is started or ended walking")] public UnityEvent<bool> walked;
        [Tooltip("Triggered when character is started or ended sprinting")] public UnityEvent<bool> sprinted;
        [Tooltip("Triggered when character is started or ended crouching")] public UnityEvent<bool> crouched;
        [Tooltip("Triggered when character is jumped")] public UnityEvent jumped;

        private ParameterFloat _forwardSpeed;
        private ParameterFloat _strafeSpeed;
        private ParameterFloat _backwardsSpeed;
        private ParameterVector3 _movementAxis;
        private ParameterFloat _drag;
        private ParameterBool _canJump;
        private ParameterBool _canSprint;
        private ParameterBool _canCrouch;
        private ParameterFloat _jumpForce;
        private ParameterTransform _directionTransform;
        private ParameterSemaphore _lockMovementInput;

        private Rigidbody _rb;
        private Vector3 _input;
        private Vector3 _moveDirection;
        private bool _sprintInput;
        private bool _crouchInput;
        private bool _willWalk;
        private bool _willSprint;
        private bool _willCrouch;
        private bool _willJump;

        public Variables Variables => variables;
        public ParametersData ParametersData => parametersData;
        public bool HasMoveInput => _input.ToVector2XZ() != Vector2.zero;
        public bool Idle => !_willWalk && !_willSprint;
        public bool Walking => _willWalk;
        public bool Sprinting => _willSprint;
        public bool Crouching => _willCrouch;
        public bool Jumping => _willJump;
        public bool CanJump => _canJump && !UnableToUncrouch;
        public bool UnableToUncrouch => dimensions.HeightState is CharacterDimensions.State.Crouch or CharacterDimensions.State.Expanding && !dimensions.CanExpand();

        #region Initialization

        private void Awake()
        {
            _rb = GetComponentInParent<Rigidbody>();

            parametersData.GetParameter(motionParameters.forwardSpeed, out _forwardSpeed);
            parametersData.GetParameter(motionParameters.strafeSpeed, out _strafeSpeed);
            parametersData.GetParameter(motionParameters.backwardsSpeed, out _backwardsSpeed);
            parametersData.GetParameter(motionParameters.movementAxis, out _movementAxis);
            parametersData.GetParameter(motionParameters.drag, out _drag);
            parametersData.GetParameter(motionParameters.canJump, out _canJump);
            parametersData.GetParameter(motionParameters.canSprint, out _canSprint);
            parametersData.GetParameter(motionParameters.canCrouch, out _canCrouch);
            parametersData.GetParameter(motionParameters.jumpForce, out _jumpForce);
            parametersData.GetParameter(motionParameters.directionTransform, out _directionTransform);
            parametersData.GetParameter(motionParameters.lockMoveInput, out _lockMovementInput);

            _directionTransform.Value = directionTransform;
        }

        private void OnEnable()
        {
            _drag.ValueChanged += DragOnValueChanged;
        }

        private void OnDisable()
        {
            _drag.ValueChanged -= DragOnValueChanged;
        }

        #endregion

        private void DragOnValueChanged(float drag)
        {
            _rb.drag = drag;
        }

        public void SetHorizontalMove(Vector2 input)
        {
            var previousInput = new Vector2(_input.x, _input.z);
            _input.Set(input.x, _input.y, input.y);

            if (previousInput != input)
                horizontalInputChanged.Invoke(new Vector2(_input.x, _input.z));
        }

        public void SetVerticalMove(float input) => _input.Set(_input.x, input, _input.z);
        public void SetSprint(bool input) => _sprintInput = input;
        public void SetCrouch(bool input) => _crouchInput = input;
        public void SetKinematic(bool input) => _rb.isKinematic = input;
        public void SetUseGravity(bool input) => _rb.useGravity = input;

        public void TriggerJump()
        {
            if (CanJump)
            {
                _willJump = true;
            }
        }

        private void FixedUpdate()
        {
            UpdateConditions();

            if (_willJump)
            {
                _willJump = false;
                _rb.AddForce(_directionTransform.Value.up * _jumpForce, ForceMode.Impulse);
                jumped.Invoke();
            }

            if (_willWalk || _willSprint || _willCrouch)
            {
                _moveDirection = GetMoveDirection();
                _rb.AddForce(_moveDirection, ForceMode.Acceleration);
            }
        }

        private void UpdateConditions()
        {
            var wannaCrouch = _crouchInput && _canCrouch;
            var wannaSprint = _input != Vector3.zero && _sprintInput && _canSprint;
            var wannaWalk = HasMoveInput;

            var willCrouch = !wannaSprint && wannaCrouch && !_lockMovementInput && dimensions.HeightState is not CharacterDimensions.State.Expanding || UnableToUncrouch;
            var willSprint = wannaSprint && !willCrouch && !_lockMovementInput;
            var willWalk = wannaWalk && !willCrouch && !willSprint && !_lockMovementInput;

            var willWalkChanged = _willWalk != willWalk;
            var willSprintChanged = _willSprint != willSprint;
            var willCrouchChanged = _willCrouch != willCrouch;

            _willWalk = willWalk;
            _willSprint = willSprint;
            _willCrouch = willCrouch;

            if (willWalkChanged)
                walked.Invoke(_willWalk);

            if (willSprintChanged)
                sprinted.Invoke(_willSprint);

            if (willCrouchChanged)
                crouched.Invoke(_willCrouch);
        }

        private Vector3 GetMoveDirection()
        {
            var speed = GetAxisSpeed();
            var t = _directionTransform.Value;
            return t.right * speed.x + t.up * speed.y + t.forward * speed.z;
        }

        private Vector3 GetAxisSpeed()
        {
            var fwd = _forwardSpeed.Value;
            var stf = _strafeSpeed.Value;
            var bwd = _backwardsSpeed.Value;

            var speed = MathTool.ScaleInRange(_input,
                new Vector2(stf, stf),
                new Vector2(fwd, fwd),
                new Vector2(bwd, fwd));

            return Vector3.Scale(_movementAxis.Value, speed);
        }

#if UNITY_EDITOR
        [Header("Editor")]
        [SerializeField] private bool editorShowInput = true;
        [SerializeField] private bool editorShowVelocity = true;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        internal void EditorSetMotionData(ParametersData parameters) => parametersData = parameters;

        private void OnDrawGizmos()
        {
            if (_rb && _directionTransform)
            {
                var t = _directionTransform.Value;
                var p = t.TransformPoint(dimensions.BodyCollider.center);

                if (editorShowInput)
                {
                    if (_input != Vector3.zero)
                    {
                        UnityEditor.Handles.color = Color.cyan;
                        UnityEditor.Handles.ArrowHandleCap(0, p, Quaternion.LookRotation(t.TransformDirection(_input)), _input.magnitude, EventType.Repaint);
                    }
                }

                if (editorShowVelocity)
                {
                    var velocity = _rb.velocity;
                    if (velocity != Vector3.zero)
                    {
                        var magnitude = velocity.magnitude * 0.3f;
                        UnityEditor.Handles.color = Color.magenta;
                        UnityEditor.Handles.ArrowHandleCap(0, p, Quaternion.LookRotation(velocity), magnitude, EventType.Repaint);
                        UnityEditor.Handles.Label(p + magnitude * velocity.normalized, velocity.ToString());
                    }
                }
            }
        }
#endif
    }
}
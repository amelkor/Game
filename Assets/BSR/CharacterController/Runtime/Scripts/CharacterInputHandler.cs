using System;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bsr.CharacterController
{
    [Unity.VisualScripting.TypeOptionsAdd]
    [RequireComponent(typeof(PlayerInput))]
    public class CharacterInputHandler : MonoBehaviour
    {
        [Serializable]
        private class ActionNames
        {
            [SerializeField] public string movement = "Movement";
            [SerializeField] public string vertical = "Vertical";
            [SerializeField] public string looking = "Looking";
            [SerializeField] public string jump = "Jump";
            [SerializeField] public string sprint = "Sprint";
            [SerializeField] public string crouch = "Crouch";
        }

        [SerializeField] private ActionNames actionNames;
        [SerializeField] private PlayerInput input;
        [SerializeField] private MotionProcessor motion;
        [SerializeField] private FirstPersonRotator rotator;

        [Header("Registered Actions")]
        [SerializeField] private InputAction movementAction;
        [SerializeField] private InputAction verticalAction;
        [SerializeField] private InputAction lookingAction;
        [SerializeField] private InputAction jumpAction;
        [SerializeField] private InputAction sprintAction;
        [SerializeField] private InputAction crouchAction;

        private void Awake()
        {
            if (!FindActions()) Debug.LogError("Failed to find Player input actions");
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            SubscribeMovement(true);
            SubscribeVertical(true);
            SubscribeLooking(true);
            SubscribeJumping(true);
            SubscribeSprinting(true);
            SubscribeCrouching(true);
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            SubscribeMovement(false);
            SubscribeVertical(false);
            SubscribeLooking(false);
            SubscribeJumping(false);
            SubscribeSprinting(false);
            SubscribeCrouching(false);

            motion.SetHorizontalMove(Vector2.zero);
            rotator.SetInput(Vector2.zero);
        }

        #region subscribtions

        private void SubscribeLooking(bool subscribe)
        {
            if (subscribe)
            {
                lookingAction.performed += LookingActionOnPerformed;
                lookingAction.canceled += LookingActionOnCanceled;
            }
            else
            {
                lookingAction.performed -= LookingActionOnPerformed;
                lookingAction.canceled -= LookingActionOnCanceled;
            }
        }

        private void SubscribeMovement(bool subscribe)
        {
            if (subscribe)
            {
                movementAction.performed += MovementActionOnPerformed;
                movementAction.canceled += MovementActionOnCanceled;
            }
            else
            {
                movementAction.performed -= MovementActionOnPerformed;
                movementAction.canceled -= MovementActionOnCanceled;
            }
        }
        
        private void SubscribeVertical(bool subscribe)
        {
            if (subscribe)
            {
                verticalAction.performed += VerticalActionOnPerformed;
                verticalAction.canceled += VerticalActionOnCanceled;
            }
            else
            {
                verticalAction.performed -= VerticalActionOnPerformed;
                verticalAction.canceled -= VerticalActionOnCanceled;
            }
        }

        private void SubscribeJumping(bool subscribe)
        {
            if (subscribe)
            {
                jumpAction.performed += JumpActionOnPerformed;
            }
            else
            {
                jumpAction.performed -= JumpActionOnPerformed;
            }
        }

        private void SubscribeSprinting(bool subscribe)
        {
            if (subscribe)
            {
                sprintAction.performed += SprintingActionOnPerformed;
                sprintAction.canceled += SprintingActionOnCanceled;
            }
            else
            {
                sprintAction.performed -= SprintingActionOnPerformed;
                sprintAction.canceled -= SprintingActionOnCanceled;
            }
        }
        
        private void SubscribeCrouching(bool subscribe)
        {
            if (subscribe)
            {
                crouchAction.performed += CrouchingActionOnPerformed;
                crouchAction.canceled += CrouchingActionOnCanceled;
            }
            else
            {
                crouchAction.performed -= CrouchingActionOnPerformed;
                crouchAction.canceled -= CrouchingActionOnCanceled;
            }
        }

        #endregion

        #region action callbacks

        private void MovementActionOnPerformed(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            motion.SetHorizontalMove(value);
        }

        private void MovementActionOnCanceled(InputAction.CallbackContext context)
        {
            motion.SetHorizontalMove(Vector2.zero);
        }
        
        private void VerticalActionOnPerformed(InputAction.CallbackContext context)
        {
            motion.SetVerticalMove(context.ReadValue<float>());
        }

        private void VerticalActionOnCanceled(InputAction.CallbackContext context)
        {
            motion.SetVerticalMove(0f);
        }

        private void LookingActionOnPerformed(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            rotator.SetInput(value);
        }

        private void LookingActionOnCanceled(InputAction.CallbackContext context)
        {
            rotator.SetInput(Vector2.zero);
        }

        private void JumpActionOnPerformed(InputAction.CallbackContext context)
        {
            motion.TriggerJump();
        }

        private void SprintingActionOnPerformed(InputAction.CallbackContext context)
        {
            motion.SetSprint(true);
        }

        private void SprintingActionOnCanceled(InputAction.CallbackContext context)
        {
            motion.SetSprint(false);
        }
        
        private void CrouchingActionOnPerformed(InputAction.CallbackContext context)
        {
            motion.SetCrouch(true);
        }

        private void CrouchingActionOnCanceled(InputAction.CallbackContext context)
        {
            motion.SetCrouch(false);
        }

        #endregion

        #region private

        private bool FindActions()
        {
            movementAction = input.actions.FindAction(actionNames.movement);
            verticalAction = input.actions.FindAction(actionNames.vertical);
            lookingAction = input.actions.FindAction(actionNames.looking);
            jumpAction = input.actions.FindAction(actionNames.jump);
            sprintAction = input.actions.FindAction(actionNames.sprint);
            crouchAction = input.actions.FindAction(actionNames.crouch);

            return movementAction != null
                   && verticalAction != null
                   && lookingAction != null
                   && jumpAction != null
                   && sprintAction != null
                   && crouchAction != null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _editor_this = this;

            if (!input) TryGetComponent(out input);
            if (input)
            {
                input.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

                if (!FindActions())
                    Debug.LogError("One of the action is missing in the assigned input asset");
            }
        }

        private static CharacterInputHandler _editor_this;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [DidReloadScripts]
        private static void ResolveReferences()
        {
            if (_editor_this && !_editor_this.input.uiInputModule)
            {
                var uiInputModule = FindObjectOfType<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                if (uiInputModule)
                    _editor_this.input.uiInputModule = uiInputModule;
            }
        }
#endif

        #endregion
    }
}
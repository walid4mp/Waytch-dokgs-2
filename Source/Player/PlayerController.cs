// =====================================================================
//  Neon Cipher — Third-Person Player Controller
//  File:    PlayerController.cs
//  Notes:   Reads InputActions asset (PlayerActions.inputactions) —
//           supports keyboard&mouse, gamepad, and touch (mobile virtual
//           joystick + jump button) on Android 10+.
//           SOLID: high-level gameplay state is independent of input;
//           swap PlayerActions.inputactions and the controller still works.
// =====================================================================
using UnityEngine;
using UnityEngine.InputSystem;

namespace NeonCipher.Player
{
    /// <summary>
    /// Drives the protagonist (Kade "Cipher" Mercer). Single MonoBehaviour
    /// glued to a character prefab (CharacterController + camera rig child).
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Locomotion")]
        [SerializeField] private float walkSpeed   = 4.5f;
        [SerializeField] private float runSpeed    = 7.5f;
        [SerializeField] private float jumpHeight  = 1.6f;
        [SerializeField] private float gravity     = -24f;
        [SerializeField] private float coyoteTime  = 0.10f;
        [SerializeField] private Transform cameraRig; // Cinemachine vcam "LookAt"

        private CharacterController _cc;
        private PlayerActions _actions;
        private Vector2       _move;
        private float         _vertical;
        private float         _lastGroundTime;
        private bool          _isRunning;
        private bool          _crouching;
        private bool          _hackingHeld;

        public bool IsRunning => _isRunning;
        public float CurrentSpeed { get; private set; }

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _actions = new PlayerActions();
            _actions.Player.Move.performed  += ctx => _move = ctx.ReadValue<Vector2>();
            _actions.Player.Move.canceled   += _  => _move = Vector2.zero;
            _actions.Player.Run.started     += _  => _isRunning = true;
            _actions.Player.Run.canceled    += _  => _isRunning = false;
            _actions.Player.Jump.performed  += OnJump;
            _actions.Player.Crouch.started  += _  => _crouching = true;
            _actions.Player.Crouch.canceled += _  => _crouching = false;
            _actions.Player.Hack.started    += _  => _hackingHeld = true;
            _actions.Player.Hack.canceled   += _  => _hackingHeld = false;
        }

        private void OnEnable()  => _actions.Enable();
        private void OnDisable() => _actions.Disable();

        private void OnJump(InputAction.CallbackContext _)
        {
            if (Time.time - _lastGroundTime < coyoteTime) _vertical = Mathf.Sqrt(-2f * gravity * jumpHeight);
        }

        private void Update()
        {
            bool grounded = _cc.isGrounded;
            if (grounded) _lastGroundTime = Time.time;

            if (grounded && _vertical < 0f) _vertical = -2f; // stick to ground
            _vertical += gravity * Time.deltaTime;

            // Convert 2D input to world-space (camera-relative).
            Vector3 forward = cameraRig ? cameraRig.forward : Vector3.forward;
            Vector3 right   = cameraRig ? cameraRig.right   : Vector3.right;
            forward.y = right.y = 0; // flatten
            forward.Normalize(); right.Normalize();

            Vector3 lateral = (forward * _move.y + right * _move.x)
                              * (_isRunning ? runSpeed : walkSpeed)
                              * (_crouching ? 0.55f : 1f);

            Vector3 motion  = lateral + Vector3.up * _vertical;
            _cc.Move(motion * Time.deltaTime);
            CurrentSpeed = lateral.magnitude;

            if (lateral.sqrMagnitude > 0.1f)
            {
                var rot = Quaternion.LookRotation(new Vector3(lateral.x, 0, lateral.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 12f * Time.deltaTime);
            }
        }

        private void OnDestroy() { _actions?.Dispose(); _actions = null; }
    }
}

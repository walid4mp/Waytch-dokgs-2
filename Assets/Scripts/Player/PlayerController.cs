// Neon Cipher — Third-Person Player Controller
using NeonCipher.InputLayer;
using UnityEngine;

namespace NeonCipher.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 4.5f, runSpeed = 7.5f, crouchSpeed = 2.4f;
        [SerializeField] private float jumpHeight = 1.6f, gravity = -24f, coyoteTime = 0.10f;
        [SerializeField] private Transform cameraRig;
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private LayerMask interactMask = ~0;

        private CharacterController _cc;
        private IInputProvider _input;
        private float _vertical, _lastGroundTime;
        private bool _crouching;

        public bool IsRunning { get; private set; }
        public bool IsCrouching => _crouching;
        public float CurrentSpeed { get; private set; }
        public Transform CameraRig { get => cameraRig; set => cameraRig = value; }
        public void SetInputProvider(IInputProvider p) => _input = p;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
#if ENABLE_INPUT_SYSTEM
            _input ??= new UnityInputReader();
#endif
        }

        private void Update()
        {
            if (_input == null) return;
            Vector2 move = _input.Move;
            IsRunning = _input.Run && !_crouching;
            if (_input.Crouch) _crouching = !_crouching;

            bool grounded = _cc.isGrounded;
            if (grounded) _lastGroundTime = Time.time;
            if (grounded && _vertical < 0f) _vertical = -2f;
            _vertical += gravity * Time.deltaTime;
            if (_input.Jump && Time.time - _lastGroundTime < coyoteTime)
                _vertical = Mathf.Sqrt(-2f * gravity * jumpHeight);

            Vector3 forward = cameraRig ? cameraRig.forward : transform.forward;
            Vector3 right = cameraRig ? cameraRig.right : transform.right;
            forward.y = right.y = 0f; forward.Normalize(); right.Normalize();

            float speed = _crouching ? crouchSpeed : (IsRunning ? runSpeed : walkSpeed);
            Vector3 lateral = (forward * move.y + right * move.x) * speed;
            Vector3 motion = lateral + Vector3.up * _vertical;
            _cc.Move(motion * Time.deltaTime);
            CurrentSpeed = lateral.magnitude;

            if (lateral.sqrMagnitude > 0.1f)
            {
                var rot = Quaternion.LookRotation(new Vector3(lateral.x, 0f, lateral.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 12f * Time.deltaTime);
            }
            if (_input.Interact) TryInteract();
        }

        private void TryInteract()
        {
            var origin = transform.position + Vector3.up * 1.2f;
            var dir = transform.forward;
            if (!Physics.SphereCast(origin, 0.4f, dir, out var hit, interactRange, interactMask, QueryTriggerInteraction.Collide)) return;
            hit.collider.GetComponentInParent<IInteractable>()?.Interact(this);
        }
    }

    public interface IInteractable
    {
        string Prompt { get; }
        void Interact(PlayerController player);
    }
}

// Neon Cipher — Third-Person Camera Rig
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace NeonCipher.Camera
{
    public sealed class ThirdPersonCameraRig : MonoBehaviour
    {
        public Transform Target;
        public float Distance = 4.5f, Height = 1.65f;
        public float MouseSensitivity = 0.15f, GamepadSensitivity = 3.5f, TouchSensitivity = 0.35f;
        public float MinPitch = -30f, MaxPitch = 65f;
        public LayerMask ObstacleMask = ~0;
        public float ObstaclePadding = 0.25f;
        private float _yaw, _pitch, _currentDistance;

        private void Start() { _currentDistance = Distance; if (Target != null) _yaw = Target.eulerAngles.y; }

        private void LateUpdate()
        {
            if (Target == null) return;
            Vector2 look = ReadLook();
            bool touch = Application.isMobilePlatform;
            _yaw += look.x * (touch ? TouchSensitivity : MouseSensitivity);
            _pitch -= look.y * (touch ? TouchSensitivity : MouseSensitivity);
            _pitch = Mathf.Clamp(_pitch, MinPitch, MaxPitch);
            var rot = Quaternion.Euler(_pitch, _yaw, 0f);
            var pivot = Target.position + Vector3.up * Height;
            var desired = pivot - rot * Vector3.forward * Distance;
            float dist = Distance;
            if (Physics.Linecast(pivot, desired, out var hit, ObstacleMask, QueryTriggerInteraction.Ignore))
                dist = Mathf.Max(0.6f, hit.distance - ObstaclePadding);
            _currentDistance = Mathf.Lerp(_currentDistance, dist, 12f * Time.deltaTime);
            transform.position = pivot - rot * Vector3.forward * _currentDistance;
            transform.rotation = rot;
        }

        private Vector2 ReadLook()
        {
#if ENABLE_INPUT_SYSTEM
            Vector2 v = Vector2.zero;
            var m = Mouse.current;
            if (m != null && !Application.isMobilePlatform) v += m.delta.ReadValue();
            var gp = Gamepad.current; if (gp != null) v += gp.rightStick.ReadValue() * GamepadSensitivity;
            var ts = Touchscreen.current; if (ts != null && ts.primaryTouch.press.isPressed) v += ts.primaryTouch.delta.ReadValue();
            return v;
#else
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
        }

        public void SnapYawTo(Vector3 worldForward)
        {
            worldForward.y = 0f;
            if (worldForward.sqrMagnitude < 0.001f) return;
            _yaw = Quaternion.LookRotation(worldForward.normalized).eulerAngles.y;
        }
    }
}

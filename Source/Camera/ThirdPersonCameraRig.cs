// =====================================================================
//  Neon Cipher — Third-Person Camera Rig (Cinemachine-free fallback)
//  File:    ThirdPersonCameraRig.cs
//  Notes:   Cinemachine is preferred; this stand-in runs everywhere.
// =====================================================================
using UnityEngine;

namespace NeonCipher.Camera
{
    public sealed class ThirdPersonCameraRig : MonoBehaviour
    {
        public Transform Target;
        public float Distance = 4f;
        public float Height   = 1.6f;
        public float MouseSensitivity = 0.15f;
        public float GamepadSensitivity = 4f;
        public float TouchSensitivity  = 0.6f;
        public float MinPitch = -30f;
        public float MaxPitch = 65f;
        private float _yaw, _pitch;
        private float _currentDistance;

        private void LateUpdate()
        {
            if (Target == null) return;
            Vector2 look = ReadLook();
            _yaw   += look.x * (Application.isMobilePlatform ? TouchSensitivity : MouseSensitivity);
            _pitch -= look.y * (Application.isMobilePlatform ? TouchSensitivity : MouseSensitivity);
            _pitch  = Mathf.Clamp(_pitch, MinPitch, MaxPitch);
            var rot = Quaternion.Euler(_pitch, _yaw, 0);
            _currentDistance = Mathf.Lerp(_currentDistance, Distance, 6f * Time.deltaTime);
            var aim = Target.position + Vector3.up * Height;
            transform.position = aim - rot * Vector3.forward * _currentDistance;
            transform.rotation = rot;
        }

        private Vector2 ReadLook()
        {
            #if ENABLE_INPUT_SYSTEM
            var p = NeonCipher.Player.PlayerActions_Helper.ReadLook();
            return p;
            #else
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            #endif
        }
    }

    internal static class PlayerActions_Helper
    {
        public static Vector2 ReadLook()
        {
            var dev = UnityEngine.InputSystem.Mouse.current;
            var gp  = UnityEngine.InputSystem.Gamepad.current;
            if (dev != null) return new Vector2(dev.delta.x.ReadValue(), dev.delta.y.ReadValue());
            if (gp  != null) return gp.rightStick.ReadValue() * 4f;
            return Vector2.zero;
        }
    }
}

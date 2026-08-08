// Neon Cipher — High-level Input Provider (reads Input System directly)
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace NeonCipher.InputLayer
{
    public interface IInputProvider
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool Run { get; }
        bool Jump { get; }
        bool Crouch { get; }
        bool Interact { get; }
        bool Hack { get; }
        bool Phone { get; }
        bool Pause { get; }
    }

#if ENABLE_INPUT_SYSTEM
    public sealed class UnityInputReader : IInputProvider
    {
        public Vector2 Move
        {
            get
            {
                Vector2 v = Vector2.zero;
                var kb = Keyboard.current;
                if (kb != null)
                {
                    if (kb.wKey.isPressed) v.y += 1;
                    if (kb.sKey.isPressed) v.y -= 1;
                    if (kb.aKey.isPressed) v.x -= 1;
                    if (kb.dKey.isPressed) v.x += 1;
                }
                var gp = Gamepad.current;
                if (gp != null) v += gp.leftStick.ReadValue();
                return Vector2.ClampMagnitude(v, 1f);
            }
        }
        public Vector2 Look
        {
            get
            {
                Vector2 v = Vector2.zero;
                var m = Mouse.current; if (m != null) v += m.delta.ReadValue();
                var gp = Gamepad.current; if (gp != null) v += gp.rightStick.ReadValue() * 5f;
                return v;
            }
        }
        public bool Run      => (Keyboard.current?.leftShiftKey.isPressed ?? false) || (Gamepad.current?.buttonWest.isPressed ?? false);
        public bool Jump     => (Keyboard.current?.spaceKey.wasPressedThisFrame ?? false) || (Gamepad.current?.buttonSouth.wasPressedThisFrame ?? false);
        public bool Crouch   => (Keyboard.current?.cKey.wasPressedThisFrame ?? false) || (Gamepad.current?.buttonEast.wasPressedThisFrame ?? false);
        public bool Interact => (Keyboard.current?.eKey.wasPressedThisFrame ?? false) || (Gamepad.current?.buttonNorth.wasPressedThisFrame ?? false);
        public bool Hack     => (Keyboard.current?.qKey.isPressed ?? false) || (Gamepad.current?.rightTrigger.isPressed ?? false);
        public bool Phone    => (Keyboard.current?.tabKey.wasPressedThisFrame ?? false) || (Gamepad.current?.selectButton.wasPressedThisFrame ?? false);
        public bool Pause    => (Keyboard.current?.escapeKey.wasPressedThisFrame ?? false) || (Gamepad.current?.startButton.wasPressedThisFrame ?? false);
    }
#endif

    public sealed class FakeInputProvider : IInputProvider
    {
        public Vector2 Move { get; set; }
        public Vector2 Look { get; set; }
        public bool Run { get; set; }
        public bool Jump { get; set; }
        public bool Crouch { get; set; }
        public bool Interact { get; set; }
        public bool Hack { get; set; }
        public bool Phone { get; set; }
        public bool Pause { get; set; }
    }
}

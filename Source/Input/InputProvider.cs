// =====================================================================
//  Neon Cipher — High-level Input Provider (testable facade)
// =====================================================================
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using NeonCipher.Core;

namespace NeonCipher.InputLayer
{
    public interface IInputProvider
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool    Run { get; }
        bool    Jump { get; }
        bool    Crouch { get; }
        bool    Interact { get; }
        bool    Hack { get; }
        bool    Phone { get; }
        bool    Pause { get; }
    }

#if ENABLE_INPUT_SYSTEM
    public sealed class UnityInputProvider : IInputProvider
    {
        private PlayerActions _a;
        public UnityInputProvider(PlayerActions generated) { _a = generated; _a.Enable(); }
        public Vector2 Move    => _a.Player.Move.ReadValue<Vector2>();
        public Vector2 Look    => _a.Player.Look.ReadValue<Vector2>();
        public bool    Run     => _a.Player.Run.IsPressed();
        public bool    Jump    => _a.Player.Jump.WasPressedThisFrame();
        public bool    Crouch  => _a.Player.Crouch.IsPressed();
        public bool    Interact=> _a.Player.Interact.WasPressedThisFrame();
        public bool    Hack    => _a.Player.Hack.IsPressed();
        public bool    Phone   => _a.Player.Phone.WasPressedThisFrame();
        public bool    Pause   => _a.Player.Pause.WasPressedThisFrame();
    }
#endif

    /// <summary>Test double — record/replay. Lets engine-free unit tests validate input reactions.</summary>
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

// =====================================================================
//  Neon Cipher — Mobile Virtual Stick / Jump / Hack touch buttons
//  File:    MobileHudOverlay.cs
//  Notes:   Auto-enabled on Android / iOS. Uses UI Toolkit or UGUI.
//           Builds dynamic visual joysticks so we don't pre-bake sprites
//           and don't depend on protected package assets.
// =====================================================================
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

namespace NeonCipher.Player
{
    /// <summary>
    /// Hosts the on-screen controls on touch devices. The actual UI is wired
    /// in Unity Editor (Assets/UI/Mobile.*.uxml) — this script just toggles
    /// the canvas based on the device.
    /// </summary>
    public sealed class MobileHudOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject _mobileUI;
        [SerializeField] private OnScreenStick _stick;
        [SerializeField] private OnScreenButton _jump;

        private void Awake()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (_mobileUI) _mobileUI.SetActive(true);
#else
            if (_mobileUI) _mobileUI.SetActive(false);
#endif
        }
    }
}

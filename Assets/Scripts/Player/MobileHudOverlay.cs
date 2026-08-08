// Neon Cipher — Mobile virtual stick / buttons (touch platforms)
using UnityEngine;

namespace NeonCipher.Player
{
    public sealed class MobileHudOverlay : MonoBehaviour
    {
        public bool ForceEnable = false;
        public Vector2 Move { get; private set; }
        public bool Jump { get; private set; }
        public bool Run { get; private set; }
        public bool Crouch { get; private set; }
        public bool Interact { get; private set; }
        public bool Hack { get; private set; }
        public bool Phone { get; private set; }

        private void Awake()
        {
            bool show = ForceEnable || Application.isMobilePlatform ||
                        Application.platform == RuntimePlatform.Android ||
                        Application.platform == RuntimePlatform.IPhonePlayer;
            gameObject.SetActive(show);
        }
    }
}

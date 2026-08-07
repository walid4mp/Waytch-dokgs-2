// =====================================================================
//  Neon Cipher — HUD (HP / WantedLevel / Time-of-day / minimap stub)
//  File:    HudController.cs
// =====================================================================
using UnityEngine;
using UnityEngine.UI;

namespace NeonCipher.UI
{
    public sealed class HudController : MonoBehaviour
    {
        [SerializeField] private Slider    _hp;
        [SerializeField] private Slider    _wanted;
        [SerializeField] private Text      _timeText;
        [SerializeField] private Text      _wantedText;
        [SerializeField] private CanvasGroup _phonePanel;
        [SerializeField] private GameObject _pauseMenu;

        private void OnEnable()
        {
            if (GameServices.Current == null) return;
            GameServices.Current.Get<IWorldClock>().HourChanged += time => RefreshTimeText(time);
            GameServices.Current.Get<PhoneController>().ScreenChanged += OnPhoneScreen;
        }

        private void RefreshTimeText(float time)
        {
            int hour = Mathf.FloorToInt(time);
            int min  = Mathf.FloorToInt((time - hour) * 60f);
            _timeText.text   = $"{hour:00}:{min:00}";
            _wantedText.text = string.Empty; // bound externally
        }

        private void OnPhoneScreen(PhoneScreen s)
        {
            _phonePanel.alpha = (s != PhoneScreen.Locked && s != PhoneScreen.Settings) ? 1 : 0;
            _phonePanel.blocksRaycasts = _phonePanel.alpha > 0.5f;
        }

        public void TogglePause() => _pauseMenu.SetActive(!_pauseMenu.activeSelf);
    }
}

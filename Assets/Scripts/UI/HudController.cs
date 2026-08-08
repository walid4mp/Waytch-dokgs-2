// Neon Cipher — Runtime HUD Controller
using NeonCipher.Core;
using NeonCipher.Hacking;
using NeonCipher.Mission;
using NeonCipher.PhoneUI;
using UnityEngine;
using UnityEngine.UI;

namespace NeonCipher.UI
{
    public sealed class HudController : MonoBehaviour
    {
        [SerializeField] private Slider _hp, _wanted, _hackBar;
        [SerializeField] private Text _timeText, _objectiveText, _hintText, _moneyText;
        [SerializeField] private CanvasGroup _phonePanel;
        [SerializeField] private GameObject _pauseMenu;
        private IHackingBus _hack;

        public void Bind(Slider hp, Slider wanted, Slider hackBar, Text time, Text obj,
                         Text hint, Text money, CanvasGroup phone, GameObject pause)
        {
            _hp = hp; _wanted = wanted; _hackBar = hackBar; _timeText = time;
            _objectiveText = obj; _hintText = hint; _moneyText = money;
            _phonePanel = phone; _pauseMenu = pause;
        }

        private void OnEnable()
        {
            if (GameServices.Current == null) return;
            if (GameServices.Current.TryGet<IWorldClock>(out var clk)) clk.HourChanged += RefreshTimeText;
            if (GameServices.Current.TryGet<PhoneController>(out var ph)) ph.ScreenChanged += OnPhoneScreen;
            if (GameServices.Current.TryGet<IHackingBus>(out _hack)) _hack.ProgressChanged += OnHackProgress;
        }
        private void OnDisable()
        {
            if (GameServices.Current == null) return;
            if (GameServices.Current.TryGet<IWorldClock>(out var clk)) clk.HourChanged -= RefreshTimeText;
            if (GameServices.Current.TryGet<PhoneController>(out var ph)) ph.ScreenChanged -= OnPhoneScreen;
            if (_hack != null) _hack.ProgressChanged -= OnHackProgress;
        }
        private void Update()
        {
            if (_hack != null && _hackBar != null) _hackBar.gameObject.SetActive(_hack.IsBusy);
            if (_moneyText != null && GameServices.Current != null &&
                GameServices.Current.TryGet<NeonCipher.Inventory.IInventory>(out var inv))
                _moneyText.text = $"$ {inv.Money}";
            if (_objectiveText != null && GameServices.Current != null &&
                GameServices.Current.TryGet<MissionController>(out var mc))
            {
                var step = mc.CurrentStep();
                _objectiveText.text = step != null ? step.Title : "";
            }
        }
        private void RefreshTimeText(float time)
        {
            if (_timeText == null) return;
            int hour = Mathf.FloorToInt(time);
            int min = Mathf.FloorToInt((time - hour) * 60f);
            _timeText.text = $"{hour:00}:{min:00}";
        }
        private void OnPhoneScreen(PhoneScreen s)
        {
            if (_phonePanel == null) return;
            bool show = s != PhoneScreen.Locked;
            _phonePanel.alpha = show ? 1f : 0f;
            _phonePanel.blocksRaycasts = show;
            _phonePanel.interactable = show;
        }
        private void OnHackProgress(HackProgress p) { if (_hackBar != null) _hackBar.value = p.Ratio; }
        public void SetHint(string t) { if (_hintText != null) _hintText.text = t ?? ""; }
        public void TogglePause() { if (_pauseMenu != null) _pauseMenu.SetActive(!_pauseMenu.activeSelf); }
    }
}

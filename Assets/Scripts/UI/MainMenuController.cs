// Neon Cipher — Settings Panel Controller
using NeonCipher.Core;
using UnityEngine;
using UnityEngine.UI;

namespace NeonCipher.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Dropdown _quality;
        [SerializeField] private Slider _masterVol, _musicVol, _sfxVol;
        [SerializeField] private Toggle _vibration, _subtitles;
        [SerializeField] private Dropdown _language;

        public void Bind(Dropdown q, Slider m, Slider mu, Slider sfx, Toggle vib, Toggle sub, Dropdown lang)
        { _quality = q; _masterVol = m; _musicVol = mu; _sfxVol = sfx; _vibration = vib; _subtitles = sub; _language = lang; }

        private void OnEnable() { if (GameServices.Current != null) Hydrate(); }

        private void Hydrate()
        {
            var s = GameServices.Current.Get<IGameSettings>();
            if (_quality) _quality.value = (int)s.GraphicsQuality;
            if (_masterVol) _masterVol.value = s.MasterVolume;
            if (_musicVol) _musicVol.value = s.MusicVolume;
            if (_sfxVol) _sfxVol.value = s.SfxVolume;
            if (_vibration) _vibration.isOn = s.VibrationEnabled;
            if (_subtitles) _subtitles.isOn = s.SubtitlesEnabled;
            if (_language) _language.value = Index(s.Language);

            if (_quality) _quality.onValueChanged.AddListener(i => { s.GraphicsQuality = (GraphicsQuality)i; s.RaiseSettingsChanged(); });
            if (_masterVol) _masterVol.onValueChanged.AddListener(v => { s.MasterVolume = v; s.RaiseSettingsChanged(); });
            if (_musicVol) _musicVol.onValueChanged.AddListener(v => { s.MusicVolume = v; s.RaiseSettingsChanged(); });
            if (_sfxVol) _sfxVol.onValueChanged.AddListener(v => { s.SfxVolume = v; s.RaiseSettingsChanged(); });
            if (_vibration) _vibration.onValueChanged.AddListener(b => { s.VibrationEnabled = b; s.RaiseSettingsChanged(); });
            if (_subtitles) _subtitles.onValueChanged.AddListener(b => { s.SubtitlesEnabled = b; s.RaiseSettingsChanged(); });
            if (_language) _language.onValueChanged.AddListener(i =>
            {
                var code = Code(i); s.Language = code;
                GameServices.Current.Get<ILocalization>().SetLanguage(code);
                s.RaiseSettingsChanged();
            });
        }
        private int Index(string code) => code switch { "en" => 0, "fr" => 1, "ar" => 2, "ja" => 3, _ => 0 };
        private string Code(int idx) => idx switch { 0 => "en", 1 => "fr", 2 => "ar", 3 => "ja", _ => "en" };
    }
}

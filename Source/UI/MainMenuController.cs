// =====================================================================
//  Neon Cipher — Settings & Pause Menu
//  File:    MainMenuController.cs
// =====================================================================
using UnityEngine;
using UnityEngine.UI;

namespace NeonCipher.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Dropdown _quality;
        [SerializeField] private Slider    _masterVol, _musicVol, _sfxVol;
        [SerializeField] private Toggle    _vibration, _subtitles;
        [SerializeField] private Dropdown  _language;

        private void Start()
        {
            var s = GameServices.Current.Get<IGameSettings>();
            _quality.value     = (int)s.GraphicsQuality;
            _masterVol.value   = s.MasterVolume;
            _musicVol.value    = s.MusicVolume;
            _sfxVol.value      = s.SfxVolume;
            _vibration.isOn    = s.VibrationEnabled;
            _subtitles.isOn    = s.SubtitlesEnabled;
            _language.value    = Index(s.Language);

            _quality.onValueChanged.AddListener(i => { s.GraphicsQuality = (GraphicsQuality)i; s.SettingsChanged?.Invoke(); });
            _masterVol.onValueChanged.AddListener(v => { s.MasterVolume = v; s.SettingsChanged?.Invoke(); });
            _musicVol .onValueChanged.AddListener(v => { s.MusicVolume  = v; s.SettingsChanged?.Invoke(); });
            _sfxVol   .onValueChanged.AddListener(v => { s.SfxVolume    = v; s.SettingsChanged?.Invoke(); });
            _vibration.onValueChanged.AddListener(b => { s.VibrationEnabled = b; s.SettingsChanged?.Invoke(); });
            _subtitles.onValueChanged.AddListener(b => { s.SubtitlesEnabled = b; s.SettingsChanged?.Invoke(); });
            _language.onValueChanged.AddListener(i => { var code = Code(i); s.Language = code; GameServices.Current.Get<ILocalization>().SetLanguage(code); s.SettingsChanged?.Invoke(); });
        }

        private int Index(string code)    => code switch { "en"=>0,"fr"=>1,"ar"=>2,"ja"=>3,_=>0 };
        private string Code(int idx)      => idx switch { 0=>"en",1=>"fr",2=>"ar",3=>"ja",_=>"en" };
    }
}

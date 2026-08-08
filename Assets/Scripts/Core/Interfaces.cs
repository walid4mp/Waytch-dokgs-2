// Neon Cipher — Core Service Interfaces (Dependency Inversion contracts)
using System;
using UnityEngine;

namespace NeonCipher.Core
{
    public interface IWorldClock
    {
        float CurrentTime { get; }
        float DayLengthSeconds { get; set; }
        DayPhase Phase { get; }
        event Action<float> HourChanged;
        event Action<DayPhase> PhaseChanged;
        void Tick(float deltaSeconds);
    }

    public enum DayPhase { Dawn = 0, Day = 1, Dusk = 2, Night = 3 }

    public interface IGameSettings
    {
        GraphicsQuality GraphicsQuality { get; set; }
        float MasterVolume { get; set; }
        float MusicVolume { get; set; }
        float SfxVolume { get; set; }
        bool VibrationEnabled { get; set; }
        bool SubtitlesEnabled { get; set; }
        string Language { get; set; }
        float FrameRateCap { get; set; }
        event Action SettingsChanged;
        void RaiseSettingsChanged();
    }

    public enum GraphicsQuality { Low, Medium, High, Ultra }

    public interface ISaveSystem
    {
        bool Save(int slot, SaveData data);
        bool Load(int slot, out SaveData data);
        bool Delete(int slot);
    }

    public interface ILocalization
    {
        string CurrentLanguage { get; }
        void SetLanguage(string isoCode);
        string T(string key);
        event Action<string> LanguageChanged;
    }

    public interface IAudioBus
    {
        void Play(string id, Vector3? pos = null);
        void SetBusVolume(AudioBusKind bus, float linear);
        void StopAll();
    }

    public enum AudioBusKind { Master, Music, Sfx, Ambient, Voice }

    public interface IWeather
    {
        WeatherState Current { get; }
        event Action<WeatherState> WeatherChanged;
        void SetTarget(WeatherState target, float transitionSeconds = 30f);
    }

    public enum WeatherState { Clear, Overcast, Rain, HeavyRain, Fog, NeonStorm }
}

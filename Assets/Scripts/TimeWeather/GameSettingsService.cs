// Neon Cipher — Game Settings Service
using System;
using System.IO;
using NeonCipher.Core;
using UnityEngine;

namespace NeonCipher.TimeWeather
{
    public sealed class GameSettingsService : IGameSettings
    {
        public GraphicsQuality GraphicsQuality { get; set; } = GraphicsQuality.High;
        public float MasterVolume { get; set; } = 1f;
        public float MusicVolume { get; set; } = 0.7f;
        public float SfxVolume { get; set; } = 1f;
        public bool VibrationEnabled { get; set; } = true;
        public bool SubtitlesEnabled { get; set; } = true;
        public string Language { get; set; } = "en";
        public float FrameRateCap { get; set; } = 60f;
        public event Action SettingsChanged;
        public void RaiseSettingsChanged() => SettingsChanged?.Invoke();
        private string Path => System.IO.Path.Combine(Application.persistentDataPath, "settings.json");
        public void Save() { try { File.WriteAllText(Path, JsonUtility.ToJson(this, false)); RaiseSettingsChanged(); } catch (Exception e) { Debug.LogWarning(e.Message); } }
        public void Load() { try { if (File.Exists(Path)) JsonUtility.FromJsonOverwrite(File.ReadAllText(Path), this); } catch (Exception e) { Debug.LogWarning(e.Message); } }
    }
}

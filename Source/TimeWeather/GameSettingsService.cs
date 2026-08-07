// =====================================================================
//  Neon Cipher — Game Settings
//  File:    GameSettingsService.cs
// =====================================================================
using System;
using System.IO;
using NeonCipher.Core;
using UnityEngine;

namespace NeonCipher.Localization
{
    public sealed class GameSettingsService : IGameSettings
    {
        public GraphicsQuality GraphicsQuality { get; set; } = GraphicsQuality.High;
        public float MasterVolume { get; set; } = 1f;
        public float MusicVolume  { get; set; } = 0.7f;
        public float SfxVolume    { get; set; } = 1f;
        public bool VibrationEnabled { get; set; } = true;
        public bool SubtitlesEnabled { get; set; } = true;
        public string Language { get; set; } = "en";
        public float FrameRateCap { get; set; } = 60f;
        public event Action SettingsChanged;

        private string Path => System.IO.Path.Combine(
            Application.persistentDataPath, "settings.json");

        public void Save() { File.WriteAllText(Path, JsonUtility.ToJson(this, false)); SettingsChanged?.Invoke(); }
        public void Load() { if (File.Exists(Path)) JsonUtility.FromJsonOverwrite(File.ReadAllText(Path), this); }
    }
}

// =====================================================================
//  Neon Cipher — Localization
//  File:    LocalizationService.cs
//  Notes:   Loads simple key→value tables from JSON in Assets/Localization.
//           No dependency on Unity Localization package to keep DI clean.
// =====================================================================
using System.Collections.Generic;
using System.IO;
using NeonCipher.Core;
using UnityEngine;

namespace NeonCipher.Localization
{
    public sealed class LocalizationService : ILocalization
    {
        private readonly Dictionary<string, Dictionary<string, string>> _tables = new();
        public string CurrentLanguage { get; private set; } = "en";
        public event Action<string> LanguageChanged;

        public LocalizationService() => LoadAll();

        public void SetLanguage(string isoCode)
        {
            if (isoCode == CurrentLanguage) return;
            CurrentLanguage = isoCode;
            LanguageChanged?.Invoke(isoCode);
        }

        public string T(string key)
        {
            if (_tables.TryGetValue(CurrentLanguage, out var t) && t.TryGetValue(key, out var v))
                return v;
            if (_tables.TryGetValue("en", out var en) && en.TryGetValue(key, out var fallback))
                return fallback;
            return $"#{key}"; // missing-key marker (helpful during development)
        }

        private void LoadAll()
        {
            string root = Path.Combine(Application.streamingAssetsPath, "Localization");
            if (!Directory.Exists(root)) { Debug.LogWarning($"[Loc] no tables at {root}"); return; }
            foreach (var file in Directory.GetFiles(root, "*.json"))
            {
                string lang = Path.GetFileNameWithoutExtension(file);
                string raw = File.ReadAllText(file);
                var dict = JsonUtility.FromJson<Dict>(raw)?.entries ?? new List<Entry>();
                var map = new Dictionary<string, string>();
                foreach (var e in dict) if (!string.IsNullOrEmpty(e.k)) map[e.k] = e.v;
                _tables[lang.ToLowerInvariant()] = map;
            }
            Debug.Log($"[Loc] loaded {_tables.Count} languages: {string.Join(", ", _tables.Keys)}");
        }

        [System.Serializable] private sealed class Dict { public List<Entry> entries = new(); }
        [System.Serializable] private sealed class Entry { public string k; public string v; }
    }
}
